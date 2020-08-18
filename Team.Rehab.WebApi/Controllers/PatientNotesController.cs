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
using Team.Rehab.WebApi.Helpers;
using Team.Rehab.BusinessEntities;

namespace Team.Rehab.WebApi.Controllers
{
    public class PatientNotesController : ApiController
    {
        IPatientRepository _patientRepository;
        

        public PatientNotesController(IPatientRepository patientRepository)
        {

            this._patientRepository = patientRepository;
        }
        [HttpGet]
        [Route("~/api/PatientNotes/{patientId}")]
        public List<SP_PatientNotes_Result> GetPatientNoteList(int patientId)
        {
            return _patientRepository.GetPatientNotes(patientId);
        }

        [HttpGet]
        [Route("~/api/GetPatientNotesByID/{patientId}/{noteId}")]            
        public SP_PatientNotes_Result GetPatientNotesByID(int patientId,int noteId)
        {
            try
            {
                //int noteID = 1622602;
                return _patientRepository.GetPatientNotesByID(patientId, noteId);
            }
            catch (Exception ex)            {
               
                throw new ApiDataException(1053, "No patient found for this id.", HttpStatusCode.NotFound);
              //  throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };

                //throw new Exception("No product found for this id");


            }
        }
        [HttpGet]
        [Route("~/api/Notes/NoteSummary/{patientId}")]
        public List<NotesSummaryEntity> GetNotesSummary(int patientId)
        {
            List<NotesSummaryEntity> noteSummary = new List<NotesSummaryEntity>();
            noteSummary=_patientRepository.GetNotesSummary(patientId);
            if (noteSummary.Count < 1)
            {
                throw new ApiDataException(1051, "No Summary found .", HttpStatusCode.InternalServerError);

            }
            return noteSummary;
        }
        //[Authorize]
        [HttpGet]
        [Route("~/api/Notes/Sign/{patientId}/{noteId}")]
        public string SignNotes(int patientId, int noteId)
        {
            int NPINumber = 0;
           return _patientRepository.SignNotes(patientId,  noteId, NPINumber);
        }
        [HttpGet]
        [Route("~/api/Notes/UnSign/{patientId}/{noteId}")]
        public string UnSignNotes(int patientId, int noteId)
        {
            int NPINumber = 0;
            return _patientRepository.UnSignNotes(patientId, noteId, NPINumber);
        }
        [HttpGet]
        [Route("~/api/Notes/Sign/{patientId}/{noteId}/{NPINumber}")]
        public string RefSignNotes(int patientId, int noteId,int NPINumber)
        {
            return _patientRepository.SignNotes(patientId, noteId, NPINumber);
        }
        [HttpGet]
        [Route("~/api/Notes/UnSign/{patientId}/{noteId}/{NPINumber}")]
        public string RefUnSignNotes(int patientId, int noteId, int NPINumber)
        {
            return _patientRepository.UnSignNotes(patientId, noteId, NPINumber);
        }
        [HttpGet]
        [Route("~/api/Notes/NoteSummary/{patientId}/{noteId}")]
        public HttpResponseMessage GetNotesSummary(int patientId, int noteId)
        {            


            if (patientId >0 && noteId > 0)
            {
                var summary = _patientRepository.GetNotesSummary(patientId, noteId);
                if (summary != null)
                    return Request.CreateResponse(HttpStatusCode.OK, summary);

                throw new ApiDataException(1052, "No summary found for this id.", HttpStatusCode.NotFound);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }

        [HttpGet]
        [Route("~/api/Notes/NotePDF/{patientId}/{noteId}")]
        public HttpResponseMessage GetNotePDF(int patientId, int noteId)
        {

          
            if (patientId>0 && noteId>0)
            {
                var summary = _patientRepository.GetPdf(patientId ,noteId);
                if (summary != null)
                    return Request.CreateResponse(HttpStatusCode.OK, summary);

                throw new ApiDataException(1052, "No pdf found for this id.", HttpStatusCode.NotFound);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }

        [HttpGet]
        [Route("~/api/Notes/RefSignDetails/{patientId}/{noteId}/{NPINumber}")]
        public HttpResponseMessage GetRefSignDetails(int patientId, int noteId,string NPINumber)
        {


            if (patientId > 0 && noteId > 0)
            {
                var summary = _patientRepository.GetRefSignDetails(patientId, noteId, NPINumber);
                if (summary != null)
                    return Request.CreateResponse(HttpStatusCode.OK, summary);

                throw new ApiDataException(1052, "No details found this id.", HttpStatusCode.NotFound);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }
    }
}   
