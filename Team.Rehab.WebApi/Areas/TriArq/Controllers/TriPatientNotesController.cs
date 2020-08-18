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
    public class TriPatientNotesController : ApiController
    {
        ITNotesRepository _notesRepository;
        public TriPatientNotesController(ITNotesRepository notesRepository)
        {

            this._notesRepository = notesRepository;
        }

        //[HttpGet]
        //[Route("~/Triarq/api/PatientNotes/MedicalNecessityNote/{patientId}/{noteId}")]
        ////[Authorize]
        //public JObject GetMedicalNecessity(int patinetId, int noteId)
        //{
        //    JObject jsonResponse = new JObject();
        //    List<PatientEntity> patientEntity = new List<PatientEntity>();
        //    jsonResponse = _notesRepository.GetMedicalNcessityNotes(patinetId, noteId);
        //    return jsonResponse;
        //}
        [HttpGet]
        [Route("~/FHIR/api/PatientNotes/{patientId}")]
        public List<TriPatientNotesEntity> GetPatientNoteList(int patientId)
        {
            List<TriPatientNotesEntity> triPatientNotesEntity = new List<TriPatientNotesEntity>();
            triPatientNotesEntity= _notesRepository.GetPatientNotes(patientId);
            if (triPatientNotesEntity.Count < 1)
            {
                throw new ApiDataException(1002, "No patient found for this id.", HttpStatusCode.InternalServerError);

            }
            return triPatientNotesEntity;
        }

        [HttpGet]
        [Route("~/FHIR/api/PatientNotes/{patientId}/{noteId}")]
        public JArray GetMedicalNecessity(int patientId, int noteId)
        {
            JArray jsonResponse = new JArray();
            List<PatientEntity> patientEntity = new List<PatientEntity>();
            jsonResponse = _notesRepository.GetMedicalNcessityNotes(patientId, noteId);
            return jsonResponse;
        }
        [HttpGet]
        [Route("~/FHIR/api/PatientNotesInCCDAXML/{patientId}/{noteId}")]
        public string GetCCDAInXMLFormat(int patientId, int noteId)
        {
            string jsonResponse = string.Empty;
            List<PatientEntity> patientEntity = new List<PatientEntity>();
            jsonResponse = _notesRepository.GetCCDAInXMLFormat(patientId, noteId);
            return jsonResponse;
        }

    }
}
