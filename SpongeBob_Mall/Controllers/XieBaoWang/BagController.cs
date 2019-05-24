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
        private int state=0;
        // 获取背包列表

        public async Task<ActionResult> Index()
        {
            List<Goods> goods;
            state = 0;
            await Choose(null,null);
            goods = (List<Goods>)HttpContext.Session["goods"];
            return View(goods);
        }

        //获取已上架商品
        public async Task<ActionResult> ShowShelvesList()
        {
            List<Goods> goods;
            state = 1;
            await Choose(null, null);
            goods = (List<Goods>)HttpContext.Session["goods"];
            return View(goods);
        }

        //选择查看分类
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
                goods_or = db.Goods.Where(b => b.UserID == user.UserId && b.State == state && b.Map.Rare == rare && b.Map.type == type);
            }
            else if (rare != "不限")
            {
                goods_or = db.Goods.Where(b => b.UserID == user.UserId && b.State == state && b.Map.Rare == rare);
            }
            else if (type != "不限")
            {
                goods_or = db.Goods.Where(b => b.UserID == user.UserId && b.State == state && b.Map.type == type);
            }
            else
            {
                goods_or = db.Goods.Where(b => b.UserID == user.UserId && b.State == state);
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


            
            if (time_sort == 0 && price_sort == 0 || time_sort == null && price_sort == null)
            {
                goods_ored = goods_or.OrderBy(b => b.GetDate);
            }
            else
            {
                if (time_sort == 1)
                {
                    goods_ored = goods_or.OrderBy(b => b.GetDate);
                }
                else if (time_sort == 2)
                {
                    goods_ored = goods_or.OrderByDescending(b => b.GetDate);
                }
                else
                {
                    if (price_sort == 1)
                    {
                        goods_ored = goods_or.OrderBy(b => b.Price);
                    }
                    else if (price_sort == 2)
                    {
                        goods_ored = goods_or.OrderByDescending(b => b.Price);
                    }
                    else
                    {
                        goods_ored = goods_or.OrderBy(b => b.GetDate);
                    }
                }
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
        [HttpPost]
        public async Task<ActionResult> Putaway(Models.SelectList selectList)
        {
            var data = new List<Object>();
            int goodsId = 0;
            int goodsPrice = 0;
            if (selectList.text != null)
            {
                string[] dt = selectList.text.Split(',');
                goodsId = int.Parse(dt[0]);
                goodsPrice = int.Parse(dt[1]);
            }
            else
            {
                data.Add(new
                {
                    message = "非法请求"
                });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            Goods goods = null;
            goods = await db.Goods.Where(b => b.GoodsId == goodsId).FirstOrDefaultAsync();
            if (goods == null)
            {
                // 不存在指定物品
                data.Add(new
                {
                    message = "不存在指定物品"
                });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            User user = (User)HttpContext.Session["user"];
            if (goods.UserID != user.UserId)
            {
                // 此物品不属于该用户
                data.Add(new
                {
                    message = "此物品不属于该用户"
                });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            //更新物品状态
            db.Goods.Attach(goods);
            goods.State = 1;
            goods.Price = goodsPrice;
            goods.ShelvesDate = DateTime.Now;
            await db.SaveChangesAsync();
            
            return Redirect("Index");
        }

        //下架物品
        [HttpPost]
        public async Task<ActionResult> DownShop(Models.SelectList selectList)
        {
            var data = new List<Object>();
            int goodsId = 0;
            if (selectList.text != null)
            {
                goodsId = int.Parse(selectList.text);
            }
            else
            {
                data.Add(new
                {
                    message = "非法请求"
                });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            Goods goods = null;
            goods = await db.Goods.Where(b => b.GoodsId == goodsId).FirstOrDefaultAsync();
            if (goods == null)
            {
                // 不存在指定物品
                data.Add(new
                {
                    message = "不存在指定物品"
                });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            User user = (User)HttpContext.Session["user"];
            if (goods.UserID != user.UserId)
            {
                // 此物品不属于该用户
                data.Add(new
                {
                    message = "不能下架别人的商品"
                });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            if (goods.State == 0)
            {
                // 已经处于下架状态
                data.Add(new
                {
                    message = "不要重复下架"
                });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            //更新物品状态
            db.Goods.Attach(goods);
            goods.State = 0;
            goods.Price = 0;
            goods.ShelvesDate = DateTime.Now;
            await db.SaveChangesAsync();

            List<Goods> goodss;
            await ShowShelvesList();
            goodss = (List<Goods>)HttpContext.Session["goods"];
            return View("~/Views/Bag/ShowShelvesList.cshtml", goodss);
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


