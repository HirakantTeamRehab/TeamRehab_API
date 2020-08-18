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
    public class Communication
    {
        public JObject ConvertCommunication(int patientId, int NoteId, string NoteType)
        {
            JObject rss = new JObject();
            string CCDAResponse = string.Empty;
            rss = CommunicationToCCDA(patientId, NoteId, NoteType);
            return rss;

        }

        private JObject CommunicationToCCDA(int patientId, int noteId, string noteType)
        {
            JObject Jresponse = new JObject();
            MedicalNecessity objMS = new MedicalNecessity();
            MSDateOfServiceEntity dateOfService = objMS.GetMSDateOfService(patientId, noteId, noteType);
            List<CommDocEntity> Commgetdocid = GetCommDocID(patientId, noteType);
            List<CommNoteSummaryEntity> CommNoteSummary = GetCommNoteSummary(noteId);
            List<CommReferralEntity> CommReferrals = GetCommReferals(patientId, noteId);


            JObject jss =
              new JObject(
               new JProperty("component",
                new JObject(new JProperty("structuredBody",
                 new JObject(new JProperty("component",
                   new JObject(new JProperty("section",

                    new JObject(new JProperty("templateId",
                      new JObject(new JProperty("@root", "2.16.840.1.113883.2.4.6.10.100001"))),

                     new JProperty("id",
                      new JObject(new JProperty("@extension", "C790466765836f87-546"),
                       new JProperty("2.16.840.1.113883.2.4.3.23.3.20"))),

                     new JProperty("TeamRehabID",
                    new JObject(new JProperty("PatientID", patientId),
                     new JProperty("NoteID", noteId))),

                     new JProperty("code",
                      new JObject(new JProperty("@code", "68608-9"),
                       new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
                       new JProperty("@displayName", "Note summary"))),

                     new JProperty("title", "Note summary"),
                     new JProperty("text",
                      new JObject(new JProperty("table",
                       new JObject(new JProperty("@border", "1"),
                        new JProperty("width", "100%"),
                        new JProperty("thead",
                         new JObject(new JProperty("tr",
                          new JObject(new JProperty("th",
                           new JObject(
                            new JProperty("Note row id"),
                            new JProperty("Note summary"),
                            new JProperty("CreateWCPDF"))))))),


                        new JProperty("tbody",
                          new JObject(new JProperty("tr", SummaryArray(CommNoteSummary)
                         

                         )))




                       )))) //title

                    ))), //section



                   new JObject(new JProperty("section",
                    new JObject(

                     new JProperty("title",
                      new JObject(
                       new JProperty("text",
                        new JObject(new JProperty("table",
                         new JObject(new JProperty("@border", "1"),
                          new JProperty("width", "100%"),



                          new JProperty("thead",
                          new JObject(new JProperty("tr", ReferralArray(CommReferrals)



                           )))

                         )

                        ))))) //title

                    ))) //section

                  )

                 ))))); //end wale brackets

            return jss;
        }

        private JArray ReferralArray(List<CommReferralEntity> commReferrals)
        {
            JObject jsonObject = null;

            JArray jsonArray = new JArray();
            try
            {
                foreach (CommReferralEntity obj in commReferrals)
                {

                    jsonObject = new JObject(new JProperty("th",
                                   new JObject(new JProperty(obj.PrintName
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

        private List<CommReferralEntity> GetCommReferals(int patientId, int noteId)
        {
            List<CommReferralEntity> CommReferral = new List<CommReferralEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    CommReferral = context.Database.SqlQuery<CommReferralEntity>("SP_GetPatReferral @Patientid,@Docrowid ",
                   new SqlParameter("Patientid", SqlDbType.Int) { Value = patientId },
                   new SqlParameter("Docrowid", SqlDbType.Int) { Value = noteId }
                   ).ToList();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return CommReferral;
        }

        private JArray SummaryArray(List<CommNoteSummaryEntity> commNoteSummary)
        {
            JObject jsonObject = null;

            JArray jsonArray = new JArray();
            try
            {
                foreach (CommNoteSummaryEntity obj in commNoteSummary)
                {

                    jsonObject = new JObject(new JProperty("td",
                                   new JObject(new JProperty(obj.noterowid.ToString(),
                                                       obj.notesummary,
                                                       obj.CreateWCPDF
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

        private List<CommNoteSummaryEntity> GetCommNoteSummary(int noteId)
        {
            List<CommNoteSummaryEntity> CommNoteSummary = new List<CommNoteSummaryEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    CommNoteSummary = context.Database.SqlQuery<CommNoteSummaryEntity>("SP_GetDocNoteSummary @Docrowid ",
                   new SqlParameter("Docrowid", SqlDbType.Int) { Value = noteId }

                   ).ToList();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return CommNoteSummary;
        }

        private List<CommDocEntity> GetCommDocID(int patientId, string noteType)
        {
            List<CommDocEntity> CommDocID = new List<CommDocEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    CommDocID = context.Database.SqlQuery<CommDocEntity>("SP_GetDoc @patientid,@notetype ",
                   new SqlParameter("patientid", SqlDbType.Int) { Value = patientId },
                    new SqlParameter("notetype", SqlDbType.VarChar) { Value = noteType }

                   ).ToList();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return CommDocID;
        }
    }
}
