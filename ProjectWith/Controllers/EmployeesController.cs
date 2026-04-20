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
    public class EmployeesController : Controller
    {
        private baseEntities db = new baseEntities();

        // GET: Employee
        public ActionResult Index()
        {
            return View(db.Employee.ToList());
        }

        // GET: Employee/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employee.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employee/Create
     
        public ActionResult Create()
        {
            // Vérifier si un employé de type DS existe déjà
            bool isDSCreated = db.permission.Any(p => p.Role.libelle.ToLower().Equals("ds")) && db.permission.Any(p => p.Role.libelle.ToLower().Equals("rh"));

            if (isDSCreated)
            {
                // Rediriger vers une page d'erreur ou toute autre action appropriée
                return RedirectToAction("Unauthorized", "Error"); // Remplacez par votre action et contrôleur désirés
            }

            // Si aucun employé de type DS n'a été créé, afficher la vue de création d'employé normalement
            return View();
        }




        // POST: Employee/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeFicheFonctionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Check if an employee with both roles "DS" and "RH" already exists
                bool isDSCreated = db.permission.Any(p => p.Role.libelle.ToLower().Equals("ds")) && db.permission.Any(p => p.Role.libelle.ToLower().Equals("rh"));

                if (isDSCreated)
                {
                    // Redirect to an error page or any other appropriate action
                    return RedirectToAction("Unauthorized", "Error"); // Replace with your desired action and controller
                }

                // Add the employee
               

                // Determine the role ID based on the selected value
                int roleId;
                string selectedRole = viewModel.SelectedRole.ToLower();

                // Check if the selected role already exists in the database
                bool roleExists = db.permission.Any(p => p.Role.libelle.ToLower() == selectedRole);

                if (roleExists)
                {
                    // Display an error indicating that the role already exists
                    ModelState.AddModelError(string.Empty, $"The role '{viewModel.SelectedRole}' already exists. Only one entry for each role is allowed.");
                    return View(viewModel);
                }
                else
                {
                    // Determine the role ID based on the selected value
                    if (selectedRole == "ds")
                    {
                        roleId = db.Role.Where(r => r.libelle.ToLower().Contains("ds")).Select(r => r.id).FirstOrDefault();
                    }
                    else
                    {
                        roleId = db.Role.Where(r => r.libelle.ToLower() == "rh").Select(r => r.id).FirstOrDefault();
                    }

                    // Create a permission entry with the role ID and employee matricule
                    var permission = new permission
                    {
                        MatriculeEmp = viewModel.Employee.Matricule,
                        IdRole = roleId
                    };
                    db.Employee.Add(viewModel.Employee);
                    db.SaveChanges();

                    db.permission.Add(permission);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }


        // GET: Employee/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employee.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employee/Edit/5
   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Matricule,Nom,Prenom,fonction,Responsable,email,CmptWin,dateEntree,Service,ResponsableFonctionnel")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employee.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Employee employee = db.Employee.Find(id);
            db.Employee.Remove(employee);
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
