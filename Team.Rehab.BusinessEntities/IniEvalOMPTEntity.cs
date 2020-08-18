using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
   public class IniEvalOMPTEntity
    {
        public Int16 rowid { get; set; }
        public string Description { get; set; }
        public string TestPAN { get; set; }
          public string TestID { get; set; }
        public string TestWB { get; set; }
        public string TestCP { get; set; }
        public Int16 OMPTrowid { get; set; }
        public string assessment { get; set; }
    }
}
