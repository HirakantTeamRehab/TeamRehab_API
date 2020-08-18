using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
    public class DT_POC_NotFoundEntity
    {
        public int ID { get; set; }
        public Nullable<int> XMLContent { get; set; }
        public Nullable<int> Patient_ID { get; set; }
        public Nullable<int> Note_ID { get; set; }
        public Nullable<int> Institute_ID { get; set; }
        public string POC_Status { get; set; }
    }
}
