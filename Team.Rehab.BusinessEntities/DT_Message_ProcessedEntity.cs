using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
    public class DT_Message_ProcessedEntity
    {
        public int ID { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string File_Selected_By_User { get; set; }
        public string MessageBody { get; set; }
        public Nullable<System.DateTime> Received_TimeStamp { get; set; }
        public Nullable<System.DateTime> Processed_TimeStamp { get; set; }
        public Nullable<int> PatientID { get; set; }
        public Nullable<int> Institute_ID { get; set; }
        public string Status { get; set; }
        public string UserID { get; set; }
        public byte[] Attachment { get; set; }
    }
}
