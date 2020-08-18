using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Team.Rehab.InterfaceRepository;
using Team.Rehab.Repository.Repository;
using Team.Rehab.DataModel;
using Team.Rehab.WebApi.ErrorHelper;
using Team.Rehab.InterfaceRepository.Triarq;
using System.Collections.Generic;
using Team.Rehab.BusinessEntities;
using Newtonsoft.Json.Linq;

namespace Team.Rehab.WebApi.Areas.TriArq.Controllers
{
     [RoutePrefix("api/TriPatient")]
    public class TriPatientController : ApiController
    {

        ITPatientRepository _patientRepository;
        public TriPatientController(ITPatientRepository patientRepository)
        {

            this._patientRepository = patientRepository;
        }
        //public PatientController()
        //{

           
        //}
        [HttpGet]
        [Route("~/FHIR/api/Patient/Search")]
        //[Authorize]
        public JArray Search(string FirstName,string LastName,string BirthDate,string NPINumber)
        {
            TriPatientDemographic model = new TriPatientDemographic();
            JArray jsonResponse = new JArray();
            List<PatientEntity> patientEntity = new List<PatientEntity>();
            jsonResponse = _patientRepository.GetPatientList(FirstName, LastName, BirthDate, NPINumber);
            return jsonResponse;
          //  return _patientRepository.GetPatients();
        }
    }
}
