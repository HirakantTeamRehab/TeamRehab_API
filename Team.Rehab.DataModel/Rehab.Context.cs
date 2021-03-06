﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Team.Rehab.DataModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class RehabEntities : DbContext
    {
        public RehabEntities()
            : base("name=RehabEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblMenu> tblMenus { get; set; }
        public virtual DbSet<tblPatNote> tblPatNotes { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tblUserGroup> tblUserGroups { get; set; }
        public virtual DbSet<tblUserMenu> tblUserMenus { get; set; }
        public virtual DbSet<tblDocMaster> tblDocMasters { get; set; }
        public virtual DbSet<tblTherapist> tblTherapists { get; set; }
        public virtual DbSet<tblBlockedIP> tblBlockedIPs { get; set; }
        public virtual DbSet<tblClient> tblClients { get; set; }
        public virtual DbSet<tblAppUser> tblAppUsers { get; set; }
        public virtual DbSet<tblDevice> tblDevices { get; set; }
        public virtual DbSet<tblOTP> tblOTPs { get; set; }
        public virtual DbSet<tblAppNotification> tblAppNotifications { get; set; }
        public virtual DbSet<tblAppError> tblAppErrors { get; set; }
        public virtual DbSet<tblHintsNote> tblHintsNotes { get; set; }
        public virtual DbSet<tbldFuncChar> tbldFuncChars { get; set; }
        public virtual DbSet<tblClinics> tblClinics { get; set; }
        public virtual DbSet<tblAppAdmins> tblAppAdmins { get; set; }
        public virtual DbSet<tblSchedules> tblSchedules { get; set; }
        public virtual DbSet<tblErrorCodes> tblErrorCodes { get; set; }
        public virtual DbSet<tblPatients> tblPatients { get; set; }
        public virtual DbSet<tblDirectTrustPatient> tblDirectTrustPatient { get; set; }
        public virtual DbSet<tblRefreshTokens> tblRefreshTokens { get; set; }
        public virtual DbSet<tblReferrerSign> tblReferrerSign { get; set; }
        public virtual DbSet<tblDirectTrustPatientLog> tblDirectTrustPatientLog { get; set; }
        public virtual DbSet<tbl_DT_Referrers> tbl_DT_Referrers { get; set; }
        public virtual DbSet<tbl_DT_Referrer_Emails> tbl_DT_Referrer_Emails { get; set; }
        public virtual DbSet<tbl_DT_ClinicUserMapping> tbl_DT_ClinicUserMapping { get; set; }
        public virtual DbSet<tbl_DT_PatientReferralMRN> tbl_DT_PatientReferralMRN { get; set; }
        public virtual DbSet<tbl_DT_Outgoing_Message> tbl_DT_Outgoing_Message { get; set; }
        public virtual DbSet<tblDocMasterAddl> tblDocMasterAddl { get; set; }
        public virtual DbSet<tbl_DT_POC_Processed> tbl_DT_POC_Processed { get; set; }
        public virtual DbSet<tbl_DT_POC_NotFound> tbl_DT_POC_NotFound { get; set; }
        public virtual DbSet<tblPatScannedDocs> tblPatScannedDocs { get; set; }
        public virtual DbSet<tbl_DT_Incoming_Message_Individual_Attachments> tbl_DT_Incoming_Message_Individual_Attachments { get; set; }
        public virtual DbSet<tbl_DT_MessagesSent> tbl_DT_MessagesSent { get; set; }
        public virtual DbSet<tbl_DT_PatientReferral_Processed> tbl_DT_PatientReferral_Processed { get; set; }
        public virtual DbSet<tbl_DT_Incoming_Message> tbl_DT_Incoming_Message { get; set; }
        public virtual DbSet<tblRefAddr> tblRefAddr { get; set; }
        public virtual DbSet<tblReferrer> tblReferrer { get; set; }
    
        public virtual ObjectResult<SP_ValidateUser_Result> SP_ValidateUser(string userid, string userpassword)
        {
            var useridParameter = userid != null ?
                new ObjectParameter("userid", userid) :
                new ObjectParameter("userid", typeof(string));
    
            var userpasswordParameter = userpassword != null ?
                new ObjectParameter("userpassword", userpassword) :
                new ObjectParameter("userpassword", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_ValidateUser_Result>("SP_ValidateUser", useridParameter, userpasswordParameter);
        }
    
        public virtual int SP_GetDocMaster(Nullable<int> patientid)
        {
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("Patientid", patientid) :
                new ObjectParameter("Patientid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_GetDocMaster", patientidParameter);
        }
    
        public virtual int SP_PatientNotes(Nullable<int> patientid)
        {
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("Patientid", patientid) :
                new ObjectParameter("Patientid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_PatientNotes", patientidParameter);
        }
    
        public virtual int SP_GetDocMaster_Result(Nullable<int> patientid)
        {
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("Patientid", patientid) :
                new ObjectParameter("Patientid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_GetDocMaster_Result", patientidParameter);
        }
    
        public virtual int SP_GetDocMaster1(Nullable<int> patientid)
        {
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("Patientid", patientid) :
                new ObjectParameter("Patientid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_GetDocMaster1", patientidParameter);
        }
    
        public virtual int SP_GetDocMaster2(Nullable<int> patientid)
        {
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("Patientid", patientid) :
                new ObjectParameter("Patientid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_GetDocMaster2", patientidParameter);
        }
    
        public virtual ObjectResult<SP_PatientNotes_Result> SP_PatientNotes_Result(Nullable<int> patientid)
        {
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("Patientid", patientid) :
                new ObjectParameter("Patientid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_PatientNotes_Result>("SP_PatientNotes_Result", patientidParameter);
        }
    
        public virtual ObjectResult<SP_GetDocFuncCharac_Result> SP_GetDocFuncCharac(Nullable<int> docrowid, Nullable<int> patientid, string noteType)
        {
            var docrowidParameter = docrowid.HasValue ?
                new ObjectParameter("Docrowid", docrowid) :
                new ObjectParameter("Docrowid", typeof(int));
    
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("patientid", patientid) :
                new ObjectParameter("patientid", typeof(int));
    
            var noteTypeParameter = noteType != null ?
                new ObjectParameter("NoteType", noteType) :
                new ObjectParameter("NoteType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetDocFuncCharac_Result>("SP_GetDocFuncCharac", docrowidParameter, patientidParameter, noteTypeParameter);
        }
    
        public virtual ObjectResult<SP_GetPhyMeasure_Result> SP_GetPhyMeasure(Nullable<int> docrowid, string bodypart, string level, Nullable<int> patientid)
        {
            var docrowidParameter = docrowid.HasValue ?
                new ObjectParameter("Docrowid", docrowid) :
                new ObjectParameter("Docrowid", typeof(int));
    
            var bodypartParameter = bodypart != null ?
                new ObjectParameter("bodypart", bodypart) :
                new ObjectParameter("bodypart", typeof(string));
    
            var levelParameter = level != null ?
                new ObjectParameter("level", level) :
                new ObjectParameter("level", typeof(string));
    
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("patientid", patientid) :
                new ObjectParameter("patientid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetPhyMeasure_Result>("SP_GetPhyMeasure", docrowidParameter, bodypartParameter, levelParameter, patientidParameter);
        }
    
        public virtual ObjectResult<SP_GetPhyMeasureIE2_Result> SP_GetPhyMeasureIE2(Nullable<int> docrowid, string bodypart, string level, Nullable<int> patientid)
        {
            var docrowidParameter = docrowid.HasValue ?
                new ObjectParameter("Docrowid", docrowid) :
                new ObjectParameter("Docrowid", typeof(int));
    
            var bodypartParameter = bodypart != null ?
                new ObjectParameter("bodypart", bodypart) :
                new ObjectParameter("bodypart", typeof(string));
    
            var levelParameter = level != null ?
                new ObjectParameter("level", level) :
                new ObjectParameter("level", typeof(string));
    
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("patientid", patientid) :
                new ObjectParameter("patientid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetPhyMeasureIE2_Result>("SP_GetPhyMeasureIE2", docrowidParameter, bodypartParameter, levelParameter, patientidParameter);
        }
    
        public virtual ObjectResult<SP_GetDocBillInterventions_Result> SP_GetDocBillInterventions(Nullable<int> patientid, Nullable<int> docrowid, string type)
        {
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("patientid", patientid) :
                new ObjectParameter("patientid", typeof(int));
    
            var docrowidParameter = docrowid.HasValue ?
                new ObjectParameter("Docrowid", docrowid) :
                new ObjectParameter("Docrowid", typeof(int));
    
            var typeParameter = type != null ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetDocBillInterventions_Result>("SP_GetDocBillInterventions", patientidParameter, docrowidParameter, typeParameter);
        }
    
        public virtual ObjectResult<SP_GetDocNoteInterventions_Result> SP_GetDocNoteInterventions(Nullable<int> patientid, Nullable<int> docrowid, string type)
        {
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("patientid", patientid) :
                new ObjectParameter("patientid", typeof(int));
    
            var docrowidParameter = docrowid.HasValue ?
                new ObjectParameter("Docrowid", docrowid) :
                new ObjectParameter("Docrowid", typeof(int));
    
            var typeParameter = type != null ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetDocNoteInterventions_Result>("SP_GetDocNoteInterventions", patientidParameter, docrowidParameter, typeParameter);
        }
    
        public virtual ObjectResult<SP_GetDocProgExer_Result> SP_GetDocProgExer(Nullable<int> docrowid, Nullable<int> patientid, string report)
        {
            var docrowidParameter = docrowid.HasValue ?
                new ObjectParameter("Docrowid", docrowid) :
                new ObjectParameter("Docrowid", typeof(int));
    
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("patientid", patientid) :
                new ObjectParameter("patientid", typeof(int));
    
            var reportParameter = report != null ?
                new ObjectParameter("report", report) :
                new ObjectParameter("report", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetDocProgExer_Result>("SP_GetDocProgExer", docrowidParameter, patientidParameter, reportParameter);
        }
    
        public virtual ObjectResult<SP_GetDocSumInterventions_Result> SP_GetDocSumInterventions(Nullable<int> docrowid)
        {
            var docrowidParameter = docrowid.HasValue ?
                new ObjectParameter("Docrowid", docrowid) :
                new ObjectParameter("Docrowid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetDocSumInterventions_Result>("SP_GetDocSumInterventions", docrowidParameter);
        }
    
        public virtual ObjectResult<SP_GetDocTreatDesc_Result> SP_GetDocTreatDesc(Nullable<int> docrowid, Nullable<int> patientid)
        {
            var docrowidParameter = docrowid.HasValue ?
                new ObjectParameter("Docrowid", docrowid) :
                new ObjectParameter("Docrowid", typeof(int));
    
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("patientid", patientid) :
                new ObjectParameter("patientid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetDocTreatDesc_Result>("SP_GetDocTreatDesc", docrowidParameter, patientidParameter);
        }
    
        public virtual ObjectResult<string> SP_GetOthMeasure(Nullable<int> docrowid, string bodypart, string level, Nullable<int> patientid)
        {
            var docrowidParameter = docrowid.HasValue ?
                new ObjectParameter("Docrowid", docrowid) :
                new ObjectParameter("Docrowid", typeof(int));
    
            var bodypartParameter = bodypart != null ?
                new ObjectParameter("bodypart", bodypart) :
                new ObjectParameter("bodypart", typeof(string));
    
            var levelParameter = level != null ?
                new ObjectParameter("level", level) :
                new ObjectParameter("level", typeof(string));
    
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("patientid", patientid) :
                new ObjectParameter("patientid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("SP_GetOthMeasure", docrowidParameter, bodypartParameter, levelParameter, patientidParameter);
        }
    
        public virtual ObjectResult<string> SP_GetOthMeasureIE2(Nullable<int> docrowid, string bodypart, string level, Nullable<int> patientid)
        {
            var docrowidParameter = docrowid.HasValue ?
                new ObjectParameter("Docrowid", docrowid) :
                new ObjectParameter("Docrowid", typeof(int));
    
            var bodypartParameter = bodypart != null ?
                new ObjectParameter("bodypart", bodypart) :
                new ObjectParameter("bodypart", typeof(string));
    
            var levelParameter = level != null ?
                new ObjectParameter("level", level) :
                new ObjectParameter("level", typeof(string));
    
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("patientid", patientid) :
                new ObjectParameter("patientid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("SP_GetOthMeasureIE2", docrowidParameter, bodypartParameter, levelParameter, patientidParameter);
        }
    
        public virtual ObjectResult<SP_GetActiveDocAllCPTCodes_Result> SP_GetActiveDocAllCPTCodes(Nullable<int> pTrowid)
        {
            var pTrowidParameter = pTrowid.HasValue ?
                new ObjectParameter("PTrowid", pTrowid) :
                new ObjectParameter("PTrowid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetActiveDocAllCPTCodes_Result>("SP_GetActiveDocAllCPTCodes", pTrowidParameter);
        }
    
        public virtual ObjectResult<SP_GetActiveDocFCECPTCodes_Result> SP_GetActiveDocFCECPTCodes(Nullable<int> pTrowid)
        {
            var pTrowidParameter = pTrowid.HasValue ?
                new ObjectParameter("PTrowid", pTrowid) :
                new ObjectParameter("PTrowid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetActiveDocFCECPTCodes_Result>("SP_GetActiveDocFCECPTCodes", pTrowidParameter);
        }
    
        public virtual ObjectResult<SP_GetActiveDocProInterventions_Result> SP_GetActiveDocProInterventions(Nullable<int> docrowid, Nullable<int> pTrowid)
        {
            var docrowidParameter = docrowid.HasValue ?
                new ObjectParameter("Docrowid", docrowid) :
                new ObjectParameter("Docrowid", typeof(int));
    
            var pTrowidParameter = pTrowid.HasValue ?
                new ObjectParameter("PTrowid", pTrowid) :
                new ObjectParameter("PTrowid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetActiveDocProInterventions_Result>("SP_GetActiveDocProInterventions", docrowidParameter, pTrowidParameter);
        }
    
        public virtual ObjectResult<SP_GetDocExtremityTests_Result> SP_GetDocExtremityTests(string type, Nullable<int> docrowid, Nullable<int> patientid)
        {
            var typeParameter = type != null ?
                new ObjectParameter("Type", type) :
                new ObjectParameter("Type", typeof(string));
    
            var docrowidParameter = docrowid.HasValue ?
                new ObjectParameter("Docrowid", docrowid) :
                new ObjectParameter("Docrowid", typeof(int));
    
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("patientid", patientid) :
                new ObjectParameter("patientid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetDocExtremityTests_Result>("SP_GetDocExtremityTests", typeParameter, docrowidParameter, patientidParameter);
        }
    
        public virtual ObjectResult<SP_GetDocNoteSummary_Result> SP_GetDocNoteSummary(Nullable<int> docrowid)
        {
            var docrowidParameter = docrowid.HasValue ?
                new ObjectParameter("Docrowid", docrowid) :
                new ObjectParameter("Docrowid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetDocNoteSummary_Result>("SP_GetDocNoteSummary", docrowidParameter);
        }
    
        public virtual ObjectResult<SP_GetDocOMPTTests_Result> SP_GetDocOMPTTests(string type, Nullable<int> docrowid, Nullable<int> patientid)
        {
            var typeParameter = type != null ?
                new ObjectParameter("Type", type) :
                new ObjectParameter("Type", typeof(string));
    
            var docrowidParameter = docrowid.HasValue ?
                new ObjectParameter("Docrowid", docrowid) :
                new ObjectParameter("Docrowid", typeof(int));
    
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("patientid", patientid) :
                new ObjectParameter("patientid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetDocOMPTTests_Result>("SP_GetDocOMPTTests", typeParameter, docrowidParameter, patientidParameter);
        }
    
        public virtual ObjectResult<SP_GetDocPain_Result> SP_GetDocPain(string type, Nullable<int> docrowid, Nullable<int> patientid)
        {
            var typeParameter = type != null ?
                new ObjectParameter("Type", type) :
                new ObjectParameter("Type", typeof(string));
    
            var docrowidParameter = docrowid.HasValue ?
                new ObjectParameter("Docrowid", docrowid) :
                new ObjectParameter("Docrowid", typeof(int));
    
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("patientid", patientid) :
                new ObjectParameter("patientid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetDocPain_Result>("SP_GetDocPain", typeParameter, docrowidParameter, patientidParameter);
        }
    
        public virtual ObjectResult<SP_GetDocSpinalTests_Result> SP_GetDocSpinalTests(string type, Nullable<int> docrowid, Nullable<int> patientid)
        {
            var typeParameter = type != null ?
                new ObjectParameter("Type", type) :
                new ObjectParameter("Type", typeof(string));
    
            var docrowidParameter = docrowid.HasValue ?
                new ObjectParameter("Docrowid", docrowid) :
                new ObjectParameter("Docrowid", typeof(int));
    
            var patientidParameter = patientid.HasValue ?
                new ObjectParameter("patientid", patientid) :
                new ObjectParameter("patientid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetDocSpinalTests_Result>("SP_GetDocSpinalTests", typeParameter, docrowidParameter, patientidParameter);
        }
    
        public virtual ObjectResult<SP_GetPatDiagnosis_Result> SP_GetPatDiagnosis(Nullable<int> pTrowid)
        {
            var pTrowidParameter = pTrowid.HasValue ?
                new ObjectParameter("PTrowid", pTrowid) :
                new ObjectParameter("PTrowid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetPatDiagnosis_Result>("SP_GetPatDiagnosis", pTrowidParameter);
        }
    }
}
