using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
   public class CommNoteSummaryEntity
    {
        public int noterowid { get; set; }
        public string notesummary { get; set; }
        public bool CreateWCPDF { get; set; }
    }
}
