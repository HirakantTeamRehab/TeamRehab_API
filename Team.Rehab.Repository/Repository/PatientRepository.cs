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

namespace Team.Rehab.Repository.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly IUnitOfwork _unitOfWork;
        // private readonly UnitOfWork _unitOfWork;
        private readonly IProcedureManagement _procedureManagement;

        public PatientRepository(IUnitOfwork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            //this._procedureManagement = new ProcedureManagement();
        }
        /// <summary>
        /// Get an user based on email address
        /// </summary>
        /// <param name="EmailAddress"></param>
        /// <returns>User object</returns>
        public List<tblPatients> GetPatients()
        {
            List<tblPatients> patients = _unitOfWork.PatientEntityRepo.Get().ToList();
            if (patients.Count() > 0)
            {
                return patients;
            }
            else
            {

                throw new UnauthorizedAccessException();
            }
        }

        public ViewModelPatientTherapist GetPatientsTherapist(string NPINumber)
        {
            // List<tblPatient> patients = _unitOfWork.PatientEntityRepo.Get().ToList();
            List<PatientEntity> patients = GetPatientsbyNPINumber(NPINumber);
            ViewModelPatientTherapist patTher = new ViewModelPatientTherapist();
            List<PatientEntity> patientsEntity = new List<PatientEntity>();

            Team.Rehab.DataModel.RehabEntities rehab = new RehabEntities();
            List<TherapistEntity> therapistEntity = new List<TherapistEntity>();
            //therapistEntity = (from therapist in rehab.tblTherapists

            //                       select new TherapistEntity
            //                       {
            //                           FirstName = therapist.FirstName,
            //                           LastName = therapist.LastName
            //                           //PhoneNumber = therapist.LastName,
            //                           //Email = therapist.LastName

            //                       }
            //).ToList();
            string ReferralSource = null;
            tblReferrer referrer = _unitOfWork.ReferrerRepo.Get(o => o.NPINumber == NPINumber).FirstOrDefault();
            if (referrer != null)
            {
                ReferralSource = Convert.ToString(referrer.Rrowid);
            }
            therapistEntity = (from therapist in rehab.tblTherapists
                               join docs in rehab.tblDocMasters on therapist.TherapistID equals docs.TreatingTherapistId

                               join patient in rehab.tblPatients.Where(a => a.ReferralSource == ReferralSource)
                               on docs.PTrowid equals patient.Prowid

                               //join referrer in rehab.tblReferrers on ptint.ReferralSource equals referrer.Rrowid
                               //where referrer.NPINumber = NPINumber
                               select new TherapistEntity
                               {
                                   FirstName = therapist.FirstName,
                                   LastName = therapist.LastName
                                   //PhoneNumber = therapist.LastName,
                                   //Email = therapist.LastName

                               }
            ).Distinct().ToList();


            if (patients.Count() > 0)
            {
                patTher.PatientList = patients;
            }
            if (therapistEntity.Count() > 0)
            {
                patTher.Therapist = therapistEntity;
            }
            return patTher;
        }

        /// <summary>
        /// Get an user based on user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>User object</returns>
        public tblPatients GetPatientbyID(int patientId)
        {
            var patient = _unitOfWork.PatientEntityRepo.Get(o => o.Prowid.Equals(patientId)).FirstOrDefault();
            return patient;
        }

        public List<PatientEntity> GetPatientsbyNPINumber(string NPINumber)
        {
            List<tblPatients> patients = new List<tblPatients>();
            List<PatientEntity> patientsEntity = new List<PatientEntity>();
            tblReferrer referrer = _unitOfWork.ReferrerRepo.Get(o => o.NPINumber == NPINumber).FirstOrDefault();
            if (referrer != null)
            {
                string ReferralSource = Convert.ToString(referrer.Rrowid);

                patients = _unitOfWork.PatientEntityRepo.Get(o => o.ReferralSource.Equals(ReferralSource)).ToList();
            }
            //===============Mapper===========================================
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<tblPatients, PatientEntity>();
            });

            IMapper mapper = config.CreateMapper();
            patientsEntity = mapper.Map<List<tblPatients>, List<PatientEntity>>(patients);
            //===============mapper end==========================================
            return patientsEntity;
            //Team.Rehab.DataModel.RehabEntities rehab = new RehabEntities();
            //List<PatientEntity> patients = new List<PatientEntity>();
            //patients = (from patient in rehab.tblPatients.Where(a => a.ReferralSource == ReferralSource)
            //                       select new PatientEntity
            //                       {
            //                           Prowid= patient.Prowid
            //                       }).ToList();

        }
        public SP_PatientNotes_Result GetPatientNotesByID(int patientId, int noteId)
        {
            List<SP_PatientNotes_Result> patientNotes = new List<SP_PatientNotes_Result>();
            SP_PatientNotes_Result patient = new SP_PatientNotes_Result();
            try
            {
                patientNotes = GetPatientNotes(patientId);
                patient = patientNotes.Find(o => o.Docrowid.Equals(noteId));
            }
            catch (Exception ex)
            {
                CustomLogger.SendExcepToDB(ex, "Data/SignNotes/SignDischarge");
                throw;
            }

            //byte[] buffer = GetPdf(patient.PDFName);
            //string converted = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            //patient.PDFName = converted;
            return patient;
        }
        public string GetPdf(int patientId, int noteId)
        {
            string converted = string.Empty;
           
                tblDocMaster patientNotes = _unitOfWork.DocMasterRepo.Get(o => o.PTrowid == patientId && o.Docrowid == noteId).FirstOrDefault();

                if (!string.IsNullOrEmpty(patientNotes.PDFName))
                {
                    byte[] pdfBytes = CloudeStoreg.ByteFromFile(patientNotes.PDFName);
                    //byte[] pdfBytes;
                    //string pdfName = Path.GetFileName(filePath);

                    //string networkPath = filePath.Replace("\\" + pdfName, "");
                    //string username = Convert.ToString(ConfigurationManager.AppSettings["NetworkUserName"]);
                    //string password = Convert.ToString(ConfigurationManager.AppSettings["NetworkPassword"]);
                    //var credentials = new NetworkCredential(username, password);
                    //using (new NetworkConnection(networkPath, credentials))
                    //{
                    //    pdfBytes = System.IO.File.ReadAllBytes(filePath);
                    //}


                    //   filePath = @"C:\Users\Shukraj Khadse\Downloads\Driving Licence.pdf";

                    //  byte[] pdfBytes = System.IO.File.ReadAllBytes(filePath);
                    converted = Encoding.UTF8.GetString(pdfBytes, 0, pdfBytes.Length);
                }
                else
                {
                    converted = "Note has not been signed, Please sign the note first";
                }
            
            return converted;
        }
        public ReferralSignDetailsEntity GetRefSignDetails(int patientId, int noteId, string NPINumber)
        {
            ReferralSignDetailsEntity refSignDetails = new ReferralSignDetailsEntity();
            if (!string.IsNullOrEmpty(NPINumber))
            {
                int npinum = Convert.ToInt32(NPINumber);
               tblAppUser _appuser = _unitOfWork.AppUserRepo.Get(o => o.NPINumber== npinum).FirstOrDefault();
                if (_appuser!=null)
                {
                    tblDevice _devices = _unitOfWork.DeviceRepo.Get(o => o.PhoneNumber.Equals(_appuser.PhoneNumber)).FirstOrDefault();
                    tblReferrerSign _ReferralSign = _unitOfWork.ReferrerSignRepo.Get(o => o.NPINumber== npinum &&
                    o.PTrowid==patientId && o.Docrowid==noteId
                    ).FirstOrDefault();



                    refSignDetails.EmailID = _appuser.EmailId;

                    refSignDetails.PhoneNumber = _appuser.PhoneNumber;
                    //refSignDetails.SignedDate = _ReferralSign.SignedDate ?? default;
                    refSignDetails.NoteType = _ReferralSign.NoteType;
                    refSignDetails.Sign = _ReferralSign.Signed??false;
                    refSignDetails.Signiture = _devices.SignPath;
                    refSignDetails.PDFPath = _ReferralSign.PDFName;
                        }
                         
            }
            return refSignDetails;
            }
        public List<SP_PatientNotes_Result> GetPatientNotes(int patientId)
        {

            //try
            //{
            //List<SP_PatientNotes_Result> patients = _unitOfWork.PatientNotesEntityRepo.Get();
            var _patientNotes = _unitOfWork.PatientNotesEntityRepo.ExecWithStoreProcedure("SP_GetDocMaster @Patientid", new SqlParameter("Patientid", SqlDbType.BigInt)
            { Value = patientId });
            List<SP_PatientNotes_Result> patientNotes = _patientNotes.ToList();



            //  }
            //// catch (Exception ex)
            //  {
            //   CustomLogger logger = new CustomLogger();
            //   logger.LogError(Convert.ToString(ex.Message));


            //  }
            return patientNotes;
        }

        public List<NotesSummaryEntity> GetNotesSummary(int patientId)
        {
            List<NotesSummaryEntity> notesSummary = new List<NotesSummaryEntity>();
            List<NotesSummaryEntity> notesSummaryFinal = new List<NotesSummaryEntity>();
            Team.Rehab.DataModel.RehabEntities rehab = new RehabEntities();
            List<NotificationsEntity> notificationsEntity = new List<NotificationsEntity>();
            try
            {
                notesSummary = (from notes in rehab.tblDocMasters.Where(a => a.PTrowid == patientId)
                                join thpst in rehab.tblTherapists on notes.TreatingTherapistId equals thpst.TherapistID
                                join clinic in rehab.tblClinics on thpst.DefaultClinic equals clinic.Crowid
                                join usr in rehab.tblUsers on
                                new { thpst.FirstName, thpst.LastName } equals new { usr.FirstName, usr.LastName }
                                select new NotesSummaryEntity
                                {
                                    Docrowid = notes.Docrowid,
                                    NotesType = notes.NoteType,
                                    DateOfService = notes.DateOfService,
                                    FirstName = thpst.FirstName,
                                    LastName = thpst.LastName,
                                    //  TherpistPhone = thpst.TherpistPhone,
                                    TherapistEmail = usr.UserEmail,
                                    ClinicName = clinic.ClinicName,
                                    ClinicPhone = clinic.PhoneNumber
                                    //  ClinicEmail = clinic.ClinicEmail,

                                }
          ).ToList();
                foreach (NotesSummaryEntity obj in notesSummary)
                {
                    if (obj.NotesType == "PPOC")
                    {
                        obj.NotesType = "Initial Eval";
                    }
                    if (obj.NotesType == "PPOC2")
                    {
                        obj.NotesType = "Initial Eval2";
                    }
                    if (obj.NotesType == "PPOCRE")
                    {
                        obj.NotesType = "Re-Eval";
                    }

                    if (obj.NotesType == "PTREAT")
                    {
                        obj.NotesType = "Daily Note";
                    }

                    if (obj.NotesType == "PMN")
                    {
                        obj.NotesType = "Medical Necessity";
                    }
                    if (obj.NotesType == "PDIS")
                    {
                        obj.NotesType = "Discharge";
                    }
                    if (obj.NotesType == "PCOMM")
                    {
                        obj.NotesType = "Communication";
                    }
                    if (obj.NotesType == "PMV")
                    {
                        obj.NotesType = "Missed";
                    }
                    if (obj.NotesType == "PFCE")
                    {
                        obj.NotesType = "FCE";
                    }

                    notesSummaryFinal.Add(obj);


                    //WHEN 'PFCE' then 'FCE'

                    //    Else 'Others'
                }
            }
            catch (Exception ex)
            {


                CustomLogger.SendExcepToDB(ex, "PatinetRepository");
                CustomLogger.LogError(Convert.ToString("NoteSummary in PatinetRepository- " + ex.Message));

            }
            return notesSummaryFinal;
        }
        public List<PatientSchedulesEntity> GetPatientsSchedules(int patientId)
        {
            List<PatientSchedulesEntity> schedulesEntity = new List<PatientSchedulesEntity>();

            Team.Rehab.DataModel.RehabEntities rehab = new RehabEntities();
            List<tblSchedules> tblschedulesEntity = new List<tblSchedules>();
            try
            {
                schedulesEntity = (from schedule in rehab.tblSchedules
                                   where schedule.PatientId == patientId

                                   select new PatientSchedulesEntity
                                   {
                                       Description = "Treatment",
                                       TreatmentDate = schedule.StartDateTime
                                       //PhoneNumber = therapist.LastName,
                                       //Email = therapist.LastName

                                   }
                         ).ToList();

            }
            catch (Exception ex)
            {

                throw;
            }
            return schedulesEntity;
        }

        public string SignNotes(int patientId, int noteId, int NPINumber)
        {
            DocManager objDocManager = new DocManager();
            string NoteType = string.Empty;
            string response = string.Empty;
            bool signStatus = false;
            bool refSignStatus = false;
            try
            {
                tblDocMaster patientNotes = _unitOfWork.DocMasterRepo.Get(o => o.PTrowid == patientId && o.Docrowid == noteId).FirstOrDefault();
                if (patientNotes != null)
                {
                    NoteType = patientNotes.NoteType;
                    signStatus = patientNotes.Signed;
                }


                if (NPINumber > 0)
                {
                    if (signStatus)
                    {
                        var referrerNotes = _unitOfWork.ReferrerSignRepo.Get(o => o.PTrowid == patientId && o.Docrowid == noteId && o.NPINumber == NPINumber).FirstOrDefault();
                        if (referrerNotes != null)
                        {
                            NoteType = referrerNotes.NoteType;
                            refSignStatus = referrerNotes.Signed??false;
                        }
                        if (refSignStatus)
                        {
                            response = "Notes already signed";
                        }
                        else
                        {

                            ITextMgrNotes objnotes = new ITextMgrNotes();
                            response = objnotes.ReferrerSign(patientNotes.PDFName, NPINumber);
                            if (!string.IsNullOrEmpty(response))
                            {

                                if (response.Substring(response.IndexOf(".")).ToLower() == ".pdf")
                                {
                                    tblReferrerSign rfsign = new tblReferrerSign();
                                    rfsign.Docrowid = noteId;
                                    rfsign.PTrowid = patientId;
                                    rfsign.NoteType = NoteType;
                                    rfsign.NPINumber = NPINumber;
                                    rfsign.PDFName = response;
                                    rfsign.Signed = true;
                                    rfsign.SignedDate = DateTime.Now;
                                    rfsign.createdts = DateTime.Now;
                                    rfsign.createdby = NPINumber.ToString();
                                    rfsign.updatedts = DateTime.Now;
                                    rfsign.updatedby = NPINumber.ToString();
                                    _unitOfWork.ReferrerSignRepo.Insert(rfsign);
                                    _unitOfWork.Save();
                                }

                            }
                        }
                    }
                    else
                    {
                        response = "Notes not yet signed by Therapiest";
                    }

                }
            }

            catch (Exception ex)
            {
                CustomLogger.SendExcepToDB(ex, "/PatientRepository/SignNotes");
                string msg = ex.Message;
                throw;
            }

            //  response = "under construction";
            return response;
        }
        public string UnSignNotes(int patientId, int noteId, int NPINumber)
        {
            int count = 0;
            string message = string.Empty;
            DocManager objDocManager = new DocManager();

            UnSignNotes objUnSignNotes = new UnSignNotes();
            string userRole = string.Empty;
            //if (userRole == "System Administrator")
            //{
            //}
            //else
            //{
            //    message = objDocManager.GetErrorMsg("AuthErr")



            if (NPINumber > 0)
            {

                var referrerNotes = _unitOfWork.ReferrerSignRepo.Get(o => o.PTrowid == patientId && o.Docrowid == noteId && o.NPINumber == NPINumber).FirstOrDefault();
                if (referrerNotes != null)
                {
                    _unitOfWork.ReferrerSignRepo.Delete(referrerNotes);
                    _unitOfWork.Save();
                    CloudeStoreg.DeleteFromFileStoreg(referrerNotes.PDFName);
                    message = "Note unsigned successfully";
                }
                else
                {
                    message = "Notes already unsigned";
                }

            }
            else
            {
                try
                {

                    tblDocMaster patientNotes = _unitOfWork.DocMasterRepo.Get(o => o.PTrowid == patientId && o.Docrowid == noteId).FirstOrDefault();

                    if (patientNotes.Signed)
                    {
                        message = objUnSignNotes.ValidateDocs(patientId, noteId);
                        objDocManager.IUDDocMaster("Z", noteId, 0, "", 0, "", "", "0", "", "", "0", "");
                        CloudeStoreg.DeleteFromFileStoreg(patientNotes.PDFName);
                        //int count = objUnSignNotes("Z", TryCast(Me.Master.FindControl("ctl00$ctl00$hdndocrowid"), HiddenField).Value, "0", "", 0, "", "", 0, "", "", 0, Session("User"));
                        if (string.IsNullOrEmpty(message))
                        {
                            message = "Note unsigned successfully";
                        }
                    }
                    else
                    {
                        message = "Notes already unsigned";
                    }

                }
                
            catch (Exception ex)
            {
                CustomLogger.SendExcepToDB(ex, "/PatientRepository/UnSignNotes");
                throw;
            }
           }
            return message;

        }

        public IniEvalSummaryEntity GetNotesSummary(int patientId, int noteId)
        {
            CCDAGeneration.InitialEval _iniEval = new CCDAGeneration.InitialEval();
            IniEvalSummaryEntity noteSumary = new IniEvalSummaryEntity();
            try
            {
                noteSumary = _iniEval.GetDocNoteSummary(noteId);
            }
            catch (Exception)
            {

                throw;
            }
            return noteSumary;
        }

        public List<MSGoalsEntity> GetPatientGoals(int patientId)
        {
            List<MSGoalsEntity> MSGoal = new List<MSGoalsEntity>();
            CCDAGeneration.MedicalNecessity objMS = new CCDAGeneration.MedicalNecessity();
            try
            {
                MSGoal = objMS.GetGoals(patientId, "L");
            }
            catch (Exception)
            {

                throw;
            }

            return MSGoal;
        }

        public List<PatTherapistEntity> GetDocTherapist(string NPINumber)
        {
            Team.Rehab.DataModel.RehabEntities rehab = new RehabEntities();
            List<PatTherapistEntity> patTherapistEntity = new List<PatTherapistEntity>();
            patTherapistEntity = (from users in rehab.tblUsers
                                  join thpst in rehab.tblTherapists on
                                  new { users.FirstName, users.LastName } equals new { thpst.FirstName, thpst.LastName }
                                  where thpst.NPINumber == NPINumber
                                  select new PatTherapistEntity
                                  {
                                      Name = users.FirstName.ToUpper() + ", " + users.LastName.ToUpper(),
                                      SignatureFile = thpst.SignatureFile,
                                      Path = " ",
                                      FirstName = thpst.FirstName,
                                      LastName = thpst.LastName,
                                      NPI = thpst.NPINumber,
                                      LicenseNumber = thpst.LicenseNumber
                                  }
            ).ToList();
            return patTherapistEntity;
        }

        public bool SendRefEMail(string recipient,int npinum, int patientId, int noteId)
        {
            bool isMessageSent = true;
            //Intialise Parameters  
            //   string aa = "appdatafiles\\EasyDoc\\D1\\121-108590_ALINA_YELIZAROV_PPOC_1_11-21-2017_5-19-2019-6-41-26-PM_RefSigned.pdf";
            tblPatients patientNotes = _unitOfWork.PatientEntityRepo.Get(o => o.Prowid == patientId).FirstOrDefault();
            tblReferrerSign _ReferralSign = _unitOfWork.ReferrerSignRepo.Get(o => o.NPINumber == npinum &&
                o.PTrowid == patientId && o.Docrowid == noteId
                ).FirstOrDefault();
            var smtpEmail = Convert.ToString(ConfigurationManager.AppSettings["SMTPEmail"]);
            var smtpPassword = Convert.ToString(ConfigurationManager.AppSettings["SMTPPassword"]);

            try
            {
              
                byte[] pdfBytes= CloudeStoreg.ByteFromFile(_ReferralSign.PDFName);
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                string filename = "Sign Notes " + DateTime.Now.ToString("yyyyMMdd_hhss") + ".pdf";
                mail.From = new MailAddress(smtpEmail);
                mail.To.Add(recipient);
                mail.Subject = "Referral Notes PDF for " + patientNotes.LastName+", "+ patientNotes.FirstName;
                mail.Body = "Sign note Attchment - " + patientNotes.LastName + ", " + patientNotes.FirstName; ;
                var memStream = new MemoryStream(pdfBytes);
                memStream.Position = 0;
                var contentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                var reportAttachment = new Attachment(memStream, contentType);
                reportAttachment.ContentDisposition.FileName = filename;
                mail.Attachments.Add(reportAttachment);
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(smtpEmail, smtpPassword);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

            }
           
            catch (Exception ex)
            {
                isMessageSent = false;
            }
            return isMessageSent;
        }


    }
}
