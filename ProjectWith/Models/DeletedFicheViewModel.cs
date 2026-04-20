using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectWith.Models
{
    public class DeletedFicheViewModel
    {
        public DeletedFicheFonctions DeletedFicheFonctions { get; set; }
        public IEnumerable<DeletedDiplome> DeletedDiplomes { get; set; }
        public IEnumerable<DeletedExperience> DeletedExperiences { get; set; }
        public IEnumerable<DeletedFormation> DeletedFormations { get; set; }
        public IEnumerable<DeletedCompetence> DeletedCompetences { get; set; }

        //public IEnumerable<Role> Roles { get; set; }
    }
}
