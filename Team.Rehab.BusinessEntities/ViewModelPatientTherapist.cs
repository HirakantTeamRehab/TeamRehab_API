using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
    public class ViewModelPatientTherapist
    {
        public List<PatientEntity> PatientList { get; set; }
        public List<TherapistEntity> Therapist { get; set; }
    }
}
