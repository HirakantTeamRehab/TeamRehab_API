using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Team.Rehab.BusinessEntities;
using Team.Rehab.InterfaceRepository;
using Team.Rehab.DataModel;
using Team.Rehab.Repository.UnitOfwork;
using System.Xml;
using System.Globalization;
using Team.Rehab.Repository.Data;
using System.Security.Cryptography;
using System.IO;
using System.Data;

namespace Team.Rehab.Repository.Repository
{
    public class DirectTrustReferrerRepository : IDirectTrustReferrerRepository
    {
        //  private readonly IUnitOfwork _unitOfWork;

        ////  private RehabEntities _ctx;
        //  public DirectTrustReferrerRepository(IUnitOfwork unitOfWork)
        //  {
        //      this._unitOfWork = unitOfWork;
        //     // this._unitOfWork = new UnitOfWork();
        //      //_ctx = new AtlasEntities();
        //  }
        private readonly UnitOfWork _unitOfWork;
        public DirectTrustReferrerRepository()
        {
            //this._unitOfWork = unitOfWork;
            this._unitOfWork = new UnitOfWork();
            //_ctx = new AtlasEntities();
        }
        /// <summary>
        /// Fetches product details by id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>


        /// <summary>
        /// Fetches all the products.
        /// </summary>
        /// <returns></returns>
        public List<DT_UserEntity> GetAllDTUsers()
        {
            List<DT_UserEntity> dtUsers = new List<DT_UserEntity>();

            using (RehabEntities rehab = new RehabEntities())
            {
                dtUsers = (from pd in rehab.tbl_DT_Referrers
                               //  join od in rehab.tbl_DT_Referrer_Emails on pd.Institute_ID equals od.Institute_ID
                           select new DT_UserEntity
                           {
                               RefID = pd.Institute_ID,
                               //  RefEmailID = od.ID,                              
                               Institute_Name = pd.Institute_Name,
                               //DT_Email_Address = od.DT_Email_Address
                           }).ToList();
                return dtUsers;

            }

        }

        /// <summary>
        /// Creates a product
        /// </summary>
        /// <param name="productEntity"></param>
        /// <returns></returns>
        public int CreateDTUser(DT_UserEntity[] dtUserEntity)
        {
            using (var scope = new TransactionScope())
            {
                var tbl_DT_Referrers = new tbl_DT_Referrers
                {
                    Institute_ID = dtUserEntity[0].RefID,
                    // Institute_ID = dtUserEntity.Institute_ID,
                    Institute_Name = dtUserEntity[0].Institute_Name

                };


                _unitOfWork.DT_ReferrerRepo.Insert(tbl_DT_Referrers);
                _unitOfWork.Save();

                foreach (var item in dtUserEntity)
                {
                    var tbl_DT_Referrer_Emails = new tbl_DT_Referrer_Emails
                    {
                        ID = item.RefEmailID,
                        Institute_ID = tbl_DT_Referrers.Institute_ID,
                        DT_Email_Address = item.DT_Email_Address

                    };
                    _unitOfWork.DT_Referrer_EmailsRepo.Insert(tbl_DT_Referrer_Emails);
                    _unitOfWork.Save();
                }

                scope.Complete();
                return tbl_DT_Referrers.Institute_ID;
            }
        }

        public bool CheckDocExists(DT_IncomingMessageProcessEntity xmldata)
        {
            int instituteId = 0;
            List<DT_IncomingMessagePOC> poclist = null;
            string xmlstring = xmldata.FileData;
            int? referrerInstituteId = 0;

            string oldstr = "xmlns=\"urn:hl7-org:v3\"";
            string oldstr1 = "xmlns:sdtc=\"urn:hl7-org:sdtc\"";

            xmlstring = xmlstring.Replace(oldstr, "");
            xmlstring = xmlstring.Replace(oldstr1, "");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlstring);
            string status = string.Empty;

            XmlNodeList xnList = doc.SelectNodes("ClinicalDocument");
            //XmlNodeList xnList = doc.GetElementsByTagName("ClinicalDocument");
            string TeamRehab_Patient_ID = "0";
            string TeamRehab_Note_ID = "0";
            string TR_OutgoingMessage_ID = "0";

            
            if (xmldata.Action== "Plan of care approved")
            {
                status = "Approved";
            }
            else if(xmldata.Action == "Plan of care declined")
            {
                status = "Plan of care declined";
            }

