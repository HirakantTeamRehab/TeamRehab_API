using Team.Rehab.InterfaceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Team.Rehab.BusinessEntities;
using AttributeRouting.Web.Http;
using Team.Rehab.WebApi.ErrorHelper;
using Team.Rehab.Repository.Repository;
using System.Net.Http.Headers;

namespace Team.Rehab.WebApi.Controllers
{
    public class DirectTrustReferrerController : ApiController
    {
        //private readonly IDirectTrustReferrerRepository _dtRefRepository;


        //public DirectTrustReferrerController(IDirectTrustReferrerRepository dtRefRepository)
        //{
        //    this._dtRefRepository = dtRefRepository;
        //}


        private readonly DirectTrustReferrerRepository _dtRefRepository;
        public DirectTrustReferrerController()
        {
            //this._unitOfWork = unitOfWork;
            this._dtRefRepository = new DirectTrustReferrerRepository();
            //_ctx = new AtlasEntities();
        }
        ///  [GET("allUsers")]
        //[GET("all")]
        [Route("DirectTrustReferrerList")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            //int b = 0;
            //int a = 100 / b;
            var dtUsers = _dtRefRepository.GetAllDTUsers();
            var appDTEntities = dtUsers as List<DT_UserEntity> ?? dtUsers.ToList();
            //if (appDTEntities.Any())
            return Request.CreateResponse(HttpStatusCode.OK, appDTEntities);
            //throw new ApiDataException(1000, "user not found", HttpStatusCode.NotFound);
        }

        [Route("DirectTrustReferrerDetail")]
        [HttpGet]
        public HttpResponseMessage GetDirectTrustReferrerDetail(int instituteId)
        {

            var dtUser = _dtRefRepository.GetDirectTrustReferrerDetail(instituteId);
            var appDTEntities = dtUser as List<DT_UserEntity> ?? dtUser;
            if (appDTEntities != null)
                return Request.CreateResponse(HttpStatusCode.OK, appDTEntities);
            throw new ApiDataException(1000, "user not found", HttpStatusCode.NotFound);
        }

        /// [POST("CreateDirect")]
         //[POST("Register")]
        [Route("CreateReferrer")]
        [HttpPost]
        public int Post([FromBody] DT_UserEntity[] appUserEntity)
        {
            return _dtRefRepository.CreateDTUser(appUserEntity);
        }
        // [PUT("Update/AppUserid/{id}")]
        //  [PUT("Modify/AppUserid/{id}")]
        [Route("UpdateReferrer")]
        [HttpPost]
        public bool Put([FromBody] DT_UserEntity[] appUserEntity)
        {
            if (appUserEntity[0].RefID > 0)
            {
                return _dtRefRepository.UpdateDTUser(appUserEntity);
            }
            return false;
        }

