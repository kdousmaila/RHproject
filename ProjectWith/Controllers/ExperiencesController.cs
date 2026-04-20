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
    public class ExperiencesController : Controller
    {
        private baseEntities db = new baseEntities();

        // GET: Experience
        public ActionResult Index()
        {
            var experiences = db.Experience.Include(e => e.FicheFonction);
            return View(experiences.ToList());
        }

        // GET: Experience/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Experience experience = db.Experience.Find(id);
            if (experience == null)
            {
                return HttpNotFound();
            }
            return View(experience);
        }

        // GET: Experience/Create
        public ActionResult Create()
        {
            ViewBag.idFiche = new SelectList(db.FicheFonction, "id", "nom");
            return View();
        }

        // POST: Experience/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Titre,dateDebut,dateFin,idFiche,lieu,PDF")] Experience experience, HttpPostedFileBase ExperienceFile)
        {

            if (ExperienceFile != null)
            {
                experience.PDF = ExperienceFile.FileName;
                string path = Server.MapPath("~/Content/experiences/" + ExperienceFile.FileName);
                ExperienceFile.SaveAs(path);


            }

            if (ModelState.IsValid)
            {
                db.Experience.Add(experience);
                db.SaveChanges();
                return RedirectToAction("Details/" + experience.idFiche, "FicheFonctions");
            }

            ViewBag.idFiche = new SelectList(db.FicheFonction, "id", "nom", experience.idFiche);
            return View(experience);
        }

        // GET: Experience/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Experience experience = db.Experience.Find(id);
            if (experience == null)
            {
                return HttpNotFound();
            }
            ViewBag.idFiche = new SelectList(db.FicheFonction, "id", "nom", experience.idFiche);
            return View(experience);
        }

        // POST: Experience/Edit/5
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Titre,dateDebut,dateFin,idFiche,lieu")] Experience experience)
        {
            if (ModelState.IsValid)
            {
                db.Entry(experience).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idFiche = new SelectList(db.FicheFonction, "id", "nom", experience.idFiche);
            return View(experience);
        }

        // GET: Experience/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Experience experience = db.Experience.Find(id);
            if (experience == null)
            {
                return HttpNotFound();
            }
            return View(experience);
        }

        // POST: Experience/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
   
        public ActionResult DeleteConfirmed(int id)
        {
            Experience experience = db.Experience.Find(id);
            if (experience == null)
            {
                return HttpNotFound();
            }

            int ficheId = (int)experience.idFiche;

            db.Experience.Remove(experience);
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
