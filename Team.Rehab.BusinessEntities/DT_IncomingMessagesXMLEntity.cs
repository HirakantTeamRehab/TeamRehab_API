using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
    public class DT_IncomingMessagesXMLEntity
    {
        public int? InComingMessageID { get; set; }

        public int? AttachmentID { get; set; }
        public string FileName { get; set; }
        public string FileData { get; set; }
        public DT_IncomingMessagesXMLCotentEntity XMLContent { get; set; }

    }
    public class DT_IncomingMessageViewmodelEntity
    {
        public int ID { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public Nullable<System.DateTime> Received { get; set; }
        public bool? MessageProcessed { get; set; }
        public List<DT_IncomingMessagesXMLEntity> XMLattachment { get; set; }
        public List<DT_IncomingMessagesXMLCotentEntity> XMLContent { get; set; }
        public string Operation { get; set; }
    }
    public class DT_IncomingMessagesXMLCotentEntity
    {

        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateTime birthTime { get; set; }
        public string streetAddressLine { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
        public string maritalStatus { get; set; }
        public string title { get; set; }
        public string phoneHP { get; set; }
        public string phoneWP { get; set; }
        public string phoneMC { get; set; }
        public string referrerClinic { get; set; }
        public string mrn { get; set; }
        public string doctorInfo { get; set; }

        public string npinumber { get; set; }


    }
    public class DT_IncomingMessageProcessEntity
    {
        public int InComingMessageID { get; set; }
        public int AttachmentID { get; set; }
        public string FileName { get; set; }
        public string FileData { get; set; }
        public string Action { get; set; }
        public string Username { get; set; }


    }
    public class DT_IncomingMessagOperationEntity
    {
        public int InComingMessageID { get; set; }
        public string FileData { get; set; }
        public string EmailID { get; set; }



    }
    public class DT_IncomingMessageOperation
    {
        public string Operation { get; set; }
        public List<DT_PatientIncomingMessage> DT_Patients { get; set; }
        public List<DT_IncomingMessagePOC> DT_POCs { get; set; }


    }

    //public class DT_IncomingMessagePatient
    //{
    //    public int Prowid { get; set; }
    //    public short ClinicNo { get; set; }
    //    public string Title { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string Gender { get; set; }
    //    public DateTime BirthDate { get; set; }
    //    public string Address1 { get; set; }
    //    public string City { get; set; }
    //    public string State { get; set; }
    //    public string ZipCode { get; set; }
    //    public string HomePh { get; set; }
    //    public string Email { get; set; }


    //}
    public class DT_IncomingMessagePOC
    {
        public int Docrowid { get; set; }
        public int PTrowid { get; set; }
        public string NoteType { get; set; }
        public Boolean Signed { get; set; }
        public DateTime SignedDate { get; set; }
        public string PDFName { get; set; }
        public string createdby { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DateOfService { get; set; }
        public DateTime? ApprovalSent { get; set; }
        public string TherapistName { get; set; }
        public byte[] Attachment { get; set; }


    }
    public class DT_PatientIncomingMessage
    {

        public int Prowid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Gender { get; set; }
        public string BirthDate { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string CellPh { get; set; }
        public string TherapistName { get; set; }
        public string Note { get; set; }
        public string MRNNumber { get; set; }
    }
    public class DT_ClinicMapping
    {
        public string ClinicNo { get; set; }
        public string ClinicName { get; set; }
        public string EmailId { get; set; }
    }
    public class DT_Clinics
    {
        public string ClinicNo { get; set; }
        public string ClinicName { get; set; }

    }
    public class CheckedMessages
    {
        public int[] incomingmessageIds { get; set; }
        public string operation { get; set; }

    }
    public class DT_MessagesSent
    {
        public int ID { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public DateTime? Sent { get; set; }
        public List<DT_MessagesSentAttachment> Attachment { get; set; }
    }
    public class DT_MessagesSentAttachment
    {
        public int ID { get; set; }
        public int MessageSentID { get; set; }
        public string FileName { get; set; }
        public string FileData { get; set; }
    }
}
