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
    
    public partial class tblAppNotification
    {
        public int Nrowid { get; set; }
        public Nullable<int> Prowid { get; set; }
        public Nullable<int> Docrowid { get; set; }
        public Nullable<int> Frowid { get; set; }
        public Nullable<bool> IsViewed { get; set; }
        public string AlertMsg { get; set; }
        public string AlertType { get; set; }
        public Nullable<int> NPINumber { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> CreatedTs { get; set; }
        public Nullable<System.DateTime> UpdatedTs { get; set; }
    
        public virtual tblDocMaster tblDocMaster { get; set; }
        public virtual tblPatients tblPatients { get; set; }
        public virtual tblPatients tblPatients1 { get; set; }
    }
}
