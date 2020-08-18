using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.InterfaceRepository;
using Team.Rehab.Repository.Repository;
using Team.Rehab.DataModel;
using Team.Rehab.Repository.UnitOfwork;
using System.Data;
using System.Data.SqlClient;
using Team.Rehab.BusinessEntities;
using System.Net;
using System.IO;
using System.Configuration;
using AutoMapper;
using Team.Rehab.Repository.Data;
using System.Net.Mail;
using System.Transactions;
using System.Xml;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Globalization;
using Team.Rehab.Repository.Common;
namespace Team.Rehab.Repository.Repository
{
    public class DirectTrustPatientRepository : IDirectTrustPatientRepository
    {
        private readonly IUnitOfwork _unitOfWork;

        // private readonly UnitOfWork _unitOfWork;
        private readonly IProcedureManagement _procedureManagement;

        public DirectTrustPatientRepository(IUnitOfwork unitOfWork)
        {
            this._unitOfWork = unitOfWork;

            this._procedureManagement = new ProcedureManagement();
        }

        public static string encode(string text)
        {
            byte[] mybyte = System.Text.Encoding.UTF8.GetBytes(text);
            string returntext = System.Convert.ToBase64String(mybyte);
            return returntext;
        }

        public static string decode(string text)
        {
            byte[] mybyte = System.Convert.FromBase64String(text);
            string returntext = System.Text.Encoding.UTF7.GetString(mybyte);
            return returntext;
        }
        public DT_AuthenticateEntity GetUser(string userName, string password)
        {
            using (Team.Rehab.DataModel.RehabEntities rehab = new RehabEntities())
            {
                // string encodedpassword = decode(password);
               password= Encryption.AESEncryptString(password,"","");
               //password = Encryption.AesDecryptString(password, "", "");
                DT_AuthenticateEntity DTpatients = (from user in rehab.tblUsers.Where(a => a.UserId == userName && a.UserPassword == password)
                                                    join usergrp in rehab.tblUserGroups on user.UserGroup equals usergrp.UGrowid


                                                    select new DT_AuthenticateEntity
                                                    {
                                                        FirstName = user.FirstName,
                                                        LastName = user.LastName,
                                                        UserID = user.UserId,
                                                        Role = usergrp.UserGroup
                                                        //PhoneNumber = therapist.LastName,
                                                        //Email = therapist.LastName

                                                    }
               ).Distinct().ToList().FirstOrDefault();
                return DTpatients;
                //if (DTpatients != null)
                //{
                //    return DTpatients;
                //}
                //else
                //{

                //    throw new UnauthorizedAccessException();
                //}
            }

        }
        public DT_AuthenticateEntity GetUserOnUsername(string userName)
        {
            using (Team.Rehab.DataModel.RehabEntities rehab = new RehabEntities())
            {
                // string encodedpassword = decode(password);
               
                //password = Encryption.AesDecryptString(password, "", "");
                DT_AuthenticateEntity DTpatients = (from user in rehab.tblUsers.Where(a => a.UserEmail == userName)
                                                    join usergrp in rehab.tblUserGroups on user.UserGroup equals usergrp.UGrowid


                                                    select new DT_AuthenticateEntity
                                                    {
                                                        FirstName = user.FirstName,
                                                        LastName = user.LastName,
                                                        UserID = user.UserId,
                                                        Role = usergrp.UserGroup
                                                    
                                                    }
               ).Distinct().ToList().FirstOrDefault();
                return DTpatients;
            
            }

        }

        public List<DT_PatientReferralProcessedEntity> GetDTReferralProcessedPatients()
        {
            List<DT_PatientReferralProcessedEntity> processedPatinetsEntity = new List<DT_PatientReferralProcessedEntity>();
            processedPatinetsEntity = (from patient in _unitOfWork.DT_PatientProcessedRepo.Get()
                                       select new DT_PatientReferralProcessedEntity
                                       {
                                           ID = patient.ID,
                                           XMLContent = patient.File_Selected_By_User,
                                           ReceptionistComments = patient.Status,
                                           Patient_ID = patient.PatientID ?? 0

                                       }).ToList();
            foreach (DT_PatientReferralProcessedEntity item in processedPatinetsEntity)
            {
                var patientdata = _unitOfWork.PatientEntityRepo.Get().Where(x => x.Prowid == item.Patient_ID).FirstOrDefault();
                if (patientdata != null)
                {
                    item.FirstName = patientdata.FirstName;
                    item.LastName = patientdata.LastName;
                    item.State = patientdata.State;
                    item.City = patientdata.City;
                    item.DOB = patientdata.BirthDate;
                }


            }
            return processedPatinetsEntity;
        }