            XmlNodeList xnListname = doc.SelectNodes("/ClinicalDocument/id/TeamRehabID");
            foreach (XmlNode xn in xnListname)
            {
                if(xn["PatientID"]!=null)
                {
                    TeamRehab_Patient_ID = xn["PatientID"].InnerText == "" ? "0" : xn["PatientID"].InnerText;
                }
                if(xn["NoteID"]!=null)
                {
                    TeamRehab_Note_ID = xn["NoteID"].InnerText == "" ? "0" : xn["NoteID"].InnerText;
                }
                if (xn["TR_OutgoingMessage_ID"] != null)
                {
                     TR_OutgoingMessage_ID = xn["TR_OutgoingMessage_ID"].InnerText; //awaiting Input frpm Deb
                }

            }
            if (TeamRehab_Patient_ID != "0" && TeamRehab_Note_ID != "0")
            {
                int patient = Convert.ToInt32(TeamRehab_Patient_ID);
                int note = Convert.ToInt32(TeamRehab_Note_ID);
                int outgoing = Convert.ToInt32(TR_OutgoingMessage_ID);
                using (RehabEntities rehab = new RehabEntities())
                {
                    string senderemail = (from pd in rehab.tbl_DT_Incoming_Message where pd.ID == xmldata.InComingMessageID select pd.From).FirstOrDefault();

                    referrerInstituteId = (from pd in rehab.tbl_DT_Referrer_Emails where pd.DT_Email_Address == senderemail select pd.Institute_ID).FirstOrDefault();
                   
                    var docmasterdetail = (from pd in rehab.tblDocMasters
                                           where pd.PTrowid == patient
                                                && pd.Docrowid == note
                                           select pd).FirstOrDefault();
                    if(docmasterdetail!=null)
                    {
                        var docmaster = new tblDocMasterAddl
                        {
                            Docrowid = Convert.ToInt32(TeamRehab_Note_ID),
                            RSrowid = referrerInstituteId ?? 0,
                            PDFName = xmldata.FileName,
                            createdts = DateTime.UtcNow,
                            createdby = xmldata.Username
                        };

                        _unitOfWork.DT_DocMasterAddlRepo.Insert(docmaster);
                        _unitOfWork.Save();

                        if (TR_OutgoingMessage_ID != "0")
                        {
                            var outgoingrec = _unitOfWork.DT_Outgoing_MessageRepo.Get().Where(x => x.ID == outgoing).FirstOrDefault();
                            if (outgoingrec.ID > 0)
                            {
                                outgoingrec.Approval_Received_Timestamp = DateTime.Now;

                                _unitOfWork.DT_Outgoing_MessageRepo.Update(outgoingrec);
                                _unitOfWork.Save();
                            }
                            else
                            {
                                var outgngrec = _unitOfWork.DT_Outgoing_MessageRepo.Get().Where(x => x.Note_ID ==note &&
                                                         x.Patient_ID == patient).FirstOrDefault();

                                outgngrec.Approval_Received_Timestamp = DateTime.Now;

                                _unitOfWork.DT_Outgoing_MessageRepo.Update(outgngrec);
                                _unitOfWork.Save();
                            }
                        }
                        else
                        {

                            var outgoingrec = _unitOfWork.DT_Outgoing_MessageRepo.Get().Where(x => x.Note_ID == note &&
                              x.Patient_ID == patient).FirstOrDefault();

                            outgoingrec.Approval_Received_Timestamp = DateTime.Now;

                            _unitOfWork.DT_Outgoing_MessageRepo.Update(outgoingrec);
                            _unitOfWork.Save();
                        }
                        MoveIncomingtoPOCProccessed(Convert.ToInt32(TeamRehab_Patient_ID), xmldata.InComingMessageID, xmlstring, referrerInstituteId??0, status, xmldata.Username);
                        //ProcessedPOCFlow(xmldata.InComingMessageID, xmldata.Username, xmlstring, Convert.ToInt32(TeamRehab_Patient_ID), referrerInstituteId);
                    }
                    else
                    {
                        instituteId = GetInstitute_ID(xmldata.InComingMessageID);
                        MoveIncomingtoPOCProccessed(0, xmldata.InComingMessageID, xmlstring, referrerInstituteId??0, "NotFound", xmldata.Username);
                    }


                    return true;
                }
            }
            else
            {
               
                return false;
            }

        }
        public bool ProcessPOC(Int32 patientId, string operation, Int32 InComingMessageID, Int32 noteId, string Filename, string username, int attachmentID)
        {

            int? referrerInstituteId = 0;
            string xmlstring = "";
            using (RehabEntities rehab = new RehabEntities())
            {
                xmlstring = (from att in rehab.tbl_DT_Incoming_Message_Individual_Attachments
                             where att.ID == attachmentID
                             select att.FileData).FirstOrDefault();
            }
            using (RehabEntities rehab = new RehabEntities())
            {
                var sender = (from pd in rehab.tbl_DT_Incoming_Message where pd.ID == InComingMessageID select pd).FirstOrDefault();

                referrerInstituteId = (from pd in rehab.tbl_DT_Referrer_Emails where pd.DT_Email_Address == sender.From select pd.Institute_ID).FirstOrDefault();
                var docmaster = new tblDocMasterAddl
                {
                    Docrowid = noteId,
                    RSrowid = referrerInstituteId ?? 0,
                    PDFName = Filename,
                    createdts = DateTime.UtcNow,
                    createdby = username
                };
                _unitOfWork.DT_DocMasterAddlRepo.Insert(docmaster);
                _unitOfWork.Save();


                var outgoingrec = _unitOfWork.DT_Outgoing_MessageRepo.Get().Where(x => x.Note_ID == noteId &&
                     x.Patient_ID == patientId && x.Approval_Received_Timestamp==null).FirstOrDefault();

                outgoingrec.Approval_Received_Timestamp = DateTime.Now;

                _unitOfWork.DT_Outgoing_MessageRepo.Update(outgoingrec);
                _unitOfWork.Save();

                
                if(operation== "Plan of care approved")
                {
                    MoveIncomingtoPOCProccessed(patientId, InComingMessageID, xmlstring, referrerInstituteId ?? 0, "Approved", username);
                }
                else if(operation == "Plan of care declined")
                {
                    MoveIncomingtoPOCProccessed(patientId, InComingMessageID, xmlstring, referrerInstituteId ?? 0, "Declined", username);
                }
               

                return true;
            }

        }
        public DT_IncomingMessageOperation GetPOClistbyPatientID(Int32 patientId, string operation)
        {
            DT_IncomingMessageOperation poclist = new DT_IncomingMessageOperation();
            var outgoingrec = _unitOfWork.DT_Outgoing_MessageRepo.Get().Where(x => x.Approval_Received_Timestamp == null &&
                          x.Patient_ID == patientId).FirstOrDefault();

            string pdfFilePath = "c:/PDF/testfile.pdf";
            System.IO.MemoryStream memStream;

            byte[] Filebytes = System.IO.File.ReadAllBytes(pdfFilePath);
            byte[] fileData;

            memStream = new MemoryStream(Filebytes);
            MemoryStream expandableStream = new MemoryStream();
            memStream.CopyTo(expandableStream);
            fileData = expandableStream.ToArray();

            int noteid = outgoingrec?.Note_ID ?? 0;
          
            using (RehabEntities rehab = new RehabEntities())
            {
                var patientTherapist = (from pd in rehab.tblPatients where pd.Prowid == patientId select pd.TherapistID).FirstOrDefault();
                var therapist = (from th in rehab.tblTherapists where th.TherapistID == patientTherapist select th).FirstOrDefault();
                //var doc_poc = (
                //               from pd in rehab.tblPatients.ToList()
                //               //join cd in rehab.tbl_DT_Outgoing_Message.ToList() on
                //               //pd.DPTrowid equals cd.Patient_ID
                //               join th in rehab.tblTherapists.ToList() on
                //               pd.TherapistID equals th.TherapistID where pd.Prowid == patientId
                //               join nt in rehab.tblDocMasters.ToList() on
                //               outgoingrec.Note_ID equals nt.Docrowid
                //               //where pd.Prowid == patientId

                //               //where pd.PTrowid == patientId && pd.Docrowid== outgoingrec.Note_ID && cd.Approval_Received_Timestamp == null
                //               select new DT_IncomingMessagePOC
                //               {
                //                   Docrowid = nt.Docrowid,
                //                   PTrowid = nt.PTrowid ?? 0,
                //                   NoteType = nt.NoteType,
                //                   PDFName = nt.PDFName,
                //                   ApprovalSent= outgoingrec.Sent_Timestamp,
                //                   TherapistName=th.FirstName+" "+th.LastName,
                //                   Attachment= Filebytes
                //               }).ToList();

                var doc_poc = (from ab in rehab.tblDocMasters
                              where ab.Docrowid== noteid
                              
                               select new DT_IncomingMessagePOC
                              {
                                  Docrowid = ab.Docrowid,
                                  PTrowid = patientId,
                                  NoteType = ab.NoteType,
                                  PDFName = ab.PDFName,
                                  ApprovalSent = outgoingrec.Sent_Timestamp,
                                  TherapistName = therapist.FirstName + " " + therapist.LastName,
                                  Attachment = fileData
                               }).ToList();


                poclist.DT_POCs = doc_poc;
                poclist.Operation = operation;
            }


            return poclist;

        }

        public byte[] GetPOCFile(Int32 Note_ID)
        {
            byte[] doc_poc = null;
            using (RehabEntities rehab = new RehabEntities())
            {


                doc_poc = (from ab in rehab.tbl_DT_Outgoing_Message
                               where ab.Note_ID == Note_ID
                               select ab.Attachment).FirstOrDefault();

            }
           // string pdfFilePath = "c:/PDF/testfile.pdf";
            System.IO.MemoryStream memStream;

            //byte[] Filebytes = System.IO.File.ReadAllBytes(pdfFilePath);

            byte[] Filebytes = doc_poc;
            byte[] fileData;

            memStream = new MemoryStream(Filebytes);
            MemoryStream expandableStream = new MemoryStream();
            memStream.CopyTo(expandableStream);
            fileData = expandableStream.ToArray();

            return fileData;

        }

