using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using ProjectWith.Models;
using ProjectWith.Models.ath_windows;

namespace ProjectWith.Controllers
{
    public class EntretienAnnuelsController : Controller
    {
        private baseEntities db = new baseEntities();

        public ActionResult GrantAccess()
        {
            string role = CurrentUser.getRole();
            if (role != "RH" && role != "DS")
            {
                return RedirectToAction("Unauthorized", "Error");
            }

           
        
                // Set session variable to indicate access is granted
                Session["AccessGranted"] = true;
          
            return RedirectToAction("Index");
        }


        public ActionResult RevokeAccess()
        {
            string role = CurrentUser.getRole();
            if (role != "RH" && role != "DS")
            {
                return RedirectToAction("Unauthorized", "Error");
            }


                // Set session variable to indicate access is granted
                Session["AccessGranted"] = false;
            

            return RedirectToAction("Index");
        }













        public ActionResult Index(string matricule, string searchLetter, string IconClass = "fa-sort-asc", int PageNo = 1)
        {
            // Get the role and matricule of the current user
            string role = CurrentUser.getRole();
            string currentUserMatricule = CurrentUser.getMatricule();

            // Check if the user has the required role to access this action
            if (role != "RH" && role != "DS")
            {
                // Check if access has been granted (RH or DS has clicked the grant access button)
                if (Session["AccessGranted"] == null || !(bool)Session["AccessGranted"])
                {
                    // Redirect the user to an unauthorized page or action
                    return RedirectToAction("Unauthorized", "Error");
                }
            }

            ViewBag.AccessGranted = (Session["AccessGranted"] != null && (bool)Session["AccessGranted"]);
            IQueryable<EntretienAnnuel> entretiensQuery = db.EntretienAnnuel;

            // Filtering based on searchLetter
            if (!string.IsNullOrEmpty(searchLetter))
            {
                string searchLower = searchLetter.ToLower();
                entretiensQuery = entretiensQuery.Where(e => e.Nom.ToLower().StartsWith(searchLower) || e.Prenom.ToLower().StartsWith(searchLower) || e.Matricule.ToLower().StartsWith(searchLower) || e.DateEntretien.Year.ToString().StartsWith(searchLower));
            }

            // Filtering based on matricule
            if (string.IsNullOrEmpty(matricule))
            {
                // If matricule is null or empty, show only entretiens of the current user
                entretiensQuery = entretiensQuery.Where(e => e.Matricule == currentUserMatricule);
            }
            else
            {
                // If matricule is not null, show only entretiens with the given matricule
                entretiensQuery = entretiensQuery.Where(e => e.Matricule == matricule);
            }

            // Universal filtering logic
            entretiensQuery = entretiensQuery.Where(x =>
                // The owner of the entretien can always see it
                x.Matricule == currentUserMatricule ||
                // For RH and DS, they can see all entretiens except those with status "en cours" if the matricule does not match
                (x.status1 == "en cours" && x.status2 != "envoyé" && x.Matricule == currentUserMatricule) ||
                (x.status2 == "envoyé" &&
                 db.Employee.Any(emp => emp.Matricule == x.Matricule && emp.ResponsableFonctionnel == currentUserMatricule)) ||
                // New condition: RH and DS can see entretiens with status3 "fini"
                ((role == "RH" || role == "DS") && x.status3 == "fini"));

            // Pagination
            int NoOfRecordsPerPage = 7;
            int NoOfRecordsToSkip = (PageNo - 1) * NoOfRecordsPerPage;
            int totalCount = entretiensQuery.Count();

            // Paginate the query results
            var entretiensPaged = entretiensQuery
                                   .OrderByDescending(e => e.DateEntretien) // Order by entretien date, for example
                                   .Skip(NoOfRecordsToSkip)
                                   .Take(NoOfRecordsPerPage)
                                   .ToList();

            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalCount) / Convert.ToDouble(NoOfRecordsPerPage)));
            ViewBag.PageNo = PageNo;
            ViewBag.NoOfPages = NoOfPages;
            ViewBag.TotalCount = totalCount;
            ViewBag.Matricule = matricule;

            return View(entretiensPaged);
        }





        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var entretien = db.EntretienAnnuel.Find(id);
            string role = CurrentUser.getRole();
            if ((role == "RH" || role == "DS")&& CurrentUser.getMatricule() != entretien.Matricule && entretien.status3 != "fini")
            {
                // Redirect the user to an unauthorized page or action
                return RedirectToAction("Unauthorized", "Error");
            }

            if (CurrentUser.getMatricule()== entretien.matriculeResp && entretien.status2 != "envoyé")
            {
                // Redirect the user to an unauthorized page or action
                return RedirectToAction("Unauthorized", "Error");
            }
            // Check if the current user has the role RH or DS

            //if (role != "RH" && role != "DS")
            //{
            // Check if access has been granted (RH or DS has clicked the grant access button)
            if (Session["AccessGranted"] == null || !(bool)Session["AccessGranted"])
                {
                    // Redirect the user to an unauthorized page or action
                    return RedirectToAction("Unauthorized", "Error");
                }
            //}

            var entretienViewModel = new entretienViewModel();
            entretienViewModel.entretienAnnuels = db.EntretienAnnuel.Find(id);
            entretienViewModel.objSpecifiques = db.ObjSpecifique.Where(d => d.id == id).ToList();
            entretienViewModel.objectifsAVenirs = db.ObjectifsAVenir.Where(d => d.id == id).ToList();
          
            entretienViewModel.evaluatinGenN_1s = db.EvaluatinGenN_1.Where(d => d.id == id).ToList();
          

            return View(entretienViewModel);
        }
        public ActionResult GeneratePdf(int id)
        {
            // Fetch the model data based on the id
            var model = new entretienViewModel();

            // Render the view to a string
            string htmlContent = RenderRazorViewToString("Edit", model);

            // Convert the HTML string to a PDF
            byte[] pdfBytes;
            using (var ms = new MemoryStream())
            {
                using (var document = new Document(PageSize.A4))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    document.Open();

                    // Using XMLWorkerHelper to parse the HTML to PDF
                    using (var sr = new StringReader(htmlContent))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, sr);
                    }

                    document.Close();
                }
                pdfBytes = ms.ToArray();
            }

            return File(pdfBytes, "application/pdf", "EntretienAnnuels.pdf");
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }


        // GET: EntretienAnnuel/Create
        public ActionResult Create()
        {
            // Check if the current user has the role RH or DS
            string role = CurrentUser.getRole();
           
                // Check if access has been granted (RH or DS has clicked the grant access button)
                if (Session["AccessGranted"] == null || !(bool)Session["AccessGranted"])
                {
                    // Redirect the user to an unauthorized page or action
                    return RedirectToAction("Unauthorized", "Error");
                }
            

            return View();
        }


		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Nom,Prenom,LibelleFonction,DateEntree,DateDerniereRevue,Service,NomRespHierarchiqueN_1,NomRespFonctionel,DateEntretien,id,autoEvalSal1,competenceCaractUtiles,competenceCaractAcquerir,question1,orientationResulttat,agilite_Adaptabilite,ouvertureEsprit,qualitesRelationnelles,travailEquipe,ResolutionProbleme,gestionProjet,communication,Leadership,gestionBudget,charisme,maitriseSoi,ResultatActFormation,FormationMesureDev,ResourceNecessaire,comntSouhaitEvolution,comntSouhaitMobilite,comntProchaineEtape,dateProchainEnretien,comntGenereaux,Signature1RespN_1,date1,comntSalaire,signature2Salarie,date2,AvisN_2,NomSignatureN_2,date3,AvisRespFonctionel,NomSignatureResFonct_Depart,date4,comOrientationResulttat,ComAgilite,comOuv,comQualité,comEquipe,comResolP,comGP,comCommunication,comLeadership,comGBud,comMaitriseSoi,comCharisme,c1,c2,c3,c4,comC1,comC2,comC3,comC4, Matricule, Note, status1, dateCreation, matriculeResp")] EntretienAnnuel entretienAnnuel)
		{
			// Check if the current user has the role RH or DS
			string role = CurrentUser.getRole();

			// Check if access has been granted (RH or DS has clicked the grant access button)
			if (Session["AccessGranted"] == null || !(bool)Session["AccessGranted"])
			{
				// Redirect the user to an unauthorized page or action
				return RedirectToAction("Unauthorized", "Error");
			}


			string matricule = entretienAnnuel.Matricule;
			bool matriculeExists = db.Employee.Any(e => e.Matricule == matricule);
			DateTime dateEntretien = entretienAnnuel.DateEntretien;
			int yearOfEntretien = dateEntretien.Year;
			entretienAnnuel.status1 = "en cours";
			entretienAnnuel.dateCreation = DateTime.Now.Date;
			var employee = db.Employee.FirstOrDefault(emp => emp.Matricule == entretienAnnuel.Matricule);
			entretienAnnuel.matriculeResp = employee.ResponsableFonctionnel;

			// Check if an entretien has already been done for the given matricule in the specified year
			bool hasEntretienThisYear = db.EntretienAnnuel.Any(e => e.Matricule == matricule && e.DateEntretien.Year == yearOfEntretien);

			// If an entretien has already been done for the given matricule in the specified year, show an error message
			if (hasEntretienThisYear)
			{
				ModelState.AddModelError(string.Empty, "An entretien has already been done for this employee in the specified year.");
				//  return View(entretienAnnuel);
				return View(new entretienViewModel { entretienAnnuels = entretienAnnuel });
			}
			if (!matriculeExists)
			{
				// Matricule doesn't exist, return the view with an error message
				ModelState.AddModelError(string.Empty, "Matricule does not exist in fiche fonction.");
				ViewBag.MatriculeExists = false;
				return View(new entretienViewModel { entretienAnnuels = entretienAnnuel });
			}
			if (ModelState.IsValid)
			{
				db.EntretienAnnuel.Add(entretienAnnuel);
				db.SaveChanges();
				return RedirectToAction("Index", new { matricule = entretienAnnuel.Matricule });
			}

			return View(entretienAnnuel);
		}














		[HttpPost]
        public ActionResult CheckMatricule(string matricule)
        {
            bool matriculeExists = db.Employee.Any(e => e.Matricule == matricule);
            return Json(matriculeExists); // Return true if matricule exists, false otherwise
        }


        [HttpPost]
        public ActionResult CheckDateEntretien(DateTime dateEntretien, string matricule)
        {
            int yearOfEntretien = dateEntretien.Year;

            // Check if an entretien has already been done for the given matricule in the specified year
            bool hasEntretienThisYear = db.EntretienAnnuel.Any(e => e.Matricule == matricule && e.DateEntretien.Year == yearOfEntretien);

            return Json(!hasEntretienThisYear); // Return true if date is valid (no entretien for the year), false otherwise
        }





        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Check if the current user has the role RH or DS
            string role = CurrentUser.getRole();
           

            EntretienAnnuel entretienAnnuel = db.EntretienAnnuel.Find(id);
            if (entretienAnnuel == null)
            {
                return HttpNotFound();
            }

        

            return View(entretienAnnuel);
        }

        public bool IsCurrentUserResponsableFonctionnel(string matricule)
        {
            // Retrieve the responsable fonctionnel for the employee with the given matricule
            var responsableFonctionnel = db.Employee
                .Where(e => e.Matricule == matricule)
                .Select(e => e.ResponsableFonctionnel)
                .FirstOrDefault();
       
            // Check if the current user's matricule matches the responsable fonctionnel
            return CurrentUser.getMatricule() == responsableFonctionnel;
        }




        [HttpPost]
        public ActionResult Edit([Bind(Include = "Nom,Prenom,LibelleFonction,DateEntree,DateDerniereRevue,Service,NomRespHierarchiqueN_1,NomRespFonctionel,DateEntretien,id,autoEvalSal1,competenceCaractUtiles,competenceCaractAcquerir,question1,orientationResulttat,agilite_Adaptabilite,ouvertureEsprit,qualitesRelationnelles,travailEquipe,ResolutionProbleme,gestionProjet,communication,Leadership,gestionBudget,charisme,maitriseSoi,ResultatActFormation,FormationMesureDev,ResourceNecessaire,comntSouhaitEvolution,comntSouhaitMobilite,comntProchaineEtape,dateProchainEnretien,comntGenereaux,Signature1RespN_1,date1,comntSalaire,signature2Salarie,date2,AvisN_2,NomSignatureN_2,date3,AvisRespFonctionel,NomSignatureResFonct_Depart,date4,comOrientationResulttat,ComAgilite,comOuv,comQualité,comEquipe,comResolP,comGP,comCommunication,comLeadership,comGBud,comMaitriseSoi,comCharisme,c1,c2,c3,c4,comC1,comC2,comC3,comC4,Matricule, Note, checkC1, checkC2 , checkC3 , checkC4, status1 ,status2 , status3, dateEnvoi, dateValidation, matriculeResp")] EntretienAnnuel entretienAnnuel, int idEntretien , string matricule)
        {


            string role = CurrentUser.getRole();

         
            var entretien = db.EntretienAnnuel.Find(idEntretien);
            if (ModelState.IsValid)
            {
                // Rechercher la fiche de fonctions dans la base de données


                // Vérifier si la fiche existe
                if (entretien != null)
                {
                    // Mettre à jour les propriétés de la fiche existante avec les valeurs envoyées
                    entretien.Nom = entretienAnnuel.Nom;
                    entretien.Prenom = entretienAnnuel.Prenom;
                    entretien.Service = entretienAnnuel.Service;
                    entretien.LibelleFonction = entretienAnnuel.LibelleFonction;
                    entretien.DateEntree = entretienAnnuel.DateEntree;
                    entretien.DateDerniereRevue = entretienAnnuel.DateDerniereRevue;
                    entretien.NomRespHierarchiqueN_1 = entretienAnnuel.NomRespHierarchiqueN_1;
                    entretien.NomRespFonctionel = entretienAnnuel.NomRespFonctionel;
                    entretien.DateEntretien = entretienAnnuel.DateEntretien;
                    entretien.autoEvalSal1 = entretienAnnuel.autoEvalSal1;
                    entretien.competenceCaractUtiles = entretienAnnuel.competenceCaractUtiles;
                    entretien.competenceCaractAcquerir = entretienAnnuel.competenceCaractAcquerir;
                    entretien.question1 = entretienAnnuel.question1;
                    entretien.orientationResulttat = entretienAnnuel.orientationResulttat;
                    entretien.agilite_Adaptabilite = entretienAnnuel.agilite_Adaptabilite;
                    entretien.ouvertureEsprit = entretienAnnuel.ouvertureEsprit;
                    entretien.qualitesRelationnelles = entretienAnnuel.qualitesRelationnelles;
                    entretien.travailEquipe = entretienAnnuel.travailEquipe;
                    entretien.ResolutionProbleme = entretienAnnuel.ResolutionProbleme;
                    entretien.gestionProjet = entretienAnnuel.gestionProjet;
                    entretien.communication = entretienAnnuel.communication;
                    entretien.Leadership = entretienAnnuel.Leadership;
                    entretien.gestionBudget = entretienAnnuel.gestionBudget;
                    entretien.charisme = entretienAnnuel.charisme;
                    entretien.maitriseSoi = entretienAnnuel.maitriseSoi;
                    entretien.ResultatActFormation = entretienAnnuel.ResultatActFormation;
                    entretien.FormationMesureDev = entretienAnnuel.FormationMesureDev;
                    entretien.ResourceNecessaire = entretienAnnuel.ResourceNecessaire;
                    entretien.comntSouhaitEvolution = entretienAnnuel.comntSouhaitEvolution;
                    entretien.comntSouhaitMobilite = entretienAnnuel.comntSouhaitMobilite;
                    entretien.comntProchaineEtape = entretienAnnuel.comntProchaineEtape;
                    entretien.dateProchainEnretien = entretienAnnuel.dateProchainEnretien;
                    entretien.comntGenereaux = entretienAnnuel.comntGenereaux;
                    entretien.Signature1RespN_1 = entretienAnnuel.Signature1RespN_1;
                    entretien.date1 = entretienAnnuel.date1;
                    entretien.comntSalaire = entretienAnnuel.comntSalaire;
                    entretien.signature2Salarie = entretienAnnuel.signature2Salarie;
                    entretien.date2 = entretienAnnuel.date2;
                    entretien.AvisN_2 = entretienAnnuel.AvisN_2;
                    entretien.NomSignatureN_2 = entretienAnnuel.NomSignatureN_2;
                    entretien.date3 = entretienAnnuel.date3;
                    entretien.AvisRespFonctionel = entretienAnnuel.AvisRespFonctionel;
                    entretien.NomSignatureResFonct_Depart = entretienAnnuel.NomSignatureResFonct_Depart;
                    entretien.date4 = entretienAnnuel.date4;
                    entretien.comOrientationResulttat = entretienAnnuel.comOrientationResulttat;
                    entretien.ComAgilite = entretienAnnuel.ComAgilite;
                    entretien.comOuv = entretienAnnuel.comOuv;
                    entretien.comQualité = entretienAnnuel.comQualité;
                    entretien.comEquipe = entretienAnnuel.comEquipe;
                    entretien.comResolP = entretienAnnuel.comResolP;
                    entretien.comGP = entretienAnnuel.comGP;
                    entretien.comCommunication = entretienAnnuel.comCommunication;
                    entretien.comLeadership = entretienAnnuel.comLeadership;
                    entretien.comGBud = entretienAnnuel.comGBud;
                    entretien.comMaitriseSoi = entretienAnnuel.comMaitriseSoi;
                    entretien.comCharisme = entretienAnnuel.comCharisme;
                    entretien.c1 = entretienAnnuel.c1;
                    entretien.c2 = entretienAnnuel.c2;
                    entretien.c3 = entretienAnnuel.c3;
                    entretien.c4 = entretienAnnuel.c4;
                    entretien.comC1 = entretienAnnuel.comC1;
                    entretien.comC2 = entretienAnnuel.comC2;
                    entretien.comC3 = entretienAnnuel.comC3;
                    entretien.comC4 = entretienAnnuel.comC4;
                    entretien.Matricule = entretienAnnuel.Matricule;
                    entretien.Note = entretienAnnuel.Note;
                    entretien.checkC1 = entretienAnnuel.checkC1;
                    entretien.checkC2 = entretienAnnuel.checkC2;
                    entretien.checkC3 = entretienAnnuel.checkC3;
                    entretien.checkC4 = entretienAnnuel.checkC4;

                    string currentUserMatricule = CurrentUser.getMatricule();
               


                    if (currentUserMatricule == entretien.Matricule)
                    {
                        entretien.status2 = entretienAnnuel.status2;
                        entretien.dateEnvoi = DateTime.Now.Date;

                    }
                    var employee = db.Employee.FirstOrDefault(emp => emp.Matricule == entretien.Matricule);
                
                    if (employee != null && employee.ResponsableFonctionnel == currentUserMatricule)
                    {
                       
                        //entretien.status3 = "fini";
                        entretien.status3 = entretienAnnuel.status3;
                        entretien.dateValidation = DateTime.Now.Date;
                    }
                    db.Entry(entretien).State = EntityState.Modified;
                    // Enregistrer les modifications dans la base de données
                    db.SaveChanges();
                 
                    // Retourner un succès JSON

                    if (string.IsNullOrEmpty(matricule))
                    {
                        // If matricule is null or empty, redirect to the Index action without matricule parameter
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // If matricule is not null, redirect to the Index action with matricule parameter preserved
                        return RedirectToAction("Index", new { matricule = matricule });
                    }


                }
                else
                {
                    // Retourner une erreur JSON si la fiche n'est pas trouvée
                    return Json("Fiche non trouvée", JsonRequestBehavior.AllowGet);
                }
            }

            // Retourner une erreur JSON si le modèle n'est pas valide
            return Json("Erreur de validation du modèle", JsonRequestBehavior.AllowGet);
        }




        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Check if the current user has the role RH or DS
            string role = CurrentUser.getRole();
            if (role != "RH" && role != "DS")
            {
                // Redirect the user to an unauthorized page or action
                return RedirectToAction("Unauthorized", "Error");
            }
            if (Session["AccessGranted"] == null || !(bool)Session["AccessGranted"])
            {
                // Redirect the user to an unauthorized page or action
                return RedirectToAction("Unauthorized", "Error");
            }

            EntretienAnnuel entretienAnnuel = db.EntretienAnnuel.Find(id);
            if (entretienAnnuel == null)
            {
                return HttpNotFound();
            }

            return View(entretienAnnuel);
        }

        // POST: EntretienAnnuel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Check if the current user has the role RH or DS
            string role = CurrentUser.getRole();
            if (role != "RH" && role != "DS")
            {
                // Redirect the user to an unauthorized page or action
                return RedirectToAction("Unauthorized", "Error");
            }

            if (Session["AccessGranted"] == null || !(bool)Session["AccessGranted"])
            {
                // Redirect the user to an unauthorized page or action
                return RedirectToAction("Unauthorized", "Error");
            }

            // Check Session for AccessGranted
           

            EntretienAnnuel entretienAnnuel = db.EntretienAnnuel.Find(id);
            if (entretienAnnuel == null)
            {
                return HttpNotFound();
            }

            db.EntretienAnnuel.Remove(entretienAnnuel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }




        //[HttpPost]
        //public ActionResult Edit([Bind(Include = "Nom,Prenom,LibelleFonction,DateEntree,DateDerniereRevue,Service,NomRespHierarchiqueN_1,NomRespFonctionel,DateEntretien,id,autoEvalSal1,competenceCaractUtiles,competenceCaractAcquerir,question1,orientationResulttat,agilite_Adaptabilite,ouvertureEsprit,qualitesRelationnelles,travailEquipe,ResolutionProbleme,gestionProjet,communication,Leadership,gestionBudget,charisme,maitriseSoi,ResultatActFormation,FormationMesureDev,ResourceNecessaire,comntSouhaitEvolution,comntSouhaitMobilite,comntProchaineEtape,dateProchainEnretien,comntGenereaux,Signature1RespN_1,date1,comntSalaire,signature2Salarie,date2,AvisN_2,NomSignatureN_2,date3,AvisRespFonctionel,NomSignatureResFonct_Depart,date4,comOrientationResulttat,ComAgilite,comOuv,comQualité,comEquipe,comResolP,comGP,comCommunication,comLeadership,comGBud,comMaitriseSoi,comCharisme,c1,c2,c3,c4,comC1,comC2,comC3,comC4,Matricule, Note, checkC1, checkC2 , checkC3 , checkC4, status1 ,status2 , status3, dateEnvoi, dateValidation, matriculeResp")] EntretienAnnuel entretienAnnuel, int idEntretien, string matricule)
        //{


        //    string role = CurrentUser.getRole();

        //    if (role != "RH" && role != "DS")
        //    {
        //        // Check if access has been granted (RH or DS has clicked the grant access button)
        //        if (Session["AccessGranted"] == null || !(bool)Session["AccessGranted"])
        //        {
        //            // Redirect the user to an unauthorized page or action
        //            return RedirectToAction("Unauthorized", "Error");
        //        }
        //    }
        //    var entretien = db.EntretienAnnuel.Find(idEntretien);
        //    if (ModelState.IsValid)
        //    {
        //        // Rechercher la fiche de fonctions dans la base de données


        //        // Vérifier si la fiche existe
        //        if (entretien != null)
        //        {
        //            // Mettre à jour les propriétés de la fiche existante avec les valeurs envoyées
        //            entretien.Nom = entretienAnnuel.Nom;
        //            entretien.Prenom = entretienAnnuel.Prenom;
        //            entretien.Service = entretienAnnuel.Service;
        //            entretien.LibelleFonction = entretienAnnuel.LibelleFonction;
        //            entretien.DateEntree = entretienAnnuel.DateEntree;
        //            entretien.DateDerniereRevue = entretienAnnuel.DateDerniereRevue;
        //            entretien.NomRespHierarchiqueN_1 = entretienAnnuel.NomRespHierarchiqueN_1;
        //            entretien.NomRespFonctionel = entretienAnnuel.NomRespFonctionel;
        //            entretien.DateEntretien = entretienAnnuel.DateEntretien;
        //            entretien.autoEvalSal1 = entretienAnnuel.autoEvalSal1;
        //            entretien.competenceCaractUtiles = entretienAnnuel.competenceCaractUtiles;
        //            entretien.competenceCaractAcquerir = entretienAnnuel.competenceCaractAcquerir;
        //            entretien.question1 = entretienAnnuel.question1;
        //            entretien.orientationResulttat = entretienAnnuel.orientationResulttat;
        //            entretien.agilite_Adaptabilite = entretienAnnuel.agilite_Adaptabilite;
        //            entretien.ouvertureEsprit = entretienAnnuel.ouvertureEsprit;
        //            entretien.qualitesRelationnelles = entretienAnnuel.qualitesRelationnelles;
        //            entretien.travailEquipe = entretienAnnuel.travailEquipe;
        //            entretien.ResolutionProbleme = entretienAnnuel.ResolutionProbleme;
        //            entretien.gestionProjet = entretienAnnuel.gestionProjet;
        //            entretien.communication = entretienAnnuel.communication;
        //            entretien.Leadership = entretienAnnuel.Leadership;
        //            entretien.gestionBudget = entretienAnnuel.gestionBudget;
        //            entretien.charisme = entretienAnnuel.charisme;
        //            entretien.maitriseSoi = entretienAnnuel.maitriseSoi;
        //            entretien.ResultatActFormation = entretienAnnuel.ResultatActFormation;
        //            entretien.FormationMesureDev = entretienAnnuel.FormationMesureDev;
        //            entretien.ResourceNecessaire = entretienAnnuel.ResourceNecessaire;
        //            entretien.comntSouhaitEvolution = entretienAnnuel.comntSouhaitEvolution;
        //            entretien.comntSouhaitMobilite = entretienAnnuel.comntSouhaitMobilite;
        //            entretien.comntProchaineEtape = entretienAnnuel.comntProchaineEtape;
        //            entretien.dateProchainEnretien = entretienAnnuel.dateProchainEnretien;
        //            entretien.comntGenereaux = entretienAnnuel.comntGenereaux;
        //            entretien.Signature1RespN_1 = entretienAnnuel.Signature1RespN_1;
        //            entretien.date1 = entretienAnnuel.date1;
        //            entretien.comntSalaire = entretienAnnuel.comntSalaire;
        //            entretien.signature2Salarie = entretienAnnuel.signature2Salarie;
        //            entretien.date2 = entretienAnnuel.date2;
        //            entretien.AvisN_2 = entretienAnnuel.AvisN_2;
        //            entretien.NomSignatureN_2 = entretienAnnuel.NomSignatureN_2;
        //            entretien.date3 = entretienAnnuel.date3;
        //            entretien.AvisRespFonctionel = entretienAnnuel.AvisRespFonctionel;
        //            entretien.NomSignatureResFonct_Depart = entretienAnnuel.NomSignatureResFonct_Depart;
        //            entretien.date4 = entretienAnnuel.date4;
        //            entretien.comOrientationResulttat = entretienAnnuel.comOrientationResulttat;
        //            entretien.ComAgilite = entretienAnnuel.ComAgilite;
        //            entretien.comOuv = entretienAnnuel.comOuv;
        //            entretien.comQualité = entretienAnnuel.comQualité;
        //            entretien.comEquipe = entretienAnnuel.comEquipe;
        //            entretien.comResolP = entretienAnnuel.comResolP;
        //            entretien.comGP = entretienAnnuel.comGP;
        //            entretien.comCommunication = entretienAnnuel.comCommunication;
        //            entretien.comLeadership = entretienAnnuel.comLeadership;
        //            entretien.comGBud = entretienAnnuel.comGBud;
        //            entretien.comMaitriseSoi = entretienAnnuel.comMaitriseSoi;
        //            entretien.comCharisme = entretienAnnuel.comCharisme;
        //            entretien.c1 = entretienAnnuel.c1;
        //            entretien.c2 = entretienAnnuel.c2;
        //            entretien.c3 = entretienAnnuel.c3;
        //            entretien.c4 = entretienAnnuel.c4;
        //            entretien.comC1 = entretienAnnuel.comC1;
        //            entretien.comC2 = entretienAnnuel.comC2;
        //            entretien.comC3 = entretienAnnuel.comC3;
        //            entretien.comC4 = entretienAnnuel.comC4;
        //            entretien.Matricule = entretienAnnuel.Matricule;
        //            entretien.Note = entretienAnnuel.Note;
        //            entretien.checkC1 = entretienAnnuel.checkC1;
        //            entretien.checkC2 = entretienAnnuel.checkC2;
        //            entretien.checkC3 = entretienAnnuel.checkC3;
        //            entretien.checkC4 = entretienAnnuel.checkC4;

        //            string currentUserMatricule = CurrentUser.getMatricule();
        //            //if (currentUserMatricule == entretien.Matricule)
        //            //{
        //            //    entretien.status2 = "envoyé";
        //            //}



        //            if (currentUserMatricule == entretien.Matricule)
        //            {
        //                entretien.status2 = entretienAnnuel.status2;
        //                entretien.dateEnvoi = DateTime.Now.Date;
        //                MailSender.SendToResponsable(this, entretien.id);


        //            }
        //            var employee = db.Employee.FirstOrDefault(emp => emp.Matricule == entretien.Matricule);

        //            if (employee != null && employee.ResponsableFonctionnel == currentUserMatricule)
        //            {

        //                //entretien.status3 = "fini";
        //                entretien.status3 = entretienAnnuel.status3;
        //                entretien.dateValidation = DateTime.Now.Date;
        //            }
        //            db.Entry(entretien).State = EntityState.Modified;
        //            // Enregistrer les modifications dans la base de données
        //            db.SaveChanges();

        //            // Retourner un succès JSON

        //            if (string.IsNullOrEmpty(matricule))
        //            {
        //                // If matricule is null or empty, redirect to the Index action without matricule parameter
        //                return RedirectToAction("Index");
        //            }
        //            else
        //            {
        //                // If matricule is not null, redirect to the Index action with matricule parameter preserved
        //                return RedirectToAction("Index", new { matricule = matricule });
        //            }


        //        }
        //        else
        //        {
        //            // Retourner une erreur JSON si la fiche n'est pas trouvée
        //            return Json("Fiche non trouvée", JsonRequestBehavior.AllowGet);
        //        }
        //    }

        //    // Retourner une erreur JSON si le modèle n'est pas valide
        //    return Json("Erreur de validation du modèle", JsonRequestBehavior.AllowGet);
        //}





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
