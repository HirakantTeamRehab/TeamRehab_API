using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Team.Rehab.BusinessEntities;
using System.Data.SqlClient;
using AutoMapper;

using System.Data;
using Team.Rehab.InterfaceRepository;
using Team.Rehab.InterfaceRepository.Data;
using Team.Rehab.DataModel;
using Newtonsoft.Json;

namespace CCDAGeneration
{
    #region Daily Note
    public class DailyNotes
    {
        public JObject ConvertDailyNote(int patientId, int NoteId, string NoteType)
        {
            JObject rss = new JObject();
            string CCDAResponse = string.Empty;
            rss = DailyNoteToCCDA(patientId, NoteId, NoteType);
            return rss;

        }

        private JObject DailyNoteToCCDA(int patientId, int NoteId, string NoteType)
        {
            JObject Jresponse = new JObject();
            MedicalNecessity objMS = new MedicalNecessity();
            MSDateOfServiceEntity dateOfService = objMS.GetMSDateOfService(patientId, NoteId, NoteType);
            List<MSIntervensionsEntity> MSIntervensions = objMS.GetIntervensions(patientId, NoteId, "B");
            List<DNInsuranceNoteEntity> DNInsuranceNotes = GetInsuranceNotes(patientId, NoteId);
            List<MSIntervensionsSumEntity> MSintervensionssumentity = objMS.GetDocSumInterventions(NoteId);
            List<MSProgExerciseEntity> MSProgExer = objMS.GetProgExer(patientId, NoteId);
            List<DNDescHintsEntity> DNDescHints = GetDescHints();
            List<DNDocTreatDesc> DNdoctreatdescWithPatientIDZero = GetTreatDescWithPatientIDZero(NoteId, 0);
            List<DNDocTreatDesc> DNdoctreatdescWithDescDocIDZero = GetTreatDescDocIDZero(0, patientId);


            //JObject jss = null;
            JObject jss =
             new JObject(
              new JProperty("component",
               new JObject(new JProperty("structuredBody",
                new JObject(new JProperty("component",
                 new JObject(new JProperty("section",

                  new JObject(new JProperty("templateId",
                    new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.21.2.3"),
                     new JProperty("@extension", "2015-08-01"))),

                     new JProperty("TeamRehabID",
                    new JObject(new JProperty("PatientID", patientId),
                     new JProperty("NoteID", NoteId))),

                   new JProperty("code",
                    new JObject(
                     new JProperty("@code", "62387-6"),
                     new JProperty("@displayName", "Interventions Provided"),
                     new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
                     new JProperty("@codeSystemName", "LOINC"))),

                   new JProperty("title", "Interventions Section"),
                   new JProperty("text",
                    new JObject(new JProperty("table",
                     new JObject(new JProperty("@border", "1"),
                      new JProperty("width", "100%"),
                      new JProperty("thead",
                       new JObject(new JProperty("tr",
                        new JObject(new JProperty("th",
                         new JObject(
                          new JProperty("CPTCode",
                           "CPTDescription",
                           "Modifiers",
                           "Timed",
                           "Minutes",
                           "Units",
                           "Previous Note Minutes"))))))),
                        new JProperty("tbody",
              new JObject(new JProperty("tr", IntervensionArray(MSIntervensions)
                       )))
                     ))))
                  ))),

                 new JObject(new JProperty("section",
                  new JObject(new JProperty("templateId",
                    new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.41"))),

                   new JProperty("code",
                    new JObject(new JProperty("@code", "8653-8"),
                     new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
                     new JProperty("@codeSystemName", "LOINC"),
                     new JProperty("@displayName", "PATIENT INSTRUCTIONS"))),

                   new JProperty("title", "Progressive Exercises"),
                   new JProperty("text",
                    new JObject(new JProperty("table",
                     new JObject(new JProperty("@border", "1"),
                      new JProperty("width", "100%"),
                      new JProperty("thead",
                       new JObject(new JProperty("tr",
                        new JObject(new JProperty("th",
                         new JObject(
                          new JProperty("Excercise",
                           "Sets",
                           "Reps",
                           "Qty",
                           "Quantity",
                           "Weight"))))))),
                       new JProperty("tbody",
              new JObject(new JProperty("tr", ProgresiveExerciseArray(MSProgExer)

              )))
              ))))     //title

              ))),

                 new JObject(new JProperty("section",
                  new JObject(new JProperty("templateId",
                    new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.10"),
                     new JProperty("@extension", "2014-06-09")),
                    new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.10"))),

                   new JProperty("code",
                    new JObject(new JProperty("@code", "18776-5"),
                     new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
                     new JProperty("@codeSystemName", "LOINC"),
                     new JProperty("@displayName", "Treatment plan"))),

                   new JProperty("title", "TREATMENT PLAN"),

                   new JProperty("text",
                    new JObject(new JProperty("table",
                     new JObject(
                      new JProperty("thead",
                       new JObject(new JProperty("tr",
                        new JObject(new JProperty("th",
                         new JObject(
                          new JProperty("Plan",
                           "DateOfService",
                           "Decription"))))))),

                      new JProperty("tbody",
              new JObject(new JProperty("tr", TreatArray(DNdoctreatdescWithPatientIDZero)

              )))
              ))))     //title

              )))))))));

            return jss;
        }


