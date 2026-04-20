using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectWith.Models
{
    public class EmployeeFicheFonctionViewModel
    {
        public Employee Employee { get; set; }
        public FicheFonction FicheFonction { get; set; }
        public permission permission { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public IEnumerable<Departement> Departements { get; set; }
        public string SelectedEmployeeChef { get; set; }
        public string EmployeeDS { get; set; }
        public string SelectedRole { get; set; }
    }
}