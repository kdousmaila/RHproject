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
    public class DepartementsController : Controller
    {
        private baseEntities db = new baseEntities();

        // GET: Departements
        public ActionResult Index()
        {
            return View(db.Departement.ToList());
        }
        //k nenzel ala departement  yhezek lel fl page lokhra 
        public ActionResult DepartementLinks()
        {
            var departements = db.Departement.ToList();
            return PartialView("_DepartementLinks", departements);
        }


        // GET: Departements/Details/5
        public ActionResult FicheFonctionList(int departmentId)
        {
            var ficheFonctions = db.FicheFonction.Where(ff => ff.idDepartement == departmentId).ToList();
            return View(ficheFonctions);
        }

        // GET: Departement/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departement departement = db.Departement.Find(id);
            if (departement == null)
            {
                return HttpNotFound();
            }
            return View(departement);
        }

        // GET: Departement/Create
        public ActionResult Create()
        {
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,nomDepartement")] Departement departement)
        {
            if (ModelState.IsValid)
            {
                db.Departement.Add(departement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(departement);
        }

        // GET: Departement/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departement departement = db.Departement.Find(id);
            if (departement == null)
            {
                return HttpNotFound();
            }
            return View(departement);
        }

        // POST: Departement/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,nomDepartement")] Departement departement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(departement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(departement);
        }

        // GET: Departement/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departement departement = db.Departement.Find(id);
            if (departement == null)
            {
                return HttpNotFound();
            }
            return View(departement);
        }

        // POST: Departement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Departement departement = db.Departement.Find(id);
            db.Departement.Remove(departement);
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




