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
            List<Goods> goods;
            await Choose(null,null);
            goods = (List<Goods>)HttpContext.Session["goods"];
            return View(goods);
        }
        public async Task<ActionResult> Choose(int? fl, string choose)
        {
            User user = (User)HttpContext.Session["user"];
            IQueryable<Goods> goods_or;
            List<Goods> goods;
 
            if (fl == null || choose == null)
            {
                HttpContext.Session["rare"] = "不限";
                HttpContext.Session["type"] = "不限";
            }
            else if (fl == 1)
            {
                HttpContext.Session["rare"] = choose;
            }
            else if (fl == 2)
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
            else if (type != "不限")
            {
                goods_or = db.Goods.Where(b => b.UserID == user.UserId && b.State == 0 && b.Map.type == type);
            }
            else
            {
                goods_or = db.Goods.Where(b => b.UserID == user.UserId && b.State == 0);
            }

            HttpContext.Session["goods_or"] = goods_or;

            await Sort(0, 0);

            goods = (List<Goods>)HttpContext.Session["goods"];

            return View("~/Views/Bag/Index.cshtml", goods);
        }

        //搜索
        [HttpGet]
        public async Task<ActionResult> Search(string search_value)
        {
            User user = (User)HttpContext.Session["user"];
            IQueryable<Goods> goods_or;

            List<Goods> goods;

            if (search_value == null)
            {
                goods_or = db.Goods.Where(b => b.UserID == user.UserId && b.State == 0);
            }
            else
            {
                goods_or = db.Goods.Where(b => b.UserID == user.UserId && b.State == 0 && b.Map.Name.Contains(search_value));
            }

            HttpContext.Session["goods_or"] = goods_or;

            await Sort(0, 0);

            goods = (List<Goods>)HttpContext.Session["goods"];

            return View("~/Views/Bag/Index.cshtml", goods);
        }

        //排序

        public async Task<ActionResult> Sort(int? time_sort, int? price_sort)
        {
            IQueryable<Goods> goods_or = (IQueryable<Goods>)HttpContext.Session["goods_or"];
            IOrderedQueryable<Goods> goods_ored;

            List<Goods> goods;


            if (time_sort == 1 || price_sort == 1)
            {
                if (time_sort == 1)
                {
                    goods_ored = goods_or.OrderBy(b => b.GetDate);
                }
                else
                {
                    goods_ored = goods_or.OrderByDescending(b => b.GetDate);
                }

                if (price_sort == 1)
                {
                    goods_ored = goods_or.OrderBy(b => b.Price);
                }
                else
                {
                    goods_ored = goods_or.OrderByDescending(b => b.Price);
                }
            }
            else
            {
                goods_ored = goods_or.OrderBy(b => b.GetDate);
            }

            HttpContext.Session["time_sort"] = time_sort == null ? 0 : time_sort;
            HttpContext.Session["price_sort"] = price_sort == null ? 0 : price_sort;
            HttpContext.Session["goods_ored"] = goods_ored;

            await ChangePage(0);

            goods = (List<Goods>)HttpContext.Session["goods"];

            return View("~/Views/Bag/Index.cshtml", goods);
        }

        //分页
        public async Task<ActionResult> ChangePage(int? change_page)
        {
            List<Goods> goods;
            List<Goods> goods_next;
            int bag_page;

            IOrderedQueryable<Goods> goods_ored = (IOrderedQueryable<Goods>)HttpContext.Session["goods_ored"];

            if (change_page == 0)
            {
                bag_page = 1;
            }
            else
            {
                HttpContext.Session["bag_page"] = HttpContext.Session["bag_page"] == null ? 1 : HttpContext.Session["bag_page"];
                bag_page = (int)HttpContext.Session["bag_page"] + (int)change_page;
            }


            try//防止越界
            {
                goods = await goods_ored.Skip(8 * (bag_page - 1)).Take(8).ToListAsync();
                goods_next = await goods_ored.Skip(8 * (bag_page)).Take(1).ToListAsync();
            }
            catch (Exception e)
            {
                goods = await goods_ored.Skip(0).Take(8).ToListAsync();
                goods_next = await goods_ored.Skip(8 * (bag_page)).Take(1).ToListAsync();
            }

            if (goods_next.Count > 0)
            {
                ViewBag.max_page = bag_page + 1;
            }
            else
            {
                ViewBag.max_page = bag_page;
            }


            HttpContext.Session["bag_page"] = bag_page;
            HttpContext.Session["goods"] = goods;

            return View("~/Views/Bag/Index.cshtml",goods);
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



            Goods addGoods = null;
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


