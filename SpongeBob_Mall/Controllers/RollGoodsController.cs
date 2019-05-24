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
    public class RollGoodsController : Controller
    {
        private MySqlContext db = new MySqlContext();
        private Roll roll;
        private List<Map> maps;

        public RollGoodsController()
        {
            roll = new Roll(db);
        }
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Roll()
        {
            User user = (User)HttpContext.Session["user"];
            if (user.Property < 1000)
            {
                return Redirect("Index");
            }
            maps = new List<Map>();
            for (int i = 0; i < 10; i++)
            {
                maps.Add(await roll.StartRoll());
            }
            foreach (Map map in maps)
            {
                Goods goods = new Goods
                {
                    UserID = user.UserId,
                    MapID = map.MapId,
                    Amount = 1,
                    Price = 0,
                    GetDate = DateTime.Now,
                    CollectionTag = 0,
                    State = 0,
                    Location = 1
                };
                db.Goods.Add(goods);
            }
            db.Users.Attach(user);
            user.Property -= 1000;
            await db.SaveChangesAsync();
            return View("~/Views/RollGoods/Index.cshtml", maps);
        }
    }
}