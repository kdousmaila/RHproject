using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectWith.Models;

namespace prjetFinal.Controllers
{
    public class HomeController : Controller
    {
        private baseEntities db = new baseEntities();

        public ActionResult Index()
        {
            // Calculate EntretienAnnuelNotes
            var entretienAnnuelNotes = db.EntretienAnnuel
                                        .Where(e => e.Note != null)
                                        .GroupBy(e => e.Matricule)
                                        .ToDictionary(
                                            g => g.Key,
                                            g => Math.Round(g.Average(e => e.Note.Value), 2)
                                        );

            ViewBag.EntretienAnnuelNotes = entretienAnnuelNotes;

            // Calculate the overall average note for all employees
            // Calculate the overall average note for all employees
            double? overallAverageNote = db.EntretienAnnuel
                                            .Where(e => e.Note != null)
                                            .Average(e => e.Note);

            if (overallAverageNote.HasValue)
            {
                ViewBag.OverallAverageNote = Math.Round(overallAverageNote.Value, 2);
            }
            else
            {
                ViewBag.OverallAverageNote = 0; // Ou toute autre valeur par défaut que vous préférez
            }

            // Calculate the number of employees with the same number of stars
            var ficheFonctions = db.FicheFonction.ToList();

            int[] starCounts = new int[6];

            foreach (var fiche in ficheFonctions)
            {
                if (ViewBag.EntretienAnnuelNotes.ContainsKey(fiche.Matricule))
                {
                    var note = Convert.ToDouble(ViewBag.EntretienAnnuelNotes[fiche.Matricule]);
                    var roundedNote = Math.Round(note);
                    var filledStars = (int)Math.Floor(roundedNote / 4.0);

                    starCounts[filledStars]++;
                }
            }

            ViewBag.StarCounts = starCounts;

            // Calculate the number of deleted employees from FicheFonction
            var deletedEmployeesCount = db.DeletedFicheFonctions.Select(df => df.Matricule).Distinct().Count();
            ViewBag.DeletedEmployeesCount = deletedEmployeesCount;

            // Calculate the number of existing FicheFonctions
            var existingFicheFonctionsCount = db.FicheFonction.Count();
            ViewBag.ExistingFicheFonctionsCount = existingFicheFonctionsCount;

            // Get current user's matricule
            string currentUserMatricule = ProjectWith.Models.ath_windows.CurrentUser.getMatricule();

            // Calculate the number of interviews passed by the current employee
            int interviewCount = db.EntretienAnnuel.Count(e => e.Matricule == currentUserMatricule);
            ViewBag.InterviewCount = interviewCount;

            // Fetch the user's notes over the years
            var userNotes = db.EntretienAnnuel
                              .Where(e => e.Matricule == currentUserMatricule && e.Note != null)
                              .OrderBy(e => e.DateEntretien)
                              .Select(e => new
                              {
                                  Year = e.DateEntretien.Year,
                                  e.Note
                              })
                              .ToList();

            ViewBag.UserNotes = userNotes;

            return View();
        }


    }
}

