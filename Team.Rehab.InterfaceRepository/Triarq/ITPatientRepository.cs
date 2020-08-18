using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.BusinessEntities;

namespace Team.Rehab.InterfaceRepository.Triarq
{
    public interface ITPatientRepository
    {
        JArray GetPatientList(string firstName, string lastName, string dateOfBirth, string NPINumber);
    }
}
