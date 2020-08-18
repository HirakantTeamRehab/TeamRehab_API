using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
    public class DNDocTreatDesc
    {
        public int treatrowid { get; set; }
        public string treatdesc { get; set; }
        public int docrowid { get; set; }
        public string DateOfService { get; set; } = null;
    }
}
