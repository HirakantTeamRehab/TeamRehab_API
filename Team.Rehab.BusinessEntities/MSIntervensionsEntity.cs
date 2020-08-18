using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
  public  class MSIntervensionsEntity
    {
        public int rowid { get; set; }
        public string CPTCOde { get; set; }
        public string CPTDescription { get; set; }
        public string modifier { get; set; }
        public bool CPTTimed { get; set; }
        public int minutes { get; set; }
        public int units { get; set; }
        public string PrevMinuts { get; set; }
    }
}
