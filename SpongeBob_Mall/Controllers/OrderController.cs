using SpongeBob_Mall.DAL;
using SpongeBob_Mall.Models;
using SpongeBob_Mall.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SpongeBob_Mall.Controllers
{
    public class OrderController : Controller
    {
        private readonly MySqlContext db = new MySqlContext();
        private List<Order> orders;
        private OrderPaging orderPaging;
        private int pageNumber = 8;
        private int bag_page = 1;

        public OrderController()
        {
            orderPaging = new OrderPaging(db, pageNumber);
        }

        //显示所有订单列表
        public async Task<ActionResult> Index()
        {
            orders = await orderPaging.GetPageByNumberAsync(0);
            SavaorderPaging();
            PageMark();
            return View(orders);
        }
        //分页
        public async Task<ActionResult> ChangePage(int? change_page)
        {
            FindorderPaging();

            if (change_page == 0)
            {
                bag_page = 1;
            }
            else
            {
                HttpContext.Session["bag_page"] = HttpContext.Session["bag_page"] == null ? 1 : HttpContext.Session["bag_page"];
                bag_page = (int)HttpContext.Session["bag_page"] + (int)change_page;
            }

            orders = await orderPaging.GetPageByNumberAsync(bag_page - 1);

            PageMark();

            HttpContext.Session["bag_page"] = bag_page;

            return View("~/Views/Order/Index.cshtml", orders);
        }
        public ActionResult ShowOrderByUserId(int userid)
        {
            return View();
        }

        public void PageMark()
        {
            if (!orderPaging.HasNextPage)
            {
                ViewBag.max_page = bag_page;
            }
            else
            {
                ViewBag.max_page = bag_page + 1;
            }
        }

        public void SavaorderPaging()
        {
            HttpContext.Session["bag_page"] = 1;
            HttpContext.Session["orderPaging"] = orderPaging;
        }

        public void FindorderPaging()
        {
            if (HttpContext.Session["orderPaging"] != null)
            {
                orderPaging = (OrderPaging)HttpContext.Session["orderPaging"];
            }
            else
            {
                orderPaging = new OrderPaging(db, pageNumber);
            }
        }
    }
}