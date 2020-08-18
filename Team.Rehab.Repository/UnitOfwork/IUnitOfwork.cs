using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.DataModel;
using Team.Rehab.InterfaceRepository;

namespace Team.Rehab.Repository
{
    public interface IUnitOfwork
    {
        void Save();
        IRepository<tblUser> UserEntityRepo { get; }
        IRepository<tblPatients> PatientEntityRepo { get; }
        IRepository<SP_PatientNotes_Result> PatientNotesEntityRepo { get; }
        IRepository<tblBlockedIP> BlockedIpRepo { get; }

        IRepository<tblDevice> DeviceRepo { get; }
        IRepository<tblClient> ClientRepo { get; }
        IRepository<tblRefreshTokens> RefreshTokenRepo { get; }
        IRepository<tblAppUser> AppUserRepo { get; }
        IRepository<tblOTP> OTPRepo { get; }
        IRepository<tblReferrer> ReferrerRepo { get; }
        IRepository<tblAppNotification> NotificationRepo { get; }
        IRepository<tblAppError> AppErrorsRepo { get; }
        IRepository<SP_GetDocFuncCharac_Result> FuncCharRepo { get; }
        IRepository<tblDocMaster> DocMasterRepo { get; }
        IRepository<tblReferrerSign> ReferrerSignRepo { get; }
        IRepository<tblDirectTrustPatient> DirectTrustPatientEntityRepo { get; }

        IRepository<tbl_DT_PatientReferral_Processed> DT_PatientProcessedRepo { get; }
        
        IRepository<tbl_DT_Referrer_Emails> DT_Referrer_EmailsRepo { get; }
        IRepository<tbl_DT_Referrers> DT_ReferrerRepo { get; }
        IRepository<tblPatScannedDocs> PatScannedDocsRepo { get; }
        IRepository<tbl_DT_PatientReferralMRN> DT_PatientReferralMRNRepo { get; }
IRepository<tbl_DT_Incoming_Message_Individual_Attachments> DT_Individial_AttachRepo { get; }
        IRepository<tbl_DT_Incoming_Message> DT_Incoming_MessageRepo { get; }
        IRepository<tbl_DT_PatientReferral_Processed> DT_PatientReferral_ProcessedRepo { get; }
        IRepository<tbl_DT_POC_Processed> DT_POC_ProcessedRepo { get; }
        IRepository<tbl_DT_Outgoing_Message> DT_Outgoing_MessageRepo { get; }
        IRepository<tbl_DT_POC_NotFound> DT_POC_NotFoundRepo { get; }
        IRepository<tblRefAddr> RefAddrRepo { get; }
        IRepository<tbl_DT_MessagesSent> DT_MessageSentRepo { get; }
    }
}
