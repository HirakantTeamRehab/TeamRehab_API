using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
   public class DT_PatientsProcessedEntity
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string DOB { get; set; }
        public bool? MatchFound { get; set; }
        public string ReceptionistComments { get; set; }
        public string XMLContent { get; set; }
    }
}
