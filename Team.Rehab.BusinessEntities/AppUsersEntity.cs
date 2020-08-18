using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
   public class AppUsersEntity
    {


        public int AUrowid { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailId { get; set; }

        public Nullable<int> NPINumber { get; set; }

        public Nullable<bool> Access { get; set; }

        public string createdby { get; set; }

        public System.DateTime createdts { get; set; }

        public string updatedby { get; set; }

        public System.DateTime updatedts { get; set; }
    }
}
