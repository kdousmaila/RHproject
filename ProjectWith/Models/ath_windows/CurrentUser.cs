using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectWith.Models.ath_windows
{
    public class CurrentUser

    {



        public static Employee GetCurrentUser(string CmptWin)

        {

            baseEntities db = new baseEntities();



            CmptWin = CmptWin.Replace("\\", "%");

            CmptWin = CmptWin.Substring(CmptWin.IndexOf("%") + 1, (CmptWin.Length) - (CmptWin.IndexOf("%") + 1));



            Employee UserConnected = db.Employee.Where(x => x.CmptWin == CmptWin).FirstOrDefault();



            return UserConnected;



        }



        public static string getName()

        {

            baseEntities db = new baseEntities();



            string UserComptWind = HttpContext.Current.User.Identity.Name;





            Employee emp = GetCurrentUser(UserComptWind);

            if (emp == null)

            {

                return UserComptWind;

            }

            else

            {

                return emp.Prenom;

            }





        }

        


        public static string getLastName()

        {

            baseEntities db = new baseEntities();



            string UserComptWind = HttpContext.Current.User.Identity.Name;





            Employee emp = GetCurrentUser(UserComptWind);

            if (emp == null)

            {

                return null;

            }

            else

            {

                return emp.Nom;

            }





        }



        public static string getRole()
        {
            baseEntities db = new baseEntities();

            string UserComptWind = HttpContext.Current.User.Identity.Name;


            Employee emp = GetCurrentUser(UserComptWind);


            List<Role> roles = new List<Role>();
        

            if (emp == null)
            {
                return string.Empty;
            }
            var RoleNames = (from u in db.Employee
                             where u.Matricule == emp.Matricule
                             join ru in db.permission on u.Matricule equals ru.MatriculeEmp
                             join sn in db.Role on ru.IdRole equals sn.id
                             select sn.libelle).ToList();


            foreach (var RoleName in RoleNames)
            {

                if (RoleName.Contains("RH"))
                {

                    return "RH";


                }
                else if (RoleName.Contains("DS"))
                {
                    return "DS";
                }
                else if (RoleName.Contains("CHEF"))
                {
                    return "CHEF";
                }
                else if (RoleName.Equals("Employee"))
                {
                    return "Employee";
                }
                else if (RoleName.Contains("EmployeeRh"))
                {
                    return "EmployeRh";
                }
                else
                {
                    return string.Empty;
                }

            }
            return string.Empty;
        }






        public static string getMatricule()

        {

            baseEntities db = new baseEntities();



            string UserComptWind = HttpContext.Current.User.Identity.Name;





            Employee emp = GetCurrentUser(UserComptWind);

            if (emp != null)

            {

                return emp.Matricule;

            }

            else

            {

                return null;

            }





        }

        public static string getEmail()
        {
            baseEntities db = new baseEntities();
            string UserComptWind = HttpContext.Current.User.Identity.Name;
            Employee emp = GetCurrentUser(UserComptWind);

            if (emp != null )
            {
                return emp.email;
            }
			else
			{
                return null;
			}
          

        }

        public static string getFonction()
        {
            baseEntities db = new baseEntities();
            string UserComptWind = HttpContext.Current.User.Identity.Name;
            Employee emp = GetCurrentUser(UserComptWind);

            if (emp != null )
            {
                return emp.fonction;
            }
            else
            {
                return null;
            }


        }

        public static string getDateEntree()
        {
            baseEntities db = new baseEntities();
            string UserComptWind = HttpContext.Current.User.Identity.Name;
            Employee emp = GetCurrentUser(UserComptWind);

            if (emp != null && emp.dateEntree.HasValue)
            {
                return emp.dateEntree.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                return null;
            }
        }

        public static string getService()

        {

            baseEntities db = new baseEntities();
            string UserComptWind = HttpContext.Current.User.Identity.Name;
            Employee emp = GetCurrentUser(UserComptWind);

            if (emp != null)

            {
                return emp.Service;
            }
            else
            {
                return null;
            }

        }


        public static string getResponsable()

        {

            baseEntities db = new baseEntities();
            string UserComptWind = HttpContext.Current.User.Identity.Name;
            Employee emp = GetCurrentUser(UserComptWind);

            if (emp != null)

            {
                return emp.Responsable;
            }
            else
            {
                return null;
            }

        }


        public static string getResponsableFonctionnel()

        {

            baseEntities db = new baseEntities();
            string UserComptWind = HttpContext.Current.User.Identity.Name;
            Employee emp = GetCurrentUser(UserComptWind);

            if (emp != null)

            {
                return emp.ResponsableFonctionnel;
            }
            else
            {
                return null;
            }

        }


        public static string getImagePath()
        {
            baseEntities db = new baseEntities();

            string UserComptWind = HttpContext.Current.User.Identity.Name;
            Employee emp = GetCurrentUser(UserComptWind);

            if (emp.image != null)
            {
                return "~/Content/image/" + emp.image; // Assuming the image path is stored in the 'image' property of the Employee entity
            }
            else
            {
                return null;
            }
        }

    }
}