using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
  public  class ReferralSignDetailsEntity
    {
      

        public string EmailID { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime SignedDate { get; set; }
        public string NoteType { get; set; }
        public Boolean Sign { get; set; }
        public string Signiture { get; set; }
        public string PDFPath { get; set; }

    }
}
