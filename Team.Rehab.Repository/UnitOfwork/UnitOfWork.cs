using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.DataModel;


namespace Team.Rehab.Repository.UnitOfwork
{
    public class UnitOfWork : IDisposable, IUnitOfwork
    {

        private RehabEntities _context;
        private IRepository<tblUser> _UserEntityRepo;
        private IRepository<tblPatients> _PatientsEntityRepo;
        private IRepository<tblBlockedIP> _BlockedIpRepo;
        private IRepository<tblDevice> _DeviceRepo;
        private IRepository<SP_PatientNotes_Result> _PatientNotesEntityRepo;
        private IRepository<tblClient> _ClientRepo;
        private IRepository<tblRefreshTokens> _RefreshTokenRepo;
        private IRepository<tblAppUser> _AppUserRepo;
        private IRepository<tblOTP> _OTPRepo;
        private IRepository<tblReferrer> _ReferrerRepo;
        private IRepository<tblAppNotification> _NotificationRepo;
        private IRepository<tblAppError> _AppErrorsRepo;
        private IRepository<SP_GetDocFuncCharac_Result> _FuncCharRepo;
        private IRepository<tblDocMaster> _DocMasterRepo;
        private IRepository<tblReferrerSign> _ReferrerSignRepo;
        private IRepository<tblDirectTrustPatient> _DirectPatientsEntityRepo;
        private IRepository<tbl_DT_PatientReferral_Processed> _DT_PatientProcessedRepo;

        private IRepository<tbl_DT_Referrer_Emails> _DT_Referrer_EmailsRepo;
        private IRepository<tbl_DT_Referrers> _DT_ReferrerRepo;
        private IRepository<tblPatScannedDocs> _PatScannedDocsRepo;
        private IRepository<tbl_DT_PatientReferralMRN> _DT_PatientReferralMRNRepo;
        private IRepository<tblDocMasterAddl> _DT_DocMasterAddlRepo;
        private IRepository<tbl_DT_Outgoing_Message> _DT_Outgoing_MessageRepo;
        private IRepository<tbl_DT_Incoming_Message> _DT_Incoming_MessageRepo;
        private IRepository<tbl_DT_Incoming_Message_Individual_Attachments> _DT_Individial_AttachRepo;
        private IRepository<tbl_DT_PatientReferral_Processed> _DT_PatientReferral_ProcessedRepo;
        private IRepository<tbl_DT_POC_Processed> _DT_POC_ProcessedRepo;
        private IRepository<tbl_DT_ClinicUserMapping> _DT_ClinicUserMappingRepo;
        private IRepository<tbl_DT_POC_NotFound> _DT_POC_NotFoundRepo;
        private IRepository<tblRefAddr> _RefAddrRepo;
        private IRepository<tbl_DT_MessagesSent> _DT_MessageSentRepo;
        public void Save()
        {
            _context.SaveChanges();
        }
        //public UnitOfWork(RehabEntities context)
        //{
        //    _context = context;
        //}

        public UnitOfWork()
        {
            _context = new RehabEntities();
        }
        /// <summary>
        /// Get/Set Property for user repository.
        /// </summary>
        public IRepository<tblUser> UserEntityRepo
        {
            get
            {
                if (this._UserEntityRepo == null)
                    this._UserEntityRepo = new GenericRepository<tblUser>(_context);
                return _UserEntityRepo;
            }
        }
        public IRepository<tbl_DT_PatientReferralMRN> DT_PatientReferralMRNRepo
        {
            get
            {
                if (this._DT_PatientReferralMRNRepo == null)
                    this._DT_PatientReferralMRNRepo = new GenericRepository<tbl_DT_PatientReferralMRN>(_context);
                return _DT_PatientReferralMRNRepo;
            }
        }
        public IRepository<tblPatScannedDocs> PatScannedDocsRepo
        {
            get
            {
                if (this._PatScannedDocsRepo == null)
                    this._PatScannedDocsRepo = new GenericRepository<tblPatScannedDocs>(_context);
                return _PatScannedDocsRepo;
            }
        }
        public IRepository<tblPatients> PatientEntityRepo
        {
            get
            {
                if (this._PatientsEntityRepo == null)
                    this._PatientsEntityRepo = new GenericRepository<tblPatients>(_context);
                return _PatientsEntityRepo;
            }
        }
        public IRepository<tblDirectTrustPatient> DirectTrustPatientEntityRepo
        {
            get
            {
                if (this._DirectPatientsEntityRepo == null)
                    this._DirectPatientsEntityRepo = new GenericRepository<tblDirectTrustPatient>(_context);
                return _DirectPatientsEntityRepo;
            }
        }

