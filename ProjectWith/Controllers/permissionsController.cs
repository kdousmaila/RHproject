using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectWith.Models;
using ProjectWith.Models.ath_windows;

namespace ProjectWith.Controllers
{
    public class permissionsController : Controller
    {
        private baseEntities db = new baseEntities();

        // GET: permission
        public ActionResult Index(string searchLetter, int PageNo = 1)
        {
            string role = CurrentUser.getRole();
          
            var permissions = db.permission.Include(p => p.Employee).Include(p => p.Role);
            if (role != "RH" && role != "DS")
            {
                // Check if access has been granted (RH or DS has clicked the grant access button)
                // Redirect the user to an unauthorized page or action
                    return RedirectToAction("Unauthorized", "Error");
                
            }
            // Si une lettre de recherche est fournie, filtrer les permissions par cette lettre
            permissions = permissions.Where(p => p.Role.id != 3 && p.Role.id != 4);
            if (!string.IsNullOrEmpty(searchLetter))
            {
                // Convertir la lettre de recherche en minuscules
                string searchLower = searchLetter.ToLower();

                // Filtrer les permissions en mémoire après avoir récupéré les données de la base de données
                permissions = permissions
                    .Where(p => p.Employee.Nom.ToLower().StartsWith(searchLower) ||
                                p.Employee.Prenom.ToLower().StartsWith(searchLower) ||
                                 p.Employee.Matricule.ToLower().StartsWith(searchLower));
            }

            // Pagination
            int NoOfRecordsPerPage = 7;
            int NoOfRecordsToSkip = (PageNo - 1) * NoOfRecordsPerPage;

            // Paginer les résultats de la requête
            var permissionsPaged = permissions
                .OrderByDescending(p => p.id) // Par exemple, trier par ID dans l'ordre décroissant
                .Skip(NoOfRecordsToSkip)
                .Take(NoOfRecordsPerPage)
                .ToList();

            int totalCount = permissions.Count();
            int NoOfPages = (int)Math.Ceiling(totalCount / (double)NoOfRecordsPerPage);

            ViewBag.PageNo = PageNo;
            ViewBag.NoOfPages = NoOfPages;
            ViewBag.TotalCount = totalCount;
            foreach (var permission in permissionsPaged)
            {
                permission.Role.libelle = MapLibelleToDisplayValue(permission.Role.libelle);
            }

            return View(permissionsPaged);
        }


   
      
        public ActionResult RedirectToFicheFonction(int id)
        {
            // Retrieve the permission based on the provided id
            var permission = db.permission.FirstOrDefault(p => p.id == id);

            if (permission == null)
            {
                // Handle the case where the permission with the provided id doesn't exist
                return HttpNotFound();
            }

            // Retrieve the corresponding FicheFonction entry based on the MatriculeEmp field matching the permission's MatriculeEmp
            var ficheFonction = db.FicheFonction.FirstOrDefault(ff => ff.Matricule == permission.MatriculeEmp);

            if (ficheFonction == null)
            {
                // Handle the case where the FicheFonction entry corresponding to the permission's MatriculeEmp doesn't exist
                return HttpNotFound();
            }

            // Redirect to the FicheFonction page with the corresponding id
            return RedirectToAction("Details", "FicheFonctions", new { id = ficheFonction.id });
        }

       
        // GET: permission/Edit/5
        public ActionResult Edit(int? id)
        {
            string role = CurrentUser.getRole();

            if (role != "RH" && role != "DS")
            {
                // Check if access has been granted (RH or DS has clicked the grant access button)
               
                    // Redirect the user to an unauthorized page or action
                    return RedirectToAction("Unauthorized", "Error");
                
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            permission permission = db.permission.Find(id);
            if (permission == null)
            {
                return HttpNotFound();
            }
            var roles = db.Role.Where(r => r.id != 3 && r.id != 4).ToList();

            var rolesWithDisplayValues = roles.Select(r => new { r.id, DisplayValue = MapLibelleToDisplayValue(r.libelle) });
            ViewBag.MatriculeEmp = new SelectList(db.Employee, "Matricule", "Nom", permission.MatriculeEmp);
            ViewBag.IdRole = new SelectList(rolesWithDisplayValues, "id", "DisplayValue", permission.IdRole);
            return View(permission);
        }

        // POST: permission/Edit/5

      



        // Create a method to map libelle values to display values
        private string MapLibelleToDisplayValue(string libelle)
        {
            switch (libelle)
            {
                case "EmployeeRh":
                    return "Employé RH";
                case "CHEF":
                    return "Responsable";
                case "Employee":
                    return "Employé";
                // Add more cases as needed
                default:
                    return libelle; // Return the original value if no mapping is found
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,IdRole,MatriculeEmp,num")] permission permission)
        {
            if (ModelState.IsValid)
            {
                var existingPermission = db.permission.Find(permission.id);
                if (existingPermission == null)
                {
                    return HttpNotFound();
                }

                // Check if the original IdRole value is 8
                if (existingPermission.IdRole == 8)
                {
                    // Display a sweet alert error message
                    TempData["ErrorMessage"] = "You cannot update a permission with role ID 8.";
                    return RedirectToAction("Edit", new { id = permission.id });
                }

                // Update other properties and save changes
                existingPermission.IdRole = permission.IdRole;
                existingPermission.MatriculeEmp = permission.MatriculeEmp;
                existingPermission.num = permission.num;

                db.Entry(existingPermission).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var roles = db.Role.Where(r => r.id != 3 && r.id != 4).ToList();
            // Map libelle values to display values for SelectList
            var rolesWithDisplayValues = roles.Select(r => new { r.id, DisplayValue = MapLibelleToDisplayValue(r.libelle) });
            ViewBag.MatriculeEmp = new SelectList(db.Employee, "Matricule", "Nom", permission.MatriculeEmp);
            ViewBag.IdRole = new SelectList(rolesWithDisplayValues, "id", "DisplayValue", permission.IdRole);

           
            return View(permission);
        }




        // GET: permission/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            permission permission = db.permission.Find(id);
            if (permission == null)
            {
                return HttpNotFound();
            }
            return View(permission);
        }

        // POST: permission/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            permission permission = db.permission.Find(id);
            db.permission.Remove(permission);
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
