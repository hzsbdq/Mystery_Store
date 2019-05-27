
using SpongeBob_Mall.DAL;
using SpongeBob_Mall.Models;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace SpongeBob_Mall.Controllers
{
    public class LoginController : Controller
    {
        private readonly MySqlContext db = new MySqlContext();

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cancellation()
        {
            if (HttpContext.Session["user"] != null)
            {
                HttpContext.Session.Remove("user");
            }
            if(HttpContext.Session["admin"] != null)
            {
                HttpContext.Session.Remove("admin");
            }
            return Redirect("Index");
        }

        public ActionResult MoneyBag()
        {
            return View();
        }

        public ActionResult UserHome()
        {
            
            return View();
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(string username,string password,string repassword)
        {
            // 密码不一致
            if (password != repassword)
            {
                Response.Redirect("Register?errorMSG=4");
                return null;
            }
            // 验证用户名和密码的合法性
            Regex regexUsername = new Regex(@"^[a-zA-Z0-9_-]{4,16}$");
            Regex regexPassword = new Regex(@"^(?=.*\d)(?=.*[a-zA-Z])(?=.*[\W_]).{6,20}$");
            if (!regexUsername.IsMatch(username))
            {
                //用户名非法
                Response.Redirect("Register?errorMSG=1");
                return null;
            }
            if (!regexPassword.IsMatch(password))
            {
                //密码非法
                Response.Redirect("Register?errorMSG=2");
                return null;
            }
            // 查询是否存在相同的用户名
            User user = await db.Users.Where(b => b.Username == username).FirstOrDefaultAsync();
            if (user != null)
            {
                //用户名已存在
                Response.Redirect("Register?errorMSG=3");
                return null;
            }

            // 增加新的用户信息
            User newUser = new User
            {
                Username = username,
                Password = password,
                Name = username,
                Sex = 2,
                Property = 100000
            };
            db.Users.Add(newUser);
            await db.SaveChangesAsync();

            return Redirect("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(string username,string password)
        {

            User user = await db.Users.Where(b => b.Username == username && b.Password == password).FirstOrDefaultAsync();
            if (user == null)
            {
                Response.Redirect("Index?errorMSG=1");
            }
            else
            {
                Admin admin = await db.Admins.Where(b => b.UserId == user.UserId).FirstOrDefaultAsync();
                HttpContext.Session["admin"] = admin;
            }

            HttpContext.Session["user"] = user;

            return View("~/Views/Home/Index.cshtml");
        }

    }
}