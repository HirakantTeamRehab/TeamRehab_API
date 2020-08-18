using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
  public  class TriPatientNotesEntity
    {
        public int Docrowid { get; set; }
       
        public string NoteDet { get; set; }
        public string NoteType { get; set; }

        public Nullable<System.DateTime> DateOfService { get; set; }
      
        public string SignedDet { get; set; }
        public Nullable<System.DateTime> SignedDate { get; set; }
       
        public string UserId { get; set; }
       
        public Nullable<int> PTrowid { get; set; }
  
    }
}