        public IRepository<SP_PatientNotes_Result> PatientNotesEntityRepo
        {
            get
            {
                if (this._PatientsEntityRepo == null)
                    this._PatientNotesEntityRepo = new GenericRepository<SP_PatientNotes_Result>(_context);
                return _PatientNotesEntityRepo;
            }
        }
        public IRepository<tblBlockedIP> BlockedIpRepo
        {
            get
            {
                if (this._BlockedIpRepo == null)
                    this._BlockedIpRepo = new GenericRepository<tblBlockedIP>(_context);
                return _BlockedIpRepo;
            }
        }
        public IRepository<tblDevice> DeviceRepo
        {
            get
            {
                if (this._DeviceRepo == null)
                    this._DeviceRepo = new GenericRepository<tblDevice>(_context);
                return _DeviceRepo;
            }
        }
        public IRepository<tblAppUser> AppUserRepo
        {
            get
            {
                if (this._AppUserRepo == null)
                    this._AppUserRepo = new GenericRepository<tblAppUser>(_context);
                return _AppUserRepo;
            }
        }
        public IRepository<tblClient> ClientRepo
        {
            get
            {
                if (this._ClientRepo == null)
                    this._ClientRepo = new GenericRepository<tblClient>(_context);
                return _ClientRepo;
            }
        }
        public IRepository<tblRefreshTokens> RefreshTokenRepo
        {
            get
            {
                if (this._RefreshTokenRepo == null)
                    this._RefreshTokenRepo = new GenericRepository<tblRefreshTokens>(_context);
                return _RefreshTokenRepo;
            }
        }
        public IRepository<tblOTP> OTPRepo
        {
            get
            {
                if (this._OTPRepo == null)
                    this._OTPRepo = new GenericRepository<tblOTP>(_context);
                return _OTPRepo;
            }
        }
        public IRepository<tblReferrer> ReferrerRepo
        {
            get
            {
                if (this._ReferrerRepo == null)
                    this._ReferrerRepo = new GenericRepository<tblReferrer>(_context);
                return _ReferrerRepo;
            }
        }
        public IRepository<tblRefAddr> RefAddrRepo
        {
            get
            {
                if (this._RefAddrRepo == null)
                    this._RefAddrRepo = new GenericRepository<tblRefAddr>(_context);
                return _RefAddrRepo;
            }
        }
        public IRepository<tbl_DT_MessagesSent> DT_MessageSentRepo
        {
            get
            {
                if (this._DT_MessageSentRepo == null)
                    this._DT_MessageSentRepo = new GenericRepository<tbl_DT_MessagesSent>(_context);
                return _DT_MessageSentRepo;
            }
        }
        
        public IRepository<tblAppNotification> NotificationRepo
        {
            get
            {
                if (this._NotificationRepo == null)
                    this._NotificationRepo = new GenericRepository<tblAppNotification>(_context);
                return _NotificationRepo;
            }
        }
        public IRepository<tblAppError> AppErrorsRepo
        {
            get
            {
                if (this._AppErrorsRepo == null)
                    this._AppErrorsRepo = new GenericRepository<tblAppError>(_context);
                return _AppErrorsRepo;
            }
        }
        public IRepository<SP_GetDocFuncCharac_Result> FuncCharRepo
        {
            get
            {
                if (this._FuncCharRepo == null)
                    this._FuncCharRepo = new GenericRepository<SP_GetDocFuncCharac_Result>(_context);
                return _FuncCharRepo;
            }
        }
        public IRepository<tblReferrerSign> ReferrerSignRepo
        {
            get
            {
                if (this._ReferrerSignRepo == null)
                    this._ReferrerSignRepo = new GenericRepository<tblReferrerSign>(_context);
                return _ReferrerSignRepo;
            }
        }

