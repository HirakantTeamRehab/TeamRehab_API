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
    
    public partial class tblUserGroup
    {
        public short UGrowid { get; set; }
        public string UserGroup { get; set; }
        public string createdby { get; set; }
        public System.DateTime createdts { get; set; }
        public string updatedby { get; set; }
        public System.DateTime updatedts { get; set; }
        public Nullable<bool> Inactive { get; set; }
        public Nullable<System.DateTime> InactiveDate { get; set; }
    }
}
