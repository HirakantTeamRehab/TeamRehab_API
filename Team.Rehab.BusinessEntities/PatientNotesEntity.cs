using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
   public class PatientNotesEntity
    {
        public int Docrowid { get; set; }
        public string NoteType { get; set; }
        public string NoteDet { get; set; }
        public short treatingTherapistId { get; set; }
        public string FriendlyName { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> DateOfService { get; set; }
        public bool Signed { get; set; }
        public string SignedDet { get; set; }
        public Nullable<System.DateTime> SignedDate { get; set; }
        public string PDFName { get; set; }
        public short AuthorizedBy { get; set; }
        // public string FriendlyName1 { get; set; }
        public string updatedby { get; set; }
        public string UserId { get; set; }
        public int BMrowid { get; set; }
        public string DoSNoteType { get; set; }
        public short markedaserror { get; set; }
        public string PDFShort { get; set; }
        public Nullable<int> PTrowid { get; set; }
        public string PrintName { get; set; }
        
    }
}