        public bool ProcessedPOCFlow(Int32 InComingMessageID, string username,string Filedata, int patientID,int? InstituteID)
        {
            var processed = _unitOfWork.DT_Incoming_MessageRepo.Get().Where(x => x.ID == InComingMessageID).FirstOrDefault();

            // add record in dt_poc_processed table
            //tbl_DT_POC_Processed processeddoc = new tbl_DT_POC_Processed()
            //{
            //  UserID=username,
            //  From= processed.From,
            //  To=processed.To,
            //  Subject=processed.Subject,
            //  Attachment=processed.Attachment,
            //  File_Selected_By_User= FileName,
            //  MessageBody=processed.MessageBody,
            //  Received_TimeStamp=processed.Received,
            //  PatientID= patientID,
            //  Institute_ID= InstituteID,
            //  Status="Approved",
            //};
            //// ADD RECOTD TO DT_POC_Processed TABLE
            //_unitOfWork.DT_POC_ProcessedRepo.Insert(processeddoc);
            //_unitOfWork.Save();

            MoveIncomingtoPOCProccessed(patientID, InComingMessageID, Filedata, InstituteID??0, "Patient found in easy doc", username);

            // DELETE RECORD FROM INCOMING MESSAGE TABLE
            _unitOfWork.DT_Incoming_MessageRepo.Delete(processed);
            _unitOfWork.Save();
            return true;

        }
        public List<DT_PatientIncomingMessage> CheckPatientsExists(DT_IncomingMessageProcessEntity xmldata)
        {
            List<DT_PatientIncomingMessage> patientlist = null;
            string xmlstring = xmldata.FileData;

            string oldstr = "xmlns=\"urn:hl7-org:v3\"";
            string oldstr1 = "xmlns:sdtc=\"urn:hl7-org:sdtc\"";

            xmlstring = xmlstring.Replace(oldstr, "");
            xmlstring = xmlstring.Replace(oldstr1, "");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlstring);

            XmlNodeList xnList = doc.SelectNodes("ClinicalDocument");
            //XmlNodeList xnList = doc.GetElementsByTagName("ClinicalDocument");
            string firstName = string.Empty;
            string lastName = string.Empty;
            DateTime birthTime = DateTime.MinValue;
            string streetAddressLine = string.Empty;
            string city = string.Empty;
            string state = string.Empty;
            string postalCode = string.Empty;
            string country = string.Empty;


            XmlNodeList xnList1 = doc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/patient/name");
            XmlNodeList xnListname = doc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/patient/name");
            foreach (XmlNode xn in xnListname)
            {
                firstName = xn["given"].InnerText;
                lastName = xn["family"].InnerText;

            }

            XmlNodeList xnListbirthTime = doc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/patient/birthTime");
            foreach (XmlNode xn in xnListbirthTime)
            {
                if (xn.Attributes[0].Value.Contains("/"))
                {
                    DateTime myDate = Convert.ToDateTime(xn.Attributes[0].Value);
                    xn.Attributes[0].Value = myDate.ToString("yyyyMMdd");
                }
                try
                {
                    int i = Convert.ToInt32(xn.Attributes[0].Value);
                    DateTime dt;
                    if (DateTime.TryParseExact(i.ToString(), "yyyyMMdd",
                                              CultureInfo.InvariantCulture,
                                              DateTimeStyles.None, out dt))
                    {
                        //Console.WriteLine(dt);
                        birthTime = dt;
                    }

                }

                catch (Exception ex)
                {


                    birthTime = xn.Attributes[0].Value.ToString().StringToDate();
                }
            }

            XmlNodeList xnListaddr = doc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/addr");
            foreach (XmlNode xn in xnListaddr)
            {

                streetAddressLine = xn["streetAddressLine"].InnerText;
                city = xn["city"].InnerText;
                state = xn["state"].InnerText;
                postalCode = xn["postalCode"].InnerText;
                country = xn["country"].InnerText;

            }
           var dt_birthTime = birthTime.ToShortDateString();

