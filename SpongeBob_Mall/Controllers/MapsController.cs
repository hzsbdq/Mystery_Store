using SpongeBob_Mall.DAL;
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
    public class MapsController : Controller
    {
        private MySqlContext db = new MySqlContext();
        private MapPaging mapPaging;

        public MapsController()
        {
            mapPaging = new MapPaging(db, 8);
        }

        //显示物品图鉴列表
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            HttpContext.Session["bag_page"] = 1;
            return View(await mapPaging.GetPageByNumberAsync(0));
        }
        //选择查看分类
        public async Task<ActionResult> Choose(string rare,string type)
        {

            if (rare != null)
            {
                HttpContext.Session["rare"] = rare;
            }

            if(type != null)
            {
                HttpContext.Session["type"] = type;
            }
            HttpContext.Session["bag_page"] = 1;
            return View("~/Views/Maps/Index.cshtml", await mapPaging.Choose(rare == "不限" ? null : rare, type == "不限" ? null : type));
        }

        //搜索
        [HttpGet]
        public async Task<ActionResult> Search(string search_value)
        {
            HttpContext.Session["bag_page"] = 1;
            return View("~/Views/Maps/Index.cshtml", await mapPaging.Search(search_value));
        }

        //分页
        public async Task<ActionResult> ChangePage(int? change_page)
        {
            List<Map> maps;
            int bag_page;

            if (change_page == 0)
            {
                bag_page = 1;
            }
            else
            {
                HttpContext.Session["bag_page"] = HttpContext.Session["bag_page"] == null ? 1 : HttpContext.Session["bag_page"];
                bag_page = (int)HttpContext.Session["bag_page"] + (int)change_page;
            }

            maps = await mapPaging.GetPageByNumberAsync(bag_page-1);

            HttpContext.Session["bag_page"] = bag_page;

            return View("~/Views/Maps/Index.cshtml", maps);
        }
        
        //添加物品图鉴
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
                    Place = 0,
                    Picture = f.FileName
                };
                db.Maps.Add(map);
                await db.SaveChangesAsync();
            }
            
            return Redirect("ShowMap");
        }
    }
}