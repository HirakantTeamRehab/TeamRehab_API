using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
  public  class NotificationsEntity
    {

        public int NotificationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AlertMessage { get; set; }
        public string ModifiedDate { get; set; }
    }
}
