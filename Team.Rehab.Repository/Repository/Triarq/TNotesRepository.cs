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
using Team.Rehab.InterfaceRepository.Triarq;
using AutoMapper;
using Newtonsoft.Json.Linq;

namespace Team.Rehab.Repository.Repository.Triarq
{
    public class TNotesRepository : ITNotesRepository
    {

        private readonly IUnitOfwork _unitOfWork;
        // private readonly UnitOfWork _unitOfWork;
        private readonly IProcedureManagement _procedureManagement;

        public TNotesRepository(IUnitOfwork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            //this._procedureManagement = new ProcedureManagement();
        }
        public JArray GetMedicalNcessityNotes(int patientId, int NoteId)
        {
            JObject jResponse = new JObject();
            string NoteType = string.Empty;
            string NoteTypeDec = string.Empty;
            JArray jsonPatient = new JArray();
            Team.Rehab.DataModel.RehabEntities rehab = new RehabEntities();

            try
            {
                //MedicalNecessityNotes msNote = new MedicalNecessityNotes();
                TPatientRespository objTrepo = new TPatientRespository(_unitOfWork);
                tblDocMaster patientNotes = _unitOfWork.DocMasterRepo.Get(o => o.PTrowid == patientId && o.Docrowid == NoteId).FirstOrDefault();
                tblPatients patient = _unitOfWork.PatientEntityRepo.Get(o => o.Prowid == patientId).FirstOrDefault();
                jsonPatient= objTrepo.GetPatientList(patient.FirstName, patient.LastName, patient.BirthDate,  "");
               
                //MSDateOfServiceEntity dateOfService = msNote.GetMSDateOfService(patientId, NoteId, NoteType);
                //MSHints msHints = msNote.GetHints(patientId, NoteId, NoteType);
                //List<MSFunctionalCharEntity> MSFunctionalCharEntity = msNote.GetFunctChar( patientId,  NoteId,  NoteType);
                if (patientNotes != null)
                {
                    NoteType = patientNotes.NoteType;
                    if (NoteType == "PPOC")
                    {

                        NoteTypeDec = "Initial Eval";
                        CCDAGeneration.InitialEval _iniEval = new CCDAGeneration.InitialEval();
                        jResponse = _iniEval.ConvertInitialEval(patientId, NoteId, NoteType);
                        


                    }
                    if (NoteType == "PPOC2")
                    {

                        NoteTypeDec = "Initial Eval2";
                        CCDAGeneration.InitialEval _iniEval = new CCDAGeneration.InitialEval();
                        jResponse = _iniEval.ConvertInitialEval(patientId, NoteId, NoteType);
                        //             jResponse =
                        //new JObject(
                        // new JProperty(NoteTypeDec + " note is under construction"
                        //    ));


                    }
                    if (NoteType == "PPOCRE")
                    {
                        NoteTypeDec = "Re-Eval";
                        CCDAGeneration.InitialEval _iniEval = new CCDAGeneration.InitialEval();
                        jResponse = _iniEval.ConvertInitialEval(patientId, NoteId, NoteType);
                     


                    }
                    if (NoteType == "PTREAT")
                    {

                        NoteTypeDec = "Daily Note";

                        NoteTypeDec = "Medical Necessity";
                        CCDAGeneration.DailyNotes _dailyNotes = new CCDAGeneration.DailyNotes();
                        jResponse = _dailyNotes.ConvertDailyNote(patientId, NoteId, NoteType);
                        //             jResponse =
                        //new JObject(
                        // new JProperty(NoteTypeDec + "  is under construction"
                        //    ));
                    }
                    if (NoteType == "PMN")
                    {
                        NoteTypeDec = "Medical Necessity";
                        CCDAGeneration.MedicalNecessity _patientDemographics = new CCDAGeneration.MedicalNecessity();
                        jResponse = _patientDemographics.MedicalNecessityToCCDA(patientId, NoteId, NoteType);
                    }
                    if (NoteType == "PDIS")
                    {

                        NoteTypeDec = "Discharge";

                        jResponse =
           new JObject(
            new JProperty(NoteTypeDec + " note is under construction"
               ));


                    }
                    if (NoteType == "PCOMM")
                    {

                        NoteTypeDec = "Communication";
                        CCDAGeneration.Communication _comm = new CCDAGeneration.Communication();
                        jResponse = _comm.ConvertCommunication(patientId, NoteId, NoteType);


                    }
                    if (NoteType == "PMV")
                    {

                        NoteTypeDec = "Missed";
                        jResponse =
           new JObject(
            new JProperty(NoteTypeDec + " note is under construction"
               ));
                    }
                    if (NoteType == "PFCE")
                    {

                        NoteTypeDec = "PFCE";
                        jResponse =
             new JObject(
              new JProperty(NoteTypeDec + " note is under construction"
                 ));
                    }

                }
                else
                {


                    jResponse =
                 new JObject(
                  new JProperty("No record found"
                     ));
                }




            }
            catch (Exception ex)
            {

                throw ex;
            }

            jsonPatient.Add(jResponse);

            return jsonPatient;
           // return jResponse;
        }

