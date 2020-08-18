using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
public    class OTPEntity
    {
        public string PhoneNumber { get; set; }
        public string OTPCode { get; set; }
        public DateTime IssuedUtc { get; set; }
    }
}
