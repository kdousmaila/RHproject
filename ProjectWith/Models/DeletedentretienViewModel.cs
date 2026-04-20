using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectWith.Models
{
	public class DeletedentretienViewModel
	{

		public DeletedEntretienAnnuel deletedentretienAnnuels { get; set; }
		public IEnumerable<deletedAutoEvaluation> deletedAutoEvaluations { get; set; }
		public IEnumerable<deletedEvalGen> deletedevaluatinGenN_1s { get; set; }
		public IEnumerable<deletedObjV> deletedobjectifsAVenirs { get; set; }
		public IEnumerable<deletedObjSpecifique> deletedobjSpecifiques { get; set; }
	}
}