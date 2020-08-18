using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Team.Rehab.InterfaceRepository;
using Team.Rehab.Repository.Repository;
using Team.Rehab.DataModel;
using Team.Rehab.WebApi.ErrorHelper;
using Team.Rehab.BusinessEntities;

namespace Team.Rehab.WebApi.Controllers
{
    public class PatientController : ApiController
    {
        
        IPatientRepository _patientRepository;
        public PatientController(IPatientRepository patientRepository)
        {

            this._patientRepository = patientRepository;
        }
        [HttpGet]
        [Route("~/api/Patient/GetPatientsList")]
       // [Authorize]
        public List<tblPatients> GetPatientsList()
        {
            return _patientRepository.GetPatients();
        }

        // GET: api/Patient/5
        [HttpGet]
        [Route("~/api/Patient/GetPatientbyID/{noteId}")]
        [ResponseType(typeof(tblPatients))]
      //  [Authorize]
        public IHttpActionResult GetPatientbyID(int id)
        {
            tblPatients tblPatient = _patientRepository.GetPatientbyID(id);
            if (tblPatient == null)
            {
                throw new ApiDataException(1002, "No patient found for this id.", HttpStatusCode.InternalServerError);

            }

            return Ok(tblPatient);
        }
        [HttpGet]
        [Route("~/api/Patient/GetPatientsbyNPINumber/{NPINumber}")]
        public IHttpActionResult GetPatientsbyNPINumber(string NPINumber)
        {
            List<PatientEntity> tblPatient = _patientRepository.GetPatientsbyNPINumber(NPINumber);
            if (tblPatient.Count<1)
            {
                throw new ApiDataException(1002, "No patient found for this id.", HttpStatusCode.InternalServerError);

            }

            return Ok(tblPatient);
        }
        [HttpGet]
        [Route("~/api/Patient/PatientsTherapists/{NPINumber}")]
        public IHttpActionResult GetPatientsTherapists(string NPINumber)
        {
            // string NPINumber = string.Empty;
            ViewModelPatientTherapist patientsTherapists = _patientRepository.GetPatientsTherapist(NPINumber);
            if (patientsTherapists.PatientList == null && patientsTherapists.Therapist == null)
            {
                throw new ApiDataException(1002, "No patient/Therapist found .", HttpStatusCode.InternalServerError);

            }
          else  if (patientsTherapists.PatientList.Count < 1 && patientsTherapists.Therapist.Count < 0)
                {
                    throw new ApiDataException(1002, "No patient/Therapist found .", HttpStatusCode.InternalServerError);

                }
          

            return Ok(patientsTherapists);
        }


        [HttpGet]
        [Route("~/api/Patient/PatientsSchedules/{PatientId}")]
        public IHttpActionResult GetPatientsSchedules(int PatientId)
        {
            List<PatientSchedulesEntity> schedulesEntity = new List<PatientSchedulesEntity>();
            schedulesEntity= _patientRepository.GetPatientsSchedules(PatientId);
            if (schedulesEntity.Count < 1)
            {
                throw new ApiDataException(1002, "No schedule found for given patient .", HttpStatusCode.InternalServerError);

            }

            return Ok(schedulesEntity);
        }

        [HttpGet]
        [Route("~/api/Patient/Goals/{patientId}")]
        public HttpResponseMessage GetPatientGoals(int patientId)
        {


            if (patientId > 0 )
            {
                var goals = _patientRepository.GetPatientGoals(patientId);
                if (goals != null)
                    return Request.CreateResponse(HttpStatusCode.OK, goals);

                throw new ApiDataException(1001, "No goals found for this id.", HttpStatusCode.NotFound);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }

        [HttpGet]
        [Route("~/api/Patient/SignDetails/{NPINumber}")]
        public HttpResponseMessage GetDocTherapist(string NPINumber)
        {

            if (!string.IsNullOrEmpty(NPINumber))
            {
                var details = _patientRepository.GetDocTherapist(NPINumber);
                if (details != null)
                    return Request.CreateResponse(HttpStatusCode.OK, details);

                throw new ApiDataException(1001, "No sign info found for this id.", HttpStatusCode.NotFound);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };

        }

        [HttpGet]
        [Route("~/api/Patient/Note/SendRefEmail/{recipient}/{npinum}/{patientId}/{noteId}")]
        public HttpResponseMessage SendRefEmail(string recipient, int npinum, int patientId, int noteId)
        {

            if (!string.IsNullOrEmpty(recipient) && patientId>0&& noteId>0)
            {
                bool status = _patientRepository.SendRefEMail(recipient, npinum,  patientId,  noteId);
                if (status)
                    return Request.CreateResponse(HttpStatusCode.OK, status);

                throw new ApiDataException(1001, "Error in sending email", HttpStatusCode.NotFound);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };

        }
    }
}
