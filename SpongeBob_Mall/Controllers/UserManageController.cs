using SpongeBob_Mall.DAL;
using SpongeBob_Mall.Filter;
using SpongeBob_Mall.Models;
using SpongeBob_Mall.Tools;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SpongeBob_Mall.Controllers
{
    public class UserManageController : Controller
    {
        private readonly MySqlContext db = new MySqlContext();
        private List<User> users;
        private UserPaging userPaging;
        private int pageNumber = 8;
        private int bag_page = 1;


        public UserManageController()
        {
            userPaging = new UserPaging(db, pageNumber);
        }

        //显示用户列表
        [AdminAuthorize]
        public async Task<ActionResult> Index()
        {
            users = await userPaging.GetPageByNumberAsync(0);
            SavaMapPaging();
            PageMark();
            return View(users);
        }

        //封禁用户
        [AdminAuthorize]
        public async Task<ActionResult> BanUser(int userid)
        {
            User user = await db.Users.Where(b => b.UserId == userid).FirstOrDefaultAsync();
            db.Users.Attach(user);
            user.Sex = user.Sex==1?2:1;
            await db.SaveChangesAsync();
            return Redirect("Index");
        }
        //分页
        [AdminAuthorize]
        public async Task<ActionResult> ChangePage(int? change_page)
        {
            FindMapPaging();

            if (change_page == 0)
            {
                bag_page = 1;
            }
            else
            {
                HttpContext.Session["bag_page"] = HttpContext.Session["bag_page"] == null ? 1 : HttpContext.Session["bag_page"];
                bag_page = (int)HttpContext.Session["bag_page"] + (int)change_page;
            }

            users = await userPaging.GetPageByNumberAsync(bag_page - 1);

            PageMark();

            HttpContext.Session["bag_page"] = bag_page;

            return View("~/Views/Manage/Index.cshtml", users);
        }
        public void PageMark()
        {
            if (!userPaging.HasNextPage)
            {
                ViewBag.max_page = bag_page;
            }
            else
            {
                ViewBag.max_page = bag_page + 1;
            }
        }

        public void SavaMapPaging()
        {
            HttpContext.Session["bag_page"] = 1;
            HttpContext.Session["userPaging"] = userPaging;
        }
        public void FindMapPaging()
        {
            if (HttpContext.Session["userPaging"] != null)
            {
                userPaging = (UserPaging)HttpContext.Session["userPaging"];
            }
            else
            {
                userPaging = new UserPaging(db, pageNumber);
            }
        }
    }
}