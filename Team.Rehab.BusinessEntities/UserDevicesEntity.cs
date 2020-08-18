using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
    public class UserDevicesEntity
    {
        public int Drowid { get; set; }
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string DeviceId { get; set; }
        public string EmailId { get; set; }
        public byte[] SignPath { get; set; }
        public int NPINumber { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresUTC { get; set; }
        public string OTP { get; set; }
    }
}