        // DELETE api/product/5
        // [DELETE("remove/AppUserid/{id}")]
        // [PUT("delete/AppUserid/{id}")]
        [Route("DeleteReferrer")]
        [HttpGet]
        public bool Delete(int refID, int refEmailID)
        {
            if (refID > 0)
            {
                var isSuccess = _dtRefRepository.DeleteDTUser(refID, refEmailID);
                if (isSuccess)
                {
                    return isSuccess;
                }
                throw new ApiDataException(1002, "User is already deleted or not exist in system.", HttpStatusCode.NoContent);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }
        [Route("DirectTrustIncomingMessages")]
        [HttpGet]
        public HttpResponseMessage GetDirectTrustIncomingMessages(string username, string role)
        {
            //int b = 0;
            //int a = 100 / b;
            var dtUsers = _dtRefRepository.GetDTIncoming_Message(username, role);
            var appDTEntities = dtUsers as List<DT_IncomingMessagesEntity> ?? dtUsers.ToList();
            // if (appDTEntities.Any())
            return Request.CreateResponse(HttpStatusCode.OK, appDTEntities);
            //  throw new ApiDataException(1000, "user not found", HttpStatusCode.NotFound);
        }
        [Route("DirectTrustIncomingMessageByID")]
        [HttpGet]
        public HttpResponseMessage GetDirectTrustIncomingMessageByID(int incomingmessageid)
        {
            //int b = 0;
            //int a = 100 / b;
            var incoomingMessage = _dtRefRepository.GetDTIncoming_MessageByID(incomingmessageid);
            if (incoomingMessage != null && incoomingMessage.ID > 0)
            {
                var appDTEntities = incoomingMessage as DT_IncomingMessageViewmodelEntity ?? incoomingMessage;
                return Request.CreateResponse(HttpStatusCode.OK, appDTEntities);
            }
            else
            {
                throw new ApiDataException(1000, "No message found", HttpStatusCode.NotFound);
            }

            // if (appDTEntities.Any())

            //  throw new ApiDataException(1000, "user not found", HttpStatusCode.NotFound);
        }


        [Route("DirectTrustPOCNotFoundMessages")]
        [HttpGet]
        public HttpResponseMessage GetDirectTrustPOCNotFoundMessages()
        {
            //int b = 0;
            //int a = 100 / b;
            var dtUsers = _dtRefRepository.GetDTPOCNotFound();
            var appDTEntities = dtUsers as List<DT_POC_NotFoundEntity> ?? dtUsers.ToList();
            // if (appDTEntities.Any())
            return Request.CreateResponse(HttpStatusCode.OK, appDTEntities);
            //throw new ApiDataException(1000, "user not found", HttpStatusCode.NotFound);
        }

        [Route("ProcessIncomingMessage")]
        [HttpPost]
        public DT_IncomingMessageOperation ProcessIncomingMessage([FromBody] DT_IncomingMessageProcessEntity appUserEntity)
        {
            return _dtRefRepository.ProcessIncomingMessage(appUserEntity);
        }

        [Route("PatientPOCs")]
        [HttpGet]
        public DT_IncomingMessageOperation GetPOCListbyPatientID(int patientid, string operation)
        {
            return _dtRefRepository.GetPOClistbyPatientID(patientid, operation);
        }

        [Route("SavePOC")]
        [HttpGet]
        public bool SavePOC(Int32 patientId, string operation, Int32 InComingMessageID, Int32 noteId, string Filename, string username, int attachmentID)
        {
            return _dtRefRepository.ProcessPOC(patientId, operation, InComingMessageID, noteId, Filename, username, attachmentID);
        }

        [Route("DTClinicMapping")]
        [HttpGet]
        public List<DT_ClinicMapping> GetDTClinicMapping()
        {
            return _dtRefRepository.GetClinicMapping();
        }
        [Route("DTClinics")]
        [HttpGet]
        public List<DT_Clinics> GetDTClinics()
        {
            return _dtRefRepository.GetClinics();
        }
        [Route("SaveDTClinicMapping")]
        [HttpPost]
        public bool SaveDTClinicMapping([FromBody] DT_ClinicMapping appUserEntity)
        {
            return _dtRefRepository.SaveDTClinicMapping(appUserEntity);
        }
        [Route("DeleteDTClinicMapping")]
        [HttpGet]
        public bool DeleteDTClinicMapping(string ClinicNo)
        {
            return _dtRefRepository.DeleteDTClinicMapping(ClinicNo);
        }

        [Route("DownloadPOCFile")]
        [HttpPost]
        public HttpResponseMessage DownloadPOCFile([FromBody] Int32 Note_ID)
        {
            HttpResponseMessage response;

            response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);


            byte[] temp = _dtRefRepository.GetPOCFile(Note_ID);
            response.Content = new ByteArrayContent(temp);


            response.Content.Headers.ContentLength = temp.LongLength;
            response.Content.Headers.Add("x-filename", "Sample.pdf");

            response.Content.Headers.Add("Access-Control-Expose-Headers", "x-filename");

            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "Sample.pdf";


            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;

        }
        [Route("UpdateMessageReadFlag")]
        [HttpPost]
        public bool UpdateMessageReadFlag([FromBody] Int32 incomingMessageId)
        {
            return _dtRefRepository.UpdateMessageReadFlag(incomingMessageId);
        }
        [Route("UpdateMessageReadUnReadFlag")]
        [HttpPost]
        public bool UpdateMessageReadUnReadFlag(CheckedMessages checkedMessages)
        {
           
            return _dtRefRepository.UpdateMessageReadUnreadFlag(checkedMessages.incomingmessageIds, checkedMessages.operation);
        }
        [Route("DTUserEmails")]
        [HttpGet]
        public List<string> GetUserEmails()
        {
            return _dtRefRepository.GetUserEmails();
        }
        [Route("DTUserEmailMessages")]
        [HttpGet]
        public List<DT_MessagesSent> GetUserEmailMessages(string fromId)
        {
            return _dtRefRepository.GetUserEmailMessages(fromId);
        }
        [Route("SaveDTMessageSent")]
        [HttpPost]
        public bool SaveDTMessageSent([FromBody] DT_MessagesSent appUserEntity)
        {
            return _dtRefRepository.SaveDTMessagesSent(appUserEntity);
        }
    }
}
