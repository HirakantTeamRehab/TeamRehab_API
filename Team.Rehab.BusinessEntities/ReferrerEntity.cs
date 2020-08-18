using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
   public class ReferrerEntity
    {
        public int Rrowid { get; set; }

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PrintName { get; set; }

        //public string StreetAddress { get; set; }

        //public string City { get; set; }

        //public string State { get; set; }

        //public string ZipCode { get; set; }

        public string NPINumber { get; set; }

        public string PhoneNo { get; set; }

        public string FaxNo { get; set; }

        public string Email { get; set; }

        public string ReferralType { get; set; }

        public string SendPOCBy { get; set; }

        public string createdby { get; set; }

        public System.DateTime createdts { get; set; }

        public string updatedby { get; set; }

        public System.DateTime updatedts { get; set; }

        public string Credentials { get; set; }

        public Nullable<bool> Inactive { get; set; }

        public Nullable<System.DateTime> InactiveDate { get; set; }

        public string AltContInfo { get; set; }
    }
}
