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
    public class DirectTrustPatientController : ApiController
    {

        // IPatientRepository _patientRepository;
        private readonly IDirectTrustPatientRepository _directtrustpatientRepository;
        public DirectTrustPatientController(IDirectTrustPatientRepository directtrustpatientRepository)
        {

            this._directtrustpatientRepository = directtrustpatientRepository;
        }
    
        [HttpGet]
        [Route("DirectTrustPatient/Authenticate")]
        //[ResponseType(typeof(DT_AuthenticateEntity))]
        public DT_AuthenticateEntity PostAdmin(string userName,string password)
        {
            DT_AuthenticateEntity response = new DT_AuthenticateEntity();
            response = _directtrustpatientRepository.GetUser(userName,password);

            //if (response != null)
            //{
                return response;
            //}
            //else
            //{
            //    throw new ApiDataException(1000, "user not found", HttpStatusCode.NotFound);
            //}

        }
        [HttpGet]
        [Route("DirectTrustPatient/GetUserDetails")]
        //[ResponseType(typeof(DT_AuthenticateEntity))]
        public DT_AuthenticateEntity PostAdmin(string userName)
        {
            DT_AuthenticateEntity response = new DT_AuthenticateEntity();
            response = _directtrustpatientRepository.GetUserOnUsername(userName);

            //if (response != null)
            //{
            return response;
            //}
            //else
            //{
            //    throw new ApiDataException(1000, "user not found", HttpStatusCode.NotFound);
            //}

        }



        [Route("ProcessedPatients")]
        [HttpGet]
        public List<DT_PatientReferralProcessedEntity>GetProcessedreferralPatients()
        {
            List<DT_PatientReferralProcessedEntity> processedPatients = new List<DT_PatientReferralProcessedEntity>();
            try
            {
                processedPatients = _directtrustpatientRepository.GetDTReferralProcessedPatients();
            }
            catch (Exception ex)
            {
            }
            return processedPatients;
        }
        

        [Route("CreatePatient")]
        [HttpGet]
        public bool CreatePatient(int incommingMsgID, string userID,string status, int attachmentID)
        {
            if (incommingMsgID > 0)
            {
                int isSuccess = _directtrustpatientRepository.CreateDTPatient(incommingMsgID,  userID,status, attachmentID);
                if (isSuccess > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                throw new ApiDataException(1002, "There is issue in createting direct trust patient.", HttpStatusCode.NoContent);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "There is issue in createting direct trust patient ..." };
        }

        [Route("ImportPatient")]
        [HttpGet]
        public bool ImportPatient(int patientID, int incommingMsgID, string userID, string status, int attachmentID)
        {
            if (incommingMsgID > 0)
            {
                int isSuccess = _directtrustpatientRepository.ImportPatient(patientID, incommingMsgID, userID, status, attachmentID);
                if (isSuccess > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                throw new ApiDataException(1002, "There is issue in importing direct trust patient.", HttpStatusCode.NoContent);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "There is issue in importing direct trust patient ..." };
        }
        [Route("RejectPatient")]
        [HttpGet]
        public bool RejectPatient(int patientID, int incommingMsgID, string userID, int attachmentID)
        {
            if (incommingMsgID  > 0)
            {
                int isSuccess = _directtrustpatientRepository.RejectDTPatient(patientID, incommingMsgID, userID, attachmentID);
                if (isSuccess > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                throw new ApiDataException(1002, "There is issue in rejecting direct trust patient.", HttpStatusCode.NoContent);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "There is issue in rejecting direct trust patient ..." };
        }
        [Route("ProcessedPOC")]
        [HttpGet]
        public DT_POCListEntity ProcessedPOC(string username)
        {
            return _directtrustpatientRepository.GetPOCProcessed(username);
        }
        [Route("POCSent")]
        [HttpGet]
        public List<DT_Outgoing_Message> POCSent(string username)
        {
            return _directtrustpatientRepository.GetPOCSent(username);
        }
        [Route("ProcessedMesages")]
        [HttpGet]
        public List<DT_Message_ProcessedEntity> ProcessedMesages(string username)
        {
            return _directtrustpatientRepository.GetProcessedMessages(username);
        }

        [Route("UnProcessReferralMessage")]
        [HttpGet]
        public bool UnProcessReferralMessage(string processedID)
        {
            return _directtrustpatientRepository.MoveReferrerProccessedtoIncomingMessage(processedID);
        }
    }
}
