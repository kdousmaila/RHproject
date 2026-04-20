using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectWith.Models
{
	public class entretienViewModel
	{
		public EntretienAnnuel entretienAnnuels { get; set; }
		public IEnumerable<AutoEvaluation> AutoEvaluations { get; set; }
		public IEnumerable<EvaluatinGenN_1> evaluatinGenN_1s { get; set; }
		public IEnumerable<ObjectifsAVenir> objectifsAVenirs { get; set; }
		public IEnumerable<ObjSpecifique> objSpecifiques { get; set; }
		public IEnumerable<Departement> Departements { get; set; }

		//	public IEnumerable<Note> Notes { get; set; }
	}
}