        public Stream GenerateStreamFromString(string s)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(s));
        }

        /// <summary>
        /// Creates a product
        /// </summary>
        /// <param name="productEntity"></param>
        /// <returns></returns>
        public int CreateDTPatient(int incommingMsgID, string userID,string status,int attachmentID)
        {
            try
            {
                tblPatients patient = new tblPatients();
                tblReferrer referrerRecord = new tblReferrer();
                tblRefAddr referrerAddr = new tblRefAddr();

                //  tbl_DT_Incoming_Message_Individual_Attachments tblAttachment = new tbl_DT_Incoming_Message_Individual_Attachments();
                tbl_DT_PatientReferral_Processed processedPatient = new tbl_DT_PatientReferral_Processed();
                var incomingMessagPprocessEntity = (from p in _unitOfWork.DT_Incoming_MessageRepo.Get()
                                                    join e in _unitOfWork.DT_Individial_AttachRepo.Get()
                                                    on p.ID equals e.InComingMessageID 
                                                    where p.ID == incommingMsgID && e.ID == attachmentID
                                                    select new DT_IncomingMessagOperationEntity
                                                    {
                                                        FileData = e.FileData,
                                                        EmailID = p.From

                                                    }).ToList().FirstOrDefault();
                var DT_Referrer = (from p in _unitOfWork.DT_Referrer_EmailsRepo.Get()
                                   where p.DT_Email_Address == incomingMessagPprocessEntity.EmailID
                                   select p.Institute_ID).FirstOrDefault();

                string xmlString = incomingMessagPprocessEntity.FileData.ToString();

                //referrer = _unitOfWork.DT_Referrer_EmailsRepo.Get(o => o.Institute_ID == PatientWIP.Institute_ID).FirstOrDefault();

                using (StreamReader reader = new StreamReader(GenerateStreamFromString(xmlString), Encoding.Unicode))
                {           
                    string oldstr = "xmlns=\"urn:hl7-org:v3\"";
                    string oldstr1 = "xmlns:sdtc=\"urn:hl7-org:sdtc\"";

                    xmlString = xmlString.Replace(oldstr, "");
                    xmlString = xmlString.Replace(oldstr1, "");
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlString);

                    var nameNode = xmlDoc.SelectNodes("ClinicalDocument/recordTarget/patientRole/patient/name");
                    var DOBNode = xmlDoc.SelectNodes("ClinicalDocument/recordTarget/patientRole/patient");
                    var CityNode = xmlDoc.SelectNodes("ClinicalDocument/recordTarget/patientRole/addr");
                    var referrerNode = xmlDoc.SelectNodes("ClinicalDocument/legalAuthenticator/assignedEntity/assignedPerson/name");
                    var referrerAddrNode = xmlDoc.SelectNodes("ClinicalDocument/legalAuthenticator/assignedEntity/addr");
                    var referrerPhoneNode = xmlDoc.SelectNodes("ClinicalDocument/legalAuthenticator/assignedEntity/telecom");
                    XmlNodeList xnListtelecom = xmlDoc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/telecom");
                    ////ClinicalDocument//recordTarget//patientRole//patientRole//name
                    foreach (XmlNode subNode in nameNode)
                    {
                        patient.FirstName = subNode["given"].InnerText;
                        patient.LastName = subNode["family"].InnerText;
                        patient.NickName = patient.FirstName;

                       
                    }
                    foreach (XmlNode subNode in referrerNode)
                    {
                        string given2 = string.Empty;
                        if (subNode["given"].NextSibling != null)
                        {
                            given2= subNode["given"].NextSibling.InnerText.ToStringOrEmpty();
                        }
                        referrerRecord.FirstName = subNode["given"].InnerText.ToStringOrEmpty()+" "+ given2;
                        referrerRecord.LastName = subNode["family"].InnerText.ToStringOrEmpty();
                        referrerRecord.PrintName = subNode["family"].InnerText.ToStringOrEmpty() + ", " + subNode["given"].InnerText.ToStringOrEmpty() + " " + given2 + subNode["suffix"].InnerText.ToStringOrEmpty();
                        referrerRecord.Credentials = subNode["suffix"].InnerText.ToStringOrEmpty();
                    }
                    referrerRecord.ReferralType = "DT";
                    referrerRecord.SendPOCBy= "DT";
                    referrerRecord.createdby = "DT-Load";
                    referrerRecord.createdts = DateTime.Now;
                    referrerRecord.updatedby= "DT-Load";
                    referrerRecord.updatedts = DateTime.Now;
                    referrerRecord.NoFax = false;
                    referrerRecord.ReferralInstitute = DT_Referrer;
                    referrerRecord.Title = "Dr";
                    referrerRecord.NPINumber = "";
                    referrerRecord.Email = incomingMessagPprocessEntity.EmailID;
                    foreach (XmlNode subNode in DOBNode)
                    {
                        patient.BirthDate = subNode["birthTime"].Attributes["value"].Value;

                        DateTime dt;
                        if (DateTime.TryParseExact(patient.BirthDate.ToString(), "yyyyMMdd",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.None, out dt))
                        {
                            //Console.WriteLine(dt);
                            //  patient.BirthDate = dt.ToString();
                            patient.BirthDate = dt.ToShortDateString();
                        }


                        string gender = subNode["administrativeGenderCode"].Attributes["code"].Value;
                        if (gender.ToLower() == "f")
                            patient.Gender = "Female";
                        patient.Title = "Miss";
                        if (gender.ToLower() == "m")
                            patient.Gender = "Male";
                        patient.Title = "MR";


                    }
                    foreach (XmlNode subNode in CityNode)
                    {
                        patient.City = subNode["city"].InnerText;
                        patient.State = subNode["state"].InnerText;
                        patient.Address1 = subNode["streetAddressLine"].InnerText;
                        patient.ZipCode = subNode["postalCode"].InnerText;

                    }
                    foreach (XmlNode subNode in referrerAddrNode)
                    {
                        referrerAddr.City = subNode["city"].InnerText;
                        referrerAddr.State = subNode["state"].InnerText;
                        referrerAddr.StreetAddress = subNode["streetAddressLine"].InnerText;
                        referrerAddr.ZipCode = subNode["postalCode"].InnerText;
                        referrerAddr.startdt = DateTime.Now;
                        referrerAddr.enddt = DateTime.Now;
                        referrerAddr.createdby = "DT-Load";
                        referrerAddr.createdts = DateTime.Now;
                        referrerAddr.updatedby = "DT-Load";
                        referrerAddr.updatedts = DateTime.Now;
                        
                    }
                    foreach (XmlNode xn in referrerPhoneNode)
                    {
                        if (xn.Attributes[0].Value == "WP")
                        {
                            var ph = xn.Attributes[1].Value.Replace("-", "").Replace("(","").Replace(")","");
                            referrerAddr.PhoneNo = ph.ToStringOrEmpty().Length > 0 ? ph.Substring(6) : "";
                        }
                       
                    }
                    referrerAddr.FaxNo = "";

                    DateTime NowTime = DateTime.Now;
                    int M_Current_Day = NowTime.Day; // Current Date with Day Display
                    int M_Current_Month = Convert.ToInt32(DateTime.Today.Month);  //Current Month as a Single Integer Display
                    int CurrentYear = DateTime.Today.Year; // Display Year as Integer

                    patient.ReferralDate = DateTime.Now.ToShortDateString(); //**what is referral date?

                    patient.updatedts = DateTime.Now.ToShortDateString().StringToDate();
                    patient.createdts = DateTime.Now.ToShortDateString().StringToDate();

                    //patient.ReferralDate = CurrentYear.ToString()+M_Current_Month.ToString()+M_Current_Day.ToString();  //need to find referral date.
                   // patient.ReferralSource = incomingMessagPprocessEntity.EmailID == null ? "noemail" : incomingMessagPprocessEntity.EmailID.Substring(0, 9);

                   // patient.HomePh = "000000000"; //need to find patient phone.
                   
                    foreach (XmlNode xn in xnListtelecom)
                    {
                        if (xn.Attributes[0].Value == "HP")
                        {
                            patient.HomePh = xn.Attributes[1].Value.ToStringOrEmpty().Length > 0 ? xn.Attributes[1].Value.Substring(4) : "";
                        }
                        if (xn.Attributes[0].Value == "WP")
                        {
                            patient.WorkPh = xn.Attributes[1].Value.ToStringOrEmpty().Length > 0 ? xn.Attributes[1].Value.Substring(4) : "";
                        }
                        if (xn.Attributes[0].Value == "MC")
                        {
                            patient.CellPh = xn.Attributes[1].Value.ToStringOrEmpty().Length > 0 ? xn.Attributes[1].Value.Substring(4) : "";
                        }
                    }
                    //patient.ClinicNo = 7;
                    using (RehabEntities rehab = new RehabEntities())
                    {
                        var clinicnumber = (from inc in rehab.tbl_DT_ClinicUserMapping where inc.EmailId == incomingMessagPprocessEntity.EmailID.ToString() select inc.ClinicNo).FirstOrDefault();
                        if(clinicnumber==null)
                        {
                            patient.ClinicNo = Convert.ToInt16(clinicnumber);
                        }
                        else
                        {
                            patient.ClinicNo = 0;
                        }
                       
                    }
                        
                }


                using (var scopeCreate = new TransactionScope())
                {
                    _unitOfWork.ReferrerRepo.Insert(referrerRecord);
                    _unitOfWork.Save();

                    referrerAddr.Rrowid = referrerRecord.Rrowid;
                    _unitOfWork.RefAddrRepo.Insert(referrerAddr);
                    _unitOfWork.Save();



                    patient.ReferralSource = referrerRecord.Rrowid.ToStringOrEmpty();
                    _unitOfWork.PatientEntityRepo.Insert(patient);
                    _unitOfWork.Save();
                    
                    ImportPatient(patient.Prowid, incommingMsgID, userID, status, attachmentID);

                    scopeCreate.Complete();
                    return patient.Prowid;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public int ImportPatient(int paientID, int incommingMsgID, string userID, string status, int attachmentID)
        {
            int instituteId = 0;
            int flag = 0;
            tblPatScannedDocs patScanDoc = new tblPatScannedDocs();
            tbl_DT_PatientReferralMRN MRNEntity = new tbl_DT_PatientReferralMRN();
            try
            {
                var incomingMessagPprocessEntity = (from p in _unitOfWork.DT_Incoming_MessageRepo.Get()
                                                    join e in _unitOfWork.DT_Individial_AttachRepo.Get()
                                                    on p.ID equals e.InComingMessageID
                                                    where p.ID == incommingMsgID && e.ID==attachmentID
                                                    select new DT_IncomingMessagOperationEntity
                                                    {
                                                        FileData = e.FileData,
                                                        EmailID = p.From,
                                                        InComingMessageID=p.ID

                                                    }).ToList().FirstOrDefault();


                string xmlString = incomingMessagPprocessEntity.FileData.ToString();
                if (incomingMessagPprocessEntity.InComingMessageID > 0)
                {
                    var referrer = (from S in _unitOfWork.DT_ReferrerRepo.Get()
                                    join K in _unitOfWork.DT_Referrer_EmailsRepo.Get()
                                    on S.Institute_ID equals K.Institute_ID
                                    where K.DT_Email_Address == incomingMessagPprocessEntity.EmailID
                                    select new
                                    {
                                        Institute_ID = S.Institute_ID,
                                        EmailID = K.DT_Email_Address
                                    }).SingleOrDefault();
                    MRNEntity.Institute_ID = referrer.Institute_ID.ToString();
                    instituteId = referrer.Institute_ID;
                }
                patScanDoc.PDFName = GeneratePdf(xmlString, paientID);
                patScanDoc.PTrowid = paientID;
                patScanDoc.doctype = "DT_Message";
                //patScanDoc.createdby = ;
                patScanDoc.createdts = DateTime.Now;
                MRNEntity.InstitutePatientMRN = "";

                MRNEntity.PatientId = paientID;
                var tblMrn = _unitOfWork.DT_PatientReferralMRNRepo.Get(o => o.ID == paientID).FirstOrDefault();
                using (var scopeImport = new TransactionScope())
                {

                    _unitOfWork.PatScannedDocsRepo.Insert(patScanDoc);

                    if (tblMrn != null)
                    {
                        if (tblMrn.ID > 0)
                        {
                            if (instituteId != 0 || !string.IsNullOrEmpty(MRNEntity.InstitutePatientMRN))
                                _unitOfWork.DT_PatientReferralMRNRepo.Insert(MRNEntity);

                        }
                    }
                    MoveIncomingtoReferrerProccessed(paientID, incommingMsgID, xmlString, instituteId, status, userID);
                    _unitOfWork.Save();
                    scopeImport.Complete();
                 
                    flag = 1;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return flag;
        }

        public string GeneratePdf(string htmlPdf, int patientId)
        {
            string ImportPath = Convert.ToString(ConfigurationManager.AppSettings["DirectTrustImportPDFPath"]);
            ImportPath = ImportPath + "/" + Convert.ToString(patientId) + ".pdf";
            var pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            var htmlparser = new HTMLWorker(pdfDoc);
            using (var memoryStream = new MemoryStream())
            {
                var writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                pdfDoc.Open();

                htmlparser.Parse(new StringReader(htmlPdf));
                pdfDoc.Close();

                byte[] bytes = memoryStream.ToArray();
                // File.WriteAllBytes(@"C:\file.pdf", bytes);
                File.WriteAllBytes(ImportPath, bytes);
                memoryStream.Close();

            }
            return ImportPath;
        }

        public int RejectDTPatient(int paientID, int incommingMsgID, string userID, int attachmentID)
        {
            int flag = 0;
            int instituteId = 0;
            try
            {
                var incomingMessagPprocessEntity = (from p in _unitOfWork.DT_Incoming_MessageRepo.Get()
                                                    join e in _unitOfWork.DT_Individial_AttachRepo.Get()
                                                    on p.ID equals e.InComingMessageID
                                                    where p.ID == incommingMsgID && e.ID== attachmentID
                                                    select new DT_IncomingMessagOperationEntity
                                                    {
                                                        FileData = e.FileData,
                                                        EmailID = p.From

                                                    }).ToList().FirstOrDefault();


                string xmlString = incomingMessagPprocessEntity.FileData.ToString();
                if (incomingMessagPprocessEntity.InComingMessageID > 1)
                {
                    var referrer = (from S in _unitOfWork.DT_ReferrerRepo.Get()
                                    join K in _unitOfWork.DT_Referrer_EmailsRepo.Get()
                                    on S.Institute_ID equals K.Institute_ID
                                    where K.DT_Email_Address == incomingMessagPprocessEntity.EmailID
                                    select new
                                    {
                                        Institute_ID = S.Institute_ID,
                                        EmailID = K.DT_Email_Address
                                    }).SingleOrDefault();

                    instituteId = referrer.Institute_ID;
                }
                MoveIncomingtoReferrerProccessed(paientID, incommingMsgID, xmlString, instituteId, "Rejected", userID);
            }
            catch (Exception ex)
            {

                throw;
            }

            return flag;

        }

        public bool MoveIncomingtoReferrerProccessed(int patientID, int incommingProcesID, string fileData, int instituteID, string status, string userId)
        {
            bool flag = false;
            try
            {
                tbl_DT_PatientReferral_Processed processed = new tbl_DT_PatientReferral_Processed();
                var incommingMsg = _unitOfWork.DT_Incoming_MessageRepo.Get(o => o.ID == incommingProcesID).FirstOrDefault();
                var incommingMsgAttachment = _unitOfWork.DT_Individial_AttachRepo.Get(o => o.InComingMessageID == incommingProcesID).ToList();

                processed.From = incommingMsg.From;
                processed.To = incommingMsg.To;
                processed.Subject = incommingMsg.Subject;
                processed.File_Selected_By_User = fileData;
                processed.MessageBody = incommingMsg.MessageBody;
                processed.Received_TimeStamp = incommingMsg.Received;
                processed.Processed_TimeStamp = DateTime.Now;
                processed.PatientID = patientID;
                processed.Institute_ID = instituteID;
                processed.Status = status;
                processed.UserID = userId;
                processed.Attachment = incommingMsg.Attachment;

                using (TransactionScope scope =
             new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 15, 0)))
                {
                    
                    _unitOfWork.DT_PatientReferral_ProcessedRepo.Insert(processed);
                    _unitOfWork.DT_Incoming_MessageRepo.Delete(incommingProcesID);
                    //if(status=="Rejected")
                    _unitOfWork.Save();


                    foreach (var attachment in incommingMsgAttachment)
                    {
                        attachment.ReferralProcessedID = processed.ID;
                        
                        _unitOfWork.DT_Individial_AttachRepo.Update(attachment);
                        _unitOfWork.Save();
                    }


                   
                    _unitOfWork.Save();

                    scope.Complete();
                    flag = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return flag;
        }

        public bool MoveReferrerProccessedtoIncomingMessage(string processedID)
        {
            bool flag = false;
            int ID = processedID != null ? Convert.ToInt32(processedID) : 0;
            try
            {
                tbl_DT_Incoming_Message incommingMsg = new tbl_DT_Incoming_Message();
                var processed = _unitOfWork.DT_PatientReferral_ProcessedRepo.Get(o => o.ID == ID).FirstOrDefault();
                var incommingMsgAttachment = _unitOfWork.DT_Individial_AttachRepo.Get(o => o.ReferralProcessedID == ID).ToList();

                incommingMsg.From = processed.From;
                incommingMsg.To = processed.To;
                incommingMsg.Subject = processed.Subject;

                incommingMsg.MessageBody = processed.MessageBody;
                incommingMsg.Received = processed.Received_TimeStamp;

                incommingMsg.MessageProcessed = false;

                using (TransactionScope scope =
             new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 15, 0)))
                {

                    _unitOfWork.DT_Incoming_MessageRepo.Insert(incommingMsg);
                    _unitOfWork.Save();

                    foreach(var attachment in incommingMsgAttachment)
                    {
                        attachment.ReferralProcessedID = null;
                        attachment.InComingMessageID = incommingMsg.ID;
                        _unitOfWork.DT_Individial_AttachRepo.Update(attachment);
                        _unitOfWork.Save();
                    }
                   

                  
                    _unitOfWork.DT_PatientReferral_ProcessedRepo.Delete(ID);
                    _unitOfWork.Save();

                    scope.Complete();
                    flag = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return flag;
        }


        public List<DT_ClinicUserEntity> GetDTClinicIDMapping()
        {
            List<DT_ClinicUserEntity> processedPatinetsEntity = new List<DT_ClinicUserEntity>();
            //processedPatinetsEntity = (from patient in _unitOfWork.DT_PatientProcessedRepo.Get()
            //                           select new DT_ClinicUserEntity
            //                           {
            //                               CUrowid = patient.ID,
            //                               EmailId = patient.XMLContent,
            //                               ClinicName = patient.MatchFound,
            //                               Crowid = patient.ReceptionistComments


            //                           }).ToList();
            return processedPatinetsEntity;
        }
        public string CheckClinicisAssigned(string username)
        {
            string referralemail = "integration@test.direct.team-rehab.com";
            using (RehabEntities rehab = new RehabEntities())
            {
                var clinicnum = (from inc in rehab.tblUsers where inc.UserId == username select inc.DefaultClinic).FirstOrDefault();
                if (clinicnum != null)
                {


                    var clinicnumber = (from inc in rehab.tblClinics where inc.Crowid == clinicnum select inc.ClinicNo).FirstOrDefault();
                    if (clinicnumber != null)
                    {
                        Int32 num = Convert.ToInt32(clinicnum);
                        var existsmail = (from inc in rehab.tbl_DT_ClinicUserMapping where inc.ClinicNo == clinicnumber.ToString() select inc.EmailId).FirstOrDefault();
                        if (existsmail == null)
                        {
                            referralemail = "integration@test.direct.team-rehab.com";
                        }
                        else
                        {
                            referralemail = existsmail;
                        }

                    }
                }
            }
            return referralemail;
        }
        public DT_POCListEntity GetPOCProcessed(string username)
        {
            try
            {
                string referralemail = CheckClinicisAssigned(username);
                DT_POCListEntity msterPOCList = new DT_POCListEntity();
                List<DT_PatientReferral_ProcessedEntity> processedPOC = new List<DT_PatientReferral_ProcessedEntity>();
                processedPOC = (from POC in _unitOfWork.DT_POC_ProcessedRepo.Get().Where(x => x.To == referralemail)
                                select new DT_PatientReferral_ProcessedEntity
                                {
                                    From = POC.From,
                                    To = POC.To,
                                    Subject = POC.Subject,
                                    MessageBody = POC.MessageBody,
                                    Received_TimeStamp = POC.Received_TimeStamp,
                                    Processed_TimeStamp = POC.Processed_TimeStamp,
                                    PatientID = POC.Institute_ID,
                                    Institute_ID = POC.Institute_ID,
                                    Status = POC.Status,
                                    UserID = POC.UserID

                                }).ToList();
                msterPOCList.pocProcessed = processedPOC;
                List<DT_PatientReferral_ProcessedEntity> declinedPOC = new List<DT_PatientReferral_ProcessedEntity>();
                declinedPOC = (from POC in _unitOfWork.DT_POC_NotFoundRepo.Get().Where(x => x.To == referralemail)
                               select new DT_PatientReferral_ProcessedEntity
                               {
                                   From = POC.From,
                                   To = POC.To,
                                   Subject = POC.Subject,
                                   MessageBody = POC.MessageBody,
                                   Received_TimeStamp = POC.Received_TimeStamp,
                                   Processed_TimeStamp = POC.Processed_TimeStamp,
                                   PatientID = POC.Institute_ID,
                                   Institute_ID = POC.Institute_ID,
                                   Status = POC.Status,
                                   UserID = POC.UserID

                               }).ToList();
                msterPOCList.pocDeclined = declinedPOC;
                return msterPOCList;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }

        }

        public List<DT_Outgoing_Message> GetPOCSent(string username)
        {
            try
            {
                string referralemail = CheckClinicisAssigned(username);
                List<DT_Outgoing_Message> processedPOC = new List<DT_Outgoing_Message>();
                processedPOC = (from POC in _unitOfWork.DT_Outgoing_MessageRepo.Get().Where(x => x.To == referralemail)
                                where POC.Approval_Received_Timestamp == null
                                select new DT_Outgoing_Message
                                {
                                    From = POC.From,
                                    To = POC.To,
                                    Subject = POC.Subject,
                                    MessageBody = POC.MessageBody,
                                    Sent_Timestamp = POC.Sent_Timestamp

                                }).ToList();
                return processedPOC;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
        }

        public List<DT_Message_ProcessedEntity> GetProcessedMessages(string username)
        {
            try
            {
                string referralemail = CheckClinicisAssigned(username);
                //List<tbl_DT_PatientReferral_Processed> processedMsgs = new List<tbl_DT_PatientReferral_Processed>();
                var processedMsgs = (from POC in _unitOfWork.DT_PatientReferral_ProcessedRepo.Get().Where(x => x.To == referralemail)

                                     select new DT_Message_ProcessedEntity
                                 {
                                     From = POC.From,
                                     To = POC.To,
                                     Subject = POC.Subject,
                                     MessageBody = POC.MessageBody,
                                     Received_TimeStamp = POC.Received_TimeStamp,
                                     Processed_TimeStamp = POC.Processed_TimeStamp,
                                     PatientID = POC.Institute_ID,
                                     Institute_ID = POC.Institute_ID,
                                     Status = POC.Status,
                                     UserID = POC.UserID,
                                     ID=POC.ID
                                 }
                                 ).OrderByDescending(x => x.ID).ToList();


                return processedMsgs;
            }

            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
        }
    }
}
