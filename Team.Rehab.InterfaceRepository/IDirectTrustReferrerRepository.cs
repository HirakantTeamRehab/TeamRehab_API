using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.BusinessEntities;

namespace Team.Rehab.InterfaceRepository
{
    public interface IDirectTrustReferrerRepository
    {

        List<DT_UserEntity> GetAllDTUsers();
        int CreateDTUser(DT_UserEntity[] dtUserEntity);
        bool UpdateDTUser(DT_UserEntity[] appUserEntity);
        bool DeleteDTUser(int refID, int refEmailID);
            List<DT_UserEntity> GetDirectTrustReferrerDetail(int instituteId);
        List<DT_IncomingMessagesEntity> GetDTIncoming_Message(string username,string role);
        List<DT_POC_NotFoundEntity> GetDTPOCNotFound();
        DT_IncomingMessageViewmodelEntity GetDTIncoming_MessageByID(int incomingmessageId);
        List<DT_IncomingMessagesXMLEntity> GetDTIncoming_MessageXML(int incomingmessageId);
        List<DT_ClinicMapping> GetClinicMapping();
        List<DT_Clinics> GetClinics();
        bool SaveDTClinicMapping(DT_ClinicMapping mapping);
        bool DeleteDTClinicMapping(string ClinicNo);
        byte[] GetPOCFile(Int32 Note_ID);
        bool UpdateMessageReadFlag(int incomingmessageId);
        bool UpdateMessageReadUnreadFlag(int[] incomingmessageIds, string operation);
        List<string> GetUserEmails();
        List<DT_MessagesSent> GetUserEmailMessages(string fromID);
        bool SaveDTMessagesSent(DT_MessagesSent message);
    }
}
