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
    public class DeletedEntretienAnnuelsController : Controller
    {
        private baseEntities db = new baseEntities();

        // GET: DeletedEntretienAnnuels
        public ActionResult Index(string matricule, string searchLetter, int pageNo = 1)
        {
            string role = CurrentUser.getRole();
            if (role != "RH" && role != "DS")
            {

                return RedirectToAction("Unauthorized", "Error");

            }

            // Récupérer les entretiens annuels supprimés de la base de données
            var deletedEntretienAnnuels = db.DeletedEntretienAnnuel.AsQueryable();

            deletedEntretienAnnuels = deletedEntretienAnnuels.Where(e => e.status3 == "fini");

            if (!string.IsNullOrEmpty(matricule))
            {
                deletedEntretienAnnuels = deletedEntretienAnnuels.Where(e => e.Matricule == matricule);
            }


            // Filtrer par lettre de recherche si elle est fournie
            if (!string.IsNullOrEmpty(searchLetter))
            {
                // Convertir la lettre de recherche en minuscules
                string searchLower = searchLetter.ToLower();

                // Filtrer les entretiens annuels par nom ou prénom commençant par la lettre de recherche
                deletedEntretienAnnuels = deletedEntretienAnnuels
       .Where(p => p.DateEntretien.Year.ToString().StartsWith(searchLower));

            }

            // Pagination
            int noOfRecordsPerPage = 7;
            int totalCount = deletedEntretienAnnuels.Count();
            int noOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalCount) / Convert.ToDouble(noOfRecordsPerPage)));
            int noOfRecordsToSkip = (pageNo - 1) * noOfRecordsPerPage;

            // Paginer les résultats de la requête
            var deletedEntretienAnnuelsPaged = deletedEntretienAnnuels
                                                .OrderByDescending(e => e.Nom) // Par exemple, ordonner par nom
                                                .Skip(noOfRecordsToSkip)
                                                .Take(noOfRecordsPerPage)
                                                .ToList();

            // Passer les données de pagination à la vue
            ViewBag.PageNo = pageNo;
            ViewBag.NoOfPages = noOfPages;
            ViewBag.TotalCount = totalCount;

            // Retourner la vue avec les données paginées
            return View(deletedEntretienAnnuelsPaged);
        }

        // GET: DeletedEntretienAnnuels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string role = CurrentUser.getRole();
            if (role != "RH" && role != "DS")
            {

                // Redirect the user to an unauthorized page or action
                return RedirectToAction("Unauthorized", "Error");

            }




            var entretienViewModel = new DeletedentretienViewModel();
            entretienViewModel.deletedentretienAnnuels= db.DeletedEntretienAnnuel.Find(id);
            entretienViewModel.deletedobjSpecifiques = db.deletedObjSpecifique.Where(d => d.id == id).ToList();
            entretienViewModel.deletedobjectifsAVenirs = db.deletedObjV.Where(d => d.id == id).ToList();

            entretienViewModel.deletedevaluatinGenN_1s = db.deletedEvalGen.Where(d => d.id == id).ToList();

            return View(entretienViewModel);
        }

        // GET: DeletedEntretienAnnuels/Create
       
      
      

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