        public IRepository<tblDocMaster> DocMasterRepo
        {
            get
            {
                if (this._DocMasterRepo == null)
                    this._DocMasterRepo = new GenericRepository<tblDocMaster>(_context);
                return _DocMasterRepo;
            }
        }

        public IRepository<tbl_DT_PatientReferral_Processed> DT_PatientProcessedRepo
        {
            get
            {
                if (this._DT_PatientProcessedRepo == null)
                    this._DT_PatientProcessedRepo = new GenericRepository<tbl_DT_PatientReferral_Processed>(_context);
                return _DT_PatientProcessedRepo;
            }
        }
        public IRepository<tbl_DT_Referrers> DT_ReferrerRepo
        {
            get
            {
                if (this._DT_ReferrerRepo == null)
                    this._DT_ReferrerRepo = new GenericRepository<tbl_DT_Referrers>(_context);
                return _DT_ReferrerRepo;
            }
        }
        public IRepository<tbl_DT_Referrer_Emails> DT_Referrer_EmailsRepo
        {
            get
            {
                if (this._DT_Referrer_EmailsRepo == null)
                    this._DT_Referrer_EmailsRepo = new GenericRepository<tbl_DT_Referrer_Emails>(_context);
                return _DT_Referrer_EmailsRepo;
            }
        }
        public IRepository<tblDocMasterAddl> DT_DocMasterAddlRepo
        {
            get
            {
                if (this._DT_DocMasterAddlRepo == null)
                    this._DT_DocMasterAddlRepo = new GenericRepository<tblDocMasterAddl>(_context);
                return _DT_DocMasterAddlRepo;
            }
        }
        public IRepository<tbl_DT_Outgoing_Message> DT_Outgoing_MessageRepo
        {
            get
            {
                if (this._DT_Outgoing_MessageRepo == null)
                    this._DT_Outgoing_MessageRepo = new GenericRepository<tbl_DT_Outgoing_Message>(_context);
                return _DT_Outgoing_MessageRepo;
            }
        }
        public IRepository<tbl_DT_Incoming_Message_Individual_Attachments> DT_Individial_AttachRepo
        {
            get
            {
                if (this._DT_Individial_AttachRepo == null)
                    this._DT_Individial_AttachRepo = new GenericRepository<tbl_DT_Incoming_Message_Individual_Attachments>(_context);
                return _DT_Individial_AttachRepo;
            }
        }

        public IRepository<tbl_DT_Incoming_Message> DT_Incoming_MessageRepo
        {
            get
            {
                if (this._DT_Incoming_MessageRepo == null)
                    this._DT_Incoming_MessageRepo = new GenericRepository<tbl_DT_Incoming_Message>(_context);
                return _DT_Incoming_MessageRepo;
            }
        }
        public IRepository<tbl_DT_PatientReferral_Processed> DT_PatientReferral_ProcessedRepo
        {
            get
            {
                if (this._DT_PatientReferral_ProcessedRepo == null)
                    this._DT_PatientReferral_ProcessedRepo = new GenericRepository<tbl_DT_PatientReferral_Processed>(_context);
                return _DT_PatientReferral_ProcessedRepo;
            }
        }
        public IRepository<tbl_DT_POC_Processed> DT_POC_ProcessedRepo
        {
            get
            {
                if (this._DT_POC_ProcessedRepo == null)
                    this._DT_POC_ProcessedRepo = new GenericRepository<tbl_DT_POC_Processed>(_context);
                return _DT_POC_ProcessedRepo;
            }
        }
        public IRepository<tbl_DT_ClinicUserMapping> DT_ClinicUserMappingRepo
        {
            get
            {
                if (this._DT_ClinicUserMappingRepo == null)
                    this._DT_ClinicUserMappingRepo = new GenericRepository<tbl_DT_ClinicUserMapping>(_context);
                return _DT_ClinicUserMappingRepo;
            }
        }
        public IRepository<tbl_DT_POC_NotFound> DT_POC_NotFoundRepo
        {
            get
            {
                if (this._DT_POC_NotFoundRepo == null)
                    this._DT_POC_NotFoundRepo = new GenericRepository<tbl_DT_POC_NotFound>(_context);
                return _DT_POC_NotFoundRepo;
            }
        }
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //  Debug.WriteLine("UnitOfWork is being disposed");
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
