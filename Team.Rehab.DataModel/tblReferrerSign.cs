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
    
    public partial class tblReferrerSign
    {
        public int Refrowid { get; set; }
        public Nullable<int> PTrowid { get; set; }
        public Nullable<int> Docrowid { get; set; }
        public Nullable<int> NPINumber { get; set; }
        public Nullable<bool> Signed { get; set; }
        public Nullable<System.DateTime> SignedDate { get; set; }
        public string NoteType { get; set; }
        public string PDFName { get; set; }
        public string createdby { get; set; }
        public Nullable<System.DateTime> createdts { get; set; }
        public string updatedby { get; set; }
        public Nullable<System.DateTime> updatedts { get; set; }
    }
}
