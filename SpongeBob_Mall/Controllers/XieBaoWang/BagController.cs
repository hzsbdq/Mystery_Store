using SpongeBob_Mall.DAL;
using SpongeBob_Mall.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SpongeBob_Mall.Controllers.XieBaoWang
{
    public class BagController : Controller
    {
        private MySqlContext db = new MySqlContext();
        // 获取背包列表
        public async Task<ActionResult> Index()
        {
            //根据位置排序
            User user = (User)HttpContext.Session["user"];
            List<Goods> goods = await db.Goods.Where(b => b.UserID == user.UserId && b.State==0).OrderBy(b => b.Location).ToListAsync();
            return View(goods);

            //return View();
        }

        // 丢弃物品
        [HttpPost]
        public async Task<ActionResult> Abandon(int? goodsId, int? amount)
        {
            if (goodsId == null || amount == null)
            {
                Response.Redirect("Index?errorMSG=1");
            }
            Goods goods = await db.Goods.Where(b => b.GoodsId == goodsId && b.Amount >= amount).FirstOrDefaultAsync();
            if (goods == null)
            {
                Response.Redirect("Index?errorMSG=1");
            }
            goods.Amount -= (int)amount;
            if (goods.Amount == 0)
            {
                db.Goods.Remove(goods);//若物品数量为0，删除物品
            }
            else
            {
                db.Entry(goods).State = EntityState.Modified;//若物品数量不为0，更新物品
            }
            await db.SaveChangesAsync();
            return Redirect("Index");
        }
        // 上架物品
        public async Task<ActionResult> Putaway(int goodsId, int amount, int price)
        {
            Goods goods = await db.Goods.Where(b => b.GoodsId == goodsId && b.Amount >= amount).FirstOrDefaultAsync();
            if (goods == null)
            {
                Response.Redirect("Index?errorMSG=2");
            }
            else if (goods.State == 1 || goods.State == 2)
            {
                Response.Redirect("Index?errorMSG=3");
            }
            if (goods.Amount - amount == 0)
            {
                goods.State = 1;
                db.Entry(goods).State = EntityState.Modified;
            }
            else
            {
                goods.Amount -= amount;
                db.Entry(goods).State = EntityState.Modified;
                Goods newGoods = new Goods
                {
                    UserID = goods.UserID,
                    MapID = goods.MapID,
                    Amount = amount,
                    Price = price,
                    GetDate = goods.GetDate,
                    ShelvesDate = DateTime.Now,
                    CollectionTag = 0,
                    State = 1,
                    Location = goods.Location
                };
                db.Goods.Add(newGoods);
            }
            await db.SaveChangesAsync();
            return Redirect("Index");
        }

        // TO DO 获得物品，需要便利背包空位，放在最小的空位上或同一种物品数量加1，如果没有空位就不允许存入，如果是第一手物品就记录获取时间
        [HttpPost]
        public async Task<ActionResult> GetGoods(int mapId, int amount)
        {
            // 判断物品是否存在
            Map map = await db.Maps.Where(b => b.MapId == mapId).FirstOrDefaultAsync();
            if (map == null)
            {
                return null;// TO Do 返回错误信息
            }
            // TO DO 从数据库获得用户背包的物品列表
            User user = (User)HttpContext.Session["user"];
            List<Goods> goods = await db.Goods.Where(b => b.UserID == user.UserId && b.State == 0).OrderBy(b => b.Location).ToListAsync();

            // 背包是否已满(暂时定25)
            if (goods.Count >= 25)
            {
                return null;// TO Do 返回错误信息
            }

            // TO Do 遍历列表、标记第一个空位（初始为1每便利一个加1当遍历的物品的位置大于初始值时初始值的位置就是空位）。若继续遍历若找到同一种物品直接物品数量加1
            Goods addGoods=null;
            foreach(Goods g in goods)
            {
                if (g.MapID == mapId)
                {
                    addGoods = g;
                }
            }
            if (addGoods != null)
            {
                // 存在相同的物品
                addGoods.Amount += amount;
            }
            else
            {
                // 不存在相同的物品
                int fl = 1;
                foreach(Goods g in goods)
                {
                    if (g.Location != fl)
                    {
                        addGoods = new Goods
                        {
                            UserID = user.UserId,
                            MapID = mapId,
                            Amount = amount,
                            Price = 0,
                            GetDate = DateTime.Now,
                            CollectionTag = 0,
                            State = 0,
                            Location = fl
                        };
                        break;
                    }
                    fl++;
                }
                if (addGoods == null)
                {
                    addGoods = new Goods
                    {
                        UserID = user.UserId,
                        MapID = mapId,
                        Amount = amount,
                        Price = 0,
                        GetDate = DateTime.Now,
                        CollectionTag = 0,
                        State = 0,
                        Location = fl
                    };
                }
                db.Goods.Add(addGoods);
            }

            await db.SaveChangesAsync();

            return View("~/Views/xbwHome/Index.cshtml");// To DO 需要在参数里接收调用此Action的位置，执行结束后回到来的地方
        }


    }
}


