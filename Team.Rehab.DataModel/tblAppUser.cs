//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Team.Rehab.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblAppUser
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
