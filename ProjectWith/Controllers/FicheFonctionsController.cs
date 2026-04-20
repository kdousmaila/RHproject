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
    public class FicheFonctionsController : Controller
    {
        private baseEntities db = new baseEntities();

        // GET: /FicheFonction/MaFiche
        public ActionResult MaFiche()
        {

            string role = CurrentUser.getRole();
            if (role != "RH" && role!="DS")
            {
                return View("MaFiche"); // Redirect the user to an unauthorized page or action
                // Replace with your desired action and controller
			}
			else {
                // Your logic to fetch data and pass it to the view
                return RedirectToAction("Unauthorized", "Error");
            }
        }

        public ActionResult MesEmployes(int? idDepartement, string letter, int pageNo = 1)
        {

            string role = CurrentUser.getRole();
            if (role != "CHEF" )
            {
                // Redirect the user to an unauthorized page or action
                return RedirectToAction("Unauthorized", "Error"); // Replace with your desired action and controller
            }
            var currentUserMatricule = ProjectWith.Models.ath_windows.CurrentUser.getMatricule();

            var entretienAnnuelNotes = db.EntretienAnnuel
                                        .Where(e => e.Note != null) // Filter out null values
                                        .GroupBy(e => e.Matricule)
                                        .ToDictionary(
                                            g => g.Key,
                                            g => Math.Round(g.Average(e => e.Note.Value), 2) // Round average note to two decimal places
                                        );

            ViewBag.EntretienAnnuelNotes = entretienAnnuelNotes;

            // Fetch the employee records
            IQueryable<FicheFonction> ficheFonctions = db.FicheFonction;

            // Filter by current user's matricule
            ficheFonctions = ficheFonctions.Where(f => f.ResponsableFonctionnel == currentUserMatricule);

            // Filter by letter if specified
            if (!string.IsNullOrEmpty(letter))
            {
                string searchLower = letter.ToLower();
                ficheFonctions = ficheFonctions.Where(f => f.nom.ToLower().StartsWith(searchLower) || f.prenom.ToLower().StartsWith(searchLower) || f.Matricule.ToLower().StartsWith(searchLower));
            }

            // Filter by department if specified
            if (idDepartement != null)
            {
                ficheFonctions = ficheFonctions.Where(f => f.idDepartement == idDepartement);
            }

            // Pagination
            int noOfRecordsPerPage = 9;
            int totalCount = ficheFonctions.Count();
            int noOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalCount) / Convert.ToDouble(noOfRecordsPerPage)));
            int noOfRecordsToSkip = (pageNo - 1) * noOfRecordsPerPage;

            var ficheFonctionsPaged = ficheFonctions
                                      .OrderByDescending(f => f.dateRecrutement) // Order by descending recruitment date
                                      .ThenByDescending(f => f.id) // Order by descending ID (or another unique property)
                                      .Skip(noOfRecordsToSkip)
                                      .Take(noOfRecordsPerPage)
                                      .ToList();

            // Pass pagination data to the view
            ViewBag.PageNo = pageNo;
            ViewBag.NoOfPages = noOfPages;
            ViewBag.TotalCount = totalCount;

            // Return the view with paginated data
            return View(ficheFonctionsPaged);
        }



       




        public ActionResult Index(int? idDepartement, string letter, int pageNo = 1)
        {
            string role = CurrentUser.getRole();
            if (role != "RH" && role != "DS" && role != "EmployeRh")
            {
                // Redirect the user to an unauthorized page or action
                return RedirectToAction("Unauthorized", "Error"); // Replace with your desired action and controller
            }

            // Récupérer les données des entretiens annuels avec les notes moyennes
            var entretienAnnuelNotes = db.EntretienAnnuel
                                        .Where(e => e.Note != null) // Filtrer les valeurs nulles
                                        .GroupBy(e => e.Matricule)
                                        .ToDictionary(
                                            g => g.Key,
                                            g => Math.Round(g.Average(e => e.Note.Value), 2) // Arrondir la note moyenne à deux décimales
                                        );

            ViewBag.EntretienAnnuelNotes = entretienAnnuelNotes;

            // Récupérer les fiches de fonction
            IQueryable<FicheFonction> ficheFonctions = db.FicheFonction;

            // Filtrer par département si spécifié
            if (idDepartement != null)
            {
                ficheFonctions = ficheFonctions.Where(f => f.idDepartement == idDepartement);
            }

            // Filtrer par lettre si spécifiée
            if (!string.IsNullOrEmpty(letter))
            {
                // Utiliser une comparaison insensible à la casse avec ToLower()
                string searchLower = letter.ToLower();
                ficheFonctions = ficheFonctions.Where(f => f.nom.ToLower().StartsWith(searchLower)
                                                        || f.prenom.ToLower().StartsWith(searchLower)
                                                        || f.Matricule.ToLower().StartsWith(searchLower));
            }

            // Pagination
            int noOfRecordsPerPage = 9;
            int totalCount = ficheFonctions.Count();
            int noOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalCount) / Convert.ToDouble(noOfRecordsPerPage)));
            int noOfRecordsToSkip = (pageNo - 1) * noOfRecordsPerPage;

            var ficheFonctionsPaged = ficheFonctions
                                      .OrderByDescending(f => f.dateRecrutement) // Tri par ordre décroissant de la date de recrutement
                                      .ThenByDescending(f => f.id) // Tri par ordre décroissant de l'ID (ou une autre propriété distincte)
                                      .Skip(noOfRecordsToSkip)
                                      .Take(noOfRecordsPerPage)
                                      .ToList();

            string departmentName = "";

            if (idDepartement != null)
            {
                // Assuming you have a Departments table in your database
                var department = db.Departement.FirstOrDefault(d => d.id == idDepartement);
                if (department != null)
                {
                    departmentName = department.nomDepartement;
                    ViewBag.CurrentDepartmentName = departmentName;
                }
            }

            // Pass the department name to the view
            // Passer les données de pagination à la vue
            ViewBag.PageNo = pageNo;
            ViewBag.NoOfPages = noOfPages;
            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentDepartment = idDepartement;

            // Retourner la vue avec les données paginées
            return View(ficheFonctionsPaged);
        }




        // GET: FicheFonction/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string role = CurrentUser.getRole();
            if (role != "RH" && role != "DS" && role != "EmployeRh")
            {
                // Redirect the user to an unauthorized page or action
                return RedirectToAction("Unauthorized", "Error"); // Replace with your desired action and controller
            }

            var viewModel = new ViewModel();
            viewModel.FicheFonctions = db.FicheFonction.Find(id);
            viewModel.Diplomes = db.Diplome.Where(d => d.id == id).ToList();
            viewModel.Formations = db.Formation.Where(d => d.id == id).ToList();
            viewModel.Experiences = db.Experience.Where(d => d.id == id).ToList();

            viewModel.Competences = db.competence.Where(d => d.id == id).ToList();

            var targetRoles = new List<string> { "CHEF", "DS", "RH" };

            // Initialiser ViewBag.EmployeesWithRoleChef

            var employeesWithRoleChef = db.permission
                .Join(db.Role, p => p.IdRole, r => r.id, (p, r) => new { Permission = p, Role = r })
                .Where(pr => targetRoles.Contains(pr.Role.libelle))
                .Select(pr => pr.Permission.MatriculeEmp)
                .Distinct()
                .ToList();

            ViewBag.EmployeesWithRoleChef = new SelectList(employeesWithRoleChef, viewModel.FicheFonctions.ResponsableFonctionnel);
            return View(viewModel);
        }
        // GET: FicheFonction/Create
        public ActionResult Create()
        {
            // Get the role of the current user
            string role = CurrentUser.getRole();

            // Populate the view model with necessary data
            var viewModel = new EmployeeFicheFonctionViewModel
            {
                Departements = db.Departement.ToList(), // Assuming Departements is a DbSet in your DbContext
                EmployeeDS = GetEmployeeWithRoleDS() // Populate EmployeeDS property
            };

            // Get the list of employees with the role "CHEF"
            var targetRoles = new List<string> { "CHEF", "DS", "RH" };

            // Initialiser ViewBag.EmployeesWithRoleChef
            var employeesWithRoleChef = db.permission
                .Join(db.Role, p => p.IdRole, r => r.id, (p, r) => new { Permission = p, Role = r })
                .Where(pr => targetRoles.Contains(pr.Role.libelle))
                .Select(pr => pr.Permission.MatriculeEmp)
                .Distinct()
                .ToList();
            ViewBag.EmployeesWithRoleChef = new SelectList(employeesWithRoleChef);

           

            // Check if the user has the required role to access this action
            if (role != "RH" && role != "DS" && role != "EmployeRh")
            {
                // Redirect the user to an unauthorized page or action
                return RedirectToAction("Unauthorized", "Error"); // Replace with your desired action and controller
            }

            return View(viewModel);
        }

        private string GetEmployeeWithRoleDS()
        {
            using (var db = new baseEntities())
            {
                var roleDS = (from u in db.Employee
                              join ru in db.permission on u.Matricule equals ru.MatriculeEmp
                              join sn in db.Role on ru.IdRole equals sn.id
                              where sn.libelle.Contains("DS")
                              select new { u.Nom, u.Prenom }).FirstOrDefault();

                if (roleDS != null)
                {
                    return $"{roleDS.Nom} {roleDS.Prenom}";
                }

                return string.Empty; // Return an empty string if no employee with role "DS" is found
            }
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeFicheFonctionViewModel viewModel, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {

                var existingFicheFonction = db.FicheFonction.FirstOrDefault(ff => ff.idDepartement == viewModel.FicheFonction.idDepartement && ff.ResponsableFonctionnel == "DS");
                if (existingFicheFonction != null)
                {
                    ModelState.AddModelError("", "A FicheFonction with a DS as functional manager already exists for this department.");
                    return View(viewModel);
                }

                if (MatriculeExists(viewModel.Employee.Matricule))
                {
                    ModelState.AddModelError("Employee.Matricule", "Matricule already exists.");
                    return View(viewModel);
                }

                if (db.Employee.Any(e => e.Matricule == viewModel.Employee.Matricule))
                {
                    ModelState.AddModelError("Employee.Matricule", "Matricule already exists.");
                    return View(viewModel);
                }
                // Save image file if provided
                if (imageFile != null)
                {
                    viewModel.FicheFonction.image = imageFile.FileName;
                    viewModel.Employee.image = imageFile.FileName;
                    string path = Server.MapPath("~/Content/image/" + imageFile.FileName);
                    imageFile.SaveAs(path);

                }
                var targetRoles = new List<string> { "CHEF", "DS", "RH" };

                // Initialiser ViewBag.EmployeesWithRoleChef
                var employeesWithRoleChef = db.permission
                    .Join(db.Role, p => p.IdRole, r => r.id, (p, r) => new { Permission = p, Role = r })
                    .Where(pr => targetRoles.Contains(pr.Role.libelle))
                    .Select(pr => pr.Permission.MatriculeEmp)
                    .Distinct()
                    .ToList();


                viewModel.FicheFonction.ResponsableFonctionnel = viewModel.SelectedEmployeeChef;

                // Synchronize Nom, Prenom, and Email values between Employee and FicheFonction
                viewModel.Employee.Nom = viewModel.FicheFonction.nom;
                viewModel.Employee.Prenom = viewModel.FicheFonction.prenom;
                viewModel.Employee.email = viewModel.FicheFonction.email;
                viewModel.Employee.ResponsableFonctionnel = viewModel.FicheFonction.ResponsableFonctionnel;

                viewModel.Employee.CmptWin = viewModel.FicheFonction.cmptWindows;
                string fonctionPosteValue = viewModel.Employee.fonction;
                viewModel.Employee.fonction = fonctionPosteValue;
                viewModel.FicheFonction.poste = fonctionPosteValue;
                var departmentName = db.Departement
           .Where(d => d.id == viewModel.FicheFonction.idDepartement)
           .Select(d => d.nomDepartement)
           .FirstOrDefault();

                // Set the service of the employee
                viewModel.Employee.Service = departmentName;

                DateTime? dateE= viewModel.FicheFonction.dateRecrutement;
                viewModel.FicheFonction.dateRecrutement = dateE;
                viewModel.Employee.dateEntree = dateE;







                // Add the employee to the database
                db.Employee.Add(viewModel.Employee);
                db.SaveChanges(); // Save changes to generate Matricule
                var roleId = db.Role.Where(r => r.libelle.ToLower().Contains("emp")).Select(r => r.id).FirstOrDefault();
                if (viewModel.Employee.Service== "Ressources humaines") 
                {
                    roleId = db.Role.Where(r => r.libelle.ToLower().Contains("employeerh")).Select(r => r.id).FirstOrDefault();
                  
				}
                var responsableFonctionnelIdRole = db.permission
    .Where(p => p.MatriculeEmp == viewModel.Employee.ResponsableFonctionnel)
    .Select(p => p.IdRole)
    .FirstOrDefault();

                if (responsableFonctionnelIdRole == 3)
                {
                    // Si l'IdRole est 3, récupérer l'id du rôle dont le libelle contient "employeerh"
                    roleId = db.Role
                        .Where(r => r.libelle.ToLower().Contains("employeerh"))
                        .Select(r => r.id)
                        .FirstOrDefault();
                }

                var permission = new permission
                {
                    MatriculeEmp = viewModel.Employee.Matricule,
                    IdRole = roleId // Assuming role ID for "employee" is 6
                };

                db.permission.Add(permission);
                db.SaveChanges();

                // Assign Matricule to FicheFonction
                viewModel.FicheFonction.Matricule = viewModel.Employee.Matricule;

                // Add fiche fonction to the database
                db.FicheFonction.Add(viewModel.FicheFonction);
                db.SaveChanges();
                int newFicheFonctionDepartmentId = viewModel.FicheFonction.idDepartement;

                // Redirect to the Index action with the department ID parameter
                return RedirectToAction("Index", new { idDepartement = newFicheFonctionDepartmentId });
               
            }

            // If ModelState is not valid, return the view with validation errors
            return View(viewModel);
        }

        private bool MatriculeExists(string matricule)
        {
            return db.FicheFonction.Any(f => f.Matricule == matricule) || db.DeletedFicheFonctions.Any(df => df.Matricule == matricule);
        }

        private bool EmailExists(string email)
        {
            return db.FicheFonction.Any(f => f.email == email) || db.DeletedFicheFonctions.Any(df => df.email == email);
        }

        private bool WinExists(string win)
        {
            return db.Employee.Any(f => f.CmptWin == win) || db.DeletedEmployees.Any(df => df.CmptWin == win);
        }

        [HttpGet]

        public JsonResult CheckMatricule(string matricule)
        {
            bool existsInFicheFonction = db.FicheFonction.Any(f => f.Matricule == matricule);
            bool existsInDeletedFicheFonction = db.DeletedFicheFonctions.Any(df => df.Matricule == matricule);

            bool exists = existsInFicheFonction || existsInDeletedFicheFonction;

            return Json(new { exists = exists }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]

        public JsonResult CheckEmail(string email)
        {
            bool existsInFicheFonction = db.FicheFonction.Any(f => f.email == email);
            bool existsInDeletedFicheFonction = db.DeletedFicheFonctions.Any(df => df.email == email);

            bool exists = existsInFicheFonction || existsInDeletedFicheFonction;

            return Json(new { exists = exists }, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public JsonResult CheckWin(string win)
        {
            bool existsInEmployee = db.FicheFonction.Any(f => f.cmptWindows == win);
            bool existsInDeletedEmployee = db.DeletedEmployees.Any(df => df.CmptWin == win);

            bool exists = existsInEmployee || existsInDeletedEmployee;

            return Json(new { exists = exists }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult ChecknumCin(int numCin)
        {
            bool existsInEmployee = db.FicheFonction.Any(f => f.numCin == numCin);
            //bool existsInDeletedEmployee = db.DeletedEmployees.Any(df => df. == numCin);

            bool exists = existsInEmployee /*|| existsInDeletedEmployee*/;

            return Json(new { exists = exists }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult ChecknNumero(int numero)
        {
            bool existsInFicheFonction = db.FicheFonction.Any(f => f.numero == numero);
            bool existsInDeletedFicheFonction = db.DeletedFicheFonctions.Any(df => df.numero == numero);

            bool exists = existsInFicheFonction || existsInDeletedFicheFonction;

            return Json(new { exists = exists }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ChecknOtherNumero(int numero, int currentRecordId)
        {
            bool existsInFicheFonction = db.FicheFonction.Any(f => f.numero == numero && f.id != currentRecordId);
            bool existsInDeletedFicheFonction = db.DeletedFicheFonctions.Any(df => df.numero == numero && df.Id != currentRecordId);

            bool exists = existsInFicheFonction || existsInDeletedFicheFonction;

            return Json(new { exists = exists }, JsonRequestBehavior.AllowGet);
        }

       
      

        // GET: FicheFonction/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FicheFonction ficheFonction = db.FicheFonction.Find(id);
            if (ficheFonction == null)
            {
                return HttpNotFound();
            }
            var targetRoles = new List<string> { "CHEF", "DS", "RH" };

            // Initialiser ViewBag.EmployeesWithRoleChef

            var employeesWithRoleChef = db.permission
                .Join(db.Role, p => p.IdRole, r => r.id, (p, r) => new { Permission = p, Role = r })
                .Where(pr => targetRoles.Contains(pr.Role.libelle))
                .Select(pr => pr.Permission.MatriculeEmp)
                .Distinct()
                .ToList();

            ViewBag.EmployeesWithRoleChef = new SelectList(employeesWithRoleChef);


            return View(ficheFonction);
        }



        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }




        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,nom,prenom,email,ville,numero,etatCivil,numCin,dateNaissance, poste, dateRecrutement,Matricule, image, idDepartement , cmptWindows , ResponsableFonctionnel")] FicheFonction ficheFonctionsSS, HttpPostedFileBase imageFile, string ResponsableFonctionnel)
        {
            var fiche = db.FicheFonction.Find(ficheFonctionsSS.id);

            if (fiche == null)
            {
                return Json("Fiche non trouvée", JsonRequestBehavior.AllowGet);
            }

            if (!IsValidEmail(ficheFonctionsSS.email))
            {
                ModelState.AddModelError("FicheFonction.email", "L'adresse email n'est pas valide.");
            }

            if (ModelState.IsValid)
            {
                var employee = db.Employee.FirstOrDefault(e => e.Matricule == ficheFonctionsSS.Matricule);

                if (employee != null)
                {
                    employee.Nom = ficheFonctionsSS.nom;
                    employee.Prenom = ficheFonctionsSS.prenom;
                    employee.email = ficheFonctionsSS.email;
                    employee.fonction = ficheFonctionsSS.poste;
                    employee.ResponsableFonctionnel = ficheFonctionsSS.ResponsableFonctionnel;

                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges(); // Save changes to the employee
                }

                // Update the FicheFonction entity
                fiche.nom = ficheFonctionsSS.nom;
                fiche.prenom = ficheFonctionsSS.prenom;
                fiche.email = ficheFonctionsSS.email;
                fiche.ville = ficheFonctionsSS.ville;
                fiche.numero = ficheFonctionsSS.numero;
                fiche.etatCivil = ficheFonctionsSS.etatCivil;
                fiche.numCin = ficheFonctionsSS.numCin;
                fiche.dateNaissance = ficheFonctionsSS.dateNaissance;
                fiche.poste = ficheFonctionsSS.poste;
                fiche.dateRecrutement = ficheFonctionsSS.dateRecrutement;
                fiche.Matricule = ficheFonctionsSS.Matricule;
                fiche.ResponsableFonctionnel = ResponsableFonctionnel;

                if (imageFile != null)
                {
                    // Save the new image for FicheFonction
                    fiche.image = imageFile.FileName;
                    employee.image = imageFile.FileName;

                    string path = Server.MapPath("~/Content/image/" + imageFile.FileName);
                    imageFile.SaveAs(path);
                }

                // Mark the fiche entity as modified
                db.Entry(fiche).State = EntityState.Modified;

                // Save changes to the database
                db.SaveChanges();

                return Json("Success", JsonRequestBehavior.AllowGet);
            }

            var targetRoles = new List<string> { "CHEF", "DS", "RH" };

            // Initialiser ViewBag.EmployeesWithRoleChef
            var employeesWithRoleChef = db.permission
                .Join(db.Role, p => p.IdRole, r => r.id, (p, r) => new { Permission = p, Role = r })
                .Where(pr => targetRoles.Contains(pr.Role.libelle))
                .Select(pr => pr.Permission.MatriculeEmp)
                .Distinct()
                .ToList();

            ViewBag.EmployeesWithRoleChef = new SelectList(employeesWithRoleChef);
            
            // Return validation errors if the model state is not valid
            return Json("Erreur de validation du modèle", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIsResponsableFonctionnel(int? id)
        {
            // Logic to get isResponsableFonctionnel
            // For example:
            FicheFonction ficheFonction = db.FicheFonction.Find(id);
            if (ficheFonction == null)
            {
                return HttpNotFound();
            }

            bool isResponsableFonctionnel = db.FicheFonction.Any(f => f.ResponsableFonctionnel == ficheFonction.Matricule && f.id != ficheFonction.id);

            ViewBag.IsResponsableFonctionnel = isResponsableFonctionnel;
            return Json(new { isResponsableFonctionnel = isResponsableFonctionnel }, JsonRequestBehavior.AllowGet);

        }

        // GET: FicheFonction/Delete/5

        public ActionResult Delete(int? id, string reason, int? idDepartement, int pageNo = 1)
        {
            string currentUserMatricule = CurrentUser.getMatricule();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            FicheFonction ficheFonction = db.FicheFonction.Find(id);
            if (ficheFonction == null)
            {
                return HttpNotFound();
            }

            int newFicheFonctionDepartmentId = ficheFonction.idDepartement;

            // Redirect to the Index action with the department ID parameter
        

            var isResponsableFonctionnel = db.FicheFonction.Any(f => f.ResponsableFonctionnel == ficheFonction.Matricule && f.id != ficheFonction.id);

            ViewBag.IsResponsableFonctionnel = isResponsableFonctionnel;

            // Check if the employee is a responsible functionary in any other records
            if (isResponsableFonctionnel)
            {
                // Return some indication that the employee is a responsible functionary in other records
                return RedirectToAction("Index", new { idDepartement = newFicheFonctionDepartmentId });

            }
            else
            {
                var entretienAnnuelToDelete = db.EntretienAnnuel.Where(e => e.Matricule == ficheFonction.Matricule).ToList();

                foreach (var entretien in entretienAnnuelToDelete)
                {
                    // Register in DeletedEntretienAnnuel table if needed
                    DeletedEntretienAnnuel deletedEntretien = new DeletedEntretienAnnuel
                    {
                        // Copy necessary attributes from entretien to deletedEntretien
                        Nom = entretien.Nom,
                        Prenom = entretien.Prenom,
                        LibelleFonction = entretien.LibelleFonction,
                        DateEntree = entretien.DateEntree,
                        DateDerniereRevue = entretien.DateDerniereRevue,
                        Service = entretien.Service,
                        NomRespHierarchiqueN_1 = entretien.NomRespHierarchiqueN_1,
                        NomRespFonctionel = entretien.NomRespFonctionel,
                        DateEntretien = entretien.DateEntretien,
                        autoEvalSal1 = entretien.autoEvalSal1,
                        competenceCaractUtiles = entretien.competenceCaractUtiles,
                        competenceCaractAcquerir = entretien.competenceCaractAcquerir,
                        question1 = entretien.question1,
                        orientationResulttat = entretien.orientationResulttat,
                        agilite_Adaptabilite = entretien.agilite_Adaptabilite,
                        ouvertureEsprit = entretien.ouvertureEsprit,
                        qualitesRelationnelles = entretien.qualitesRelationnelles,
                        travailEquipe = entretien.travailEquipe,
                        ResolutionProbleme = entretien.ResolutionProbleme,
                        gestionProjet = entretien.gestionProjet,
                        communication = entretien.communication,
                        Leadership = entretien.Leadership,
                        gestionBudget = entretien.gestionBudget,
                        charisme = entretien.charisme,
                        maitriseSoi = entretien.maitriseSoi,
                        ResultatActFormation = entretien.ResultatActFormation,
                        FormationMesureDev = entretien.FormationMesureDev,
                        ResourceNecessaire = entretien.ResourceNecessaire,
                        comntSouhaitEvolution = entretien.comntSouhaitEvolution,
                        comntSouhaitMobilite = entretien.comntSouhaitMobilite,
                        comntProchaineEtape = entretien.comntProchaineEtape,
                        dateProchainEnretien = entretien.dateProchainEnretien,
                        comntGenereaux = entretien.comntGenereaux,
                        Signature1RespN_1 = entretien.Signature1RespN_1,
                        date1 = entretien.date1,
                        comntSalaire = entretien.comntSalaire,
                        signature2Salarie = entretien.signature2Salarie,
                        date2 = entretien.date2,
                        AvisN_2 = entretien.AvisN_2,
                        NomSignatureN_2 = entretien.NomSignatureN_2,
                        date3 = entretien.date3,
                        AvisRespFonctionel = entretien.AvisRespFonctionel,
                        NomSignatureResFonct_Depart = entretien.NomSignatureResFonct_Depart,
                        date4 = entretien.date4,
                        comOrientationResulttat = entretien.comOrientationResulttat,
                        ComAgilite = entretien.ComAgilite,
                        comOuv = entretien.comOuv,
                        comQualité = entretien.comQualité,
                        comEquipe = entretien.comEquipe,
                        comResolP = entretien.comResolP,
                        comGP = entretien.comGP,
                        comCommunication = entretien.comCommunication,
                        comLeadership = entretien.comLeadership,
                        comGBud = entretien.comGBud,
                        comMaitriseSoi = entretien.comMaitriseSoi,
                        comCharisme = entretien.comCharisme,
                        c1 = entretien.c1,
                        c2 = entretien.c2,
                        c3 = entretien.c3,
                        c4 = entretien.c4,
                        comC1 = entretien.comC1,
                        comC2 = entretien.comC2,
                        comC3 = entretien.comC3,
                        comC4 = entretien.comC4,
                        Matricule = entretien.Matricule,
                        Note = entretien.Note,
                        deletedId = entretien.id,
                        checkC1 = entretien.checkC1,
                        checkC2 = entretien.checkC2,
                        checkC3 = entretien.checkC3,
                        checkC4 = entretien.checkC4,
                        status1=entretien.status1,
                        status2=entretien.status2,
                        status3=entretien.status3,
                        dateCreation=entretien.dateCreation,
                        dateValidation=entretien.dateValidation,
                        dateEnvoi=entretien.dateEnvoi,



                    };
                    db.DeletedEntretienAnnuel.Add(deletedEntretien);
                    db.SaveChanges();
                    int deletedEntretienId = deletedEntretien.id;

                    var objSpecifiqueToDelete = db.ObjSpecifique.Where(o => o.idEntretien == entretien.id).ToList();

                    foreach (var objSpecifique in objSpecifiqueToDelete)
                    {
                        // Register in DeletedObjSpecifique table if needed
                        deletedObjSpecifique deletedObjSpecifique = new deletedObjSpecifique
                        {

                            ObjSpecifique = objSpecifique.ObjSpecifique1,
                            Synthese = objSpecifique.Synthese,
                            idEntretien = objSpecifique.idEntretien,
                            idDeletedEntretien = deletedEntretienId
                        };
                        db.deletedObjSpecifique.Add(deletedObjSpecifique);
                        db.SaveChanges();

                        // Now delete the ObjSpecifique entry itself
                        db.ObjSpecifique.Remove(objSpecifique);
                    }


                    var autoEvalToDelete = db.AutoEvaluation.Where(o => o.idEntretien == entretien.id).ToList();

                    foreach (var autoEval in autoEvalToDelete)
                    {
                        // Register in DeletedObjSpecifique table if needed
                        deletedAutoEvaluation deletedAutoEvaluation = new deletedAutoEvaluation
                        {

                            theme = autoEval.theme,
                            lieu = autoEval.lieu,
                            duree = autoEval.duree,
                            idEntretien = autoEval.idEntretien,
                            idDeletedEntretien = deletedEntretienId
                        };
                        db.deletedAutoEvaluation.Add(deletedAutoEvaluation);
                        db.SaveChanges();

                        // Now delete the ObjSpecifique entry itself
                        db.AutoEvaluation.Remove(autoEval);
                    }

                    var evalGenToDel = db.EvaluatinGenN_1.Where(o => o.idEntretien == entretien.id).ToList();

                    foreach (var ev in evalGenToDel)
                    {
                        // Register in DeletedObjSpecifique table if needed
                        deletedEvalGen deletedEvalGen = new deletedEvalGen
                        {

                            performance = ev.performance,
                            commentaire = ev.commentaire,
                            tendance = ev.tendance,
                            idEntretien = ev.idEntretien,
                            idDeletedEntretien = deletedEntretienId
                        };
                        db.deletedEvalGen.Add(deletedEvalGen);
                        db.SaveChanges();

                        // Now delete the ObjSpecifique entry itself
                        db.EvaluatinGenN_1.Remove(ev);
                    }



                    var objVToDelete = db.ObjectifsAVenir.Where(o => o.idEntretien == entretien.id).ToList();

                    foreach (var objV in objVToDelete)
                    {
                        // Register in DeletedObjSpecifique table if needed
                        deletedObjV deletedObjV = new deletedObjV
                        {

                            ObjectifsPrincipeauxSmart = objV.ObjectifsPrincipeauxSmart,
                            indicateur = objV.indicateur,
                            ARealiserEnT1__T4 = objV.ARealiserEnT1__T4,
                            idEntretien = objV.idEntretien,
                            idDeletedEntretien = deletedEntretienId
                        };
                        db.deletedObjV.Add(deletedObjV);
                        db.SaveChanges();

                        // Now delete the ObjSpecifique entry itself
                        db.ObjectifsAVenir.Remove(objV);
                    }


                    db.SaveChanges();

                    db.EntretienAnnuel.Remove(entretien);
                }



                Employee employee = db.Employee.FirstOrDefault(e => e.Matricule == ficheFonction.Matricule);


                if (ficheFonction != null)
                {

                    // Add to DeletedFicheFonctions
                    DeletedFicheFonctions deletedFicheFonction = new DeletedFicheFonctions
                    {
                        FicheFonctionId = ficheFonction.id,
                        DeletedAt = DateTime.Now,
                        nom = ficheFonction.nom,
                        prenom = ficheFonction.prenom,
                        email = ficheFonction.email,
                        ville = ficheFonction.ville,
                        numero = ficheFonction.numero,
                        etatCivil = ficheFonction.etatCivil,
                        numCin = ficheFonction.numCin,
                        dateNaissance = ficheFonction.dateNaissance,
                        poste = ficheFonction.poste,
                        dateRecrutement = ficheFonction.dateRecrutement,
                        image = ficheFonction.image,
                        Matricule = ficheFonction.Matricule,
                        idDepartement = ficheFonction.idDepartement,
                        DeletionReason = reason,
                        cmptWindows = ficheFonction.cmptWindows,
                        Responsable = ficheFonction.ResponsableFonctionnel,
                    };
                    db.DeletedFicheFonctions.Add(deletedFicheFonction);
                    db.SaveChanges();

                    if (employee != null)
                    {
                        DeletedEmployees deletedEmployee = new DeletedEmployees
                        {
                            Matricule = employee.Matricule,
                            Nom = employee.Nom,
                            Prenom = employee.Prenom,
                            fonction = employee.fonction,
                            Responsable = employee.Responsable,
                            email = employee.email,
                            CmptWin = employee.CmptWin,
                            dateEntree = employee.dateEntree,
                            Service = employee.Service,
                            ResponsableFonctionnel = employee.ResponsableFonctionnel,
                            // Reference to the deleted FicheFonction
                            DeletedAt = DateTime.Now
                        };
                        db.DeletedEmployees.Add(deletedEmployee);
                        db.SaveChanges();
                    }

                    foreach (var diplome in ficheFonction.Diplome.ToList())
                    {
                        DeletedDiplome deletedDiplome = new DeletedDiplome
                        {
                            DiplomeId = diplome.id,
                            NomDiplome = diplome.NomDimplome,
                            idFiche = diplome.idFiche,
                            lieu = diplome.lieu,
                            DateDebut = diplome.DateDebut,
                            DateFin = diplome.DateFin,
                            PDF = diplome.PDF,
                            idDeletedFiche = deletedFicheFonction.Id
                        };
                        db.DeletedDiplome.Add(deletedDiplome);
                        db.SaveChanges();

                        // Remove diplome from Diplomes table
                        db.Diplome.Remove(diplome);
                        db.SaveChanges();
                    }


                    foreach (var formation in ficheFonction.Formation.ToList())
                    {
                        DeletedFormation deletedFormation = new DeletedFormation
                        {
                            FormationId = formation.id,
                            nom = formation.nom,
                            dateDebut = formation.dateDebut,
                            lieu = formation.lieu,
                            dateFin = formation.dateFin,
                            PDF = formation.PDF,
                            idFiche = formation.idFiche,

                            idDeletedFiche = deletedFicheFonction.Id
                        };
                        db.DeletedFormation.Add(deletedFormation);
                        db.SaveChanges();

                        // Remove diplome from Diplomes table
                        db.Formation.Remove(formation);
                        db.SaveChanges();
                    }


                    foreach (var competence in ficheFonction.competence.ToList())
                    {
                        DeletedCompetence deletedCompetence = new DeletedCompetence
                        {
                            CompetenceId = competence.id,
                            nom = competence.nom,
                            type = competence.type,
                            niveau = competence.niveau,

                            PDF = competence.PDF,
                            idFiche = competence.idFiche,

                            idDeletedFiche = deletedFicheFonction.Id
                        };
                        db.DeletedCompetence.Add(deletedCompetence);
                        db.SaveChanges();

                        // Remove diplome from Diplomes table
                        db.competence.Remove(competence);
                        db.SaveChanges();
                    }


                    foreach (var experience in ficheFonction.Experience.ToList())
                    {
                        DeletedExperience deletedExperience = new DeletedExperience
                        {
                            ExperienceId = experience.id,
                            Titre = experience.Titre,
                            dateDebut = experience.dateDebut,
                            lieu = experience.lieu,
                            dateFin = experience.dateFin,
                            PDF = experience.PDF,
                            idFiche = experience.idFiche,

                            idDeletedFiche = deletedFicheFonction.Id
                        };
                        db.DeletedExperience.Add(deletedExperience);
                        db.SaveChanges();

                        // Remove diplome from Diplomes table
                        db.Experience.Remove(experience);
                        db.SaveChanges();
                    }


                    // Now delete from FicheFonction
                    db.FicheFonction.Remove(ficheFonction);
                    db.SaveChanges();

                    // Redirect or return success

                    // Delete the corresponding Employee if found
                    if (employee != null)
                    {
                        db.Employee.Remove(employee);
                        db.SaveChanges();
                    }

                    // Redirect or return success
                }

                // Redirect to the index page
                return RedirectToAction("Index", new { idDepartement = newFicheFonctionDepartmentId });
            }
        }





        // POST: FicheFonction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FicheFonction ficheFonction = db.FicheFonction.Find(id);
            if (ficheFonction == null)
            {
                return HttpNotFound();
            }

            // Remove the FicheFonction
            db.FicheFonction.Remove(ficheFonction);
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
