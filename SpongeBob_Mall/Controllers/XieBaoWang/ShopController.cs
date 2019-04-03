
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
        public async Task<ActionResult> Index(int? choose,int? sort)
        {
            //IEnumerable
        //XElement
        // XmlDocument
        //List<Goods> goods;
        //int? choose_number;
        //ViewBag.page_number = 1;
        //if (HttpContext.Session["choose_number"] == null)
        //{
        //    choose_number = null;
        //}
        //else
        //{
        //    choose_number = (int?)HttpContext.Session["choose_number"];
        //}


            //if (choose == null && sort == null)
            //{
            //    goods = await db.Goods.SqlQuery("select * from spongebob_mall.goods where state=1 order by shelvesdate limit 0,15").ToListAsync();
            //    return View(goods);
            //}
            //else if (sort == null)
            //{
            //    if (choose_number != null&&choose_number!=0)//之前指定过分类
            //    {
            //        if (choose_number > 999999)//指定过’类型‘分类
            //        {
            //            if (choose_number % 1000000 != 0)//指定过‘稀有度’分类
            //            {
            //                if (choose > 999999)//当前请求指定的是‘类型’分类
            //                {
            //                    choose_number = choose_number % 1000000 + choose;
            //                }
            //                else if (choose == -2)//当前指定的是‘类型不限’
            //                {
            //                    choose_number = choose_number % 1000000;
            //                }
            //                else if(choose == -1)//当前指定的是‘稀有度不限’
            //                {
            //                    choose_number = choose_number / 1000000 * 1000000;
            //                }
            //                else//当前请求指定的是‘稀有度’分类
            //                {
            //                    choose_number = (choose_number / 1000000 * 1000000) + choose;
            //                }
            //            }
            //            else//没有指定过‘稀有度’分类
            //            {
            //                if (choose > 999999)//当前请求指定的是‘类型’分类
            //                {
            //                    choose_number = choose;
            //                }
            //                else if (choose == -2)//当前指定的是‘类型不限’
            //                {
            //                    choose_number = 0;
            //                }
            //                else//当前请求指定的是‘稀有度’分类
            //                {
            //                    choose_number += choose;
            //                }
            //            }
            //        }
            //        else//只指定过‘稀有度’分类
            //        {
            //            if (choose > 999999)//当前请求指定的是‘类型’分类
            //            {
            //                choose_number += choose;
            //            }
            //            else if (choose == -1)//当前指定的是‘稀有度不限’
            //            {
            //                choose_number = 0;
            //            }
            //            else//当前请求指定的是‘稀有度’分类
            //            {
            //                choose_number = choose;
            //            }
            //        }
            //    }
            //    else//没有指定过分类
            //    {
            //        if (choose == -1 || choose == -2)
            //        {
            //            choose_number = 0;
            //        }
            //        else
            //        {
            //            choose_number = choose;
            //        }
            //    }
            //}
            //else // TO DO 排序逻辑
            //{
            //    return null;
            //}
            //int mapid = (int)choose_number;
            //if (mapid > 999999)
            //{
            //    if (mapid % 1000000 == 0)
            //    {
            //        goods = await db.Goods.SqlQuery("select * from spongebob_mall.goods where " +
            //        "floor(mapid/1000000)={0} and state=1 order by shelvesdate limit 0,15", mapid / 1000000).ToListAsync();
            //    }
            //    else
            //    {
            //        goods = await db.Goods.SqlQuery("select * from spongebob_mall.goods where " +
            //        "floor(mapid/10000)={0} and state=1 order by shelvesdate limit 0,15", mapid / 10000).ToListAsync();
            //    }
            //}
            //else if (mapid == 0)
            //{
            //    goods = await db.Goods.SqlQuery("select * from spongebob_mall.goods where state=1 order by shelvesdate limit 0,15").ToListAsync();
            //}
            //else
            //{
            //    goods = await db.Goods.SqlQuery("select * from spongebob_mall.goods where " +
            //        "floor(mapid%1000000/10000)={0} and state=1 order by shelvesdate limit 0,15",mapid/10000).ToListAsync();

            //}
            //HttpContext.Session["choose_number"] = choose_number;
            //return View(goods);

            return View();
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