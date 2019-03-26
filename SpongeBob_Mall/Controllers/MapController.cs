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
    public class MapController : Controller
    {
        private MySqlContext db = new MySqlContext();

        // GET: Map
        public async Task<ActionResult> Index()
        {
            return View(await db.Maps.ToListAsync());
        }

        // GET: Map/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Map map = await db.Maps.FindAsync(id);
            if (map == null)
            {
                return HttpNotFound();
            }
            return View(map);
        }

        // GET: Map/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Map/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MapId,Name,Rare,Place,Validity,Picture")] Map map)
        {
            if (ModelState.IsValid)
            {
                db.Maps.Add(map);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(map);
        }

        // GET: Map/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Map map = await db.Maps.FindAsync(id);
            if (map == null)
            {
                return HttpNotFound();
            }
            return View(map);
        }

        // POST: Map/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MapId,Name,Rare,Place,Validity,Picture")] Map map)
        {
            if (ModelState.IsValid)
            {
                db.Entry(map).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(map);
        }

        // GET: Map/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Map map = await db.Maps.FindAsync(id);
            if (map == null)
            {
                return HttpNotFound();
            }
            return View(map);
        }

        // POST: Map/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Map map = await db.Maps.FindAsync(id);
            db.Maps.Remove(map);
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
