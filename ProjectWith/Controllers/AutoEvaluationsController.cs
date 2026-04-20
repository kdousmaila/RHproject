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
    public class AutoEvaluationsController : Controller
    {
        private baseEntities db = new baseEntities();

        // GET: AutoEvaluations
        public ActionResult Index()
        {
            var autoEvaluations = db.AutoEvaluation.Include(a => a.EntretienAnnuel);
            return View(autoEvaluations.ToList());
        }

        // GET: AutoEvaluations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AutoEvaluation autoEvaluation = db.AutoEvaluation.Find(id);
            if (autoEvaluation == null)
            {
                return HttpNotFound();
            }
            return View(autoEvaluation);
        }

        // GET: AutoEvaluations/Create
        public ActionResult Create()
        {
            ViewBag.idEntretien = new SelectList(db.EntretienAnnuel, "id", "Nom");
            return View();
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
     

        public ActionResult Create([Bind(Include = "id,theme,lieu,duree,idEntretien")] AutoEvaluation autoEvaluation)
        {
            if (ModelState.IsValid)
            {
                db.AutoEvaluation.Add(autoEvaluation);
                db.SaveChanges();

                // Return the newly created object as JSON
                return Json(autoEvaluation);
            }

            ViewBag.idEntretien = new SelectList(db.EntretienAnnuel, "id", "Nom", autoEvaluation.idEntretien);
            return View(autoEvaluation);
        }




        // GET: AutoEvaluations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AutoEvaluation autoEvaluation = db.AutoEvaluation.Find(id);
            if (autoEvaluation == null)
            {
                return HttpNotFound();
            }
            ViewBag.idEntretien = new SelectList(db.EntretienAnnuel, "id", "Nom", autoEvaluation.idEntretien);
            return View(autoEvaluation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,theme,lieu,duree,idEntretien")] AutoEvaluation autoEvaluation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(autoEvaluation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idEntretien = new SelectList(db.EntretienAnnuel, "id", "Nom", autoEvaluation.idEntretien);
            return View(autoEvaluation);
        }

        // GET: AutoEvaluations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AutoEvaluation autoEvaluation = db.AutoEvaluation.Find(id);
            if (autoEvaluation == null)
            {
                return HttpNotFound();
            }
            return View(autoEvaluation);
        }

        // POST: AutoEvaluations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AutoEvaluation autoEvaluation = db.AutoEvaluation.Find(id);
            db.AutoEvaluation.Remove(autoEvaluation);
            db.SaveChanges();
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
