using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
    public class DT_Outgoing_Message
    {

        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }

        public string MessageBody { get; set; }


        public Nullable<System.DateTime> Sent_Timestamp { get; set; }
    }
}