        public List<TriPatientNotesEntity> GetPatientNotes(int patientId)
        {
            List<TriPatientNotesEntity> triPatientNotesEntity = new List<TriPatientNotesEntity>();
            try
            {
                //var patient = _unitOfWork.DocMasterRepo.Get(o => o.PTrowid == patientId).FirstOrDefault();
                //if (patient != null)
                //{
                List<SP_PatientNotes_Result> patientNotes = new List<SP_PatientNotes_Result>();
                //List<SP_PatientNotes_Result> patients = _unitOfWork.PatientNotesEntityRepo.Get();
                patientNotes = _unitOfWork.PatientNotesEntityRepo.ExecWithStoreProcedure("SP_GetDocMaster @Patientid", new SqlParameter("Patientid", SqlDbType.BigInt)
                { Value = patientId }).ToList();

                //===============Mapper===========================================
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<SP_PatientNotes_Result, TriPatientNotesEntity>();
                });

                IMapper mapper = config.CreateMapper();
                triPatientNotesEntity = mapper.Map<List<SP_PatientNotes_Result>, List<TriPatientNotesEntity>>(patientNotes);
                //===============mapper end==========================================
                //  }

            }
            catch (Exception ex)
            {
                CustomLogger.LogMessage(ex.Message, "");
                CustomLogger.SendExcepToDB(ex, "TNotesRepository");

                throw;
            }
            return triPatientNotesEntity;

        }

        public string GetCCDAInXMLFormat(int patientId, int NoteId)
        {
            string jResponse = string.Empty;
            string NoteType = string.Empty;
            string NoteTypeDec = string.Empty;
            JArray jsonPatient = new JArray();
            Team.Rehab.DataModel.RehabEntities rehab = new RehabEntities();

            try
            {
               
                TPatientRespository objTrepo = new TPatientRespository(_unitOfWork);
                tblDocMaster patientNotes = _unitOfWork.DocMasterRepo.Get(o => o.PTrowid == patientId && o.Docrowid == NoteId).FirstOrDefault();
                tblPatients patient = _unitOfWork.PatientEntityRepo.Get(o => o.Prowid == patientId).FirstOrDefault();
                //jsonPatient = objTrepo.GetPatientList(patient.FirstName, patient.LastName, patient.BirthDate, "");

          
                if (patientNotes != null)
                {
                    NoteType = patientNotes.NoteType;
                    if (NoteType == "PPOC")
                    {

                        NoteTypeDec = "Initial Eval";
                        CCDAGeneration.InitialEval _iniEval = new CCDAGeneration.InitialEval();
                        jResponse = _iniEval.InitialEvalToCCDA_XML(patientId, NoteId, NoteType, patient);



                    }
                    if (NoteType == "PPOC2")
                    {

                        NoteTypeDec = "Initial Eval2";
                        CCDAGeneration.InitialEval _iniEval = new CCDAGeneration.InitialEval();
                        jResponse = _iniEval.InitialEvalToCCDA_XML(patientId, NoteId, NoteType, patient);
                        //             jResponse =
                        //new JObject(
                        // new JProperty(NoteTypeDec + " note is under construction"
                        //    ));


                    }
                    if (NoteType == "PPOCRE")
                    {
                        NoteTypeDec = "Re-Eval";
                        CCDAGeneration.InitialEval _iniEval = new CCDAGeneration.InitialEval();
                        jResponse = _iniEval.InitialEvalToCCDA_XML(patientId, NoteId, NoteType, patient);



                    }
                    if (NoteType == "PTREAT")
                    {

                        NoteTypeDec = "Daily Note";

                        NoteTypeDec = "Medical Necessity";
                        CCDAGeneration.DailyNotes _dailyNotes = new CCDAGeneration.DailyNotes();
                       // jResponse = _dailyNotes.ConvertDailyNote(patientId, NoteId, NoteType);
                        //             jResponse =
                        //new JObject(
                        // new JProperty(NoteTypeDec + "  is under construction"
                        //    ));
                    }
                    if (NoteType == "PMN")
                    {
                        NoteTypeDec = "Medical Necessity";
                        CCDAGeneration.MedicalNecessity _patientDemographics = new CCDAGeneration.MedicalNecessity();
                       // jResponse = _patientDemographics.MedicalNecessityToCCDA(patientId, NoteId, NoteType);
                    }
                    if (NoteType == "PDIS")
                    {

                        NoteTypeDec = "Discharge";

                        jResponse = NoteTypeDec + " note is under construction";
         


                    }
                    if (NoteType == "PCOMM")
                    {

                        NoteTypeDec = "Communication";
                        CCDAGeneration.Communication _comm = new CCDAGeneration.Communication();
                       // jResponse = _comm.ConvertCommunication(patientId, NoteId, NoteType);


                    }
                    if (NoteType == "PMV")
                    {

                        NoteTypeDec = "Missed";
                        jResponse = NoteTypeDec + " note is under construction";
                        
                    }
                    if (NoteType == "PFCE")
                    {

                        NoteTypeDec = "PFCE";
                        jResponse = NoteTypeDec + " note is under construction";
                    }

                }
                else
                {

                    jResponse =  "No record found";
                   
                }




            }
            catch (Exception ex)
            {

                throw ex;
            }

            

            return jResponse;
            // return jResponse;
        }
    }
}
