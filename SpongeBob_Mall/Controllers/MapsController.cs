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
    public class MapsController : Controller
    {
        private MySqlContext db = new MySqlContext();
        private List<Map> maps;
        private MapPaging mapPaging;
        private int pageNumber=8;
        private int bag_page = 1;

        public MapsController()
        {
            mapPaging = new MapPaging(db, pageNumber);
        }

        //显示物品图鉴列表
        [AdminAuthorize]
        public async Task<ActionResult> Index()
        {
            HttpContext.Session["bag_page"] = 1;
            maps = await mapPaging.GetPageByNumberAsync(0);
            SavaMapPaging();
            PageMark();
            return View(maps);
        }
        //选择查看分类
        [AdminAuthorize]
        public async Task<ActionResult> Choose(string rare,string type)
        {
            FindMapPaging();
            if (rare != null)
            {
                HttpContext.Session["rare"] = rare;
            }

            if(type != null)
            {
                HttpContext.Session["type"] = type;
            }
            HttpContext.Session["bag_page"] = 1;
            maps = await mapPaging.Choose(rare,type);
            SavaMapPaging();
            PageMark();
            return View("~/Views/Maps/Index.cshtml", maps);
        }

        //搜索
        [HttpGet]
        [AdminAuthorize]
        public async Task<ActionResult> Search(string search_value)
        {
            FindMapPaging();
            HttpContext.Session["bag_page"] = 1;
            maps = await mapPaging.Search(search_value);
            SavaMapPaging();
            PageMark();
            return View("~/Views/Maps/Index.cshtml", maps);
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

            maps = await mapPaging.GetPageByNumberAsync(bag_page-1);

            PageMark();

            HttpContext.Session["bag_page"] = bag_page;

            return View("~/Views/Maps/Index.cshtml", maps);
        }

        //添加物品图鉴
        [AdminAuthorize]
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
            
            return Redirect("Index");
        }

        public void PageMark()
        {
            if (!mapPaging.HasNextPage)
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
            HttpContext.Session["mapPaging"] = mapPaging;
        }

        public void FindMapPaging()
        {
            if (HttpContext.Session["mapPaging"] != null)
            {
                mapPaging = (MapPaging)HttpContext.Session["mapPaging"];
            }
            else
            {
                mapPaging = new MapPaging(db, pageNumber);
            }
        }
    }
}