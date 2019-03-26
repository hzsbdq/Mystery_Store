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

namespace SpongeBob_Mall.Controllers.XieBaoWang
{
    public class ShopController : Controller
    {

        private readonly MySqlContext db = new MySqlContext();
        // TO DO 查看所有上架物品 根据上架时间排序并分页
        public async Task<ActionResult> Index(int? pageNumber)
        {
            if (pageNumber == null)
            {
                pageNumber = 1;
            }
             List<Goods> goods =  await db.Goods.Where(b => b.State == 1).OrderBy(b => b.ShelvesDate).ToListAsync();
             return View(goods.ToPagedList((int)pageNumber,10));
        }

        // TO DO 查看同一种物品的列表，分页

        // TO DO 通过分类查看物品，分页

        // TO DO 收藏操作

        // TO DO 购买（直接发到背包所以需要判断背包是否还有空间，后期可能要改做一个保存箱）直接跳转到GetGoods的Action
    }
}