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
    public class InitialEval
    {
        public JObject ConvertInitialEval(int patientId, int NoteId, string NoteType)
        {
            JObject rss = new JObject();
            string CCDAResponse = string.Empty;
            rss = InitialEvalToCCDA(patientId, NoteId, NoteType);

            //foreach (PatientEntity paient in patientEntity)
            //{
            //    rss = PatientlistToCCDA(paient);
            //}
            return rss;

        }
        public JObject InitialEvalToCCDA(int patientId, int NoteId, string NoteType)
        {
            JObject Jresponse = new JObject();
            MedicalNecessity objMS = new MedicalNecessity();
            MSDateOfServiceEntity dateOfService = objMS.GetMSDateOfService(patientId, NoteId, NoteType);
            List<IniEvalDiagnosisCodesEntity> diagnosisCodes = GetDiagnosisCodes(patientId, NoteId, NoteType);

            //==================Pain==================
            List<IniEvalDocPainEntity> docpain = new List<IniEvalDocPainEntity>();
            if (NoteId > 0)
            {
                docpain = GetDocPain(0, NoteId, "D");
            }
            else { docpain = GetDocPain(patientId, 0, "P"); }
            //=========================================

            //=================Fun Char==========================

            List<MSFunctionalCharEntity> functChar = objMS.GetFunctChar(0, NoteId, NoteType);
            List<MSFunctionalCharEntity> functCharReminder = objMS.GetFunctChar(patientId, 0, NoteType);
            string strfunctCharReminder = string.Empty;
            string strfunctChar = string.Empty;
            int i = 1;
            if (functChar.Count > 0)
            {
                if (i == 1)
                {
                    strfunctChar = strfunctChar + functChar.FirstOrDefault().funccharac;
                }
                foreach (MSFunctionalCharEntity obj in functCharReminder)
                {

                    strfunctCharReminder = strfunctCharReminder + obj.funccharac;
                    strfunctCharReminder = strfunctCharReminder + Environment.NewLine;
                }
            }
            //=================Fun Char end==========================
            MSHints hints = objMS.GetHints(patientId, NoteId, NoteType);
            List<IniEvalExtremityEntity> extremityTests = new List<IniEvalExtremityEntity>();

            //==================extremityTests==================
            if (NoteId > 0)
            {
                extremityTests = GetExtremityTests(0, NoteId, "D");
            }
            else { extremityTests = GetExtremityTests(patientId, 0, "P"); }
            //=========================================
            List<IniEvalSpinalTestEntity> spinalTest = new List<IniEvalSpinalTestEntity>();
            //==================SpinalTest==================
            if (NoteId > 0)
            {
                spinalTest = GetSpinalTests(0, NoteId, "D");
            }
            else { spinalTest = GetSpinalTests(patientId, 0, "P"); }
            //=========================================
            List<IniEvalOMPTEntity> oMPTTest = new List<IniEvalOMPTEntity>();
            //==================OMPTTest==================
            if (NoteId > 0)
            {
                oMPTTest = GetOMPT(0, NoteId, "D");
            }
            else { oMPTTest = GetOMPT(patientId, 0, "P"); }
            //=========================================

            //===================GOAL=================
            List<MSGoalsEntity> shortTermGoal = objMS.GetGoals(patientId, "S");
            List<MSGoalsEntity> longTermGoal = objMS.GetGoals(patientId, "L");
            List<MSGoalsEntity> freeTermGoal = objMS.GetGoals(patientId, "F");
            //==============
            if (NoteType == "PFCE")
            {
                string SPName = "SP_GetActiveDocFCECPTCodes";
                List<IniEvalInterventionEntity> ActiveDocFCECPTCodes = GetActiveDocFCECPTCodes(patientId, SPName);
            }
            else
            {
                string SPName = "SP_GetActiveDocAllCPTCodes";
                List<IniEvalInterventionEntity> ActiveDocAllCPTCodes = GetActiveDocFCECPTCodes(patientId, SPName);
            }
            //=================Pro Intervention==================

            List<IniEvalProInterventionEntity> proIntervention;
            if (NoteId > 0)
            {

                proIntervention = GetActiveDocProInterventions(0, NoteId);
            }
            else
            {
                proIntervention = GetActiveDocProInterventions(patientId, NoteId);
            }
            IniEvalSummaryEntity noteSumary = GetDocNoteSummary(NoteId);


            List<MSShoulderEntity> shoulderIE = objMS.GetMeasurements(patientId, NoteId, "Shoulder", "IE", NoteType);
            string OtherMeasurementShoulderIE = objMS.GetOthMeasurements(patientId, NoteId, "Shoulder", "IE", NoteType);
            List<MSShoulderEntity> shoulderGoal = objMS.GetMeasurements(patientId, NoteId, "Shoulder", "Goal", NoteType);
            string OtherMeasurementShoulderGoal = objMS.GetOthMeasurements(patientId, NoteId, "Shoulder", "Goal", NoteType);
            List<MSShoulderEntity> shoulderLM = objMS.GetMeasurements(patientId, NoteId, "Shoulder", "LM", NoteType);
            string OtherMeasurementShoulderLM = objMS.GetOthMeasurements(patientId, NoteId, "Shoulder", "LM", NoteType);
            List<MSShoulderEntity> shoulderCM = objMS.GetMeasurements(patientId, NoteId, "Shoulder", "CM", NoteType);
            string OtherMeasurementshoulderCM = objMS.GetOthMeasurements(patientId, NoteId, "Shoulder", "CM", NoteType);

            //Elbow
            List<MSShoulderEntity> elbowIE = objMS.GetMeasurements(patientId, NoteId, "Elbow", "IE", NoteType);
            string OtherMeasurementElbowIE = objMS.GetOthMeasurements(patientId, NoteId, "Elbow", "IE", NoteType);
            List<MSShoulderEntity> elbowGoal = objMS.GetMeasurements(patientId, NoteId, "Elbow", "Goal", NoteType);
            string OtherMeasurementElbowGoal = objMS.GetOthMeasurements(patientId, NoteId, "Elbow", "Goal", NoteType);
            List<MSShoulderEntity> elbowLM = objMS.GetMeasurements(patientId, NoteId, "Elbow", "LM", NoteType);
            string OtherMeasurementElbowLM = objMS.GetOthMeasurements(patientId, NoteId, "Elbow", "LM", NoteType);
            List<MSShoulderEntity> elbowCM = objMS.GetMeasurements(patientId, NoteId, "Elbow", "CM", NoteType);
            string OtherMeasurementElbowCM = objMS.GetOthMeasurements(patientId, NoteId, "Elbow", "CM", NoteType);

            //Forearm
            List<MSShoulderEntity> ForearmIE = objMS.GetMeasurements(patientId, NoteId, "Forearm", "IE", NoteType);
            string OtherMeasurementForearmIE = objMS.GetOthMeasurements(patientId, NoteId, "Forearm", "IE", NoteType);
            List<MSShoulderEntity> ForearmGoal = objMS.GetMeasurements(patientId, NoteId, "Forearm", "Goal", NoteType);
            string OtherMeasurementForearmGoal = objMS.GetOthMeasurements(patientId, NoteId, "Forearm", "Goal", NoteType);
            List<MSShoulderEntity> ForearmLM = objMS.GetMeasurements(patientId, NoteId, "Forearm", "LM", NoteType);
            string OtherMeasurementForearmLM = objMS.GetOthMeasurements(patientId, NoteId, "Forearm", "LM", NoteType);
            List<MSShoulderEntity> ForearmCM = objMS.GetMeasurements(patientId, NoteId, "Forearm", "CM", NoteType);
            string OtherMeasurementForearmCM = objMS.GetOthMeasurements(patientId, NoteId, "Forearm", "CM", NoteType);

            //Wrist
            List<MSShoulderEntity> WristIE = objMS.GetMeasurements(patientId, NoteId, "Wrist", "IE", NoteType);
            string OtherMeasurementWristIE = objMS.GetOthMeasurements(patientId, NoteId, "Wrist", "IE", NoteType);
            List<MSShoulderEntity> WristGoal = objMS.GetMeasurements(patientId, NoteId, "Wrist", "Goal", NoteType);
            string OtherMeasurementWristGoal = objMS.GetOthMeasurements(patientId, NoteId, "Wrist", "Goal", NoteType);
            List<MSShoulderEntity> WristLM = objMS.GetMeasurements(patientId, NoteId, "Wrist", "LM", NoteType);
            string OtherMeasurementWristLM = objMS.GetOthMeasurements(patientId, NoteId, "Wrist", "LM", NoteType);
            List<MSShoulderEntity> WristCM = objMS.GetMeasurements(patientId, NoteId, "Wrist", "CM", NoteType);
            string OtherMeasurementWristCM = objMS.GetOthMeasurements(patientId, NoteId, "Wrist", "CM", NoteType);

            //Hip
            List<MSShoulderEntity> HipIE = objMS.GetMeasurements(patientId, NoteId, "Hip", "IE", NoteType);
            string OtherMeasurementHipIE = objMS.GetOthMeasurements(patientId, NoteId, "Hip", "IE", NoteType);
            List<MSShoulderEntity> HipGoal = objMS.GetMeasurements(patientId, NoteId, "Hip", "Goal", NoteType);
            string OtherMeasurementHipGoal = objMS.GetOthMeasurements(patientId, NoteId, "Hip", "Goal", NoteType);
            List<MSShoulderEntity> HipLM = objMS.GetMeasurements(patientId, NoteId, "Hip", "LM", NoteType);
            string OtherMeasurementHipLM = objMS.GetOthMeasurements(patientId, NoteId, "Hip", "LM", NoteType);
            List<MSShoulderEntity> HipCM = objMS.GetMeasurements(patientId, NoteId, "Hip", "CM", NoteType);
            string OtherMeasurementHipCM = objMS.GetOthMeasurements(patientId, NoteId, "Hip", "CM", NoteType);

            //Knee
            List<MSShoulderEntity> KneeIE = objMS.GetMeasurements(patientId, NoteId, "Knee", "IE", NoteType);
            string OtherMeasurementKneeIE = objMS.GetOthMeasurements(patientId, NoteId, "Knee", "IE", NoteType);
            List<MSShoulderEntity> KneeGoal = objMS.GetMeasurements(patientId, NoteId, "Knee", "Goal", NoteType);
            string OtherMeasurementKneeGoal = objMS.GetOthMeasurements(patientId, NoteId, "Knee", "Goal", NoteType);
            List<MSShoulderEntity> KneeLM = objMS.GetMeasurements(patientId, NoteId, "Knee", "LM", NoteType);
            string OtherMeasurementKneeLM = objMS.GetOthMeasurements(patientId, NoteId, "Knee", "LM", NoteType);
            List<MSShoulderEntity> KneeCM = objMS.GetMeasurements(patientId, NoteId, "Knee", "CM", NoteType);
            string OtherMeasurementKneeCM = objMS.GetOthMeasurements(patientId, NoteId, "Knee", "CM", NoteType);
            //Ankle
            List<MSShoulderEntity> AnkleIE = objMS.GetMeasurements(patientId, NoteId, "Ankle", "IE", NoteType);
            string OtherMeasurementAnkleIE = objMS.GetOthMeasurements(patientId, NoteId, "Ankle", "IE", NoteType);
            List<MSShoulderEntity> AnkleGoal = objMS.GetMeasurements(patientId, NoteId, "Ankle", "Goal", NoteType);
            string OtherMeasurementAnkleGoal = objMS.GetOthMeasurements(patientId, NoteId, "Ankle", "Goal", NoteType);
            List<MSShoulderEntity> AnkleLM = objMS.GetMeasurements(patientId, NoteId, "Ankle", "LM", NoteType);
            string OtherMeasurementAnkleLM = objMS.GetOthMeasurements(patientId, NoteId, "Ankle", "LM", NoteType);
            List<MSShoulderEntity> AnkleCM = objMS.GetMeasurements(patientId, NoteId, "Ankle", "CM", NoteType);
            string OtherMeasurementAnkleCM = objMS.GetOthMeasurements(patientId, NoteId, "Ankle", "CM", NoteType);

            //CervicalLR
            List<MSShoulderEntity> CervicalLRIE = objMS.GetMeasurements(patientId, NoteId, "CervicalLR", "IE", NoteType);
            string OtherMeasurementCervicalLRIE = objMS.GetOthMeasurements(patientId, NoteId, "CervicalLR", "IE", NoteType);
            List<MSShoulderEntity> CervicalLRGoal = objMS.GetMeasurements(patientId, NoteId, "CervicalLR", "Goal", NoteType);
            string OtherMeasurementCervicalLRGoal = objMS.GetOthMeasurements(patientId, NoteId, "CervicalLR", "Goal", NoteType);
            List<MSShoulderEntity> CervicalLRLM = objMS.GetMeasurements(patientId, NoteId, "CervicalLR", "LM", NoteType);
            string OtherMeasurementCervicalLRLM = objMS.GetOthMeasurements(patientId, NoteId, "CervicalLR", "LM", NoteType);
            List<MSShoulderEntity> CervicalLRCM = objMS.GetMeasurements(patientId, NoteId, "CervicalLR", "CM", NoteType);
            string OtherMeasurementCervicalLRCM = objMS.GetOthMeasurements(patientId, NoteId, "CervicalLR", "CM", NoteType);

            //Cervical
            List<MSShoulderEntity> CervicalIE = objMS.GetMeasurements(patientId, NoteId, "Cervical", "IE", NoteType);
            List<MSShoulderEntity> CervicalGoal = objMS.GetMeasurements(patientId, NoteId, "Cervical", "Goal", NoteType);
            List<MSShoulderEntity> CervicalLM = objMS.GetMeasurements(patientId, NoteId, "Cervical", "LM", NoteType);
            List<MSShoulderEntity> CervicalCM = objMS.GetMeasurements(patientId, NoteId, "Cervical", "CM", NoteType);

            //LumbarLR
            List<MSShoulderEntity> LumbarLRIE = objMS.GetMeasurements(patientId, NoteId, "LumbarLR", "IE", NoteType);
            string OtherMeasurementLumbarLRIE = objMS.GetOthMeasurements(patientId, NoteId, "LumbarLR", "IE", NoteType);
            List<MSShoulderEntity> LumbarLRGoal = objMS.GetMeasurements(patientId, NoteId, "LumbarLR", "Goal", NoteType);
            string OtherMeasurementLumbarLRGoal = objMS.GetOthMeasurements(patientId, NoteId, "LumbarLR", "Goal", NoteType);
            List<MSShoulderEntity> LumbarLRLM = objMS.GetMeasurements(patientId, NoteId, "LumbarLR", "LM", NoteType);
            string OtherMeasurementLumbarLRLM = objMS.GetOthMeasurements(patientId, NoteId, "LumbarLR", "LM", NoteType);
            List<MSShoulderEntity> LumbarLRCM = objMS.GetMeasurements(patientId, NoteId, "LumbarLR", "CM", NoteType);
            string OtherMeasurementLumbarLRCM = objMS.GetOthMeasurements(patientId, NoteId, "LumbarLR", "CM", NoteType);

            //Cervical
            List<MSShoulderEntity> LumbarIE = objMS.GetMeasurements(patientId, NoteId, "Lumbar", "IE", NoteType);
            List<MSShoulderEntity> LumbarGoal = objMS.GetMeasurements(patientId, NoteId, "Lumbar", "Goal", NoteType);
            List<MSShoulderEntity> LumbarLM = objMS.GetMeasurements(patientId, NoteId, "Lumbar", "LM", NoteType);
            List<MSShoulderEntity> LumbarCM = objMS.GetMeasurements(patientId, NoteId, "Lumbar", "CM", NoteType);

            string OtherMeasurementOtherIE = objMS.GetOthMeasurements(patientId, NoteId, "Other", "IE", NoteType);
            string OtherMeasurementOtherRGoal = objMS.GetOthMeasurements(patientId, NoteId, "Other", "Goal", NoteType);
            string OtherMeasurementOtherLM = objMS.GetOthMeasurements(patientId, NoteId, "Other", "LM", NoteType);
            string OtherMeasurementOtherCM = objMS.GetOthMeasurements(patientId, NoteId, "Other", "CM", NoteType);

            List<MSGoalsEntity> MSGoal1 = objMS.GetGoals(patientId, "L");
            List<MSGoalsEntity> MSGoal2 = objMS.GetGoals(patientId, "F");
            List<MSGoalsEntity> MSGoal = MSGoal1.Union(MSGoal2).ToList();
            List<IniEvalReferrerEntity> Referrer = GetReferrer(patientId, NoteId);
            #region
            JObject jss =
             new JObject(
              new JProperty("component",
               new JObject(new JProperty("structuredBody",
                new JObject(new JProperty("component",
                 new JObject(new JProperty("section",
                  new JObject(new JProperty("@nullFlavor", "NI"),

                   new JProperty("templateId",
                    new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.43"))),
                    new JProperty("TeamRehabID",
                    new JObject(new JProperty("PatientID", patientId),
                     new JProperty("NoteID", NoteId))),
                   new JProperty("code",
                    new JObject(new JProperty("@code", "29308-4"),
                     new JProperty("@displayName", "Diagnosis"),
                     new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
                     new JProperty("@codeSystemName", "LOINC"))),

                   new JProperty("title", "Diagnosis"),
                   new JProperty("text",
                    new JObject(new JProperty("table",
                     new JObject(new JProperty("@border", "1"),
                      new JProperty("width", "100%"),
                      new JProperty("thead",
                       new JObject(new JProperty("tr",
                        new JObject(new JProperty("th",
                         new JObject(
                          new JProperty("Priority",
                         "Onset Date",
                           "DiagnosisCode",
                           "Description"))))))), //thead

                      new JProperty("tbody",
                       new JObject(new JProperty("tr", DiagnosisArray(diagnosisCodes)




                       ))) // tbody

                     )))
                   )))), //section
                         //==============Pain ========================
                 new JObject(new JProperty("section",
                  new JObject(new JProperty("templateId",
                    new JObject(new JProperty("@root", "1.2.276.0.76.10.40361.2.276.0.76.10.4036"))),
                   new JProperty("id",
                    new JObject(new JProperty("@root", "1.2.276.0.76.4.17.9814184919"),
                     new JProperty("@extension", "10c1eb7e-dc2d-4d1f-806a-2ad65eba0396"))),

                   new JProperty("code",
                    new JObject(new JProperty("@code", "86631-9"),
                     new JProperty("@displayName", "Pain"),
                     new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
                     new JProperty("@codeSystemName", "LOINC"))),

                   new JProperty("title", "Pain"),
                      new JProperty("text",
                       new JObject(new JProperty("table",
                        new JObject(new JProperty("@border", "1"),
                         new JProperty("width", "100%"),
                         new JProperty("thead",
                          new JObject(new JProperty("tr",
                           new JObject(new JProperty("th",
                            new JObject(
                             new JProperty("Pain",
                                    "At Rest",
                                    "With Activity",
                                    "Exacerbating Factors",
                                    "Relieving Factors")
                             )))))),

                           new JProperty("tbody",
                            new JObject(new JProperty("tr", PainArray(docpain)



                       ))) // tbody

                     )))
                   )))),
              //===========Pain End================
              //==========Reminder Section =================
              new JObject(new JProperty("section",
              new JObject(new JProperty("templateId",
              new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.8"))),

              new JProperty("code",
              new JObject(new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
              new JProperty("@codeSystemName", "LOINC"),
              new JProperty("@code", "51848-0"),
              new JProperty("@displayName", "Reminder"))),

              new JProperty("title", "Reminder"),
              new JProperty("text",
              new JObject(new JProperty("table",
              new JObject(new JProperty("@border", "1"),
              new JProperty("width", "100%"),
              new JProperty("thead",
              new JObject(new JProperty("tr",
              new JObject(new JProperty("th",
              new JObject(
              new JProperty("Date of Service"),
              new JProperty("Reminder"))))))),


              new JProperty("tbody",
              new JObject(new JProperty("tr",
              new JObject(new JProperty("td",
              new JObject(new JProperty(dateOfService.DateOfService),
              new JProperty(hints.FuncCharRem))))

              //    new JObject(new JProperty("td",
              //new JObject(new JProperty("7/19/2017"),
              //new JProperty("The domestic dog"))))
              )))




              ))))     //title

              ))),//section


                // ====================Section Functional Analysis===================
                new JObject(new JProperty("section",
              new JObject(new JProperty("templateId",
              new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.8"))),

              new JProperty("code",
              new JObject(new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
              new JProperty("@codeSystemName", "LOINC"),
              new JProperty("@code", "51848-0"),
              new JProperty("@displayName", "Functional Analysis"))),

              new JProperty("title", "Functional Analysis"),

              new JProperty("text",
              new JObject(new JProperty("paragraph",
              new JObject(new JProperty(strfunctChar))))
              )

              ))),  //section
                    // ====================Section Hints===================
                  new JObject(new JProperty("section",
              new JObject(new JProperty("templateId",
              new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.8"))),

              new JProperty("code",
              new JObject(new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
              new JProperty("@codeSystemName", "LOINC"),
              new JProperty("@code", "51848-0"),
              new JProperty("@displayName", "Hints"))),

              new JProperty("title", "Hints"),

              new JProperty("text",
              new JObject(new JProperty("paragraph",
              new JObject(new JProperty(hints.FuncCharacHint))))
              )

              ))),  //section

            new JObject(new JProperty("section",
            new JObject(new JProperty("templateId",
            new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.60"))),

            new JProperty("code",
            new JObject(new JProperty("@code", "61146-7"),
            new JProperty("@displayName", "Goals"),
            new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
            new JProperty("@codeSystemName", "LOINC"))),

            new JProperty("title", "Goals Section"),
            new JProperty("text",
            new JObject(new JProperty("table",
            new JObject(new JProperty("@border", "1"),
            new JProperty("width", "100%"),
            new JProperty("thead",
            new JObject(new JProperty("tr",
            new JObject(new JProperty("th",
            new JObject(
            new JProperty("Long Term Goals",
                                "Not Met",
                                "Partial Met",
                                "Met"))))))),


            new JProperty("tbody",
            new JObject(new JProperty("tr", objMS.GoalsArray(MSGoal)

            )))
            ))))     //title

            ))),

   new JObject(new JProperty("section",
            new JObject(new JProperty("templateId",
            new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.21.2.3"),
            new JProperty("@extension", "2015-08-01"))),


            new JProperty("code",
            new JObject(new JProperty("@code", "62387-6"),
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
            new JObject(new JProperty("th", "Planned Intervention"))))),


            new JProperty("tbody",
              new JObject(new JProperty("tr", IntervensionArray(proIntervention)
           )))
            ))))     //title

            ))),

            new JObject(new JProperty("section",
                     new JObject(new JProperty("templateId",
                     new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.21.2.3"),
                     new JProperty("@extension", "2015-08-01"))),


           new JProperty("code",
                      new JObject(new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
                     new JProperty("@codeSystemName", "LOINC"),
                    new JProperty("@code", "51848-0"),
                     new JProperty("@displayName", "Referrer"))),

                      new JProperty("title", "Referrer"),
                      new JProperty("text",
                      new JObject(new JProperty("table",
                      new JObject(new JProperty("@border", "1"),
                      new JProperty("width", "100%"),
                      new JProperty("thead",
                      new JObject(new JProperty("tr",
                      new JObject(new JProperty("th", "Referrer"))))),


                         new JProperty("tbody",
                         new JObject(new JProperty("tr", ReferrerArray(Referrer)




                         )))
                         ))))     //title

                         ))),

                // ====================Section Functional Analysis===================
                new JObject(new JProperty("section",
              new JObject(new JProperty("templateId",
              new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.8"))),

              new JProperty("code",
              new JObject(new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
              new JProperty("@codeSystemName", "LOINC"),
              new JProperty("@code", "51848-0"),
              new JProperty("@displayName", "Summary"))),

              new JProperty("title", "Summary"),

              new JProperty("text",
              new JObject(new JProperty("paragraph",
              new JObject(new JProperty(noteSumary.noteSummary))))
              )

              ))),  //section
            new JObject(new JProperty("section",
            new JObject(new JProperty("templateId",
            new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.8"))),

            new JProperty("code",
            new JObject(new JProperty("@code", "54755-4"),
            new JProperty("@displayName", "Extremity test"),
            new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
            new JProperty("@codeSystemName", "LOINC")
           )),

            new JProperty("title", "Extremity test"),
            new JProperty("text",
            new JObject(new JProperty("table",
            new JObject(new JProperty("@border", "1"),
            new JProperty("width", "100%"),
            new JProperty("thead",
            new JObject(new JProperty("tr",
            new JObject(new JProperty("th",
            new JObject(
            new JProperty("Description"),
            new JProperty("Type"))))))),


            new JProperty("tbody",
             new JObject(new JProperty("tr", ExtremityArray(extremityTests)

             )))


            ))))     //title

            ))),

   new JObject(new JProperty("section",
            new JObject(new JProperty("templateId",
            new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.8"))),

            new JProperty("code",
            new JObject(new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
            new JProperty("@codeSystemName", "LOINC"),
            new JProperty("@code", "51848-0"),
            new JProperty("@displayName", "OMTP Test"))),

            new JProperty("title", "OMTP Test"),
            new JProperty("text",
            new JObject(new JProperty("table",
            new JObject(new JProperty("@border", "1"),
            new JProperty("width", "100%"),
            new JProperty("thead",
            new JObject(new JProperty("tr",
            new JObject(new JProperty("th",
            new JObject(
            new JProperty("Decription",
                                 "Produced / Abolished / Not Affected",
                                 "Increased / Decreased",
                                 "Worse / Better",
                                 "Centralized / Perip"))))))),


            new JProperty("tbody",
            new JObject(new JProperty("tr",
           new JObject(new JProperty("tr", OMPTArray(oMPTTest)
           ))

            )))
            ))))     //title

            ))),


     new JObject(new JProperty("section",
            new JObject(new JProperty("templateId",
            new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.8"))),

            new JProperty("code",
            new JObject(new JProperty("@code", "54755-4"),
            new JProperty("@displayName", "Spinal test"),
            new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
            new JProperty("@codeSystemName", "LOINC")
           )),

            new JProperty("title", "Spinal test"),
            new JProperty("text",
            new JObject(new JProperty("table",
            new JObject(new JProperty("@border", "1"),
            new JProperty("width", "100%"),
            new JProperty("thead",
            new JObject(new JProperty("tr",
            new JObject(new JProperty("th",
            new JObject(
            new JProperty("Description"),
            new JProperty("Type"))))))),


            new JProperty("tbody",
             new JObject(new JProperty("tr", SpinalTestArray(spinalTest)

             )))


            ))))     //title

            ))),

      //==========OMTP ASSESSMENT===========================

      new JObject(new JProperty("section",
            new JObject(new JProperty("templateId",
            new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.8"))),

            new JProperty("code",
            new JObject(new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
            new JProperty("@codeSystemName", "LOINC"),
            new JProperty("@code", "51848-0"),
            new JProperty("@displayName", "OMTP ASSESSMENT"))),

            new JProperty("title", "OMTP ASSESSMENT"),

            new JProperty("text",
            new JObject(new JProperty("paragraph", "The domestic dog (Canis lupus familiaris or Canis familiaris) is a member of genus Canis (canines) that forms part of the wolf-like canids,[3] and is the most widely abundant")
           ))
            )

            )),




                   //======================Measurement=====================
                   new JObject(new JProperty("section",
                      new JObject(new JProperty("templateId",
                      new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.4.1")),

                      new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.4.1"),
                                  new JProperty("@extension", "2015-08-01"))),

                  new JProperty("code",
                  new JObject(new JProperty("@code", "8716-3"),
                  new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
                  new JProperty("@codeSystemName", "LOINC"),
                      new JProperty("@displayName", "Vital Signs"))),

                  new JProperty("title", "Vital Signs (Last Filed)"),

                   new JProperty("text",
              new JObject(new JProperty("table",
              new JObject(new JProperty("@border", "1"),
              new JProperty("width", "100%"),
              new JProperty("thead",
              new JObject(new JProperty("tr",
              new JObject(new JProperty("th",
              new JObject(
              new JProperty("@scope", "col"),
              new JProperty("#text", "L")),
              new JObject(
              new JProperty("@scope", "col"),
              new JProperty("#text", "R")),
              new JObject(
              new JProperty("@scope", "col"),
              new JProperty("#text", "L")),
              new JObject(
              new JProperty("@scope", "col"),
              new JProperty("#text", "R")),
              new JObject(
              new JProperty("@scope", "col"),
              new JProperty("#text", "L")),
              new JObject(
              new JProperty("@scope", "col"),
              new JProperty("#text", "R"))))))),

              new JProperty("tbody",
              new JObject(new JProperty("@id", "Shoulder"),
                          new JProperty("tr", objMS.MeasurementArray(shoulderCM)



                          )        //tr        


              ),
             new JObject(new JProperty("@id", "Elbow"),
                          new JProperty("tr", objMS.MeasurementArray(elbowCM)

                         )),    // new JObject

              new JObject(new JProperty("@id", "Forearm"),
                          new JProperty("tr", objMS.MeasurementArray(ForearmCM)
                         )),

               new JObject(new JProperty("@id", "Wrist"),
                          new JProperty("tr", objMS.MeasurementArray(WristCM)

                                      )),

                new JObject(new JProperty("@id", "Hip"),
                          new JProperty("tr", objMS.MeasurementArray(HipCM)



                          )),
                new JObject(new JProperty("@id", "Knee"),
                          new JProperty("tr", objMS.MeasurementArray(KneeCM)

                                      )),
                new JObject(new JProperty("@id", "Ankle"),
                          new JProperty("tr", objMS.MeasurementArray(AnkleCM)

                                      )),
                new JObject(new JProperty("@id", "Cervical"),
                          new JProperty("tr", objMS.MeasurementArray(CervicalCM)

                                      )),
                  new JObject(new JProperty("@id", "Lumbar"),
                          new JProperty("tr",

                           objMS.MeasurementArray(LumbarCM)
                                      )),
                                         new JObject(new JProperty("@id", "Other"),
                          new JProperty("tr",
                          OtherMeasurementOtherCM
                                      ))

              )          //tbody 


                                   ))))


                  ))), //section

                   new JObject(new JProperty("section",
                      new JObject(new JProperty("templateId",
                      new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.4.1")),

                      new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.4.1"),
                                  new JProperty("@extension", "2015-08-01"))),

                  new JProperty("code",
                  new JObject(new JProperty("@code", "8716-3"),
                  new JProperty("@codeSystem", "2.16.840.1.113883.6.1"),
                  new JProperty("@codeSystemName", "LOINC"),
                      new JProperty("@displayName", "Vital Signs"))),

                  new JProperty("title", "Vital Signs (Previous Measurement)"),

                   new JProperty("text",
              new JObject(new JProperty("table",
              new JObject(new JProperty("@border", "1"),
              new JProperty("width", "100%"),
              new JProperty("thead",
              new JObject(new JProperty("tr",
              new JObject(new JProperty("th",
              new JObject(
              new JProperty("@scope", "col"),
              new JProperty("#text", "L")),
              new JObject(
              new JProperty("@scope", "col"),
              new JProperty("#text", "R")),
              new JObject(
              new JProperty("@scope", "col"),
              new JProperty("#text", "L")),
              new JObject(
              new JProperty("@scope", "col"),
              new JProperty("#text", "R")),
              new JObject(
              new JProperty("@scope", "col"),
              new JProperty("#text", "L")),
              new JObject(
              new JProperty("@scope", "col"),
              new JProperty("#text", "R"))))))),

              new JProperty("tbody",
              new JObject(new JProperty("@id", "Shoulder-Goal"),
                          new JProperty("tr",
                          objMS.MeasurementArray(shoulderGoal)
                          )),

              new JObject(new JProperty("@id", "Shoulder-Last Measurement"),
                          new JProperty("tr", objMS.MeasurementArray(shoulderLM)


                          )),

               new JObject(new JProperty("@id", "Shoulder-Initial Measurement"),
                          new JProperty("tr", objMS.MeasurementArray(shoulderIE)
                          )        //tr 
                          ),

                new JObject(new JProperty("@id", "Elbow-Goal"),
                          new JProperty("tr", objMS.MeasurementArray(elbowGoal)

                                      )),    // new JObject

                new JObject(new JProperty("@id", "Elbow-Last Measurement"),
                          new JProperty("tr",
                        objMS.MeasurementArray(elbowLM)
                                      )),    // new JObject

                new JObject(new JProperty("@id", "Elbow-Initial Measurement"),
                          new JProperty("tr", objMS.MeasurementArray(elbowIE)

                                      )),    // new JObject

              new JObject(new JProperty("@id", "Forearm-Goal"),
                          new JProperty("tr", objMS.MeasurementArray(ForearmGoal)
                                      )),    // new JObject

                new JObject(new JProperty("@id", "Forearm-Last Measurement"),
                          new JProperty("tr",
                         objMS.MeasurementArray(ForearmLM)
                                      )),    // new JObject

                new JObject(new JProperty("@id", "Forearm-Initial Measurement"),
                          new JProperty("tr",
                           objMS.MeasurementArray(ForearmIE)
                                      )),    // new JObject

               new JObject(new JProperty("@id", "Wrist-Goal"),
                          new JProperty("tr",
                          objMS.MeasurementArray(WristGoal)
                                      )),
                 new JObject(new JProperty("@id", "Wrist-Last Measurement"),
                          new JProperty("tr",
                          objMS.MeasurementArray(WristLM)
                                      )),
                 new JObject(new JProperty("@id", "Wrist-Initial Measurement"),
                          new JProperty("tr",
                          objMS.MeasurementArray(WristIE)
                                      )),

                new JObject(new JProperty("@id", "Hip-Goal"),
                          new JProperty("tr",
                           objMS.MeasurementArray(HipGoal)
                            )),
                 new JObject(new JProperty("@id", "Hip-Last Measurement"),
                          new JProperty("tr",
                          objMS.MeasurementArray(HipLM)
                                      )),
                 new JObject(new JProperty("@id", "Hip-Initial Measurement"),
                          new JProperty("tr",
                          objMS.MeasurementArray(HipIE)
                          )),



                new JObject(new JProperty("@id", "Knee-Goal"),
                          new JProperty("tr",
                        objMS.MeasurementArray(KneeGoal)
                                      )),
                 new JObject(new JProperty("@id", "Knee-Last Measurement"),
                          new JProperty("tr",
                      objMS.MeasurementArray(KneeLM)
                                      )),
                    new JObject(new JProperty("@id", "Knee-Initial Measurement"),
                          new JProperty("tr",
                          objMS.MeasurementArray(KneeIE)
                                      )),

                new JObject(new JProperty("@id", "Ankle-Goal"),
                          new JProperty("tr",
                            objMS.MeasurementArray(AnkleGoal)
                                      )),
                  new JObject(new JProperty("@id", "Ankle-Last Measurement"),
                          new JProperty("tr",
                      objMS.MeasurementArray(AnkleLM)
                                      )),
                    new JObject(new JProperty("@id", "Ankle-Initial Measurement"),
                          new JProperty("tr",
                          objMS.MeasurementArray(AnkleIE)
                                      )),


                new JObject(new JProperty("@id", "Cervical-Goal"),
                          new JProperty("tr",
                           objMS.MeasurementArray(CervicalGoal)
                                      )),
                  new JObject(new JProperty("@id", "Cervical-Last Measurement"),
                          new JProperty("tr",
                     objMS.MeasurementArray(CervicalLM)
                                      )),
                    new JObject(new JProperty("@id", "Cervical-Initial Measurement"),
                          new JProperty("tr",
                          objMS.MeasurementArray(CervicalIE)
                                      )),

                 new JObject(new JProperty("@id", "Lumbar-Goal"),
                          new JProperty("tr",
                         objMS.MeasurementArray(LumbarGoal)
                                      )),
                   new JObject(new JProperty("@id", "Lumbar-Last Measurement"),
                          new JProperty("tr",
                      objMS.MeasurementArray(LumbarLM)
                                      )),
                    new JObject(new JProperty("@id", "Lumbar-Initial Measurement"),
                          new JProperty("tr",
                          objMS.MeasurementArray(LumbarIE)
                                      )),

                      new JObject(new JProperty("@id", "Other-Goal"),
                          new JProperty("tr",
                          OtherMeasurementOtherRGoal
                                      )),
                   new JObject(new JProperty("@id", "Other-Last Measurement"),
                          new JProperty("tr",
                    OtherMeasurementOtherLM
                                      )),
                    new JObject(new JProperty("@id", "Other-Initial Measurement"),
                          new JProperty("tr",
                          OtherMeasurementOtherIE
                                      ))

                                   //new JObject(new JProperty("@id", "Other Measurement"),
                                   //         new JProperty("tr",
                                   //         new JObject(new JProperty("td",
                                   //                     new JObject(new JProperty("EVAL 2"),
                                   //                                 new JProperty("MNN2")
                                   //                     ))



                                   //                     ),
                                   //          new JObject(new JProperty("td", "Hint : Girth, Palpation, Posture, Myotomes," +
                                   //          " Reflexes, Dermatomes, Sensation, Grip Strength, ROM, Other Joints."))
                                   //                     ))



                                   //tbody 


                                   ))))


                  ))) //section


              )

              ))))));    //end wale brackets

            #endregion
            return jss;
        }

        public string InitialEvalToCCDA_XML(int patientId, int NoteId, string NoteType, tblPatients patient)
        {
            JObject Jresponse = new JObject();
            MedicalNecessity objMS = new MedicalNecessity();
            MSDateOfServiceEntity dateOfService = objMS.GetMSDateOfService(patientId, NoteId, NoteType);
            List<IniEvalDiagnosisCodesEntity> diagnosisCodes = GetDiagnosisCodes(patientId, NoteId, NoteType);

            //==================Pain==================
            List<IniEvalDocPainEntity> docpain = new List<IniEvalDocPainEntity>();
            if (NoteId > 0)
            {
                docpain = GetDocPain(0, NoteId, "D");
            }
            else { docpain = GetDocPain(patientId, 0, "P"); }
            //=========================================

            //=================Fun Char==========================

            List<MSFunctionalCharEntity> functChar = objMS.GetFunctChar(0, NoteId, NoteType);
            List<MSFunctionalCharEntity> functCharReminder = objMS.GetFunctChar(patientId, 0, NoteType);
            string strfunctCharReminder = string.Empty;
            string strfunctChar = string.Empty;
            int i = 1;
            if (functChar.Count > 0)
            {
                if (i == 1)
                {
                    strfunctChar = strfunctChar + functChar.FirstOrDefault().funccharac;
                }
                foreach (MSFunctionalCharEntity obj in functCharReminder)
                {

                    strfunctCharReminder = strfunctCharReminder + obj.funccharac;
                    strfunctCharReminder = strfunctCharReminder + Environment.NewLine;
                }
            }
            //=================Fun Char end==========================
            MSHints hints = objMS.GetHints(patientId, NoteId, NoteType);
            List<IniEvalExtremityEntity> extremityTests = new List<IniEvalExtremityEntity>();

            //==================extremityTests==================
            if (NoteId > 0)
            {
                extremityTests = GetExtremityTests(0, NoteId, "D");
            }
            else { extremityTests = GetExtremityTests(patientId, 0, "P"); }
            //=========================================
            List<IniEvalSpinalTestEntity> spinalTest = new List<IniEvalSpinalTestEntity>();
            //==================SpinalTest==================
            if (NoteId > 0)
            {
                spinalTest = GetSpinalTests(0, NoteId, "D");
            }
            else { spinalTest = GetSpinalTests(patientId, 0, "P"); }
            //=========================================
            List<IniEvalOMPTEntity> oMPTTest = new List<IniEvalOMPTEntity>();
            //==================OMPTTest==================
            if (NoteId > 0)
            {
                oMPTTest = GetOMPT(0, NoteId, "D");
            }
            else { oMPTTest = GetOMPT(patientId, 0, "P"); }
            //=========================================

            //===================GOAL=================
            List<MSGoalsEntity> shortTermGoal = objMS.GetGoals(patientId, "S");
            List<MSGoalsEntity> longTermGoal = objMS.GetGoals(patientId, "L");
            List<MSGoalsEntity> freeTermGoal = objMS.GetGoals(patientId, "F");
            //==============
            if (NoteType == "PFCE")
            {
                string SPName = "SP_GetActiveDocFCECPTCodes";
                List<IniEvalInterventionEntity> ActiveDocFCECPTCodes = GetActiveDocFCECPTCodes(patientId, SPName);
            }
            else
            {
                string SPName = "SP_GetActiveDocAllCPTCodes";
                List<IniEvalInterventionEntity> ActiveDocAllCPTCodes = GetActiveDocFCECPTCodes(patientId, SPName);
            }
            //=================Pro Intervention==================

            List<IniEvalProInterventionEntity> proIntervention;
            if (NoteId > 0)
            {

                proIntervention = GetActiveDocProInterventions(0, NoteId);
            }
            else
            {
                proIntervention = GetActiveDocProInterventions(patientId, NoteId);
            }
            IniEvalSummaryEntity noteSumary = GetDocNoteSummary(NoteId);


            List<MSShoulderEntity> shoulderIE = objMS.GetMeasurements(patientId, NoteId, "Shoulder", "IE", NoteType);
            string OtherMeasurementShoulderIE = objMS.GetOthMeasurements(patientId, NoteId, "Shoulder", "IE", NoteType);
            List<MSShoulderEntity> shoulderGoal = objMS.GetMeasurements(patientId, NoteId, "Shoulder", "Goal", NoteType);
            string OtherMeasurementShoulderGoal = objMS.GetOthMeasurements(patientId, NoteId, "Shoulder", "Goal", NoteType);
            List<MSShoulderEntity> shoulderLM = objMS.GetMeasurements(patientId, NoteId, "Shoulder", "LM", NoteType);
            string OtherMeasurementShoulderLM = objMS.GetOthMeasurements(patientId, NoteId, "Shoulder", "LM", NoteType);
            List<MSShoulderEntity> shoulderCM = objMS.GetMeasurements(patientId, NoteId, "Shoulder", "CM", NoteType);
            string OtherMeasurementshoulderCM = objMS.GetOthMeasurements(patientId, NoteId, "Shoulder", "CM", NoteType);

            //Elbow
            List<MSShoulderEntity> elbowIE = objMS.GetMeasurements(patientId, NoteId, "Elbow", "IE", NoteType);
            string OtherMeasurementElbowIE = objMS.GetOthMeasurements(patientId, NoteId, "Elbow", "IE", NoteType);
            List<MSShoulderEntity> elbowGoal = objMS.GetMeasurements(patientId, NoteId, "Elbow", "Goal", NoteType);
            string OtherMeasurementElbowGoal = objMS.GetOthMeasurements(patientId, NoteId, "Elbow", "Goal", NoteType);
            List<MSShoulderEntity> elbowLM = objMS.GetMeasurements(patientId, NoteId, "Elbow", "LM", NoteType);
            string OtherMeasurementElbowLM = objMS.GetOthMeasurements(patientId, NoteId, "Elbow", "LM", NoteType);
            List<MSShoulderEntity> elbowCM = objMS.GetMeasurements(patientId, NoteId, "Elbow", "CM", NoteType);
            string OtherMeasurementElbowCM = objMS.GetOthMeasurements(patientId, NoteId, "Elbow", "CM", NoteType);

            //Forearm
            List<MSShoulderEntity> ForearmIE = objMS.GetMeasurements(patientId, NoteId, "Forearm", "IE", NoteType);
            string OtherMeasurementForearmIE = objMS.GetOthMeasurements(patientId, NoteId, "Forearm", "IE", NoteType);
            List<MSShoulderEntity> ForearmGoal = objMS.GetMeasurements(patientId, NoteId, "Forearm", "Goal", NoteType);
            string OtherMeasurementForearmGoal = objMS.GetOthMeasurements(patientId, NoteId, "Forearm", "Goal", NoteType);
            List<MSShoulderEntity> ForearmLM = objMS.GetMeasurements(patientId, NoteId, "Forearm", "LM", NoteType);
            string OtherMeasurementForearmLM = objMS.GetOthMeasurements(patientId, NoteId, "Forearm", "LM", NoteType);
            List<MSShoulderEntity> ForearmCM = objMS.GetMeasurements(patientId, NoteId, "Forearm", "CM", NoteType);
            string OtherMeasurementForearmCM = objMS.GetOthMeasurements(patientId, NoteId, "Forearm", "CM", NoteType);

            //Wrist
            List<MSShoulderEntity> WristIE = objMS.GetMeasurements(patientId, NoteId, "Wrist", "IE", NoteType);
            string OtherMeasurementWristIE = objMS.GetOthMeasurements(patientId, NoteId, "Wrist", "IE", NoteType);
            List<MSShoulderEntity> WristGoal = objMS.GetMeasurements(patientId, NoteId, "Wrist", "Goal", NoteType);
            string OtherMeasurementWristGoal = objMS.GetOthMeasurements(patientId, NoteId, "Wrist", "Goal", NoteType);
            List<MSShoulderEntity> WristLM = objMS.GetMeasurements(patientId, NoteId, "Wrist", "LM", NoteType);
            string OtherMeasurementWristLM = objMS.GetOthMeasurements(patientId, NoteId, "Wrist", "LM", NoteType);
            List<MSShoulderEntity> WristCM = objMS.GetMeasurements(patientId, NoteId, "Wrist", "CM", NoteType);
            string OtherMeasurementWristCM = objMS.GetOthMeasurements(patientId, NoteId, "Wrist", "CM", NoteType);

            //Hip
            List<MSShoulderEntity> HipIE = objMS.GetMeasurements(patientId, NoteId, "Hip", "IE", NoteType);
            string OtherMeasurementHipIE = objMS.GetOthMeasurements(patientId, NoteId, "Hip", "IE", NoteType);
            List<MSShoulderEntity> HipGoal = objMS.GetMeasurements(patientId, NoteId, "Hip", "Goal", NoteType);
            string OtherMeasurementHipGoal = objMS.GetOthMeasurements(patientId, NoteId, "Hip", "Goal", NoteType);
            List<MSShoulderEntity> HipLM = objMS.GetMeasurements(patientId, NoteId, "Hip", "LM", NoteType);
            string OtherMeasurementHipLM = objMS.GetOthMeasurements(patientId, NoteId, "Hip", "LM", NoteType);
            List<MSShoulderEntity> HipCM = objMS.GetMeasurements(patientId, NoteId, "Hip", "CM", NoteType);
            string OtherMeasurementHipCM = objMS.GetOthMeasurements(patientId, NoteId, "Hip", "CM", NoteType);

            //Knee
            List<MSShoulderEntity> KneeIE = objMS.GetMeasurements(patientId, NoteId, "Knee", "IE", NoteType);
            string OtherMeasurementKneeIE = objMS.GetOthMeasurements(patientId, NoteId, "Knee", "IE", NoteType);
            List<MSShoulderEntity> KneeGoal = objMS.GetMeasurements(patientId, NoteId, "Knee", "Goal", NoteType);
            string OtherMeasurementKneeGoal = objMS.GetOthMeasurements(patientId, NoteId, "Knee", "Goal", NoteType);
            List<MSShoulderEntity> KneeLM = objMS.GetMeasurements(patientId, NoteId, "Knee", "LM", NoteType);
            string OtherMeasurementKneeLM = objMS.GetOthMeasurements(patientId, NoteId, "Knee", "LM", NoteType);
            List<MSShoulderEntity> KneeCM = objMS.GetMeasurements(patientId, NoteId, "Knee", "CM", NoteType);
            string OtherMeasurementKneeCM = objMS.GetOthMeasurements(patientId, NoteId, "Knee", "CM", NoteType);
            //Ankle
            List<MSShoulderEntity> AnkleIE = objMS.GetMeasurements(patientId, NoteId, "Ankle", "IE", NoteType);
            string OtherMeasurementAnkleIE = objMS.GetOthMeasurements(patientId, NoteId, "Ankle", "IE", NoteType);
            List<MSShoulderEntity> AnkleGoal = objMS.GetMeasurements(patientId, NoteId, "Ankle", "Goal", NoteType);
            string OtherMeasurementAnkleGoal = objMS.GetOthMeasurements(patientId, NoteId, "Ankle", "Goal", NoteType);
            List<MSShoulderEntity> AnkleLM = objMS.GetMeasurements(patientId, NoteId, "Ankle", "LM", NoteType);
            string OtherMeasurementAnkleLM = objMS.GetOthMeasurements(patientId, NoteId, "Ankle", "LM", NoteType);
            List<MSShoulderEntity> AnkleCM = objMS.GetMeasurements(patientId, NoteId, "Ankle", "CM", NoteType);
            string OtherMeasurementAnkleCM = objMS.GetOthMeasurements(patientId, NoteId, "Ankle", "CM", NoteType);

            //CervicalLR
            List<MSShoulderEntity> CervicalLRIE = objMS.GetMeasurements(patientId, NoteId, "CervicalLR", "IE", NoteType);
            string OtherMeasurementCervicalLRIE = objMS.GetOthMeasurements(patientId, NoteId, "CervicalLR", "IE", NoteType);
            List<MSShoulderEntity> CervicalLRGoal = objMS.GetMeasurements(patientId, NoteId, "CervicalLR", "Goal", NoteType);
            string OtherMeasurementCervicalLRGoal = objMS.GetOthMeasurements(patientId, NoteId, "CervicalLR", "Goal", NoteType);
            List<MSShoulderEntity> CervicalLRLM = objMS.GetMeasurements(patientId, NoteId, "CervicalLR", "LM", NoteType);
            string OtherMeasurementCervicalLRLM = objMS.GetOthMeasurements(patientId, NoteId, "CervicalLR", "LM", NoteType);
            List<MSShoulderEntity> CervicalLRCM = objMS.GetMeasurements(patientId, NoteId, "CervicalLR", "CM", NoteType);
            string OtherMeasurementCervicalLRCM = objMS.GetOthMeasurements(patientId, NoteId, "CervicalLR", "CM", NoteType);

            //Cervical
            List<MSShoulderEntity> CervicalIE = objMS.GetMeasurements(patientId, NoteId, "Cervical", "IE", NoteType);
            List<MSShoulderEntity> CervicalGoal = objMS.GetMeasurements(patientId, NoteId, "Cervical", "Goal", NoteType);
            List<MSShoulderEntity> CervicalLM = objMS.GetMeasurements(patientId, NoteId, "Cervical", "LM", NoteType);
            List<MSShoulderEntity> CervicalCM = objMS.GetMeasurements(patientId, NoteId, "Cervical", "CM", NoteType);

            //LumbarLR
            List<MSShoulderEntity> LumbarLRIE = objMS.GetMeasurements(patientId, NoteId, "LumbarLR", "IE", NoteType);
            string OtherMeasurementLumbarLRIE = objMS.GetOthMeasurements(patientId, NoteId, "LumbarLR", "IE", NoteType);
            List<MSShoulderEntity> LumbarLRGoal = objMS.GetMeasurements(patientId, NoteId, "LumbarLR", "Goal", NoteType);
            string OtherMeasurementLumbarLRGoal = objMS.GetOthMeasurements(patientId, NoteId, "LumbarLR", "Goal", NoteType);
            List<MSShoulderEntity> LumbarLRLM = objMS.GetMeasurements(patientId, NoteId, "LumbarLR", "LM", NoteType);
            string OtherMeasurementLumbarLRLM = objMS.GetOthMeasurements(patientId, NoteId, "LumbarLR", "LM", NoteType);
            List<MSShoulderEntity> LumbarLRCM = objMS.GetMeasurements(patientId, NoteId, "LumbarLR", "CM", NoteType);
            string OtherMeasurementLumbarLRCM = objMS.GetOthMeasurements(patientId, NoteId, "LumbarLR", "CM", NoteType);

            //Cervical
            List<MSShoulderEntity> LumbarIE = objMS.GetMeasurements(patientId, NoteId, "Lumbar", "IE", NoteType);
            List<MSShoulderEntity> LumbarGoal = objMS.GetMeasurements(patientId, NoteId, "Lumbar", "Goal", NoteType);
            List<MSShoulderEntity> LumbarLM = objMS.GetMeasurements(patientId, NoteId, "Lumbar", "LM", NoteType);
            List<MSShoulderEntity> LumbarCM = objMS.GetMeasurements(patientId, NoteId, "Lumbar", "CM", NoteType);

            string OtherMeasurementOtherIE = objMS.GetOthMeasurements(patientId, NoteId, "Other", "IE", NoteType);
            string OtherMeasurementOtherRGoal = objMS.GetOthMeasurements(patientId, NoteId, "Other", "Goal", NoteType);
            string OtherMeasurementOtherLM = objMS.GetOthMeasurements(patientId, NoteId, "Other", "LM", NoteType);
            string OtherMeasurementOtherCM = objMS.GetOthMeasurements(patientId, NoteId, "Other", "CM", NoteType);

            List<MSGoalsEntity> MSGoal1 = objMS.GetGoals(patientId, "L");
            List<MSGoalsEntity> MSGoal2 = objMS.GetGoals(patientId, "F");
            List<MSGoalsEntity> MSGoal = MSGoal1.Union(MSGoal2).ToList();
            List<IniEvalReferrerEntity> Referrer = GetReferrer(patientId, NoteId);
            DateTime dateOfbirth = Convert.ToDateTime(patient.BirthDate);
            string birthDate = dateOfbirth.ToString("yyyyMMdd");
            
            string CCDA_XML = string.Empty;
            #region
            XDocument bandsDocument = new XDocument(
           new XDeclaration("1.0", "UTF-8", null),
           new XProcessingInstruction("xml-stylesheet",
                                       "type=\"text/xsl\" href=\"CCDA.xsl\""),


         new XElement("ClinicalDocument", new XAttribute("Xmlns", "urn:hl7-org:v3"),
                                          new XAttribute(XNamespace.Xmlns + "voc", "urn:hl7-org:v3/voc"),
                                          new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),

                                     new XElement("realmCode",
                                                    new XAttribute("code", "US")
                                                  ),
                                     new XElement("typeId",
                                                      new XAttribute("root", "2.16.840.1.113883.1.3"),
                                                      new XAttribute("extension", "POCD_HD000040")
                                                  ),
                                      new XElement("typeId",
                                                      new XAttribute("root", "2.16.840.1.113883.1.3")

                                                  ),
                                       new XElement("typeId",
                                                      new XAttribute("root", "2.16.840.1.113883.1.1")

                                                 ),
                                        new XElement("id",

                                                      new XAttribute("extension", "999021"),
                                                      new XAttribute("root", "2.16.840.1.113883.19"),
                                                       new XAttribute("PatientID", patientId),
                                                       new XAttribute("NoteId", NoteId)
                                                  ),
                                         new XElement("Code",

                                                      new XAttribute("codeSystem", "2.16.840.1.113883.6.1"),
                                                      new XAttribute("codeSystemName", "LOINC"),
                                                       new XAttribute("displayName", "Subsequent evaluation note")
                                                  ),
                                            new XElement("title", "Progress Note"),
                                            new XElement("effectiveTime",
                                                      new XAttribute("value", "20050329171504+0500")
                                                  ),
                                             new XElement("languageCode",
                                                      new XAttribute("code", "en-US")
                                                  ),
                                             new XElement("setId",

                                                      new XAttribute("extension", "111199021"),
                                                      new XAttribute("root", "2.16.840.1.113883.19")
                                                  ),
                                               new XElement("versionNumber",
                                                      new XAttribute("value", "1")
                                                  ),
                                               new XElement("recordTarget",
                                                               new XElement("patientRole",
                                                                               new XElement("id",
                                                                               new XAttribute("extension", "123456"),
                                                                                new XAttribute("root", "2.16.840.1.113883.19.5")
                                                                                           ),
                                                                                new XElement("addr",
                                                                                 new XElement("streetAddressLine", patient.Address1),
                                                                                 new XElement("city", patient.City),
                                                                                 new XElement("state", patient.State),
                                                                                 new XElement("postalCode", patient.ZipCode),
                                                                                 new XElement("country", "USA")
                                                                                           ),
                                                                                  new XElement("telecom",
                                                                                   new XAttribute("value", patient.City),
                                                                                   new XAttribute("use", "HP")
                                                                                               ),

                                                                                   new XElement("patient",
                                                                                            new XElement("name",
                                                                                                         new XElement("given", patient.FirstName),
                                                                                                          new XElement("family", patient.LastName)
                                                                                                         ),
                                                                                             new XElement("administrativeGenderCode",
                                                                                                              new XAttribute("code", "M"),
                                                                                                               new XAttribute("codeSystem", "22.16.840.1.113883.5.1")
                                                                                                           ),
                                                                                              new XElement("birthTime",
                                                                                               new XAttribute("value", birthDate)
                                                                                                           )
                                                                                               )
                                                                           //, new XElement("providerOrganization",
                                                                           //               new XElement("id",
                                                                           //                     new XAttribute("root", "2.16.840.1.113883.19.5")
                                                                           //                           ),
                                                                           //                new XElement("Name", "Team Rehab"),
                                                                           //                 new XElement("telecom",
                                                                           //                  new XAttribute("value", "tel:(555)555-1212"),
                                                                           //                  new XAttribute("use", "WP")
                                                                           //                              ),
                                                                           //                  new XElement("addr",
                                                                           //                        new XElement("streetAddressLine", "21 North Ave"),
                                                                           //                        new XElement("city", "Burlington"),
                                                                           //                        new XElement("state", "MI"),
                                                                           //                        new XElement("postalCode", "01803"),
                                                                           //                        new XElement("country", "USA")
                                                                           //                                  )
                                                                           //              )
                                                                           )
                                                           ),

          //============================================================================================

          new XElement("component",
           new XElement("structuredBody",
            //<!-- ******************* Evaluation Notes********************** -->
             new XElement("component",
                             new XElement("section",
                                  new XAttribute("nullFlavor", "NI"),
                                              new XElement("templateId",
                                               new XAttribute("root", "2.16.840.1.113883.10.20.22.2.43")
                                               ),
                                              new XElement("Code",
                                               new XAttribute("code", "29308-4"),
                                                new XAttribute("displayName", "Diagnosis"),
                                                 new XAttribute("codeSystem", "2.16.840.1.113883.6.1"),
                                                  new XAttribute("codeSystemName", "LOINC")
                                               ),
                                               new XElement("title", "Diagnosis"),
                                               new XElement("text",
                                                   new XElement("table",
                                                               new XAttribute("border", "1"),
                                                               new XAttribute("width", "100%"),
                                                                new XElement("thead",
                                                                          new XElement("tr",
                                                                           new XElement("th", "Priority"),
                                                                             // new XElement("th", "Diagnosis Main Code"),
                                                                             new XElement("th", "DiagnosisCode"),
                                                                              new XElement("th", "Description"),
                                                                               new XElement("th", "Onset Date")
                                                                                      // new XElement("th", "ICD Type")
                                                                                      )
                                                                          ),
                                                                 new XElement("tbody",  GetXMLArrayDiagnosisCodes(diagnosisCodes)
                                                                                 //new XElement("tr", from s in diagnosisCodes select new XElement("td", ""),
                                                                                 //                   from s in diagnosisCodes select new XElement("td", ""),
                                                                                 //                    from s in diagnosisCodes select new XElement("td", ""),
                                                                                 //                     from s in diagnosisCodes select new XElement("td", "")
                                                                                 //new XElement("tr",
                                                                                 //new XElement("td", ""),
                                                                                 //new XElement("td", ""),
                                                                                 //new XElement("td", ""),
                                                                                 //new XElement("td", ""),
                                                                                 //new XElement("td", " "),
                                                                                 //new XElement("td", " ")
                                                                                
                                                                                 //new XElement("td", "Priority"),
                                                                                 //new XElement("td", "Diagnosis Main Code"),
                                                                                 //new XElement("td", "DiagnosisCode"),
                                                                                 //new XElement("td", "Description"),
                                                                                 //new XElement("td", "Onset Date"),
                                                                                 //new XElement("td", "ICD Type")
                                                                                 //)
                                                                              )

                                                               )


                                                          )
                                         )
               )
             , new XElement("component",
                          new XElement("section",
                        
                                           new XElement("templateId",
                                            new XAttribute("root", "2.16.840.1.113883.10.20.22.2.43")
                                            ),
                                            new XElement("id",
                                            new XAttribute("root", "1.2.276.0.76.4.17.9814184919"),
                                             new XAttribute("extension", "10c1eb7e-dc2d-4d1f-806a-2ad65eba0396")
                                            ),
                                           new XElement("Code",
                                            new XAttribute("code", "86631-9"),
                                             new XAttribute("displayName", "Pain"),
                                              new XAttribute("codeSystem", "2.16.840.1.113883.6.1"),
                                               new XAttribute("codeSystemName", "LOINC")
                                            ),
                                            new XElement("title", "Pain"),
                                            new XElement("text",
                                                new XElement("table",
                                                            new XAttribute("border", "1"),
                                                            new XAttribute("width", "100%"),
                                                             new XElement("thead",
                                                                       new XElement("tr",
                                                                        new XElement("th", "Pain"),
                                                                         new XElement("th", "At Rest"),
                                                                          new XElement("th", "With Activity")
                                                                                   // new XElement("th", "Exacerbating Factors"),
                                                                                   //  new XElement("th", "Relieving Factors ")

                                                                                   )
                                                                       ),
                                                              new XElement("tbody", GetXMLArrayDocPain(docpain)
                                                             //  new XElement("tr",
                                                             //new XElement("td", ""),
                                                             //                 new XElement("td", ""),
                                                             //                 new XElement("td", "")
                                                                              

                                                             //                 //new XElement("td", "Arm"),
                                                             //                 //new XElement("td", "2"),
                                                             //                 //new XElement("td", "7"),
                                                             //                 //new XElement("td", "Carry"),
                                                             //                 // new XElement("td", "Abdominal drawing")

                                                             //                 )
                                                                          )

                                                            )


                                                       )
                                      )
            )//Pain end 
             // < !--Functional Characteristics - Reminder-- >
              , new XElement("component",
                            new XElement("section",

                                             new XElement("templateId",
                                              new XAttribute("root", "2.16.840.1.113883.10.20.22.2.8")
                                              ),

                                             new XElement("Code",
                                              new XAttribute("code", "51848-0"),
                                               new XAttribute("displayName", "Reminder"),
                                                new XAttribute("codeSystem", "2.16.840.1.113883.6.1"),
                                                 new XAttribute("codeSystemName", "LOINC")
                                              ),
                                              new XElement("title", "Reminder"),
                                              new XElement("text",
                                                  new XElement("table",
                                                              new XAttribute("border", "1"),
                                                              new XAttribute("width", "100%"),
                                                               new XElement("thead",
                                                                         new XElement("tr",
                                                                          new XElement("th", "Date of Service"),
                                                                           new XElement("th", "Reminder")

                                                                                     )
                                                                         ),
                                                                new XElement("tbody",

                                                                 new XElement("tr",
                                                                                // from s in hints select new XElement("td", s.)
                                                                                new XElement("td", dateOfService.DateOfService),
                                                                                new XElement("td", hints.FuncCharRem)

                                                                                )
                                                                             )

                                                              )


                                                         )
                                        )
              )

               //====  Functional Characteristics - Hints  
               , new XElement("component",
                             new XElement("section",
                                new XElement("templateId",
                                              new XAttribute("root", "2.16.840.1.113883.10.20.22.2.8")
                                              ),
                                new XElement("Code",
                                               new XAttribute("code", "51848-0"),
                                                new XAttribute("displayName", "Hints"),
                                                 new XAttribute("codeSystem", "2.16.840.1.113883.6.1"),
                                                  new XAttribute("codeSystemName", "LOINC")
                                               )
                                         ),
                              new XElement("title", "Hints"),
                              new XElement("text",
                              new XElement("paragraph", hints.FuncCharacHint)
                            

                              )


               )//end
                // <!--        Long term goals          -->
               , new XElement("component",
                             new XElement("section",

                                              new XElement("templateId",
                                               new XAttribute("root", "2.16.840.1.113883.10.20.22.2.60")
                                               ),

                                              new XElement("Code",
                                               new XAttribute("code", "61146-7"),
                                                new XAttribute("displayName", "Goals"),
                                                 new XAttribute("codeSystem", "2.16.840.1.113883.6.1"),
                                                  new XAttribute("codeSystemName", "LOINC")
                                               ),
                                               new XElement("title", "Goals Section"),
                                               new XElement("text",
                                                   new XElement("table",
                                                               new XAttribute("border", "1"),
                                                               new XAttribute("width", "100%"),
                                                                new XElement("thead",
                                                                          new XElement("tr",
                                                                           new XElement("th", " Long Term Goals"),
                                                                            new XElement("th", "Not Met"),
                                                                             new XElement("th", "Partial Met"),
                                                                              new XElement("th", "Met")


                                                                                      )
                                                                          ),
                                                                 new XElement("tbody", GetXMLArrayGoal(MSGoal)
                                                                  //new XElement("tr",
                                                                  //             //new XElement("td", "Ambulation/Standing"),
                                                                  //             //new XElement("td", ""),
                                                                  //             //new XElement("td", ""),
                                                                  //             //new XElement("td", ""),
                                                                  //             //new XElement("td", "Y")

                                                                  //               )
                                                                              )

                                                               )


                                                          )
                                         )
               )//long goal end
                //<!--     Planned Interventions     -->
               , new XElement("component",
                             new XElement("section",

                                              new XElement("templateId",
                                               new XAttribute("root", "2.16.840.1.113883.10.20.21.2.3"),
                                                new XAttribute("extension", "2015-08-01")
                                               ),

                                              new XElement("Code",
                                               new XAttribute("code", "62387-6"),
                                                new XAttribute("displayName", "Interventions Provided"),
                                                 new XAttribute("codeSystem", "2.16.840.1.113883.6.1"),
                                                  new XAttribute("codeSystemName", "LOINC")
                                               ),
                                               new XElement("title", "Interventions Section"),
                                               new XElement("text",
                                                   new XElement("table",
                                                               new XAttribute("border", "1"),
                                                               new XAttribute("width", "100%"),
                                                                new XElement("thead",
                                                                          new XElement("tr",
                                                                           new XElement("th", "Planned Interventions")


                                                                                      )
                                                                          ),
                                                                 new XElement("tbody", GetXMLArrayPlannedIntervension(proIntervention)
                                                                              //new XElement("tr",
                                                                              //             new XElement("td", "Arm")
                                                                              //               )
                                                                              )

                                                               )


                                                          )
                                         )
               )//plan untervension end
                //===================<!--         Referrer               -->
               , new XElement("component",
                             new XElement("section",

                                              new XElement("templateId",
                                               new XAttribute("root", "2.16.840.1.113883.10.20.22.2.8")
                                               ),

                                              new XElement("Code",
                                               new XAttribute("code", "51848-0"),
                                                new XAttribute("displayName", "Referrer"),
                                                 new XAttribute("codeSystem", "2.16.840.1.113883.6.1"),
                                                  new XAttribute("codeSystemName", "LOINC")
                                               ),
                                               new XElement("title", "Referrer"),
                                               new XElement("text",
                                                   new XElement("table",
                                                               new XAttribute("border", "1"),
                                                               new XAttribute("width", "100%"),
                                                                new XElement("thead",
                                                                          new XElement("tr",

                                                                               new XElement("th", "Referrer ")

                                                                                      )
                                                                          ),
                                                                 new XElement("tbody",GetXMLArrayReferrer(Referrer)
                                                                  //new XElement("tr",

                                                                  //             new XElement("td", "Milia, Marc MD")

                                                                  //               )
                                                                              )

                                                               )


                                                          )
                                         )
               )//referrer end
                //<!--          Extremity test                -->
   , new XElement("component",
                             new XElement("section",

                                              new XElement("templateId",
                                               new XAttribute("root", "2.16.840.1.113883.10.20.22.2.8")
                                               ),

                                              new XElement("Code",
                                               new XAttribute("code", "54755-4"),
                                                new XAttribute("displayName", "Extremity test"),
                                                 new XAttribute("codeSystem", "2.16.840.1.113883.6.1"),
                                                  new XAttribute("codeSystemName", "LOINC")
                                               ),
                                               new XElement("title", "Extremity test"),
                                               new XElement("text",
                                                   new XElement("table",
                                                               new XAttribute("border", "1"),
                                                               new XAttribute("width", "100%"),
                                                                new XElement("thead",
                                                                          new XElement("tr",

                                                                              new XElement("th", "Description"),
                                                                               new XElement("th", "Type")

                                                                                      )
                                                                          ),
                                                                 new XElement("tbody",
                                                                  new XElement("tr",

                                                                               new XElement("td", "Ankle and Foot"),
                                                                               new XElement("td", "Positive")

                                                                                 )
                                                                              )

                                                               )


                                                          )
                                         )
               )//extrimity test end
                //< !--OMTP Test-- >
    , new XElement("component",
                             new XElement("section",

                                              new XElement("templateId",
                                               new XAttribute("root", "2.16.840.1.113883.10.20.22.2.8")
                                               ),

                                              new XElement("Code",
                                               new XAttribute("code", "51848-0"),
                                                new XAttribute("displayName", "OMTP Test"),
                                                 new XAttribute("codeSystem", "2.16.840.1.113883.6.1"),
                                                  new XAttribute("codeSystemName", "LOINC")
                                               ),
                                               new XElement("title", "OMTP Test"),
                                               new XElement("text",
                                                   new XElement("table",
                                                               new XAttribute("border", "1"),
                                                               new XAttribute("width", "100%"),
                                                                new XElement("thead",
                                                                          new XElement("tr",
                                                                           new XElement("th", "Decription"),
                                                                            new XElement("th", "Produced / Abolished / Not Affected"),
                                                                             new XElement("th", "Increased / Decreased"),
                                                                              new XElement("th", "Worse / Better "),
                                                                               new XElement("th", "Centralized / Perip")

                                                                                      )
                                                                          ),
                                                                 new XElement("tbody",GetXMLArrayOMPT(oMPTTest)
                                                                  //new XElement("tr",
                                                                  //             new XElement("td", "Arm"),
                                                                  //             new XElement("td", "2"),
                                                                  //             new XElement("td", "7"),
                                                                  //             new XElement("td", "Carry"),
                                                                  //             new XElement("td", "Abdominal drawing")

                                                                  //               )
                                                                              )

                                                               )


                                                          )
                                         )
               )//ompt test end
    , new XElement("component",
                             new XElement("section",

                                              new XElement("templateId",
                                               new XAttribute("root", "2.16.840.1.113883.10.20.22.2.8")
                                               ),

                                              new XElement("Code",
                                               new XAttribute("code", "54755-4"),
                                                new XAttribute("displayName", "Spinal test"),
                                                 new XAttribute("codeSystem", "2.16.840.1.113883.6.1"),
                                                  new XAttribute("codeSystemName", "LOINC")
                                               ),
                                               new XElement("title", "Spinal test"),
                                               new XElement("text",
                                                   new XElement("table",
                                                               new XAttribute("border", "1"),
                                                               new XAttribute("width", "100%"),
                                                                new XElement("thead",
                                                                          new XElement("tr",

                                                                              new XElement("th", "Description"),
                                                                               new XElement("th", "Type")

                                                                                      )
                                                                          ),
                                                                 new XElement("tbody", GetXMLArraySpinalTest(spinalTest)
                                                               
                                                                              )

                                                               )


                                                          )
                                         )
               )//spinal test e nd
                //< !--Spinal Assessment-- >
                //, new XElement("component",
                //                      new XElement("section",

               //                                       new XElement("templateId",
               //                                        new XAttribute("root", "2.16.840.1.113883.10.20.22.2.8")
               //                                        ),

               //                                       new XElement("Code",
               //                                        new XAttribute("code", "51848-0"),
               //                                         new XAttribute("displayName", "SPINAL ASSESSMENT"),
               //                                          new XAttribute("codeSystem", "2.16.840.1.113883.6.1"),
               //                                           new XAttribute("codeSystemName", "LOINC")
               //                                        ),
               //                                        new XElement("title", "SPINAL ASSESSMENT"),
               //                                        new XElement("text",
               //                                           new XElement("paragraph", "he domestic dog (Canis lupus familiaris")

               //                                                   )
               //                                  )
               //        )//Spinal Assessment end
               //< !--OMTP Assessment  -- >
               //, new XElement("component",
               //              new XElement("section",

               //                               new XElement("templateId",
               //                                new XAttribute("root", "2.16.840.1.113883.10.20.22.2.8")
               //                                ),

               //                               new XElement("Code",
               //                                new XAttribute("code", "51848-0"),
               //                                 new XAttribute("displayName", "SPINAL ASSESSMENT"),
               //                                  new XAttribute("codeSystem", "2.16.840.1.113883.6.1"),
               //                                   new XAttribute("codeSystemName", "LOINC")
               //                                ),
               //                                new XElement("title", "SPINAL ASSESSMENT"),
               //                                new XElement("text",
               //                                   new XElement("paragraph", "he domestic dog (Canis lupus familiaris")

               //                                           )
               //                          )
               //)
               //<!--         Current              -->
               , new XElement("component",
                             new XElement("section",

                                              new XElement("templateId",
                                               new XAttribute("root", "2.16.840.1.113883.10.20.22.2.4.1")
                                               ),

                                              new XElement("Code",
                                               new XAttribute("code", "8716-3"),
                                                new XAttribute("displayName", "Vital Signs"),
                                                 new XAttribute("codeSystem", "2.16.840.1.113883.6.1"),
                                                  new XAttribute("codeSystemName", "LOINC")
                                               ),
                                               new XElement("title", "Vital Signs (Last Filed)"),
                                               new XElement("text",
                                                   new XElement("table",
                                                               new XAttribute("border", "1"),
                                                               new XAttribute("width", "100%"),
                                                                new XElement("thead",
                                                                          new XElement("tr",
                                                                           new XElement("th", "L"),
                                                                            new XElement("th", "R"),
                                                                             new XElement("th", "L"),
                                                                              new XElement("th", "R"),
                                                                               new XElement("th", "L"),
                                                                               new XElement("th", "R")

                                                                                      )
                                                                          ),
                                                                 new XElement("tbody",
                                                                 new XAttribute("id", "Shoulder"),
                                                                 GetXMLArrayShoulder(shoulderCM)

                                                               )//--
                                                                ,new XElement("tbody",
                                                                 new XAttribute("id", "Elbow"),
                                                                 GetXMLArrayShoulder(elbowCM)

                                                               )//--
                                                                , new XElement("tbody",
                                                                 new XAttribute("id", "Forearm"),
                                                                 GetXMLArrayShoulder(ForearmCM)

                                                               )//--
                                                                  , new XElement("tbody",
                                                                 new XAttribute("id", "Wrist"),
                                                                 GetXMLArrayShoulder(WristCM)

                                                               )//--
                                                                   , new XElement("tbody",
                                                                 new XAttribute("id", "Hip"),
                                                                 GetXMLArrayShoulder(HipCM)

                                                               )//--
                                                                   , new XElement("tbody",
                                                                 new XAttribute("id", "Knee"),
                                                                 GetXMLArrayShoulder(KneeCM)

                                                               )//--
                                                                    , new XElement("tbody",
                                                                 new XAttribute("id", "Ankle"),
                                                                 GetXMLArrayShoulder(AnkleCM)

                                                               )//--
                                                                       , new XElement("tbody",
                                                                 new XAttribute("id", "Cervical"),
                                                                 GetXMLArrayShoulder(CervicalCM)

                                                               )//--
                                                                         , new XElement("tbody",
                                                                 new XAttribute("id", "Lumbar"),
                                                                 GetXMLArrayShoulder(LumbarCM)

                                                               )//--
                                                                          , new XElement("tbody",
                                                                 new XAttribute("id", "Other"),
                                                                OtherMeasurementOtherCM

                                                               )//--
                                                          )
                                                   )
                                         )
               )//current measurment end
                // < !--Previous Measurement-- >
                , new XElement("component",
                             new XElement("section",

                                              new XElement("templateId",
                                               new XAttribute("root", "2.16.840.1.113883.10.20.22.2.4.1"),
                                                 new XAttribute("extension", "2015-08-01")
                                               ),

                                              new XElement("Code",
                                               new XAttribute("code", "8716-3"),
                                                new XAttribute("displayName", "Vital Signs"),
                                                 new XAttribute("codeSystem", "2.16.840.1.113883.6.1"),
                                                  new XAttribute("codeSystemName", "LOINC")
                                               ),
                                               new XElement("title", "Vital Signs (Previous Measurement)"),
                                               new XElement("text",
                                                   new XElement("table",
                                                               new XAttribute("border", "1"),
                                                               new XAttribute("width", "100%"),
                                                                new XElement("thead",
                                                                          new XElement("tr",
                                                                           new XElement("th", "L"),
                                                                            new XElement("th", "R"),
                                                                             new XElement("th", "L"),
                                                                              new XElement("th", "R"),
                                                                               new XElement("th", "L"),
                                                                               new XElement("th", "R")

                                                                                      )
                                                                          ),

                                                                 new XElement("tbody",
                                                                 new XAttribute("id", "Shoulder-Goal"),
                                                                 GetXMLArrayShoulder(shoulderGoal)

                                                               )//--
                                                                 , new XElement("tbody",
                                                                 new XAttribute("id", "Shoulder-Last Measurement"),
                                                                 GetXMLArrayShoulder(shoulderLM)

                                                               )//--
                                                                  , new XElement("tbody",
                                                                 new XAttribute("id", "Shoulder-Initial Measurement"),
                                                                 GetXMLArrayShoulder(shoulderIE)

                                                               )//--
                                                                   , new XElement("tbody",
                                                                 new XAttribute("id", "Elbow-Goal"),
                                                                 GetXMLArrayShoulder(elbowGoal)

                                                               )//--
                                                                    , new XElement("tbody",
                                                                 new XAttribute("id", "Elbow-Last Measurement"),
                                                                 GetXMLArrayShoulder(elbowLM)

                                                               )//--
                                                                     , new XElement("tbody",
                                                                 new XAttribute("id", "Elbow-Initial Measurement"),
                                                                 GetXMLArrayShoulder(elbowIE)

                                                               )//--
                                                                        , new XElement("tbody",
                                                                 new XAttribute("id", "Forearm-Goal"),
                                                                 GetXMLArrayShoulder(ForearmGoal)

                                                               )//--
                                                                           , new XElement("tbody",
                                                                 new XAttribute("id", "Forearm-Last Measurement"),
                                                                 GetXMLArrayShoulder(ForearmLM)

                                                               )//--
                                                                            , new XElement("tbody",
                                                                 new XAttribute("id", "Forearm-Initial Measurement"),
                                                                 GetXMLArrayShoulder(ForearmIE)

                                                               )//--
                                                                             , new XElement("tbody",
                                                                 new XAttribute("id", "Wrist-Goal"),
                                                                 GetXMLArrayShoulder(WristGoal)

                                                               )//--
                                                                             , new XElement("tbody",
                                                                 new XAttribute("id", "Wrist-Last Measurement"),
                                                                 GetXMLArrayShoulder(WristLM)

                                                               )//--
                                                                              , new XElement("tbody",
                                                                 new XAttribute("id", "Wrist-Initial Measurement"),
                                                                 GetXMLArrayShoulder(WristIE)

                                                               )//--
                                                                              , new XElement("tbody",
                                                                 new XAttribute("id", "Hip-Goal"),
                                                                 GetXMLArrayShoulder(HipGoal)

                                                               )//--
                                                                                , new XElement("tbody",
                                                                 new XAttribute("id", "Hip-Last Measurement"),
                                                                 GetXMLArrayShoulder(HipLM)

                                                               )//--
                                                                                  , new XElement("tbody",
                                                                 new XAttribute("id", "Hip-Initial Measurement"),
                                                                 GetXMLArrayShoulder(HipIE)

                                                               )//--
                                                                                   , new XElement("tbody",
                                                                 new XAttribute("id", "Knee-Goal"),
                                                                 GetXMLArrayShoulder(KneeGoal)

                                                               )//--
                                                                                   , new XElement("tbody",
                                                                 new XAttribute("id", "Knee-Last Measurement"),
                                                                 GetXMLArrayShoulder(KneeLM)

                                                               )//--
                                                                                   , new XElement("tbody",
                                                                 new XAttribute("id", "Knee-Initial Measurement"),
                                                                 GetXMLArrayShoulder(KneeIE)

                                                               )//--
                                                                                   , new XElement("tbody",
                                                                 new XAttribute("id", "Ankle-Goal"),
                                                                 GetXMLArrayShoulder(AnkleGoal)

                                                               )//--
                                                                                   , new XElement("tbody",
                                                                 new XAttribute("id", "Ankle-Last Measurement"),
                                                                 GetXMLArrayShoulder(AnkleLM)

                                                               )//--
                                                                                       , new XElement("tbody",
                                                                 new XAttribute("id", "Ankle-Initial Measurement"),
                                                                 GetXMLArrayShoulder(AnkleIE)

                                                               )//--
                                                                                          , new XElement("tbody",
                                                                 new XAttribute("id", "Cervical-Goal"),
                                                                 GetXMLArrayShoulder(CervicalGoal)

                                                               )//--
                                                                                          , new XElement("tbody",
                                                                 new XAttribute("id", "Cervical-Last Measurement"),
                                                                 GetXMLArrayShoulder(CervicalLM)

                                                               )//--
                                                                                           , new XElement("tbody",
                                                                 new XAttribute("id", "Cervical-Initial Measurement"),
                                                                 GetXMLArrayShoulder(CervicalIE)

                                                               )//--
                                                                                                   , new XElement("tbody",
                                                                 new XAttribute("id", "Lumbar-Goal"),
                                                                 GetXMLArrayShoulder(LumbarGoal)

                                                               )//--
                                                                                                    , new XElement("tbody",
                                                                 new XAttribute("id", "Lumbar-Last Measurement"),
                                                                 GetXMLArrayShoulder(LumbarLM)

                                                               )//--
                                                                                                      , new XElement("tbody",
                                                                 new XAttribute("id", "Lumbar-Initial Measurement"),
                                                                 GetXMLArrayShoulder(LumbarIE)

                                                               )//--

                                                                                                      , new XElement("tbody",
                                                                 new XAttribute("id", "Other-Goal"),
                                                               OtherMeasurementOtherRGoal

                                                               )//--

                                                                                                      , new XElement("tbody",
                                                                 new XAttribute("id", "Other-Last Measurement"),
                                                                OtherMeasurementOtherLM

                                                               )//--

                                                                                                      , new XElement("tbody",
                                                                 new XAttribute("id", "Other-Initial Measurement"),
                                                               OtherMeasurementOtherIE

                                                               )//--
                                                      )
                                                  )
                                         )
               )
           )//Structure body end
          )


                     )//close ClinicalDocument
                    );
            #endregion

            CCDA_XML = bandsDocument.Declaration + Environment.NewLine + bandsDocument.ToString();

            return CCDA_XML;
        }
        public List<IniEvalDiagnosisCodesEntity> GetDiagnosisCodes(int patientId, int NoteId, string NoteType)
        {
            List<IniEvalDiagnosisCodesEntity> diagnosisCodes = new List<IniEvalDiagnosisCodesEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    diagnosisCodes = context.Database.SqlQuery<IniEvalDiagnosisCodesEntity>("SP_GetPatDiagnosis @Patid ",
                                   new SqlParameter("Patid", SqlDbType.BigInt) { Value = patientId }


                  ).ToList();

                
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return diagnosisCodes;
        }
       
        public List<IniEvalDocPainEntity> GetDocPain(int patientId, int NoteId, string NoteType)
        {
            List<IniEvalDocPainEntity> docPain = new List<IniEvalDocPainEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    docPain = context.Database.SqlQuery<IniEvalDocPainEntity>("SP_GetDocPain @Type,@Docrowid,@patientid ",
                  new SqlParameter("Type", SqlDbType.VarChar) { Value = NoteType },
                   new SqlParameter("Docrowid", SqlDbType.BigInt) { Value = NoteId },
                    new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId }
                  ).ToList();

                    ////===============Mapper===========================================
                    //var config = new MapperConfiguration(cfg =>
                    //{
                    //    cfg.CreateMap<SP_GetDocFuncCharac_Result, MSFunctionalCharEntity>();
                    //});

                    //IMapper mapper = config.CreateMapper();
                    //MSFunctionalCharEntity = mapper.Map<List<SP_GetDocFuncCharac_Result>, List<MSFunctionalCharEntity>>(functChar);
                    ////===============mapper end==========================================
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return docPain;
        }

        public List<IniEvalDocPainFactorEntity> GetDocPainFactor(int Painrowid, string FactorType)
        {
            List<IniEvalDocPainFactorEntity> docPainFactors = new List<IniEvalDocPainFactorEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    docPainFactors = context.Database.SqlQuery<IniEvalDocPainFactorEntity>("SP_GetDocPainFactor @Painrowid,@FactorType ",
                  new SqlParameter("Painrowid", SqlDbType.BigInt) { Value = Painrowid },
                   new SqlParameter("FactorType", SqlDbType.VarChar) { Value = FactorType }

                  ).ToList();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return docPainFactors;
        }
        public List<IniEvalReferrerEntity> GetReferrer(int Painrowid, int Docrowid)
        {
            List<IniEvalReferrerEntity> docPainFactors = new List<IniEvalReferrerEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    docPainFactors = context.Database.SqlQuery<IniEvalReferrerEntity>("SP_GetPatReferral @Painrowid,@Docrowid ",
                  new SqlParameter("Painrowid", SqlDbType.BigInt) { Value = Painrowid },
                   new SqlParameter("Docrowid", SqlDbType.BigInt) { Value = Docrowid }

                  ).ToList();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return docPainFactors;
        }

        public List<IniEvalExtremityEntity> GetExtremityTests(int patientId, int NoteId, string NoteType)
        {
            List<IniEvalExtremityEntity> docPain = new List<IniEvalExtremityEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    docPain = context.Database.SqlQuery<IniEvalExtremityEntity>("SP_GetDocExtremityTests @Type,@Docrowid,@patientid ",
                  new SqlParameter("Type", SqlDbType.VarChar) { Value = NoteType },
                   new SqlParameter("Docrowid", SqlDbType.BigInt) { Value = NoteId },
                    new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId }
                  ).ToList();

                    ////===============Mapper===========================================
                    //var config = new MapperConfiguration(cfg =>
                    //{
                    //    cfg.CreateMap<SP_GetDocFuncCharac_Result, MSFunctionalCharEntity>();
                    //});

                    //IMapper mapper = config.CreateMapper();
                    //MSFunctionalCharEntity = mapper.Map<List<SP_GetDocFuncCharac_Result>, List<MSFunctionalCharEntity>>(functChar);
                    ////===============mapper end==========================================
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return docPain;
        }

        public List<IniEvalSpinalTestEntity> GetSpinalTests(int patientId, int NoteId, string NoteType)
        {
            List<IniEvalSpinalTestEntity> spinalTest = new List<IniEvalSpinalTestEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    spinalTest = context.Database.SqlQuery<IniEvalSpinalTestEntity>("SP_GetDocSpinalTests @Type,@Docrowid,@patientid ",
                  new SqlParameter("Type", SqlDbType.VarChar) { Value = NoteType },
                   new SqlParameter("Docrowid", SqlDbType.BigInt) { Value = NoteId },
                    new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId }
                  ).ToList();

                    ////===============Mapper===========================================
                    //var config = new MapperConfiguration(cfg =>
                    //{
                    //    cfg.CreateMap<SP_GetDocFuncCharac_Result, MSFunctionalCharEntity>();
                    //});

                    //IMapper mapper = config.CreateMapper();
                    //MSFunctionalCharEntity = mapper.Map<List<SP_GetDocFuncCharac_Result>, List<MSFunctionalCharEntity>>(functChar);
                    ////===============mapper end==========================================
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return spinalTest;
        }

        public List<IniEvalOMPTEntity> GetOMPT(int patientId, int NoteId, string NoteType)
        {
            List<IniEvalOMPTEntity> oMPTTest = new List<IniEvalOMPTEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    oMPTTest = context.Database.SqlQuery<IniEvalOMPTEntity>("SP_GetDocOMPTTests @Type,@Docrowid,@patientid ",
                  new SqlParameter("Type", SqlDbType.VarChar) { Value = NoteType },
                   new SqlParameter("Docrowid", SqlDbType.BigInt) { Value = NoteId },
                    new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId }
                  ).ToList();

                    ////===============Mapper===========================================
                    //var config = new MapperConfiguration(cfg =>
                    //{
                    //    cfg.CreateMap<SP_GetDocFuncCharac_Result, MSFunctionalCharEntity>();
                    //});

                    //IMapper mapper = config.CreateMapper();
                    //MSFunctionalCharEntity = mapper.Map<List<SP_GetDocFuncCharac_Result>, List<MSFunctionalCharEntity>>(functChar);
                    ////===============mapper end==========================================
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return oMPTTest;
        }

        public List<IniEvalInterventionEntity> GetActiveDocFCECPTCodes(int patientId, string SPName)
        {
            List<IniEvalInterventionEntity> ActiveDocFCECPTCode = new List<IniEvalInterventionEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    ActiveDocFCECPTCode = context.Database.SqlQuery<IniEvalInterventionEntity>(SPName + " @patientid ",

                    new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId }
                  ).ToList();

                    ////===============Mapper===========================================
                    //var config = new MapperConfiguration(cfg =>
                    //{
                    //    cfg.CreateMap<SP_GetDocFuncCharac_Result, MSFunctionalCharEntity>();
                    //});

                    //IMapper mapper = config.CreateMapper();
                    //MSFunctionalCharEntity = mapper.Map<List<SP_GetDocFuncCharac_Result>, List<MSFunctionalCharEntity>>(functChar);
                    ////===============mapper end==========================================
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ActiveDocFCECPTCode;
        }

        public List<IniEvalProInterventionEntity> GetActiveDocProInterventions(int patientId, int NoteId)
        {
            List<IniEvalProInterventionEntity> proIntervension = new List<IniEvalProInterventionEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    proIntervension = context.Database.SqlQuery<IniEvalProInterventionEntity>("SP_GetActiveDocProInterventions @Docrowid,@patientid ",

                   new SqlParameter("Docrowid", SqlDbType.BigInt) { Value = NoteId },
                    new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId }
                  ).ToList();

                    ////===============Mapper===========================================
                    //var config = new MapperConfiguration(cfg =>
                    //{
                    //    cfg.CreateMap<SP_GetDocFuncCharac_Result, MSFunctionalCharEntity>();
                    //});

                    //IMapper mapper = config.CreateMapper();
                    //MSFunctionalCharEntity = mapper.Map<List<SP_GetDocFuncCharac_Result>, List<MSFunctionalCharEntity>>(functChar);
                    ////===============mapper end==========================================
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return proIntervension;
        }

        public IniEvalSummaryEntity GetDocNoteSummary(int NoteId)
        {
            // List<IniEvalSummaryEntity> summary = new List<IniEvalSummaryEntity>();
            IniEvalSummaryEntity summary = new IniEvalSummaryEntity();
            try
            {
                using (var context = new RehabEntities())
                {
                    summary = context.Database.SqlQuery<IniEvalSummaryEntity>("SP_GetDocNoteSummary @Docrowid",

                   new SqlParameter("Docrowid", SqlDbType.BigInt) { Value = NoteId }

                  ).ToList().FirstOrDefault();



                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return summary;
        }

        public JArray DiagnosisArray(List<IniEvalDiagnosisCodesEntity> Diagnosis)
        {
            JObject jsonObject = null;

            JArray jsonArray = new JArray();
            try
            {
                foreach (IniEvalDiagnosisCodesEntity obj in Diagnosis)
                {

                    jsonObject = new JObject(new JProperty("td",
                                   new JObject(new JProperty(Convert.ToString(obj.Priority),
                                                       obj.OnsetDate,
                                                       obj.DiagnosisCode,
                                                      obj.Description

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

        public JArray PainArray(List<IniEvalDocPainEntity> Pains)
        {
            JObject jsonObject = null;
            List<IniEvalDocPainFactorEntity> EpainFactors = new List<IniEvalDocPainFactorEntity>();
            List<IniEvalDocPainFactorEntity> RpainFactors = new List<IniEvalDocPainFactorEntity>();
            JArray jsonArray = new JArray();
            JArray painFactorEArray = new JArray();
            JArray painFactorRArray = new JArray();
            try
            {
                foreach (IniEvalDocPainEntity obj in Pains)
                {
                    EpainFactors = GetDocPainFactor(obj.Painrowid, "E");
                    foreach (IniEvalDocPainFactorEntity subObj in EpainFactors)
                    {
                        painFactorEArray.Add(subObj.Description);
                    }
                    RpainFactors = GetDocPainFactor(obj.Painrowid, "E");
                    foreach (IniEvalDocPainFactorEntity subObj in RpainFactors)
                    {
                        painFactorRArray.Add(subObj.Description);
                    }


                    jsonObject = new JObject(new JProperty("td",
                                   new JObject(new JProperty(obj.PainSite,
                                   obj.PainscaleAR,
                                    obj.PainScaleWA,
                                    painFactorEArray,
                                    painFactorRArray
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

        public JArray IntervensionArray(List<IniEvalProInterventionEntity> Intervensions)
        {
            JObject jsonObject = null;

            JArray jsonArray = new JArray();
            try
            {
                foreach (IniEvalProInterventionEntity obj in Intervensions)
                {

                    jsonObject = new JObject(new JProperty("td",
                                   new JObject(new JProperty(
                                                       obj.CPTCode,
                                                       obj.CPTDescription

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
        public JArray ExtremityArray(List<IniEvalExtremityEntity> extremityTests)
        {
            JObject jsonObject = null;

            JArray jsonArray = new JArray();
            try
            {
                foreach (IniEvalExtremityEntity obj in extremityTests)
                {

                    jsonObject = new JObject(new JProperty("td",
                                   new JObject(new JProperty(
                                                       obj.Description,
                                                       obj.TestPN

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

        public JArray SpinalTestArray(List<IniEvalSpinalTestEntity> spinalTest)
        {
            JObject jsonObject = null;

            JArray jsonArray = new JArray();
            try
            {
                foreach (IniEvalSpinalTestEntity obj in spinalTest)
                {

                    jsonObject = new JObject(new JProperty("td",
                                   new JObject(new JProperty(
                                                       obj.Description,
                                                       obj.TestPN

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

        public JArray OMPTArray(List<IniEvalOMPTEntity> oMPTTest)
        {
            JObject jsonObject = null;

            JArray jsonArray = new JArray();
            try
            {
                foreach (IniEvalOMPTEntity obj in oMPTTest)
                {

                    jsonObject = new JObject(new JProperty("td",
                                   new JObject(new JProperty(
                                                       obj.Description,
                                                        obj.TestPAN,
                                                       obj.TestID,
                                                        obj.TestWB,
                                                            obj.TestCP

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

        public JArray ReferrerArray(List<IniEvalReferrerEntity> referrar)
        {
            JObject jsonObject = null;

            JArray jsonArray = new JArray();
            try
            {
                foreach (IniEvalReferrerEntity obj in referrar)
                {

                    jsonObject = new JObject(new JProperty("td",
                                   new JObject(new JProperty(
                                                       obj.PrintName


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
        
        private IEnumerable<XElement> GetXMLArrayDiagnosisCodes(List<IniEvalDiagnosisCodesEntity> DiagnosisCodes)
        {
            for (int i = 0; i < DiagnosisCodes.Count; i++)
            {
                yield return new XElement("tr",
                    new XElement("td", DiagnosisCodes[i].Priority),
                     new XElement("td", DiagnosisCodes[i].DiagnosisCode),
                       new XElement("td", DiagnosisCodes[i].Description),
                        new XElement("td", DiagnosisCodes[i].OnsetDate)



                );
            };
        }

        private IEnumerable<XElement> GetXMLArrayDocPain(List<IniEvalDocPainEntity> DocPain)
        {
            for (int i = 0; i < DocPain.Count; i++)
            {
                yield return new XElement("tr",
                     new XElement("td", DocPain[i].PainSite),
                    new XElement("td", DocPain[i].PainscaleAR),
                     new XElement("td", DocPain[i].PainScaleWA)
                       


                );
            };
        }

       
             private IEnumerable<XElement> GetXMLArrayGoal(List<MSGoalsEntity> Goal)
        {
            for (int i = 0; i < Goal.Count; i++)
            {
                yield return new XElement("tr",
                     new XElement("td", Goal[i].Description),
                    new XElement("td", Goal[i].GoalNMet),
                     new XElement("td", Goal[i]. GoalPMet),
                      new XElement("td", Goal[i].GoalMetM)



                );
            };
        }
        private IEnumerable<XElement> GetXMLArrayPlannedIntervension(List<IniEvalProInterventionEntity> obj)
        {
            for (int i = 0; i < obj.Count; i++)
            {
                yield return new XElement("tr",
                     new XElement("td", obj[i].CPTDescription)
                   



                );
            };
        }
        private IEnumerable<XElement> GetXMLArrayReferrer(List<IniEvalReferrerEntity> obj)
        {
            for (int i = 0; i < obj.Count; i++)
            {
                yield return new XElement("tr",
                     new XElement("td", obj[i].PrintName)




                );
            };
        }
        private IEnumerable<XElement> GetXMLArrayExtremity(List<IniEvalExtremityEntity> obj)
        {
            for (int i = 0; i < obj.Count; i++)
            {
                yield return new XElement("tr",
                     new XElement("td", obj[i].Description),
                       new XElement("td", obj[i].TestPN)




                );
            };
        }
        private IEnumerable<XElement> GetXMLArrayOMPT(List<IniEvalOMPTEntity> obj)
        {
            for (int i = 0; i < obj.Count; i++)
            {
                yield return new XElement("tr",
                     new XElement("td", obj[i].Description),
                       new XElement("td", obj[i].TestPAN),
                        new XElement("td", obj[i].TestID),
                         new XElement("td", obj[i].TestWB),
                          new XElement("td", obj[i].TestCP)





                );
            };
        }
        private IEnumerable<XElement> GetXMLArraySpinalTest(List<IniEvalSpinalTestEntity> obj)
        {
            for (int i = 0; i < obj.Count; i++)
            {
                yield return new XElement("tr",
                     new XElement("td", obj[i].Description),
                       new XElement("td", obj[i].TestPN)
                        





                );
            };
        }
        private IEnumerable<XElement> GetXMLArrayShoulder(List<MSShoulderEntity> obj)
        {
           
            for (int i = 0; i < obj.Count; i++)
            {
                yield return new XElement("tr",
                     new XElement("th",  new XAttribute("scope", "row"), obj[i].MeasurementType) , 
                       new XElement("td", obj[i].SL),
                        new XElement("td", obj[i].SR),
                         new XElement("td", obj[i].AL),
                           new XElement("td", obj[i].AR),
                             new XElement("td", obj[i].PR)






                );
            };
        }
    
      
    }
   


}
