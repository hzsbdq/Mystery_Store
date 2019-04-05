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
        public async Task<ActionResult> Index(int? fl,string choose,int? time_sort,int? price_sort,int? change_page)
        {
            User user = (User)HttpContext.Session["user"];
            List<Goods> goods;
            List<Goods> goods_next;
            IQueryable<Goods> goods_or;
            IOrderedQueryable<Goods> goods_ored;
            int bag_page = 0;                             //初始化背包页面的值
            if (HttpContext.Session["bag_page"] == null)  //如果session中没有值，初始化值
            {
                HttpContext.Session["bag_page"] = 1;
            }
            if (change_page != null)//如果是页操作，更新背包页的值
            {
                bag_page = (int)((int)HttpContext.Session["bag_page"] + change_page);
            }
            else//否则将背包页的值归一，并重置排序
            {
                bag_page = 1;
                HttpContext.Session["time_sort"] = HttpContext.Session["time_sort"] == null || time_sort == null ? 0 : time_sort;
                HttpContext.Session["price_sort"] = HttpContext.Session["price_sort"] == null || price_sort == null ? 0 : price_sort;
            }
            if (bag_page < 1)//如果是非法操作，归零
            {
                HttpContext.Session["bag_page"] = 1;
            }
            else
            {
                HttpContext.Session["bag_page"] = bag_page;
            }
            int time_sort_clone = (int)HttpContext.Session["time_sort"];
            int price_sort_clone = (int)HttpContext.Session["price_sort"];
            if (time_sort_clone != 0)
            {
                price_sort = 0;
            }
            if(price_sort_clone != 0)
            {
                time_sort = 0;
            }

            if ((fl == null || choose == null)&&time_sort==null&&price_sort==null&&change_page==null)
            {
                HttpContext.Session["rare"] = "不限";
                HttpContext.Session["type"] = "不限";
            }
            else if (fl == 1 && time_sort == null && price_sort == null && change_page == null)
            {
                HttpContext.Session["rare"] = choose;
            }
            else if(fl ==2 && time_sort == null && price_sort == null && change_page == null)
            {
                HttpContext.Session["type"] = choose;
            }
            string rare = (string)HttpContext.Session["rare"];
            string type = (string)HttpContext.Session["type"];
            if (rare != "不限" && type != "不限")
            {
                goods_or = db.Goods.Where(b => b.UserID == user.UserId && b.State == 0 && b.Map.Rare == rare && b.Map.type == type);
            }
            else if (rare != "不限")
            {
                goods_or = db.Goods.Where(b => b.UserID == user.UserId && b.State == 0 && b.Map.Rare == rare);
            }
            else if(type != "不限")
            {
                goods_or = db.Goods.Where(b => b.UserID == user.UserId && b.State == 0 && b.Map.type == type);
            }
            else
            {
                goods_or = db.Goods.Where(b => b.UserID == user.UserId && b.State == 0);
            }

            if (time_sort_clone != 0)
            {
                if (time_sort_clone == 1)
                {
                    goods_ored = goods_or.OrderBy(b => b.GetDate);
                }
                else
                {
                    goods_ored = goods_or.OrderByDescending(b => b.GetDate);
                }
            }
            else
            {
                if (price_sort_clone == 1)
                {
                    goods_ored = goods_or.OrderBy(b => b.Price);
                }
                else
                {
                    goods_ored = goods_or.OrderByDescending(b => b.Price);
                }
            }

            try//防止越界
            {
                goods = await goods_ored.Skip(8 * (bag_page - 1)).Take(8).ToListAsync();
                goods_next = await goods_ored.Skip(8 * (bag_page)).Take(1).ToListAsync();
            }
            catch(Exception e)
            {
                goods = await goods_ored.Skip(0).Take(8).ToListAsync();
                goods_next = await goods_ored.Skip(8 * (bag_page)).Take(1).ToListAsync();
            }

            if (goods_next.Count > 0)
            {
                HttpContext.Session["bag_max_page"] = bag_page+1;
            }
            else
            {
                HttpContext.Session["bag_max_page"] = bag_page;
            }

            return View(goods);
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
        public async Task<ActionResult> GetGoods(int mapId)
        {
            // 判断物品是否存在
            Map map = await db.Maps.Where(b => b.MapId == mapId).FirstOrDefaultAsync();
            if (map == null)
            {
                return null;// TO Do 返回错误信息
            }

            User user = (User)HttpContext.Session["user"];
           

            
            Goods addGoods=null;
            addGoods = new Goods
            {
                UserID = user.UserId,
                MapID = mapId,
                Amount = 1,
                Price = 0,
                GetDate = DateTime.Now,
                CollectionTag = 0,
                State = 0,
                Location = 1
            };
            db.Goods.Add(addGoods);
            

            await db.SaveChangesAsync();

            return View("~/Views/TestGoods/Index.cshtml");// To DO 需要在参数里接收调用此Action的位置，执行结束后回到来的地方
        }


    }
}


