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
    
    public partial class tbl_DT_Incoming_Message
    {
        public int ID { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public Nullable<System.DateTime> Received { get; set; }
        public Nullable<bool> MessageProcessed { get; set; }
        public byte[] Attachment { get; set; }
        public Nullable<bool> IsRead { get; set; }
    }
}
