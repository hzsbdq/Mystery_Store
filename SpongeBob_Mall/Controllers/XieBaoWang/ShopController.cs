
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

namespace SpongeBob_Mall.Controllers.XieBaoWang
{
    public class ShopController : Controller
    {

        private readonly MySqlContext db = new MySqlContext();
        // TO DO 查看所有上架物品 根据上架时间排序并分页
        public async Task<ActionResult> Index(int? fl, string choose, int? time_sort, int? price_sort, int? change_page)
        {
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
            if (bag_page < 1)//如果是非法操作，归一
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
            if (price_sort_clone != 0)
            {
                time_sort = 0;
            }

            if ((fl == null || choose == null) && time_sort == null && price_sort == null && change_page == null)
            {
                HttpContext.Session["rare"] = "不限";
                HttpContext.Session["type"] = "不限";
            }
            else if (fl == 1 && time_sort == null && price_sort == null && change_page == null)
            {
                HttpContext.Session["rare"] = choose;
            }
            else if (fl == 2 && time_sort == null && price_sort == null && change_page == null)
            {
                HttpContext.Session["type"] = choose;
            }
            string rare = (string)HttpContext.Session["rare"];
            string type = (string)HttpContext.Session["type"];
            if (rare != "不限" && type != "不限")
            {
                goods_or = db.Goods.Where(b => b.State == 1 && b.Map.Rare == rare && b.Map.type == type);
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

            return View(goods);
    }

        public async Task<ActionResult> ChangePage(int? pageNumber)
        {
            return null;
        }

        // TO DO 查看同一种物品的列表，分页

        // TO DO 通过分类查看物品，分页

        // TO DO 收藏操作

        // TO DO 购买（直接发到背包所以需要判断背包是否还有空间，后期可能要改做一个保存箱）直接跳转到GetGoods的Action
    }
}