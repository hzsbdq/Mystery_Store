using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SpongeBob_Mall.DAL;
using SpongeBob_Mall.Models;

namespace SpongeBob_Mall.Controllers
{
    public class GoodsController : Controller
    {
        private MySqlContext db = new MySqlContext();

        // GET: Goods
        public async Task<ActionResult> Index()
        {
            var goods = db.Goods.Include(g => g.Map).Include(g => g.User);
            return View(await goods.ToListAsync());
        }

        // GET: Goods/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Goods goods = await db.Goods.FindAsync(id);
            if (goods == null)
            {
                return HttpNotFound();
            }
            return View(goods);
        }

        // GET: Goods/Create
        public ActionResult Create()
        {
            ViewBag.MapID = new SelectList(db.Maps, "MapId", "Name");
            ViewBag.UserID = new SelectList(db.Users, "UserId", "Username");
            return View();
        }

        // POST: Goods/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "GoodsId,UserID,MapID,Amount,Price,GetDate,ShelvesDate,CollectionTag,State,Location")] Goods goods)
        {
            if (ModelState.IsValid)
            {
                db.Goods.Add(goods);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MapID = new SelectList(db.Maps, "MapId", "Name", goods.MapID);
            ViewBag.UserID = new SelectList(db.Users, "UserId", "Username", goods.UserID);
            return View(goods);
        }

        // GET: Goods/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Goods goods = await db.Goods.FindAsync(id);
            if (goods == null)
            {
                return HttpNotFound();
            }
            ViewBag.MapID = new SelectList(db.Maps, "MapId", "Name", goods.MapID);
            ViewBag.UserID = new SelectList(db.Users, "UserId", "Username", goods.UserID);
            return View(goods);
        }

        // POST: Goods/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "GoodsId,UserID,MapID,Amount,Price,GetDate,ShelvesDate,CollectionTag,State,Location")] Goods goods)
        {
            if (ModelState.IsValid)
            {
                db.Entry(goods).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MapID = new SelectList(db.Maps, "MapId", "Name", goods.MapID);
            ViewBag.UserID = new SelectList(db.Users, "UserId", "Username", goods.UserID);
            return View(goods);
        }

        // GET: Goods/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Goods goods = await db.Goods.FindAsync(id);
            if (goods == null)
            {
                return HttpNotFound();
            }
            return View(goods);
        }

        // POST: Goods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Goods goods = await db.Goods.FindAsync(id);
            db.Goods.Remove(goods);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
