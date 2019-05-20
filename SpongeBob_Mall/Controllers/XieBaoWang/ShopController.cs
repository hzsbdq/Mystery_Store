
using SpongeBob_Mall.DAL;
using SpongeBob_Mall.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Xml;
using System.Xml.Linq;
using System.Collections;
using System.Web.Script.Serialization;
using System.IO;

namespace SpongeBob_Mall.Controllers.XieBaoWang
{
    public class ShopController : Controller
    {

        private readonly MySqlContext db = new MySqlContext();


        public async Task<ActionResult> Index()
        {
            List<Goods> goods;
            await Choose(null, null);
            goods = (List<Goods>)HttpContext.Session["goods"];
            return View(goods);
        }
        public async Task<ActionResult> Choose(int? fl, string choose)
        {
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
                goods_or = db.Goods.Where(b=>b.State == 1 && b.Map.Rare == rare && b.Map.type == type);
            }
            else if (rare != "不限")
            {
                goods_or = db.Goods.Where(b => b.State == 1 && b.Map.Rare == rare);
            }
            else if (type != "不限")
            {
                goods_or = db.Goods.Where(b => b.State == 1 && b.Map.type == type);
            }
            else
            {
                goods_or = db.Goods.Where(b => b.State == 1);
            }

            HttpContext.Session["goods_or"] = goods_or;

            await Sort(0, 0);

            goods = (List<Goods>)HttpContext.Session["goods"];

            return View("~/Views/Shop/Index.cshtml", goods);
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
                goods_or = db.Goods.Where(b =>b.State == 1);
            }
            else
            {
                goods_or = db.Goods.Where(b =>b.State == 1 && b.Map.Name.Contains(search_value));
            }

            HttpContext.Session["goods_or"] = goods_or;

            await Sort(0, 0);

            goods = (List<Goods>)HttpContext.Session["goods"];

            return View("~/Views/Shop/Index.cshtml", goods);
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

            return View("~/Views/Shop/Index.cshtml", goods);
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

            return View("~/Views/Bag/Index.cshtml", goods);
        }

        // TO DO 收藏操作
        public async Task<ActionResult> Collect(int goodsId)
        {
            return Redirect("index");
        }

        // 购买（直接发到背包所以需要判断背包是否还有空间，后期可能要改做一个保存箱）直接跳转到GetGoods的Action
        [HttpPost]
        public async Task<ActionResult> Pay(Models.SelectList selectList)
        {
            var data = new List<Object>();

            if (selectList.text != null)
            {
                int goodsId = int.Parse(selectList.text);

                Goods goods = null;
                goods = await db.Goods.Where(b => b.GoodsId == goodsId).FirstOrDefaultAsync();
                if (goods == null)
                {
                    //to do 不存在指定物品
                    data.Add(new
                    {
                        message = "不存在指定物品"
                    });
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                User user = (User)HttpContext.Session["user"];
                if (goods.UserID == user.UserId)
                {
                    //to do 不能购买自己的物品
                    data.Add(new
                    {
                        message = "不能购买自己的物品"
                    });
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                if (user.Property < goods.Price)
                {
                    //to do 余额不足
                    data.Add(new
                    {
                        message = "余额不足"
                    });
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                User old_user = goods.User;
                //扣除余额
                db.Users.Attach(user);
                user.Property -= goods.Price;
                //更新物品状态
                db.Goods.Attach(goods);
                goods.UserID = user.UserId;
                goods.State = 0;
                goods.GetDate = DateTime.Now;
                //添加订单信息
                Order neworder = new Order
                {
                    Sell = old_user,
                    Pay = user,
                    Price = goods.Price,
                    Complete_Time = DateTime.Now
                };
                db.Orders.Add(neworder);
                await db.SaveChangesAsync();

            }

            

            return Redirect("index");
        }
    }
}