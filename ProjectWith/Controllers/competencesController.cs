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
    public class competencesController : Controller
    {
        private baseEntities db = new baseEntities();

        // GET: competences
        public ActionResult Index()
        {
            var competences = db.competence.Include(c => c.FicheFonction);
            return View(competences.ToList());
        }

        // GET: competences/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            competence competence = db.competence.Find(id);
            if (competence == null)
            {
                return HttpNotFound();
            }
            return View(competence);
        }

        // GET: competences/Create
        public ActionResult Create()
        {
            ViewBag.idFiche = new SelectList(db.FicheFonction, "id", "nom");
            return View();
        }

        // POST: competences/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,nom,type,niveau,idFiche,PDF")] competence competence, HttpPostedFileBase CompetenceFile)
        {

            if (CompetenceFile != null)
            {
                competence.PDF = CompetenceFile.FileName;
                string path = Server.MapPath("~/Content/competences/" + CompetenceFile.FileName);
                CompetenceFile.SaveAs(path);


            }

            if (ModelState.IsValid)
            {
                db.competence.Add(competence);
                db.SaveChanges();
                return RedirectToAction("Details/" + competence.idFiche, "FicheFonctions");
            }


            ViewBag.idFiche = new SelectList(db.FicheFonction, "id", "nom", competence.idFiche);
            return View(competence);
        }

        // GET: competences/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            competence competence = db.competence.Find(id);
            if (competence == null)
            {
                return HttpNotFound();
            }
            ViewBag.idFiche = new SelectList(db.FicheFonction, "id", "nom", competence.idFiche);
            return View(competence);
        }

        // POST: competences/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,nom,type,niveau,idFiche")] competence competence)
        {
            if (ModelState.IsValid)
            {
                db.Entry(competence).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idFiche = new SelectList(db.FicheFonction, "id", "nom", competence.idFiche);
            return View(competence);
        }

        // GET: competences/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            competence competence = db.competence.Find(id);
            if (competence == null)
            {
                return HttpNotFound();
            }
            return View(competence);
        }

        // POST: competences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
    

        public ActionResult DeleteConfirmed(int id)
        {
            competence competence = db.competence.Find(id);
            if (competence == null)
            {
                return HttpNotFound();
            }

            int ficheId = (int)competence.idFiche;

            db.competence.Remove(competence);
            db.SaveChanges();

            // Rediriger vers l'action Details de FicheFonctions avec l'ID de la fiche associée à l'expérience
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
