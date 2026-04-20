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
    public class FormationsController : Controller
    {
        private baseEntities db = new baseEntities();

        // GET: Formation
        public ActionResult Index()
        {
            var formations = db.Formation.Include(f => f.FicheFonction);
            return View(formations.ToList());
        }

        // GET: Formation/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formation formation = db.Formation.Find(id);
            if (formation == null)
            {
                return HttpNotFound();
            }
            return View(formation);
        }

        // GET: Formation/Create
        public ActionResult Create()
        {
            ViewBag.idFiche = new SelectList(db.FicheFonction, "id", "nom");
            return View();
        }

        // POST: Formation/Create
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,nom,dateDebut,dateFin,idFiche,lieu,PDF")] Formation formation, HttpPostedFileBase FormationFile)
        {
            if (FormationFile != null)
            {
                formation.PDF = FormationFile.FileName;
                string path = Server.MapPath("~/Content/formations/" + FormationFile.FileName);
                FormationFile.SaveAs(path);


            }
            if (ModelState.IsValid)
            {
                db.Formation.Add(formation);
                db.SaveChanges();
                return RedirectToAction("Details/" + formation.idFiche, "FicheFonctions");
            }

            ViewBag.idFiche = new SelectList(db.FicheFonction, "id", "nom", formation.idFiche);
            return View(formation);
        }

        // GET: Formation/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formation formation = db.Formation.Find(id);
            if (formation == null)
            {
                return HttpNotFound();
            }
            ViewBag.idFiche = new SelectList(db.FicheFonction, "id", "nom", formation.idFiche);
            return View(formation);
        }

        // POST: Formation/Edit/5
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,nom,dateDebut,dateFin,idFiche,lieu")] Formation formation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(formation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idFiche = new SelectList(db.FicheFonction, "id", "nom", formation.idFiche);
            return View(formation);
        }

        // GET: Formation/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formation formation = db.Formation.Find(id);
            if (formation == null)
            {
                return HttpNotFound();
            }
            return View(formation);
        }

        // POST: Formation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Formation formation = db.Formation.Find(id);
            if (formation == null)
            {
                return HttpNotFound();
            }

            int ficheId = (int)formation.idFiche;

            db.Formation.Remove(formation);
            db.SaveChanges();

            // Rediriger vers l'action Details de FicheFonction avec l'ID de la fiche associée à l'expérience
            return RedirectToAction("Details/" + ficheId, "FicheFonctions");
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
