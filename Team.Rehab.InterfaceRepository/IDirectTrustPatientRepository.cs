using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.BusinessEntities;
using Team.Rehab.DataModel;

namespace Team.Rehab.InterfaceRepository
{
    public interface IDirectTrustPatientRepository
    {
 
        DT_AuthenticateEntity GetUser(string userName, string password);      
       
        int CreateDTPatient(int incommingMsgID, string userID, string status,int attachmentID);
        int ImportPatient(int patientID, int incommingMsgID, string userID, string status, int attachmentID);
        int RejectDTPatient(int patientID, int incommingMsgID, string userID, int attachmentID);
        List<DT_PatientReferralProcessedEntity> GetDTReferralProcessedPatients();
        bool MoveIncomingtoReferrerProccessed(int patientID, int incommingProcesID, string fileData, int instituteID, string status, string userId);
        DT_POCListEntity GetPOCProcessed(string username);
        List<DT_Outgoing_Message> GetPOCSent(string username);
        List<DT_Message_ProcessedEntity> GetProcessedMessages(string username);
        DT_AuthenticateEntity GetUserOnUsername(string userName);
        bool MoveReferrerProccessedtoIncomingMessage(string processedID);
    }
}
