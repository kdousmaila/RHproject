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
    public class EvaluatinGenN_1Controller : Controller
    {
        private baseEntities db = new baseEntities();

        // GET: EvaluatinGenN_1
        public ActionResult Index()
        {
            var evaluatinGenN_1 = db.EvaluatinGenN_1.Include(e => e.EntretienAnnuel);
            return View(evaluatinGenN_1.ToList());
        }

        // GET: EvaluatinGenN_1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluatinGenN_1 evaluatinGenN_1 = db.EvaluatinGenN_1.Find(id);
            if (evaluatinGenN_1 == null)
            {
                return HttpNotFound();
            }
            return View(evaluatinGenN_1);
        }

        // GET: EvaluatinGenN_1/Create
        public ActionResult Create()
        {
            ViewBag.idEntretien = new SelectList(db.EntretienAnnuel, "id", "Nom");
            return View();

           
        }

        // POST: EvaluatinGenN_1/Create
  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,performance,commentaire,tendance,idEntretien")] EvaluatinGenN_1 evaluatinGenN_1)
        {
            if (ModelState.IsValid)
            {
                db.EvaluatinGenN_1.Add(evaluatinGenN_1);
                db.SaveChanges();
             
                return Json(new
                {
                    Performance = evaluatinGenN_1.performance,
                    commentaire = evaluatinGenN_1.commentaire,
                    tendance = evaluatinGenN_1.tendance,
                    id = evaluatinGenN_1.id,
                });
            }
           

            ViewBag.idEntretien = new SelectList(db.EntretienAnnuel, "id", "Nom", evaluatinGenN_1.idEntretien);
            return View(evaluatinGenN_1);
        }

        // GET: EvaluatinGenN_1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluatinGenN_1 evaluatinGenN_1 = db.EvaluatinGenN_1.Find(id);
            if (evaluatinGenN_1 == null)
            {
                return HttpNotFound();
            }
            ViewBag.idEntretien = new SelectList(db.EntretienAnnuel, "id", "Nom", evaluatinGenN_1.idEntretien);
            return View(evaluatinGenN_1);
        }

        // POST: EvaluatinGenN_1/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,performance,commentaire,tendance,idEntretien")] EvaluatinGenN_1 evaluatinGenN_1)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evaluatinGenN_1).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idEntretien = new SelectList(db.EntretienAnnuel, "id", "Nom", evaluatinGenN_1.idEntretien);
            return View(evaluatinGenN_1);
        }

        // GET: EvaluatinGenN_1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluatinGenN_1 evaluatinGenN_1 = db.EvaluatinGenN_1.Find(id);
            if (evaluatinGenN_1 == null)
            {
                return HttpNotFound();
            }
            return View(evaluatinGenN_1);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EvaluatinGenN_1 evaluatinGenN_1 = db.EvaluatinGenN_1.Find(id);
            if (evaluatinGenN_1 != null)
            {
                db.EvaluatinGenN_1.Remove(evaluatinGenN_1);
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
