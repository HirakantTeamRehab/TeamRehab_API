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
    
    public partial class tblOTP
    {
        public short OtpId { get; set; }
        public string PhoneNumber { get; set; }
        public string OTPCode { get; set; }
        public System.DateTime IssuedUtc { get; set; }
    }
}
