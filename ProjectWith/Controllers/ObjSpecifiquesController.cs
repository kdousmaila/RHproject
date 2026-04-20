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
    public class ObjSpecifiquesController : Controller
    {
        private baseEntities db = new baseEntities();

        // GET: ObjSpecifique
        public ActionResult Index()
        {
            var objSpecifiques = db.ObjSpecifique.Include(o => o.EntretienAnnuel);
            return View(objSpecifiques.ToList());
        }

        // GET: ObjSpecifique/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObjSpecifique objSpecifique = db.ObjSpecifique.Find(id);
            if (objSpecifique == null)
            {
                return HttpNotFound();
            }
            return View(objSpecifique);
        }

        // GET: ObjSpecifique/Create
        public ActionResult Create()
        {
            ViewBag.idEntretien = new SelectList(db.EntretienAnnuel, "id", "Nom");
            return View();
        }

        // POST: ObjSpecifique/Create
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,ObjSpecifique1,Synthese,idEntretien")] ObjSpecifique objSpecifique)
        {
            if (ModelState.IsValid)
            {
                db.ObjSpecifique.Add(objSpecifique);
                db.SaveChanges();

                // Return the newly created object as JSON
                return Json(new
                {
                    ObjectifSpecifique = objSpecifique.ObjSpecifique1,
                    Synthese = objSpecifique.Synthese,
                    id = objSpecifique.id,
                });
            }

            ViewBag.idEntretien = new SelectList(db.EntretienAnnuel, "id", "Nom", objSpecifique.idEntretien);
            return View(objSpecifique);
        }



        // GET: ObjSpecifique/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObjSpecifique objSpecifique = db.ObjSpecifique.Find(id);
            if (objSpecifique == null)
            {
                return HttpNotFound();
            }
            ViewBag.idEntretien = new SelectList(db.EntretienAnnuel, "id", "Nom", objSpecifique.idEntretien);
            return View(objSpecifique);
        }

        // POST: ObjSpecifique/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,ObjSpecifique1,Synthese,idEntretien")] ObjSpecifique objSpecifique)
        {
            if (ModelState.IsValid)
            {
                db.Entry(objSpecifique).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idEntretien = new SelectList(db.EntretienAnnuel, "id", "Nom", objSpecifique.idEntretien);
            return View(objSpecifique);
        }

        //GET: ObjSpecifique/Delete/5
        //      public ActionResult Delete(int? id)
        //      {
        //          if (id == null)
        //          {
        //              return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //          }
        //          ObjSpecifique objSpecifique = db.ObjSpecifique.Find(id);
        //          if (objSpecifique == null)
        //          {
        //              return HttpNotFound();
        //          }
        //          return View(objSpecifique);
        //      }




        //      [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //	ObjSpecifique objSpecifique = db.ObjSpecifique.Find(id);
        //	db.ObjSpecifique.Remove(objSpecifique);
        //	db.SaveChanges();
        //	return RedirectToAction("Index");
        //}


        // GET: ObjSpecifique/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObjSpecifique objSpecifique = db.ObjSpecifique.Find(id);
            if (objSpecifique == null)
            {
                return HttpNotFound();
            }
            return View(objSpecifique);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ObjSpecifique objSpecifique = db.ObjSpecifique.Find(id);
            if (objSpecifique != null)
            {
                db.ObjSpecifique.Remove(objSpecifique);
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
