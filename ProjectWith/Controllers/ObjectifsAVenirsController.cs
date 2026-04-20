using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectWith.Models;

namespace ProjectWith.Controllers
{
    public class ObjectifsAVenirsController : Controller
    {
        private baseEntities db = new baseEntities();

        // GET: ObjectifsAVenir
        public ActionResult Index()
        {
            var objectifsAVenirs = db.ObjectifsAVenir.Include(o => o.EntretienAnnuel);
            return View(objectifsAVenirs.ToList());
        }

        // GET: ObjectifsAVenir/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObjectifsAVenir objectifsAVenir = db.ObjectifsAVenir.Find(id);
            if (objectifsAVenir == null)
            {
                return HttpNotFound();
            }
            return View(objectifsAVenir);
        }

        // GET: ObjectifsAVenir/Create
        public ActionResult Create()
        {
            ViewBag.idEntretien = new SelectList(db.EntretienAnnuel, "id", "Nom");
            return View();
        }

        // POST: ObjectifsAVenir/Create
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,ObjectifsPrincipeauxSmart,indicateur,ARealiserEnT1__T4,idEntretien")] ObjectifsAVenir objectifsAVenir)
        {
            if (ModelState.IsValid)
            {
                db.ObjectifsAVenir.Add(objectifsAVenir);
                db.SaveChanges();
                return Json(new
                {
                    ObjectifsPrincipeauxSmart = objectifsAVenir.ObjectifsPrincipeauxSmart,
                    indicateur = objectifsAVenir.indicateur,
                    ARealiserEnT1__T4 = objectifsAVenir.ARealiserEnT1__T4,
                    id = objectifsAVenir.id,
                });
            }

            ViewBag.idEntretien = new SelectList(db.EntretienAnnuel, "id", "Nom", objectifsAVenir.idEntretien);
            return View(objectifsAVenir);
        }

        // GET: ObjectifsAVenir/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObjectifsAVenir objectifsAVenir = db.ObjectifsAVenir.Find(id);
            if (objectifsAVenir == null)
            {
                return HttpNotFound();
            }
            ViewBag.idEntretien = new SelectList(db.EntretienAnnuel, "id", "Nom", objectifsAVenir.idEntretien);
            return View(objectifsAVenir);
        }

        // POST: ObjectifsAVenir/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,ObjectifsPrincipeauxSmart,indicateur,ARealiserEnT1__T4,idEntretien")] ObjectifsAVenir objectifsAVenir)
        {
            if (ModelState.IsValid)
            {
                db.Entry(objectifsAVenir).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idEntretien = new SelectList(db.EntretienAnnuel, "id", "Nom", objectifsAVenir.idEntretien);
            return View(objectifsAVenir);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObjectifsAVenir objectifsAVenir = db.ObjectifsAVenir.Find(id);
            if (objectifsAVenir == null)
            {
                return HttpNotFound();
            }
            return View(objectifsAVenir);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ObjectifsAVenir objectifsAVenir = db.ObjectifsAVenir.Find(id);
            if (objectifsAVenir != null)
            {
                db.ObjectifsAVenir.Remove(objectifsAVenir);
                db.SaveChanges();
            }
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
