using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectWith.Models
{
	public class ViewModel
	{
		public FicheFonction FicheFonctions { get; set; }
		public IEnumerable<Diplome> Diplomes { get; set; }
		public IEnumerable<Experience> Experiences { get; set; }
		public IEnumerable<Formation> Formations { get; set; }
		public IEnumerable<competence> Competences { get; set; }

		public IEnumerable<Role> Roles { get; set; }
		public string SelectedEmployeeChef { get; set; }


	}
}