using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
    public class DT_IncomingMessagesEntity
    {
        public int ID { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }       
        public string MessageBody { get; set; }
        public Nullable<System.DateTime> Received { get; set; }
        public bool? MessageProcessed { get; set; }

        public string Operation { get; set; }
        public bool? IsRead { get; set; }
        
    }
}
