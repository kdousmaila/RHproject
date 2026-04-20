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
    public class DeletedFicheFonctionsController : Controller
    {
        private baseEntities db = new baseEntities();

        // GET: DeletedFicheFonctions
        public ActionResult Index(string searchLetter, int pageNo = 1)
        {

            // Get the role and matricule of the current user
            string role = CurrentUser.getRole();
            //string currentUserMatricule = CurrentUser.getMatricule();

            // Check if the user has the required role to access this action
            if (role != "RH" && role != "DS")
            {
               
                    // Redirect the user to an unauthorized page or action
                    return RedirectToAction("Unauthorized", "Error");
                
            }

            // Récupérer les éléments DeletedFicheFonctions de la base de données
            var deletedFicheFonctions = db.DeletedFicheFonctions.AsQueryable();

            // Si une lettre de recherche est fournie, filtrer les éléments par cette lettre
            if (!string.IsNullOrEmpty(searchLetter))
            {
                // Convertir la lettre de recherche en minuscules
                string searchLower = searchLetter.ToLower();

                // Filtrer les éléments en mémoire après avoir récupéré les données de la base de données
                deletedFicheFonctions = deletedFicheFonctions.Where(p => p.nom.ToLower().StartsWith(searchLower) ||
                                                                         p.prenom.ToLower().StartsWith(searchLower)
                                                                          ||
                                                                         p.Matricule.ToLower().StartsWith(searchLower));
            }

            // Pagination
            int noOfRecordsPerPage = 7;
            int totalCount = deletedFicheFonctions.Count();
            int noOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalCount) / Convert.ToDouble(noOfRecordsPerPage)));
            int noOfRecordsToSkip = (pageNo - 1) * noOfRecordsPerPage;

            // Paginer les résultats de la requête
            var deletedFicheFonctionsPaged = deletedFicheFonctions
                                              .OrderByDescending(e => e.nom) // Par exemple, ordonner par date d'entretien
                                              .Skip(noOfRecordsToSkip)
                                              .Take(noOfRecordsPerPage)
                                              .ToList();

            // Passer les données de pagination à la vue
            ViewBag.PageNo = pageNo;
            ViewBag.NoOfPages = noOfPages;
            ViewBag.TotalCount = totalCount;

            // Retourner la vue avec les données paginées
            return View(deletedFicheFonctionsPaged);
        }


        // GET: DeletedFicheFonctions/Details/5
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

            DeletedFicheFonctions deletedFicheFonction = db.DeletedFicheFonctions.Find(id);
            if (deletedFicheFonction == null)
            {
                return HttpNotFound();
            }

            var DeletedFicheViewModel = new DeletedFicheViewModel();
            DeletedFicheViewModel.DeletedFicheFonctions = deletedFicheFonction; // Correction ici
            DeletedFicheViewModel.DeletedDiplomes = db.DeletedDiplome.Where(d => d.id == id).ToList();
            DeletedFicheViewModel.DeletedFormations = db.DeletedFormation.Where(d => d.id == id).ToList();
            DeletedFicheViewModel.DeletedExperiences = db.DeletedExperience.Where(d => d.id == id).ToList();
            DeletedFicheViewModel.DeletedCompetences = db.DeletedCompetence.Where(d => d.id == id).ToList();

            return View(DeletedFicheViewModel);
        }

        // GET: DeletedFicheFonctions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DeletedFicheFonctions/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FicheFonctionId,DeletedAt,nom,prenom,email,ville,numero,etatCivil,numCin,dateNaissance,poste,dateRecrutement,image,Matricule")] DeletedFicheFonctions deletedFicheFonction)
        {
            if (ModelState.IsValid)
            {
                db.DeletedFicheFonctions.Add(deletedFicheFonction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(deletedFicheFonction);
        }

        // GET: DeletedFicheFonctions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeletedFicheFonctions deletedFicheFonction = db.DeletedFicheFonctions.Find(id);
            if (deletedFicheFonction == null)
            {
                return HttpNotFound();
            }
            return View(deletedFicheFonction);
        }

        // POST: DeletedFicheFonctions/Edit/5
  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FicheFonctionId,DeletedAt,nom,prenom,email,ville,numero,etatCivil,numCin,dateNaissance,poste,dateRecrutement,image,Matricule")] DeletedFicheFonctions deletedFicheFonction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deletedFicheFonction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(deletedFicheFonction);
        }

        // GET: DeletedFicheFonctions/Delete/5
        public ActionResult Delete(int? id)
        {

            string role = CurrentUser.getRole();
            if (role != "RH" && role != "DS")
            {

                // Redirect the user to an unauthorized page or action
                return RedirectToAction("Unauthorized", "Error");

            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeletedFicheFonctions deletedFicheFonction = db.DeletedFicheFonctions.Find(id);
            if (deletedFicheFonction == null)
            {
                return HttpNotFound();
            }
            return View(deletedFicheFonction);
        }

        // POST: DeletedFicheFonctions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
    

    
        public ActionResult DeleteConfirmed(int id)
        {
            DeletedFicheFonctions deletedFicheFonction = db.DeletedFicheFonctions.Find(id);

            // Fetch related entities
            var deletedFormations = db.DeletedFormation.Where(df => df.idDeletedFiche == id).ToList();
            var deletedDiplomes = db.DeletedDiplome.Where(dd => dd.idDeletedFiche == id).ToList();
            var deletedCompetences = db.DeletedCompetence.Where(dc => dc.idDeletedFiche == id).ToList();
            var deletedExperiences = db.DeletedExperience.Where(de => de.idDeletedFiche == id).ToList();

            // Delete related entities
            db.DeletedFormation.RemoveRange(deletedFormations);
            db.DeletedDiplome.RemoveRange(deletedDiplomes);
            db.DeletedCompetence.RemoveRange(deletedCompetences);
            db.DeletedExperience.RemoveRange(deletedExperiences);

            // Fetch and delete associated employee if exists
            DeletedEmployees deletedEmployee = db.DeletedEmployees.FirstOrDefault(de => de.Matricule == deletedFicheFonction.Matricule);
            if (deletedEmployee != null)
            {
                db.DeletedEmployees.Remove(deletedEmployee);
            }

            // Finally, remove the deleted fiche fonction
            db.DeletedFicheFonctions.Remove(deletedFicheFonction);
            var deletedEntretiens = db.DeletedEntretienAnnuel.Where(e => e.Matricule == deletedFicheFonction.Matricule).ToList();
            foreach (var deletedEntretien in deletedEntretiens)
            {
                // Fetch related ObjSpecifique entities
                var deletedObjSpecifiques = db.deletedObjSpecifique.Where(o => o.idDeletedEntretien == deletedEntretien.id).ToList();
                db.deletedObjSpecifique.RemoveRange(deletedObjSpecifiques);

                var deletedautoEvals = db.deletedAutoEvaluation.Where(o => o.idDeletedEntretien == deletedEntretien.id).ToList();
                db.deletedAutoEvaluation.RemoveRange(deletedautoEvals);

                var deletedEvalGen = db.deletedEvalGen.Where(o => o.idDeletedEntretien == deletedEntretien.id).ToList();
                db.deletedEvalGen.RemoveRange(deletedEvalGen);

                var deletedObjV = db.deletedObjV.Where(o => o.idDeletedEntretien == deletedEntretien.id).ToList();
                db.deletedObjV.RemoveRange(deletedObjV);


                // Remove the DeletedEntretienAnnuel
                db.DeletedEntretienAnnuel.Remove(deletedEntretien);
            }



            db.SaveChanges();
            return RedirectToAction("Index");
        }





        public ActionResult Restore(int id)
        {
            DeletedFicheFonctions deletedFicheFonction = db.DeletedFicheFonctions.Find(id);
            if (deletedFicheFonction == null)
            {
                return HttpNotFound();
            }

            DeletedEmployees deletedEmployee = db.DeletedEmployees.FirstOrDefault(e => e.Matricule == deletedFicheFonction.Matricule);
            if (deletedEmployee != null)
            {
                Employee employee = new Employee
                {
                    Matricule = deletedEmployee.Matricule,
                    Nom = deletedEmployee.Nom,
                    Prenom = deletedEmployee.Prenom,
                    fonction = deletedEmployee.fonction,
                    Responsable = deletedEmployee.Responsable,
                    email = deletedEmployee.email,
                    CmptWin = deletedEmployee.CmptWin,
                    dateEntree = deletedEmployee.dateEntree,
                    Service = deletedEmployee.Service,
                    ResponsableFonctionnel = deletedEmployee.ResponsableFonctionnel
                };

                db.Employee.Add(employee);
                db.SaveChanges();
                var roleId = db.Role.Where(r => r.libelle.ToLower().Contains("emp")).Select(r => r.id).FirstOrDefault();
                var permission = new permission
                {
                    MatriculeEmp = employee.Matricule,
                    IdRole = roleId // Assuming role ID for "employee" is 6
                };
                db.permission.Add(permission);
                db.SaveChanges();
                db.DeletedEmployees.Remove(deletedEmployee);
                db.SaveChanges();
            }

            FicheFonction ficheFonction = new FicheFonction
            {
                id = deletedFicheFonction.FicheFonctionId,
                nom = deletedFicheFonction.nom,
                prenom = deletedFicheFonction.prenom,
                email = deletedFicheFonction.email,
                ville = deletedFicheFonction.ville,
                numero = deletedFicheFonction.numero,
                etatCivil = deletedFicheFonction.etatCivil,
                numCin = deletedFicheFonction.numCin,
                dateNaissance = deletedFicheFonction.dateNaissance,
                poste = deletedFicheFonction.poste,
                dateRecrutement = deletedFicheFonction.dateRecrutement,
                image = deletedFicheFonction.image,
                Matricule = deletedFicheFonction.Matricule,
                idDepartement = deletedFicheFonction.idDepartement,
                ResponsableFonctionnel = deletedFicheFonction.Responsable,
                cmptWindows = deletedFicheFonction.cmptWindows,
            };

            db.FicheFonction.Add(ficheFonction);
            db.SaveChanges();

            var deletedDiplomas = db.DeletedDiplome.Where(d => d.idFiche == deletedFicheFonction.FicheFonctionId).ToList();
            foreach (var deletedDiploma in deletedDiplomas)
            {
                Diplome diploma = new Diplome
                {
                    idFiche = ficheFonction.id,
                    NomDimplome = deletedDiploma.NomDiplome,
                    DateDebut = deletedDiploma.DateDebut,
                    lieu = deletedDiploma.lieu,
                    DateFin = deletedDiploma.DateFin,
                    PDF = deletedDiploma.PDF,
                };

                db.Diplome.Add(diploma);
            }

            var deletedFormations = db.DeletedFormation.Where(d => d.idFiche == deletedFicheFonction.FicheFonctionId).ToList();
            foreach (var deletedFormation in deletedFormations)
            {
                Formation formation = new Formation
                {
                    nom = deletedFormation.nom,
                    dateDebut = deletedFormation.dateDebut,
                    lieu = deletedFormation.lieu,
                    dateFin = deletedFormation.dateFin,
                    PDF = deletedFormation.PDF,
                    idFiche = ficheFonction.id,
                };

                db.Formation.Add(formation);
            }

            var deletedCompeteces = db.DeletedCompetence.Where(d => d.idFiche == deletedFicheFonction.FicheFonctionId).ToList();
            foreach (var deletedCompetece in deletedCompeteces)
            {
                competence competence = new competence
                {
                    nom = deletedCompetece.nom,
                    type = deletedCompetece.type,
                    niveau = deletedCompetece.niveau,
                    PDF = deletedCompetece.PDF,
                    idFiche = ficheFonction.id,
                };

                db.competence.Add(competence);
            }

            var deletedExperiences = db.DeletedExperience.Where(d => d.idFiche == deletedFicheFonction.FicheFonctionId).ToList();
            foreach (var deletedExperience in deletedExperiences)
            {
                Experience experience = new Experience
                {
                    Titre = deletedExperience.Titre,
                    dateDebut = deletedExperience.dateDebut,
                    lieu = deletedExperience.lieu,
                    dateFin = deletedExperience.dateFin,
                    PDF = deletedExperience.PDF,
                    idFiche = ficheFonction.id,
                };

                db.Experience.Add(experience);
            }

            var deletedEntretiens = db.DeletedEntretienAnnuel.Where(e => e.Matricule == deletedFicheFonction.Matricule).ToList();
            foreach (var deletedEntretien in deletedEntretiens)
            {
                EntretienAnnuel entretien = new EntretienAnnuel
                {
                    Nom = deletedEntretien.Nom,
                    Prenom = deletedEntretien.Prenom,
                    LibelleFonction = deletedEntretien.LibelleFonction,
                    DateEntree = deletedEntretien.DateEntree,
                    DateDerniereRevue = deletedEntretien.DateDerniereRevue,
                    Service = deletedEntretien.Service,
                    NomRespHierarchiqueN_1 = deletedEntretien.NomRespHierarchiqueN_1,
                    NomRespFonctionel = deletedEntretien.NomRespFonctionel,
                    DateEntretien = deletedEntretien.DateEntretien,
                    autoEvalSal1 = deletedEntretien.autoEvalSal1,
                    competenceCaractUtiles = deletedEntretien.competenceCaractUtiles,
                    competenceCaractAcquerir = deletedEntretien.competenceCaractAcquerir,
                    question1 = deletedEntretien.question1,
                    orientationResulttat = deletedEntretien.orientationResulttat,
                    agilite_Adaptabilite = deletedEntretien.agilite_Adaptabilite,
                    ouvertureEsprit = deletedEntretien.ouvertureEsprit,
                    qualitesRelationnelles = deletedEntretien.qualitesRelationnelles,
                    travailEquipe = deletedEntretien.travailEquipe,
                    ResolutionProbleme = deletedEntretien.ResolutionProbleme,
                    gestionProjet = deletedEntretien.gestionProjet,
                    communication = deletedEntretien.communication,
                    Leadership = deletedEntretien.Leadership,
                    gestionBudget = deletedEntretien.gestionBudget,
                    charisme = deletedEntretien.charisme,
                    maitriseSoi = deletedEntretien.maitriseSoi,
                    ResultatActFormation = deletedEntretien.ResultatActFormation,
                    FormationMesureDev = deletedEntretien.FormationMesureDev,
                    ResourceNecessaire = deletedEntretien.ResourceNecessaire,
                    comntSouhaitEvolution = deletedEntretien.comntSouhaitEvolution,
                    comntSouhaitMobilite = deletedEntretien.comntSouhaitMobilite,
                    comntProchaineEtape = deletedEntretien.comntProchaineEtape,
                    dateProchainEnretien = deletedEntretien.dateProchainEnretien,
                    comntGenereaux = deletedEntretien.comntGenereaux,
                    Signature1RespN_1 = deletedEntretien.Signature1RespN_1,
                    date1 = deletedEntretien.date1,
                    comntSalaire = deletedEntretien.comntSalaire,
                    signature2Salarie = deletedEntretien.signature2Salarie,
                    date2 = deletedEntretien.date2,
                    AvisN_2 = deletedEntretien.AvisN_2,
                    NomSignatureN_2 = deletedEntretien.NomSignatureN_2,
                    date3 = deletedEntretien.date3,
                    AvisRespFonctionel = deletedEntretien.AvisRespFonctionel,
                    NomSignatureResFonct_Depart = deletedEntretien.NomSignatureResFonct_Depart,
                    date4 = deletedEntretien.date4,
                    comOrientationResulttat = deletedEntretien.comOrientationResulttat,
                    ComAgilite = deletedEntretien.ComAgilite,
                    comOuv = deletedEntretien.comOuv,
                    comQualité = deletedEntretien.comQualité,
                    comEquipe = deletedEntretien.comEquipe,
                    comResolP = deletedEntretien.comResolP,
                    comGP = deletedEntretien.comGP,
                    comCommunication = deletedEntretien.comCommunication,
                    comLeadership = deletedEntretien.comLeadership,
                    comGBud = deletedEntretien.comGBud,
                    comMaitriseSoi = deletedEntretien.comMaitriseSoi,
                    comCharisme = deletedEntretien.comCharisme,
                    c1 = deletedEntretien.c1,
                    c2 = deletedEntretien.c2,
                    c3 = deletedEntretien.c3,
                    c4 = deletedEntretien.c4,
                    comC1 = deletedEntretien.comC1,
                    comC2 = deletedEntretien.comC2,
                    comC3 = deletedEntretien.comC3,
                    comC4 = deletedEntretien.comC4,
                    Matricule = deletedEntretien.Matricule,
                    Note = deletedEntretien.Note,
                    id= (int)deletedEntretien.deletedId,
                    checkC1 = deletedEntretien.checkC1,
                    checkC2 = deletedEntretien.checkC2,
                    checkC3 = deletedEntretien.checkC3,
                    checkC4 = deletedEntretien.checkC4,
                    status1=deletedEntretien.status1,
                    status2=deletedEntretien.status2,
                    status3=deletedEntretien.status3,
                    dateCreation=deletedEntretien.dateCreation,
                    dateEnvoi=deletedEntretien.dateEnvoi,
                    dateValidation=deletedEntretien.dateValidation

                };

                db.EntretienAnnuel.Add(entretien);
                db.SaveChanges();

                // Restore related tables (ObjSpecifique, AutoEvaluation, etc.)
                // Your restoration code for related tables here

                var deletedObjSpecifiques = db.deletedObjSpecifique.Where(o => o.idDeletedEntretien == deletedEntretien.id).ToList();
                foreach (var deletedObjSpecifique in deletedObjSpecifiques)
                {
                    ObjSpecifique objSpecifique = new ObjSpecifique
                    {
                        ObjSpecifique1 = deletedObjSpecifique.ObjSpecifique,
                        Synthese = deletedObjSpecifique.Synthese,
                        idEntretien = entretien.id,
                        
                    };

                    db.ObjSpecifique.Add(objSpecifique);
                }

                var deletedAutoEvaluations = db.deletedAutoEvaluation.Where(a => a.idDeletedEntretien == deletedEntretien.id).ToList();
                foreach (var deletedAutoEvaluation in deletedAutoEvaluations)
                {
                    AutoEvaluation autoEvaluation = new AutoEvaluation
                    {

                        theme = deletedAutoEvaluation.theme,
                        lieu = deletedAutoEvaluation.lieu,
                        duree = deletedAutoEvaluation.duree,
                        idEntretien = entretien.id,

                    };

                    db.AutoEvaluation.Add(autoEvaluation);
                }


                var deletedEvalGens = db.deletedEvalGen.Where(a => a.idDeletedEntretien == deletedEntretien.id).ToList();
                foreach (var deletedEvalGen in deletedEvalGens)
                {
                    EvaluatinGenN_1 eval = new EvaluatinGenN_1
                    {
                        performance = deletedEvalGen.performance,
                        commentaire = deletedEvalGen.commentaire,
                        tendance = deletedEvalGen.tendance,

                        idEntretien = entretien.id,

                    };

                    db.EvaluatinGenN_1.Add(eval);
                }


                var deletedObjVs = db.deletedObjV.Where(a => a.idDeletedEntretien == deletedEntretien.id).ToList();
                foreach (var deletedObjV in deletedObjVs)
                {
                    ObjectifsAVenir objv = new ObjectifsAVenir
                    {
                        ObjectifsPrincipeauxSmart = deletedObjV.ObjectifsPrincipeauxSmart,
                        indicateur = deletedObjV.indicateur,
                        ARealiserEnT1__T4 = deletedObjV.ARealiserEnT1__T4,
                        idEntretien = entretien.id,


                    };

                    db.ObjectifsAVenir.Add(objv);
                }

                // Remove from DeletedEntretienAnnuel
                db.DeletedEntretienAnnuel.Remove(deletedEntretien);
            }

           


            db.DeletedFicheFonctions.Remove(deletedFicheFonction);
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