        private List<DNDocTreatDesc> GetTreatDescDocIDZero(int noteId, int patientId)
        {

            List<DNDocTreatDesc> DNDocTreatDescDocIDZero = new List<DNDocTreatDesc>();
            try
            {
                using (var context = new RehabEntities())
                {
                    DNDocTreatDescDocIDZero = context.Database.SqlQuery<DNDocTreatDesc>("SP_GetDocTreatDesc @docrowid,@patientid ",
                   new SqlParameter("docrowid", SqlDbType.BigInt) { Value = noteId },
                    new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId }

                   ).ToList();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return DNDocTreatDescDocIDZero;
        }

        private List<DNDocTreatDesc> GetTreatDescWithPatientIDZero(int patientId, int noteId)
        {
            List<DNDocTreatDesc> DNDocTreatDescPatientIDZero = new List<DNDocTreatDesc>();
            try
            {
                using (var context = new RehabEntities())
                {
                    DNDocTreatDescPatientIDZero = context.Database.SqlQuery<DNDocTreatDesc>("SP_GetDocTreatDesc @docrowid,@patientid ",
                   new SqlParameter("docrowid", SqlDbType.BigInt) { Value = noteId },
                    new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId }

                   ).ToList();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return DNDocTreatDescPatientIDZero;
        }

        private List<DNDescHintsEntity> GetDescHints()
        {
            List<DNDescHintsEntity> DNdeschintsentity = new List<DNDescHintsEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    DNdeschintsentity = context.Database.SqlQuery<DNDescHintsEntity>("SP_GetHints ").ToList();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return DNdeschintsentity;
        }


        #region Get Insurance Notes
        private List<DNInsuranceNoteEntity> GetInsuranceNotes(int patientId, int noteId)
        {
            List<DNInsuranceNoteEntity> DNinsurancenoteentity = new List<DNInsuranceNoteEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    DNinsurancenoteentity = context.Database.SqlQuery<DNInsuranceNoteEntity>("SP_GetPatInsuranceNote @PTrowid,@Docrowid ",
                  new SqlParameter("PTrowid", SqlDbType.BigInt) { Value = noteId },
                   new SqlParameter("Docrowid", SqlDbType.BigInt) { Value = patientId }
                  ).ToList();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return DNinsurancenoteentity;
        }
        #endregion

        public JArray IntervensionArray(List<MSIntervensionsEntity> MSIntervensions)
        {
            JObject jsonObject = null;

            JArray jsonArray = new JArray();
            try
            {
                foreach (MSIntervensionsEntity obj in MSIntervensions)
                {

                    jsonObject = new JObject(new JProperty("td",
                                   new JObject(new JProperty(obj.CPTCOde,
                                                       obj.CPTDescription,
                                                       obj.modifier,
                                                      obj.CPTTimed,
                                                       obj.minutes,
                                                      obj.units,
                                                      obj.PrevMinuts
                                                      ))));
                    jsonArray.Add(jsonObject);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return jsonArray;
        }

        public JArray ProgresiveExerciseArray(List<MSProgExerciseEntity> MSProgExer)
        {
            JObject jsonObject = null;

            JArray jsonArray = new JArray();
            try
            {
                foreach (MSProgExerciseEntity obj in MSProgExer)
                {

                    jsonObject = new JObject(new JProperty("td",
                                   new JObject(new JProperty(obj.Exercise,
                                                       obj.Sets,
                                                       obj.Reps,
                                                      obj.Qty,
                                                       obj.Weight

                                                      ))));
                    jsonArray.Add(jsonObject);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return jsonArray;
        }

        public JArray TreatArray(List<DNDocTreatDesc> TreatArr)
        {
            JObject jsonObject = null;

            JArray jsonArray = new JArray();
            try
            {
                foreach (DNDocTreatDesc obj in TreatArr)
                {

                    jsonObject = new JObject(new JProperty("td",
                                   new JObject(new JProperty(obj.docrowid.ToString(),
                                                       obj.treatrowid,
                                                       obj.DateOfService,
                                                      obj.treatdesc
                                                      ))));
                    jsonArray.Add(jsonObject);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return jsonArray;
        }
    }
    #endregion
}
