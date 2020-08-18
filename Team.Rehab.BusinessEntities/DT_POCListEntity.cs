using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
    public class DT_POCListEntity
    {

        public List<DT_PatientReferral_ProcessedEntity> pocProcessed { get; set; }
        public List<DT_PatientReferral_ProcessedEntity> pocDeclined { get; set; }
    }
}
