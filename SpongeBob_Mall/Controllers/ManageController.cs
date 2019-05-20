using SpongeBob_Mall.DAL;
using SpongeBob_Mall.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SpongeBob_Mall.Controllers
{
    public class ManageController : Controller
    {
        private MySqlContext db = new MySqlContext();
        // GET: Manage
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public async Task<ActionResult> ShowMap()
        {
            List<Map> maps;
            await Choose(null, null);
            maps = (List<Map>)HttpContext.Session["maps"];
            return View(maps);
        }
        //选择查看分类
        public async Task<ActionResult> Choose(int? fl, string choose)
        {
            IQueryable<Map> map_or;
            List<Map> maps;

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
                map_or = db.Maps.Where(b => b.Rare == rare && b.type == type);
            }
            else if (rare != "不限")
            {
                map_or = db.Maps.Where(b => b.Rare == rare);
            }
            else if (type != "不限")
            {
                map_or = db.Maps.Where(b => b.type == type);
            }
            else
            {
                map_or = db.Maps.Where(b=> b.MapId!=-1);
            }

            HttpContext.Session["maps_or"] = map_or;

            await Sort(0, 0);

            maps = (List<Map>)HttpContext.Session["maps"];

            return View("~/Views/Admin/Index.cshtml", maps);
        }

        //搜索
        [HttpGet]
        public async Task<ActionResult> Search(string search_value)
        {
            IQueryable<Map> maps_or;

            List<Map> maps;

            if (search_value == null)
            {
                maps_or = db.Maps.Where(b => b.MapId!=-1);
            }
            else
            {
                maps_or = db.Maps.Where(b => b.Name.Contains(search_value));
            }

            HttpContext.Session["maps_or"] = maps_or;

            await Sort(0, 0);

            maps = (List<Map>)HttpContext.Session["maps"];

            return View("~/Views/Admin/Index.cshtml", maps);
        }

        //排序

        public async Task<ActionResult> Sort(int? time_sort, int? price_sort)
        {
            IQueryable<Map> maps_or = (IQueryable<Map>)HttpContext.Session["maps_or"];
            IOrderedQueryable<Map> maps_ored;

            List<Map> maps;



            
            maps_ored = maps_or.OrderBy(b => b.MapId);

            HttpContext.Session["maps_ored"] = maps_ored;

            await ChangePage(0);

            maps = (List<Map>)HttpContext.Session["maps"];

            return View("~/Views/Admin/Index.cshtml", maps);
        }

        //分页
        public async Task<ActionResult> ChangePage(int? change_page)
        {
            List<Map> maps;
            List<Map> maps_next;
            int bag_page;

            IOrderedQueryable<Map> maps_ored = (IOrderedQueryable<Map>)HttpContext.Session["maps_ored"];

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
                maps = await maps_ored.Skip(8 * (bag_page - 1)).Take(8).ToListAsync();
                maps_next = await maps_ored.Skip(8 * (bag_page)).Take(1).ToListAsync();
            }
            catch (Exception e)
            {
                maps = await maps_ored.Skip(0).Take(8).ToListAsync();
                maps_next = await maps_ored.Skip(8 * (bag_page)).Take(1).ToListAsync();
            }

            if (maps_next.Count > 0)
            {
                ViewBag.max_page = bag_page + 1;
            }
            else
            {
                ViewBag.max_page = bag_page;
            }


            HttpContext.Session["bag_page"] = bag_page;
            HttpContext.Session["maps"] = maps;

            return View("~/Views/Admin/Index.cshtml", maps);
        }
        [AllowAnonymous]
        public async Task<ActionResult> AddMap(string name,string rare,string type)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase f = Request.Files["file1"];
                f.SaveAs(Server.MapPath("~/img/sp/"+f.FileName));
                Map map = new Map
                {
                    Name = name,
                    Rare = rare,
                    type = type,
                    Picture = f.FileName
                };
                db.Maps.Add(map);
                await db.SaveChangesAsync();
            }
            
            return Redirect("ShowMap");
        }
    }
}