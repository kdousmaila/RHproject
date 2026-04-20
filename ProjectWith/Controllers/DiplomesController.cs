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
    public class DiplomesController : Controller
    {
        private baseEntities db = new baseEntities();

        // GET: Diplome
        public ActionResult Index()
        {
            var diplomes = db.Diplome.Include(d => d.FicheFonction);
            return View(diplomes.ToList());
        }

        // GET: Diplome/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Diplome diplome = db.Diplome.Find(id);
            if (diplome == null)
            {
                return HttpNotFound();
            }
            return View(diplome);
        }


        public ActionResult Create()
        {
            ViewBag.idFiche = new SelectList(db.FicheFonction, "id", "nom");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,NomDimplome,DateDebut,idFiche,lieu,DateFin,PDF")] Diplome diplome, HttpPostedFileBase DiplomeFile)
        {
            if (DiplomeFile != null)
            {
                diplome.PDF = DiplomeFile.FileName;
                string path = Server.MapPath("~/Content/diplomes/" + DiplomeFile.FileName);
                DiplomeFile.SaveAs(path);


            }
            if (ModelState.IsValid)
            {

                db.Diplome.Add(diplome);
                db.SaveChanges();
                return RedirectToAction("Details/" + diplome.idFiche, "FicheFonctions");
            }

            ViewBag.idFiche = new SelectList(db.FicheFonction, "id", "nom", diplome.idFiche);
            return View(diplome);
        }



        // GET: Diplome/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Diplome diplome = db.Diplome.Find(id);
            if (diplome == null)
            {
                return HttpNotFound();
            }
            ViewBag.idFiche = new SelectList(db.FicheFonction, "id", "nom", diplome.idFiche);
            return View(diplome);
        }

        // POST: Diplome/Edit/5
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,NomDimplome,DateDebut,idFiche,lieu,DateFin")] Diplome diplome)
        {
            if (ModelState.IsValid)
            {
                db.Entry(diplome).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idFiche = new SelectList(db.FicheFonction, "id", "nom", diplome.idFiche);
            return View(diplome);
        }

        // GET: Diplome/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Diplome diplome = db.Diplome.Find(id);
            if (diplome == null)
            {
                return HttpNotFound();
            }
            return View(diplome);
        }

        // POST: Diplome/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]



        public ActionResult DeleteConfirmed(int id)
        {
            Diplome diplome = db.Diplome.Find(id);
            if (diplome == null)
            {
                return HttpNotFound();
            }

            int ficheId = (int)diplome.idFiche;

            db.Diplome.Remove(diplome);
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
