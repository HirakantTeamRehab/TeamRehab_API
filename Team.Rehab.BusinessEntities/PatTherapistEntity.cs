using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
  public  class PatTherapistEntity
    {
      


        public string Credentials { get; set; }
        public string LicenseNumber { get; set; }
        public string Name { get; set; }
        public byte[] SignatureFile { get; set; }
        public string Path { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NPI { get; set; }
        

    }
}
