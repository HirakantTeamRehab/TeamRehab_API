using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
   public class NotesSummaryEntity
    {
        public int Docrowid { get; set; }
        public string NotesType { get; set; }
        public string DateOfService { get; set; }
        public string FirstName { get; set; }


        public string LastName { get; set; }
        public string TherpistPhone { get; set; }
        public string TherapistEmail { get; set; }
        public string ClinicName { get; set; }
        public string ClinicEmail { get; set; }

        public string ClinicPhone { get; set; }

    }
}