            using (RehabEntities rehab = new RehabEntities())
            {
                try
                {

                    patientlist = (from pd in rehab.tblPatients
                                   join od in rehab.tblTherapists on pd.TherapistID equals od.TherapistID
                                   into ps
                                   from od in ps.DefaultIfEmpty()
                                   where (pd.FirstName == firstName &&
                                   pd.LastName == lastName &&
                                      pd.BirthDate == dt_birthTime &&
                                      pd.Address1 == streetAddressLine &&
                                      pd.City == city &&
                                      pd.State == state &&
                                      pd.ZipCode == postalCode)
                                   select new DT_PatientIncomingMessage
                                   {
                                       Prowid = pd.Prowid,
                                       FirstName = pd.FirstName,
                                       LastName = pd.LastName,
                                       BirthDate = pd.BirthDate.ToString(),
                                       Address1 = pd.Address1,
                                       State = pd.State,
                                       City = pd.City,
                                       Note = pd.Note,
                                       TherapistName = od != null ? od.FirstName + " " + od.LastName : "",
                                       CellPh = pd.CellPh

                                   }).ToList();

                    using (RehabEntities entities = new RehabEntities())
                    {

                        foreach (var patientitem in patientlist)
                        {
                            DataTable DT_MRN = new DataTable();
                            var cmd = entities.Database.Connection.CreateCommand();
                            cmd.CommandText = "Select C.ClinicNo + '-' + left(replicate( '0', 6 ), 6 - len(Prowid ) ) + cast(Prowid as varchar(6)) AS MRN "+
                                              "From tblPatients P Inner Join tblClinics C on P.ClinicNo = C.Crowid Where Prowid ="+ patientitem.Prowid;

                            cmd.Connection.Open();
                            DT_MRN.Load(cmd.ExecuteReader());
                            if (DT_MRN.Rows.Count > 0)
                            {
                                patientitem.MRNNumber = DT_MRN.Rows[0]["MRN"].ToStringOrEmpty();
                            }
                            cmd.Connection.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new UnauthorizedAccessException();
                }

            }
            
                return patientlist;
        }
        public DT_IncomingMessageOperation ProcessIncomingMessage(DT_IncomingMessageProcessEntity xmldata)
        {
            int instituteId = 0;
            //using (var scope = new TransactionScope())
            //{
                DT_IncomingMessageOperation operationdata = null;
                List<DT_PatientIncomingMessage> patientlist = null;
                List<DT_IncomingMessagePOC> poclist = null;
                if (xmldata.Action == "Referral")
                {
                    patientlist = CheckPatientsExists(xmldata);
                    if (patientlist == null && patientlist.Count()>0)
                    {
                        operationdata = new DT_IncomingMessageOperation()
                        {
                            DT_Patients = null,
                            Operation = "Referral",
                            DT_POCs = new List<DT_IncomingMessagePOC>()
                        };
                        return operationdata;
                    }
                    else
                    {
                        operationdata = new DT_IncomingMessageOperation()
                        {
                            DT_Patients = patientlist,
                            Operation = "Referral",
                            DT_POCs = new List<DT_IncomingMessagePOC>()
                        };
                        return operationdata;
                    }
                }
                else if (xmldata.Action == "Plan of care approved")
                {
                    bool paramExists = CheckDocExists(xmldata);
                    if (!paramExists)
                    {
                        // IF DOC MASTER FIELDS NOT AVAILABLE NOTE ID / PATIENT ID 
                        // FETCH PATIENT LIST
                        patientlist = CheckPatientsExists(xmldata);
                        if (patientlist.Count > 0)
                        {
                            operationdata = new DT_IncomingMessageOperation()
                            {
                                DT_Patients = patientlist,
                                Operation = "Plan of care approved",
                                DT_POCs = new List<DT_IncomingMessagePOC>()
                            };
                            return operationdata;
                        }
                        else
                        {
                            instituteId = GetInstitute_ID(xmldata.InComingMessageID);

                            MoveIncomingtoPOCProccessed(0, xmldata.InComingMessageID, xmldata.FileData, instituteId, "Patient not found", xmldata.Username);
                        }
                    }
                    else
                    {

                        operationdata = new DT_IncomingMessageOperation()
                        {
                            DT_Patients = new List<DT_PatientIncomingMessage>(),
                            Operation = "Plan of care approved",
                            DT_POCs = new List<DT_IncomingMessagePOC>()
                        };
                        return operationdata;
                    }
                }
                else if (xmldata.Action == "Plan of care declined")
                {
                    bool paramExists = CheckDocExists(xmldata);
                    if (!paramExists)
                    {
                        
                        patientlist = CheckPatientsExists(xmldata);
                        if (patientlist.Count > 0)
                        {
                            operationdata = new DT_IncomingMessageOperation()
                            {
                                DT_Patients = patientlist,
                                Operation = "Plan of care declined",
                                DT_POCs = new List<DT_IncomingMessagePOC>()
                            };
                            return operationdata;
                        }
                        else
                        {
                            instituteId = GetInstitute_ID(xmldata.InComingMessageID);

                            MoveIncomingtoPOCProccessed(0, xmldata.InComingMessageID, xmldata.FileData, instituteId, "Patient not found", xmldata.Username);
                        }
                    }
                    else
                    {

                        operationdata = new DT_IncomingMessageOperation()
                        {
                            DT_Patients = new List<DT_PatientIncomingMessage>(),
                            Operation = "Plan of care approved",
                            DT_POCs = new List<DT_IncomingMessagePOC>()
                        };
                        return operationdata;
                    }

                }

              //  scope.Complete();
                return operationdata;
           // }
        }

        /// <summary>
        /// Updates a product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="productEntity"></param>
        /// <returns></returns>
        public bool UpdateDTUser(DT_UserEntity[] appUserEntity)
        {
            var success = false;
            if (appUserEntity != null)
            {
                using (var scope = new TransactionScope())
                {

                    var dtUserEntity = _unitOfWork.DT_ReferrerRepo.GetByID(appUserEntity[0].RefID);
                    if (dtUserEntity != null)
                    {

                        //dtUserEntity.Institute_ID = appUserEntity.Institute_ID;
                        dtUserEntity.Institute_Name = appUserEntity[0].Institute_Name;

                    }
                    _unitOfWork.DT_ReferrerRepo.Update(dtUserEntity);
                    _unitOfWork.Save();

                    foreach (var item in appUserEntity)
                    {
                        var dtUserEmailEntity = _unitOfWork.DT_Referrer_EmailsRepo.GetByID(item.RefEmailID);
                        if (dtUserEmailEntity != null)
                        {
                            _unitOfWork.DT_Referrer_EmailsRepo.Delete(dtUserEmailEntity);
                            _unitOfWork.Save();

                        }

                    }
                    foreach (var item in appUserEntity)
                    {
                        tbl_DT_Referrer_Emails entity = new tbl_DT_Referrer_Emails();
                        entity.Institute_ID = appUserEntity[0].RefID;
                        entity.DT_Email_Address = item.DT_Email_Address;
                        _unitOfWork.DT_Referrer_EmailsRepo.Insert(entity);
                        _unitOfWork.Save();
                    }

                    scope.Complete();
                    success = true;
                }
            }
            return success;
        }

        /// <summary>
        /// Deletes a particular product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public bool DeleteDTUser(int refID, int refEmailID)
        {
            var success = false;
            if (refID > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var user = _unitOfWork.DT_ReferrerRepo.GetByID(refID);
                    if (user != null)
                    {

                        _unitOfWork.DT_ReferrerRepo.Delete(user);

                    }

                    var userEmail = _unitOfWork.DT_Referrer_EmailsRepo.Get().Where(x => x.Institute_ID == user.Institute_ID).ToList();
                    foreach (var item in userEmail)
                    {

                        _unitOfWork.DT_Referrer_EmailsRepo.Delete(item);

                    }
                    _unitOfWork.Save();
                    scope.Complete();
                    success = true;
                }
            }
            return success;
        }
        public List<DT_UserEntity> GetDirectTrustReferrerDetail(int instituteId)
        {
            List<DT_UserEntity> dtUser = new List<DT_UserEntity>();

            using (RehabEntities rehab = new RehabEntities())
            {
                try
                {


                    dtUser = (from pd in rehab.tbl_DT_Referrers
                              join od in rehab.tbl_DT_Referrer_Emails on pd.Institute_ID equals od.Institute_ID
                              where (pd.Institute_ID == instituteId)
                              select new DT_UserEntity
                              {
                                  RefID = pd.Institute_ID,
                                  RefEmailID = od.ID,
                                  Institute_Name = pd.Institute_Name,
                                  DT_Email_Address = od.DT_Email_Address
                              }).ToList();
                    return dtUser;
                }
                catch (Exception ex)
                {
                    throw new UnauthorizedAccessException();
                }
                //if (dtUser!=null)
                //{
                //    return dtUser;
                //}
                //else
                //{

                //    //throw new UnauthorizedAccessException();
                //}
            }

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

        public List<DT_IncomingMessagesEntity> GetDTIncoming_Message(string username,string role)
        {
            List<DT_IncomingMessagesEntity> dtinmessages = new List<DT_IncomingMessagesEntity>();

            using (RehabEntities rehab = new RehabEntities())
            {
                string referralemail = CheckClinicisAssigned(username);
                if (role == "System Administrator")
                {
                    dtinmessages = (from pd in rehab.tbl_DT_Incoming_Message
                                        // join att in rehab.tbl_DT_Incoming_Message_Individual_Attachments
                                        // on pd.ID equals att.InComingMessageID
                                    where (pd.MessageProcessed == false)

                                    select new DT_IncomingMessagesEntity
                                    {
                                        ID = pd.ID,
                                        From = pd.From,
                                        To = pd.To,
                                        Subject = pd.Subject,
                                        MessageBody = pd.MessageBody,
                                        Received = pd.Received,
                                        MessageProcessed = pd.MessageProcessed,
                                        IsRead=pd.IsRead

                                    }).ToList();
                }
                else
                {

                    dtinmessages = (from pd in rehab.tbl_DT_Incoming_Message
                                        // join att in rehab.tbl_DT_Incoming_Message_Individual_Attachments
                                        // on pd.ID equals att.InComingMessageID
                                    where (pd.To == referralemail) &&
                                     (pd.MessageProcessed == false)

                                    select new DT_IncomingMessagesEntity
                                    {
                                        ID = pd.ID,
                                        From = pd.From,
                                        To = pd.To,
                                        Subject = pd.Subject,
                                        MessageBody = pd.MessageBody,
                                        Received = pd.Received,
                                        MessageProcessed = pd.MessageProcessed,
                                        IsRead = pd.IsRead

                                    }).ToList();
                }

                return dtinmessages;

            }

        }
        public DT_IncomingMessageViewmodelEntity GetDTIncoming_MessageByID(int incomingmessageId)
        {


            DT_IncomingMessageViewmodelEntity vwinmessages = new DT_IncomingMessageViewmodelEntity();
            List<DT_IncomingMessagesEntity> dtinmessages = new List<DT_IncomingMessagesEntity>();
            

            using (RehabEntities rehab = new RehabEntities())
            {
                dtinmessages = (from pd in rehab.tbl_DT_Incoming_Message
                                join att in rehab.tbl_DT_Incoming_Message_Individual_Attachments
                                on pd.ID equals att.InComingMessageID
                                where pd.ID == incomingmessageId
                                select new DT_IncomingMessagesEntity
                                {
                                    ID = pd.ID,
                                    From = pd.From,
                                    To = pd.To,
                                    Subject = pd.Subject,
                                    MessageBody = pd.MessageBody,
                                    Received = pd.Received,
                                    MessageProcessed = pd.MessageProcessed,
                                    Operation = null,
                                    IsRead = pd.IsRead
                                }).ToList();

                foreach (var message in dtinmessages)
                {
                    vwinmessages.ID = message.ID;
                    vwinmessages.From = message.From;
                    vwinmessages.To = message.To;
                    vwinmessages.Subject = message.Subject;
                    vwinmessages.MessageBody = message.MessageBody;
                    vwinmessages.Received = message.Received;
                    vwinmessages.MessageProcessed = message.MessageProcessed;
                    vwinmessages.Operation = null;
                    vwinmessages.XMLattachment = GetDTIncoming_MessageXML(message.ID);
                    //vwinmessages.XMLContent = GetDTIncoming_MessageXMLContent(message.ID);
                    vwinmessages.Received = vwinmessages.Received.StringToDate();

                }
               
                   
                    return vwinmessages;

                }

            }

        public List<DT_IncomingMessagesXMLCotentEntity> GetDTIncoming_MessageXMLContent(int incomingmessageId)
        {
            List<DT_IncomingMessagesXMLEntity> dtinmessages = new List<DT_IncomingMessagesXMLEntity>();
            List<DT_IncomingMessagesXMLCotentEntity> contentlist = new List<DT_IncomingMessagesXMLCotentEntity>();
            string xmlstring = "";
            string firstName = string.Empty;
            string lastName = string.Empty;
            DateTime birthTime = DateTime.MinValue;
            string streetAddressLine = string.Empty;
            string city = string.Empty;
            string state = string.Empty;
            string postalCode = string.Empty;
            string country = string.Empty;

            using (RehabEntities rehab = new RehabEntities())
            {
                dtinmessages = (from att in rehab.tbl_DT_Incoming_Message_Individual_Attachments
                                where att.InComingMessageID == incomingmessageId

                                select new DT_IncomingMessagesXMLEntity
                                {
                                    FileData = att.FileData,
                                }).ToList();

                foreach (var message in dtinmessages)
                {
                    string oldstr = "xmlns=\"urn:hl7-org:v3\"";
                    string oldstr1 = "xmlns:sdtc=\"urn:hl7-org:sdtc\"";
                    xmlstring = message.FileData;

                    xmlstring = xmlstring.Replace(oldstr, "");
                    xmlstring = xmlstring.Replace(oldstr1, "");
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlstring);
                    try
                    {


                        XmlNodeList xnList = doc.SelectNodes("ClinicalDocument");
                        //XmlNodeList xnList = doc.GetElementsByTagName("ClinicalDocument");



                        XmlNodeList xnList1 = doc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/patient/name");
                        XmlNodeList xnListname = doc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/patient/name");
                        foreach (XmlNode xn in xnListname)
                        {
                            firstName = xn["given"].InnerText;
                            lastName = xn["family"].InnerText;

                        }

                        XmlNodeList xnListbirthTime = doc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/patient/birthTime");
                        foreach (XmlNode xn in xnListbirthTime)
                        {
                            if (xn.Attributes[0].Value.Contains("/"))
                            {
                                DateTime myDate = Convert.ToDateTime(xn.Attributes[0].Value);
                                xn.Attributes[0].Value = myDate.ToString("yyyyMMdd");
                            }
                            int i = Convert.ToInt32(xn.Attributes[0].Value);
                            DateTime dt;
                            if (DateTime.TryParseExact(i.ToString(), "yyyyMMdd",
                                                      CultureInfo.InvariantCulture,
                                                      DateTimeStyles.None, out dt))
                            {
                                //Console.WriteLine(dt);
                                birthTime = dt;
                            }


                        }

                        XmlNodeList xnListaddr = doc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/addr");
                        foreach (XmlNode xn in xnListaddr)
                        {

                            streetAddressLine = xn["streetAddressLine"].InnerText;
                            city = xn["city"].InnerText;
                            state = xn["state"].InnerText;
                            postalCode = xn["postalCode"].InnerText;
                            country = xn["country"].InnerText;

                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    DT_IncomingMessagesXMLCotentEntity xmlcontent = new DT_IncomingMessagesXMLCotentEntity()
                    {
                        firstName = firstName,
                        lastName = lastName,
                        city = city,
                        state = state,
                        postalCode = postalCode,
                        country = country,
                        birthTime = birthTime
                    };
                    contentlist.Add(xmlcontent);

                }

                return contentlist;

            }
        }


            public List<DT_IncomingMessagesXMLEntity> GetDTIncoming_MessageXML(int incomingmessageId)
        {
            List<DT_IncomingMessagesXMLEntity> dtinmessages = new List<DT_IncomingMessagesXMLEntity>();
            //List<DT_IncomingMessagesXMLCotentEntity> contentlist = new List<DT_IncomingMessagesXMLCotentEntity>();
            string xmlstring = "";
            string firstName = string.Empty;
            string lastName = string.Empty;
            DateTime birthTime = DateTime.MinValue;
            string streetAddressLine = string.Empty;
            string city = string.Empty;
            string state = string.Empty;
            string postalCode = string.Empty;
            string country = string.Empty;
            string maritalStatus = string.Empty;
            string title = string.Empty;
            string HPphone = string.Empty;
            string WPphone = string.Empty;
            string MCphone = string.Empty;
            string clinicName = string.Empty;
            string MRN = string.Empty;
            string doctorInfo = string.Empty;
            string NPINumber = string.Empty;

            using (RehabEntities rehab = new RehabEntities())
            {
                dtinmessages = (from att in rehab.tbl_DT_Incoming_Message_Individual_Attachments
                                where att.InComingMessageID == incomingmessageId

                                select new DT_IncomingMessagesXMLEntity
                                {
                                    InComingMessageID = att.InComingMessageID,
                                    FileName = att.FileName,
                                    FileData = att.FileData,
                                    AttachmentID = att.ID

                                }).ToList();
                foreach (var message in dtinmessages)
                {
                    string oldstr = "xmlns=\"urn:hl7-org:v3\"";
                    string oldstr1 = "xmlns:sdtc=\"urn:hl7-org:sdtc\"";
                    xmlstring = message.FileData;

                    xmlstring = xmlstring.Replace(oldstr, "");
                    xmlstring = xmlstring.Replace(oldstr1, "");
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlstring);
                    try
                    {


                        XmlNodeList xnList = doc.SelectNodes("ClinicalDocument");
                        //XmlNodeList xnList = doc.GetElementsByTagName("ClinicalDocument");

                        //var referrerNode = doc.SelectNodes("ClinicalDocument/legalAuthenticator/assignedEntity/assignedPerson/name");
                        //foreach (XmlNode subNode in referrerNode)
                        //{
                        //    var te = subNode["given"].ChildNodes[2].InnerText.ToStringOrEmpty();

                        //   var given = subNode["given"].InnerText.ToStringOrEmpty() + " " + subNode["given"].InnerText.ToStringOrEmpty();

                        //}

                        XmlNodeList xnList1 = doc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/patient/name");
                        XmlNodeList xnListname = doc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/patient/name");
                        foreach (XmlNode xn in xnListname)
                        {
                            firstName = xn["given"].InnerText;
                            lastName = xn["family"].InnerText;

                        }

                        XmlNodeList xnListbirthTime = doc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/patient/birthTime");
                        foreach (XmlNode xn in xnListbirthTime)
                        {
                            if(xn.Attributes[0].Value.Contains("/"))
                            {
                                DateTime myDate = Convert.ToDateTime(xn.Attributes[0].Value);
                                xn.Attributes[0].Value = myDate.ToString("yyyyMMdd");
                            }
                            int i = Convert.ToInt32(xn.Attributes[0].Value);
                            DateTime dt;
                            if (DateTime.TryParseExact(i.ToString(), "yyyyMMdd",
                                                      CultureInfo.InvariantCulture,
                                                      DateTimeStyles.None, out dt))
                            {
                                //Console.WriteLine(dt);
                                birthTime = dt;
                            }


                        }
                        var dt_birthTime = birthTime.ToShortDateString();
                        XmlNodeList xnListmaritalStatusCode = doc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/patient/maritalStatusCode");
                        foreach (XmlNode xn in xnListmaritalStatusCode)
                        {
                            maritalStatus = xn.Attributes[2].Value;

                        }
                        XmlNodeList xnListtitle = doc.SelectNodes("/ClinicalDocument");
                        foreach (XmlNode xn in xnListtitle)
                        {
                            title = xn["title"].InnerText;
                        }
                        XmlNodeList xnListaddr = doc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/addr");
                        foreach (XmlNode xn in xnListaddr)
                        {

                            streetAddressLine = xn["streetAddressLine"].InnerText;
                            city = xn["city"].InnerText;
                            state = xn["state"].InnerText;
                            postalCode = xn["postalCode"].InnerText;
                            country = xn["country"].InnerText;

                        }
                        XmlNodeList xnListtelecom = doc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/telecom");
                        foreach (XmlNode xn in xnListtelecom)
                        {
                            if (xn.Attributes[0].Value == "HP")
                            {
                                HPphone = xn.Attributes[1].Value.ToStringOrEmpty().Length > 0 ? xn.Attributes[1].Value.Substring(4) : "";
                            }
                            if (xn.Attributes[0].Value == "WP")
                            {
                                WPphone = xn.Attributes[1].Value.ToStringOrEmpty().Length > 0 ? xn.Attributes[1].Value.Substring(4) : "";
                            }
                            if (xn.Attributes[0].Value == "MC")
                            {
                                MCphone = xn.Attributes[1].Value.ToStringOrEmpty().Length > 0 ? xn.Attributes[1].Value.Substring(4) : "";
                            }
                        }
                        XmlNodeList xnProviderOrg = doc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/providerOrganization");
                        foreach (XmlNode xn in xnProviderOrg)
                        {

                            clinicName = xn["name"].InnerText;

                        }


                        XmlNodeList xnlegalAuthenticator = doc.SelectNodes("/ClinicalDocument/legalAuthenticator/assignedEntity/assignedPerson/name");

                        if (xnlegalAuthenticator != null)
                        {


                            string given2 = string.Empty;
                            foreach (XmlNode xn in xnlegalAuthenticator)
                            {
                                if (xn["given"].NextSibling != null)
                                {
                                    given2 = xn["given"].NextSibling.InnerText.ToStringOrEmpty();
                                }

                                doctorInfo = xn["family"].InnerText.ToStringOrEmpty() + ", " + xn["given"].InnerText.ToStringOrEmpty() + " " + given2 + " " + xn["suffix"].InnerText.ToStringOrEmpty();

                            }
                        }
                        else
                        {
                            xnlegalAuthenticator = doc.SelectNodes("/ClinicalDocument/authenticator/assignedEntity/assignedPerson/name");
                            string given2 = string.Empty;
                            foreach (XmlNode xn in xnlegalAuthenticator)
                            {
                                if (xn["given"].NextSibling != null)
                                {
                                    given2 = xn["given"].NextSibling.InnerText.ToStringOrEmpty();
                                }

                                doctorInfo = xn["family"].InnerText.ToStringOrEmpty() + ", " + xn["given"].InnerText.ToStringOrEmpty() + " " + given2 + xn["suffix"].InnerText.ToStringOrEmpty();

                            }
                        }

                        
                        XmlNodeList xnNPI = doc.SelectNodes("/ClinicalDocument/authenticator/assignedEntity/id");
                        if (xnNPI != null)
                        {
                            foreach (XmlNode xn in xnNPI)
                            {
                                if (xn.Attributes[1].Value != null)
                                {
                                    if(xn.Attributes[1].Value.ToString().Length>=10)
                                    {
                                        NPINumber = xn.Attributes[1].Value;
                                    }
                                }
                            }

                        }
                        //var patientlist = (from pd in rehab.tblPatients
                        //               join od in rehab.tblTherapists on pd.TherapistID equals od.TherapistID
                        //               into ps
                        //               from od in ps.DefaultIfEmpty()
                        //               where (pd.FirstName == firstName &&
                        //               pd.LastName == lastName &&
                        //                  pd.BirthDate == dt_birthTime &&
                        //                  pd.Address1 == streetAddressLine &&
                        //                  pd.City == city &&
                        //                  pd.State == state &&
                        //                  pd.ZipCode == postalCode)
                        //               select new DT_PatientIncomingMessage
                        //               {
                        //                   Prowid = pd.Prowid,
                        //                   FirstName = pd.FirstName,
                        //                   LastName = pd.LastName,
                        //                   BirthDate = pd.BirthDate.ToString(),
                        //                   Address1 = pd.Address1,
                        //                   State = pd.State,
                        //                   City = pd.City,
                        //                   Note = pd.Note,
                        //                   TherapistName = od != null ? od.FirstName + " " + od.LastName : "",
                        //                   CellPh = pd.CellPh

                        //               }).ToList();

                        //using (RehabEntities entities = new RehabEntities())
                        //{

                        //    foreach (var patientitem in patientlist)
                        //    {
                        //        DataTable DT_MRN = new DataTable();
                        //        var cmd = entities.Database.Connection.CreateCommand();
                        //        cmd.CommandText = "Select top 1 C.ClinicNo + '-' + left(replicate( '0', 6 ), 6 - len(Prowid ) ) + cast(Prowid as varchar(6)) AS MRN " +
                        //                          "From tblPatients P Inner Join tblClinics C on P.ClinicNo = C.Crowid Where Prowid =" + patientitem.Prowid;

                        //        cmd.Connection.Open();
                        //        DT_MRN.Load(cmd.ExecuteReader());
                        //        if (DT_MRN.Rows.Count > 0)
                        //        {
                        //            MRN = DT_MRN.Rows[0]["MRN"].ToStringOrEmpty();
                        //        }
                        //    }
                        //}

                        /* Exact Node MRN Yet to be confirmed */

                        XmlNodeList xnMRN = doc.SelectNodes("/ClinicalDocument/recordTarget/patientRole/id");
                        if (xnMRN != null)
                        {
                            foreach (XmlNode xn in xnMRN)
                            {
                                if (xn.Attributes[0].Value != null)
                                {
                                    MRN = xn.Attributes[0].Value;
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    DT_IncomingMessagesXMLCotentEntity xmlcontent = new DT_IncomingMessagesXMLCotentEntity()
                    {
                        firstName = firstName,
                        lastName = lastName,
                        city = city,
                        state = state,
                        postalCode = postalCode,
                        country = country,
                        birthTime = birthTime,
                        streetAddressLine=streetAddressLine,
                        maritalStatus= maritalStatus,
                        title= title,
                        phoneHP=HPphone,
                        phoneWP = WPphone,
                        phoneMC = MCphone,
                        referrerClinic =clinicName,
                        mrn=MRN,
                        doctorInfo=doctorInfo,
                        npinumber=NPINumber
                    };
                    //contentlist.Add(xmlcontent);
                    message.XMLContent = xmlcontent;
                }
                
                return dtinmessages;

            }

        }

        public List<DT_POC_NotFoundEntity> GetDTPOCNotFound()
        {
            List<DT_POC_NotFoundEntity> dtpocmessagess = new List<DT_POC_NotFoundEntity>();

            //using (RehabEntities rehab = new RehabEntities())
            //{
            //    dtpocmessagess = (from pd in rehab.tbl_DT_POC_NotFound
            //                      select new DT_POC_NotFoundEntity
            //                      {
            //                          ID = pd.ID,
            //                          Institute_ID = pd.Institute_ID,
            //                          Note_ID = pd.Note_ID,
            //                          Patient_ID = pd.Patient_ID,
            //                          POC_Status = pd.POC_Status,

            //                      }).ToList();

                return dtpocmessagess;

                //if (dtpocmessagess.Count() > 0)
                //{
                //    return dtpocmessagess;
                //}
                //else
                //{

                //    throw new UnauthorizedAccessException();
                //}
           // }

        }

        public bool MoveIncomingtoPOCProccessed(int patientID, int incommingProcesID, string fileData, int instituteID, string status, string userId)
        {
            bool flag = false;
            try
            {

                tbl_DT_POC_Processed processed = new tbl_DT_POC_Processed();
                var incommingMsg = _unitOfWork.DT_Incoming_MessageRepo.Get(o => o.ID == incommingProcesID).FirstOrDefault();

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

                using (var scope = new TransactionScope())
                {

                    _unitOfWork.DT_POC_ProcessedRepo.Insert(processed);
                    //_unitOfWork.Save();
                    _unitOfWork.DT_Incoming_MessageRepo.Delete(incommingProcesID);
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

        public int GetInstitute_ID(int incommingMsgId)
        {
            int instituteId = 0;
            var incomingMessagPprocessEntity = (from p in _unitOfWork.DT_Incoming_MessageRepo.Get()
                                                join e in _unitOfWork.DT_Individial_AttachRepo.Get()
                                                on p.ID equals e.InComingMessageID
                                                where p.ID == incommingMsgId
                                                select new DT_IncomingMessagOperationEntity
                                                {
                                                    FileData = e.FileData,
                                                    EmailID = p.From,
                                                    InComingMessageID=p.ID

                                                }).ToList().FirstOrDefault();

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

                instituteId = referrer.Institute_ID;
            }
            return instituteId;
        }
        public List<DT_ClinicMapping> GetClinicMapping()
        {
            string referralemail = "integration@test.direct.team-rehab.com";
            using (RehabEntities rehab = new RehabEntities())
            {
                var cliniclist = (from tc in rehab.tblClinics
                                  join tm in rehab.tbl_DT_ClinicUserMapping on tc.ClinicNo equals tm.ClinicNo into ps
                                  from tm in ps.DefaultIfEmpty()
                                  select new DT_ClinicMapping
                                  {
                                      ClinicNo = tc.ClinicNo,
                                      ClinicName = tc.ClinicName,
                                      EmailId = tm.EmailId == null ? referralemail : tm.EmailId
                                  }).DefaultIfEmpty().ToList();
                return cliniclist;
            }

        }
        public List<DT_Clinics> GetClinics()
        {

            using (RehabEntities rehab = new RehabEntities())
            {
                var cliniclist = (from tc in rehab.tblClinics
                                  join tm in rehab.tbl_DT_ClinicUserMapping on tc.ClinicNo equals tm.ClinicNo into ps
                                  from tm in ps.DefaultIfEmpty()
                                  select new DT_Clinics
                                  {
                                      ClinicNo = tc.ClinicNo,
                                      ClinicName = tc.ClinicName

                                  }).DefaultIfEmpty().ToList();
                return cliniclist;
            }

        }
        public bool SaveDTClinicMapping(DT_ClinicMapping mapping)
        {

            using (RehabEntities rehab = new RehabEntities())
            {
                var clinicmappingexist = (from tc in rehab.tbl_DT_ClinicUserMapping where tc.ClinicNo == mapping.ClinicNo select tc).FirstOrDefault();
                if (clinicmappingexist != null)
                {
                    clinicmappingexist.EmailId = clinicmappingexist.EmailId;

                    _unitOfWork.DT_ClinicUserMappingRepo.Update(clinicmappingexist);
                    _unitOfWork.Save();
                }
                else
                {
                    var dtclinicmapping = new tbl_DT_ClinicUserMapping
                    {
                        ClinicNo = mapping.ClinicNo,
                        // Institute_ID = dtUserEntity.Institute_ID,
                        EmailId = mapping.EmailId

                    };

                    _unitOfWork.DT_ClinicUserMappingRepo.Insert(dtclinicmapping);
                    _unitOfWork.Save();

                }

                return true;
            }

        }
        public bool DeleteDTClinicMapping(string ClinicNo)
        {

            using (RehabEntities rehab = new RehabEntities())
            {
                var clinicmappingexist = (from tc in rehab.tbl_DT_ClinicUserMapping where tc.ClinicNo == ClinicNo select tc).FirstOrDefault();
                if (clinicmappingexist != null)
                {

                    _unitOfWork.DT_ClinicUserMappingRepo.Delete(clinicmappingexist);
                    _unitOfWork.Save();
                }

                return true;
            }

        }
        public bool UpdateMessageReadFlag(int incomingmessageId)
        {

            using (RehabEntities rehab = new RehabEntities())
            {
                var message = (from tc in rehab.tbl_DT_Incoming_Message where tc.ID == incomingmessageId select tc).FirstOrDefault();
                if (message != null)
                {
                    message.IsRead = true;

                    _unitOfWork.DT_Incoming_MessageRepo.Update(message);
                    _unitOfWork.Save();
                }
                else
                {
                    return false;
                }

                return true;
            }

        }
        public bool UpdateMessageReadUnreadFlag(int[] incomingmessageIds,string operation)
        {

            using (RehabEntities rehab = new RehabEntities())
            {
                if (operation == "Unread")
                {
                    for (int i = 0; i < incomingmessageIds.Length; i++)
                    {
                        int msgid = incomingmessageIds[i];
                        var message = (from tc in rehab.tbl_DT_Incoming_Message where tc.ID == msgid select tc).FirstOrDefault();
                        if (message != null)
                        {
                            message.IsRead = null;

                            _unitOfWork.DT_Incoming_MessageRepo.Update(message);
                            _unitOfWork.Save();
                        }
                        
                    }
                }
                else
                {
                    for (int i = 0; i < incomingmessageIds.Length; i++)
                    {
                        int msgid = incomingmessageIds[i];
                        var message = (from tc in rehab.tbl_DT_Incoming_Message where tc.ID == msgid select tc).FirstOrDefault();
                        if (message != null)
                        {
                            message.IsRead = true;

                            _unitOfWork.DT_Incoming_MessageRepo.Update(message);
                            _unitOfWork.Save();
                        }
                       
                    }
                }

                return true;
            }

        }

        public List<string> GetUserEmails()
        {
            List<string> useremailList = new List<string>();
            using (RehabEntities rehab = new RehabEntities())
            {
                 useremailList = _unitOfWork.UserEntityRepo.Get().Where(x => x.UserEmail != "").Distinct().Select(x => x.UserEmail).ToList();
                return useremailList;
            }

        }
        public bool SaveDTMessagesSent(DT_MessagesSent message)
        {
            
            message.From = _unitOfWork.UserEntityRepo.Get().Where(x => x.UserId == message.From).Select(x => x.UserEmail).FirstOrDefault();
            tbl_DT_MessagesSent messagesent = new tbl_DT_MessagesSent()
            {
                From = message.From,
                To = message.To,
                Cc=message.Cc,
                Subject=message.Subject,
                MessageBody = message.MessageBody,
                Sent = DateTime.Now
            };


            using (RehabEntities rehab = new RehabEntities())
            {
                //_unitOfWork.DT_MessageSentRepo.Insert(messagesent);
                //_unitOfWork.Save();

                //foreach (var attach in message.Attachment)
                //{
                //    tbl_DT_MessageSent_Individual_Attachments messagesentattachment = new tbl_DT_MessageSent_Individual_Attachments()
                //    {
                //        FileName = attach.FileName,
                //        FileData = Convert.FromBase64String(attach.FileData),
                //        MessageSentID = messagesent.ID

                //    };
                //    _unitOfWork.DT_MessageSent_Individual_AttachmentsRepo.Insert(messagesentattachment);
                //    _unitOfWork.Save();
                //}
                return true;
            }

        }
        public List<DT_MessagesSent> GetUserEmailMessages(string fromID)
        {
            var From = _unitOfWork.UserEntityRepo.Get().Where(x => x.UserId == fromID).Select(x => x.UserEmail).FirstOrDefault();
            using (RehabEntities rehab = new RehabEntities())
            {
                var messageList = (from tc in rehab.tbl_DT_MessagesSent
                                   //join e in rehab.tbl_DT_MessageSent_Individual_Attachments
                                   //             on tc.ID equals e.MessageSentID
                                   where tc.From== From
                                   select new DT_MessagesSent
                                  {
                                      ID=tc.ID,
                                      From = tc.From,
                                      To = tc.To,
                                      Cc=tc.Cc,
                                      Subject = tc.Subject,
                                      MessageBody = tc.MessageBody,
                                      Sent = tc.Sent
                                  }).OrderByDescending(x=>x.ID).ToList();
                return messageList;
            }

        }
        //public DT_MessagesSent GetDTSent_MessageByID(int sentmessageId)
        //{

        //    using (RehabEntities rehab = new RehabEntities())
        //    {
        //        var message = (from tc in rehab.tbl_DT_MessagesSent
        //                               //join e in rehab.tbl_DT_MessageSent_Individual_Attachments
        //                               //             on tc.ID equals e.MessageSentID
        //                           where tc.ID == sentmessageId
        //                           select new DT_MessagesSent
        //                           {
        //                               ID = tc.ID,
        //                               From = tc.From,
        //                               To = tc.To,
        //                               Cc = tc.Cc,
        //                               Subject = tc.Subject,
        //                               MessageBody = tc.MessageBody,
        //                               Sent = tc.Sent,
        //                              // Attachment= GetDTSent_MessageAttachmentByID(sentmessageId)
        //                           }).FirstOrDefault();

        //        message.Attachment = GetDTSent_MessageAttachmentByID(sentmessageId);
        //        return message;

        //    }

        //}
        //public List<DT_MessagesSentAttachment> GetDTSent_MessageAttachmentByID(int sentmessageId)
        //{
        //    using (RehabEntities rehab = new RehabEntities())
        //    {
        //        var message = (from  e in rehab.tbl_DT_MessageSent_Individual_Attachments
        //                       where e.MessageSentID == sentmessageId
        //                       select new DT_MessagesSentAttachment
        //                       {
        //                           ID=e.ID,
        //                           MessageSentID=sentmessageId,
        //                           FileName=e.FileName
                                   
        //                       }).ToList();


        //        return message;

        //    }
        //}
        //public byte[] GetMailAttachmentFile(Int32 attachmentID)
        //{
        //    System.IO.MemoryStream memStream;
        //    byte[] fileData;

        //    using (RehabEntities rehab = new RehabEntities())
        //    {
        //        var attachment = (from e in rehab.tbl_DT_MessageSent_Individual_Attachments
        //                          where e.ID == attachmentID
        //                          select e.FileData).FirstOrDefault();

        //        memStream = new MemoryStream(attachment);
        //        MemoryStream expandableStream = new MemoryStream();
        //        memStream.CopyTo(expandableStream);
        //        fileData = expandableStream.ToArray();
        //        return fileData;

        //    }
        //}
    }

}