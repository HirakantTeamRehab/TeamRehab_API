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
    public class MedicalNecessity
    {


        public JObject ConvertMedicalNecessity(int patientId, int NoteId, string NoteType)
        {
            JObject rss = new JObject();
            string CCDAResponse = string.Empty;
            rss = MedicalNecessityToCCDA(patientId, NoteId, NoteType);

            //foreach (PatientEntity paient in patientEntity)
            //{
            //    rss = PatientlistToCCDA(paient);
            //}
            return rss;

        }
        public JObject MedicalNecessityToCCDA(int patientId, int NoteId, string NoteType)
        {

            MSDateOfServiceEntity dateOfService = GetMSDateOfService(patientId, NoteId, NoteType);
            List<MSFunctionalCharEntity> functChar = GetFunctChar(0, NoteId, NoteType);
            List<MSFunctionalCharEntity> functCharReminder = GetFunctChar(patientId, 0, NoteType);
            string strfunctCharReminder = string.Empty;
            string strfunctChar = string.Empty;
            int i = 1;
            if (functCharReminder.Count > 0)
            {if(i==1)
                { 
                strfunctChar = strfunctChar + functCharReminder.FirstOrDefault().funccharac;
            }
            foreach (MSFunctionalCharEntity obj in functCharReminder)
                {
                    
                    strfunctCharReminder = strfunctCharReminder + obj.funccharac;
                    strfunctCharReminder = strfunctCharReminder + Environment.NewLine;
                }
            }
            MSHints hints = GetHints(patientId, NoteId, NoteType);

            //Shoulder
            List<MSShoulderEntity> shoulderIE = GetMeasurements(patientId, NoteId, "Shoulder", "IE", NoteType);
            string OtherMeasurementShoulderIE = GetOthMeasurements(patientId, NoteId, "Shoulder", "IE", NoteType);
            List<MSShoulderEntity> shoulderGoal = GetMeasurements(patientId, NoteId, "Shoulder", "Goal", NoteType);
            string OtherMeasurementShoulderGoal = GetOthMeasurements(patientId, NoteId, "Shoulder", "Goal", NoteType);
            List<MSShoulderEntity> shoulderLM = GetMeasurements(patientId, NoteId, "Shoulder", "LM", NoteType);
            string OtherMeasurementShoulderLM = GetOthMeasurements(patientId, NoteId, "Shoulder", "LM", NoteType);
            List<MSShoulderEntity> shoulderCM = GetMeasurements(patientId, NoteId, "Shoulder", "CM", NoteType);
            string OtherMeasurementshoulderCM = GetOthMeasurements(patientId, NoteId, "Shoulder", "CM", NoteType);

            //Elbow
            List<MSShoulderEntity> elbowIE = GetMeasurements(patientId, NoteId, "Elbow", "IE", NoteType);
            string OtherMeasurementElbowIE = GetOthMeasurements(patientId, NoteId, "Elbow", "IE", NoteType);
            List<MSShoulderEntity> elbowGoal = GetMeasurements(patientId, NoteId, "Elbow", "Goal", NoteType);
            string OtherMeasurementElbowGoal = GetOthMeasurements(patientId, NoteId, "Elbow", "Goal", NoteType);
            List<MSShoulderEntity> elbowLM = GetMeasurements(patientId, NoteId, "Elbow", "LM", NoteType);
            string OtherMeasurementElbowLM = GetOthMeasurements(patientId, NoteId, "Elbow", "LM", NoteType);
            List<MSShoulderEntity> elbowCM = GetMeasurements(patientId, NoteId, "Elbow", "CM", NoteType);
            string OtherMeasurementElbowCM = GetOthMeasurements(patientId, NoteId, "Elbow", "CM", NoteType);

            //Forearm
            List<MSShoulderEntity> ForearmIE = GetMeasurements(patientId, NoteId, "Forearm", "IE", NoteType);
            string OtherMeasurementForearmIE = GetOthMeasurements(patientId, NoteId, "Forearm", "IE", NoteType);
            List<MSShoulderEntity> ForearmGoal = GetMeasurements(patientId, NoteId, "Forearm", "Goal", NoteType);
            string OtherMeasurementForearmGoal = GetOthMeasurements(patientId, NoteId, "Forearm", "Goal", NoteType);
            List<MSShoulderEntity> ForearmLM = GetMeasurements(patientId, NoteId, "Forearm", "LM", NoteType);
            string OtherMeasurementForearmLM = GetOthMeasurements(patientId, NoteId, "Forearm", "LM", NoteType);
            List<MSShoulderEntity> ForearmCM = GetMeasurements(patientId, NoteId, "Forearm", "CM", NoteType);
            string OtherMeasurementForearmCM = GetOthMeasurements(patientId, NoteId, "Forearm", "CM", NoteType);

            //Wrist
            List<MSShoulderEntity> WristIE = GetMeasurements(patientId, NoteId, "Wrist", "IE", NoteType);
            string OtherMeasurementWristIE = GetOthMeasurements(patientId, NoteId, "Wrist", "IE", NoteType);
            List<MSShoulderEntity> WristGoal = GetMeasurements(patientId, NoteId, "Wrist", "Goal", NoteType);
            string OtherMeasurementWristGoal = GetOthMeasurements(patientId, NoteId, "Wrist", "Goal", NoteType);
            List<MSShoulderEntity> WristLM = GetMeasurements(patientId, NoteId, "Wrist", "LM", NoteType);
            string OtherMeasurementWristLM = GetOthMeasurements(patientId, NoteId, "Wrist", "LM", NoteType);
            List<MSShoulderEntity> WristCM = GetMeasurements(patientId, NoteId, "Wrist", "CM", NoteType);
            string OtherMeasurementWristCM = GetOthMeasurements(patientId, NoteId, "Wrist", "CM", NoteType);

            //Hip
            List<MSShoulderEntity> HipIE = GetMeasurements(patientId, NoteId, "Hip", "IE", NoteType);
            string OtherMeasurementHipIE = GetOthMeasurements(patientId, NoteId, "Hip", "IE", NoteType);
            List<MSShoulderEntity> HipGoal = GetMeasurements(patientId, NoteId, "Hip", "Goal", NoteType);
            string OtherMeasurementHipGoal = GetOthMeasurements(patientId, NoteId, "Hip", "Goal", NoteType);
            List<MSShoulderEntity> HipLM = GetMeasurements(patientId, NoteId, "Hip", "LM", NoteType);
            string OtherMeasurementHipLM = GetOthMeasurements(patientId, NoteId, "Hip", "LM", NoteType);
            List<MSShoulderEntity> HipCM = GetMeasurements(patientId, NoteId, "Hip", "CM", NoteType);
            string OtherMeasurementHipCM = GetOthMeasurements(patientId, NoteId, "Hip", "CM", NoteType);

            //Knee
            List<MSShoulderEntity> KneeIE = GetMeasurements(patientId, NoteId, "Knee", "IE", NoteType);
            string OtherMeasurementKneeIE = GetOthMeasurements(patientId, NoteId, "Knee", "IE", NoteType);
            List<MSShoulderEntity> KneeGoal = GetMeasurements(patientId, NoteId, "Knee", "Goal", NoteType);
            string OtherMeasurementKneeGoal = GetOthMeasurements(patientId, NoteId, "Knee", "Goal", NoteType);
            List<MSShoulderEntity> KneeLM = GetMeasurements(patientId, NoteId, "Knee", "LM", NoteType);
            string OtherMeasurementKneeLM = GetOthMeasurements(patientId, NoteId, "Knee", "LM", NoteType);
            List<MSShoulderEntity> KneeCM = GetMeasurements(patientId, NoteId, "Knee", "CM", NoteType);
            string OtherMeasurementKneeCM = GetOthMeasurements(patientId, NoteId, "Knee", "CM", NoteType);
            //Ankle
            List<MSShoulderEntity> AnkleIE = GetMeasurements(patientId, NoteId, "Ankle", "IE", NoteType);
            string OtherMeasurementAnkleIE = GetOthMeasurements(patientId, NoteId, "Ankle", "IE", NoteType);
            List<MSShoulderEntity> AnkleGoal = GetMeasurements(patientId, NoteId, "Ankle", "Goal", NoteType);
            string OtherMeasurementAnkleGoal = GetOthMeasurements(patientId, NoteId, "Ankle", "Goal", NoteType);
            List<MSShoulderEntity> AnkleLM = GetMeasurements(patientId, NoteId, "Ankle", "LM", NoteType);
            string OtherMeasurementAnkleLM = GetOthMeasurements(patientId, NoteId, "Ankle", "LM", NoteType);
            List<MSShoulderEntity> AnkleCM = GetMeasurements(patientId, NoteId, "Ankle", "CM", NoteType);
            string OtherMeasurementAnkleCM = GetOthMeasurements(patientId, NoteId, "Ankle", "CM", NoteType);

            //CervicalLR
            List<MSShoulderEntity> CervicalLRIE = GetMeasurements(patientId, NoteId, "CervicalLR", "IE", NoteType);
            string OtherMeasurementCervicalLRIE = GetOthMeasurements(patientId, NoteId, "CervicalLR", "IE", NoteType);
            List<MSShoulderEntity> CervicalLRGoal = GetMeasurements(patientId, NoteId, "CervicalLR", "Goal", NoteType);
            string OtherMeasurementCervicalLRGoal = GetOthMeasurements(patientId, NoteId, "CervicalLR", "Goal", NoteType);
            List<MSShoulderEntity> CervicalLRLM = GetMeasurements(patientId, NoteId, "CervicalLR", "LM", NoteType);
            string OtherMeasurementCervicalLRLM = GetOthMeasurements(patientId, NoteId, "CervicalLR", "LM", NoteType);
            List<MSShoulderEntity> CervicalLRCM = GetMeasurements(patientId, NoteId, "CervicalLR", "CM", NoteType);
            string OtherMeasurementCervicalLRCM = GetOthMeasurements(patientId, NoteId, "CervicalLR", "CM", NoteType);

            //Cervical
            List<MSShoulderEntity> CervicalIE = GetMeasurements(patientId, NoteId, "Cervical", "IE", NoteType);          
            List<MSShoulderEntity> CervicalGoal = GetMeasurements(patientId, NoteId, "Cervical", "Goal", NoteType);          
            List<MSShoulderEntity> CervicalLM = GetMeasurements(patientId, NoteId, "Cervical", "LM", NoteType);          
            List<MSShoulderEntity> CervicalCM = GetMeasurements(patientId, NoteId, "Cervical", "CM", NoteType);

            //LumbarLR
            List<MSShoulderEntity> LumbarLRIE = GetMeasurements(patientId, NoteId, "LumbarLR", "IE", NoteType);
            string OtherMeasurementLumbarLRIE = GetOthMeasurements(patientId, NoteId, "LumbarLR", "IE", NoteType);
            List<MSShoulderEntity> LumbarLRGoal = GetMeasurements(patientId, NoteId, "LumbarLR", "Goal", NoteType);
            string OtherMeasurementLumbarLRGoal = GetOthMeasurements(patientId, NoteId, "LumbarLR", "Goal", NoteType);
            List<MSShoulderEntity> LumbarLRLM = GetMeasurements(patientId, NoteId, "LumbarLR", "LM", NoteType);
            string OtherMeasurementLumbarLRLM = GetOthMeasurements(patientId, NoteId, "LumbarLR", "LM", NoteType);
            List<MSShoulderEntity> LumbarLRCM = GetMeasurements(patientId, NoteId, "LumbarLR", "CM", NoteType);
            string OtherMeasurementLumbarLRCM = GetOthMeasurements(patientId, NoteId, "LumbarLR", "CM", NoteType);

            //Cervical
            List<MSShoulderEntity> LumbarIE = GetMeasurements(patientId, NoteId, "Lumbar", "IE", NoteType);
            List<MSShoulderEntity> LumbarGoal = GetMeasurements(patientId, NoteId, "Lumbar", "Goal", NoteType);
            List<MSShoulderEntity> LumbarLM = GetMeasurements(patientId, NoteId, "Lumbar", "LM", NoteType);
            List<MSShoulderEntity> LumbarCM = GetMeasurements(patientId, NoteId, "Lumbar", "CM", NoteType);

            string OtherMeasurementOtherIE = GetOthMeasurements(patientId, NoteId, "Other", "IE", NoteType);
            string OtherMeasurementOtherRGoal = GetOthMeasurements(patientId, NoteId, "Other", "Goal", NoteType);
            string OtherMeasurementOtherLM = GetOthMeasurements(patientId, NoteId, "Other", "LM", NoteType);
            string OtherMeasurementOtherCM = GetOthMeasurements(patientId, NoteId, "Other", "CM", NoteType);

            List<MSIntervensionsEntity> MSIntervensions = GetIntervensions(patientId, NoteId, "B");
            List<MSIntervensionsReposrtEntity> MSIntervensionsReport = GetIntervensionsReports(patientId, NoteId, "F");
            List<MSIntervensionsSumEntity> MSSumInterventions = GetDocSumInterventions(NoteId);
            List<MSProgExerciseEntity> MSProgExer = GetProgExer(patientId, NoteId);
            List<MSDecHints> MSCurrentTreatDesc = GetTreatDesc(patientId, NoteId);
            List<MSDecHints> MSPrevTreatDesc = GetTreatDesc(patientId, NoteId);

            List<MSGoalsEntity> MSGoal1 = GetGoals(patientId, "L");
            List<MSGoalsEntity> MSGoal2 = GetGoals(patientId, "F");
            List<MSGoalsEntity> MSGoal= MSGoal1.Union(MSGoal2).ToList();
            #region 
            JObject jss =
              new JObject(
              new JProperty("component",
              new JObject(new JProperty("structuredBody",
              new JObject(new JProperty("component",
              //==========Reminder Section =================
              new JObject(new JProperty("section",
              new JObject(new JProperty("templateId",
              new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.22.2.8"))),

              new JProperty("TeamRehabID",
                    new JObject(new JProperty("PatientID", patientId),
                     new JProperty("NoteID", NoteId))),

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
              new JProperty(strfunctCharReminder))))

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

//=========================Intervension section===========================
                  new JObject(new JProperty("section",
              new JObject(new JProperty("templateId",
              new JObject(new JProperty("@root", "2.16.840.1.113883.10.20.21.2.3"),
              new JProperty("@extension", "2015-08-01"))),

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
                  // ====================Section Intervension Reports===================

                  new JObject(new JProperty("section",
              new JObject(

              new JProperty("title",
              new JObject(
              new JProperty("text",
              new JObject(new JProperty("table",
              new JObject(new JProperty("@border", "1"),
              new JProperty("width", "100%"),



              new JProperty("tbody",
              new JObject(new JProperty("tr", IntervensionReportArray(MSIntervensionsReport)



              ))))




              )))))     //title

              ))),//section



              new JObject(new JProperty("section",
              new JObject(

              new JProperty("title",
              new JObject(
              new JProperty("text",
              new JObject(new JProperty("table",
              new JObject(new JProperty("@border", "1"),
              new JProperty("width", "100%"),



              new JProperty("thead",
              new JObject(new JProperty("tr",
              new JObject(new JProperty("th",
              new JObject(new JProperty("Ultimated Minutes",
                                          "Time Units",
                                          "Units")
              )))


              ))),
              new JProperty("tbody",
              new JObject(new JProperty("tr",
              new JObject(new JProperty("td",
              new JObject(new JProperty(Convert.ToString(MSSumInterventions.FirstOrDefault().totaluntimedminutes),         // wrong O/P
                                 Convert.ToString(MSSumInterventions.FirstOrDefault().totaltimedminutes),
                                   Convert.ToString(MSSumInterventions.FirstOrDefault().totalunits))
              )))


              )))
              )

              )))))     //title

              ))),//section


              //======================Progresive Exercise=====================
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
                                             "Weight"))))))),


              new JProperty("tbody",
              new JObject(new JProperty("tr",ProgresiveExerciseArray(MSProgExer)
             
              
                                 
              )))
              ))))     //title

              ))),
                  //======================GOALS=====================
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
              new JObject(new JProperty("tr", GoalsArray(MSGoal)

              )))
              ))))     //title

              ))),
                  //======================TREATMENT PLAN=====================

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
                      new JObject(new JProperty("tr",
                      new JObject(new JProperty("@ID", "CurrentTreat"),
                      new JProperty("td",
                      new JObject(new JProperty("Current Plan",
                      new JObject(new JProperty("content", dateOfService.DateOfService),
                                  new JProperty(strfunctChar)))))),


                      new JObject(new JProperty("@ID", "PrevTreat"),
                      new JProperty("td",
                      new JObject(new JProperty("Previous Plan",
                      new JObject(new JProperty("content", dateOfService.DateOfService),
                                  new JProperty(strfunctCharReminder))))))    )  //tr

                      ))  //tbody

                  ))))

                  ))),  //section

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
                          new JProperty("tr",MeasurementArray(shoulderCM)
                          
                         

                          )        //tr        


              ),
             new JObject(new JProperty("@id", "Elbow"),
                          new JProperty("tr", MeasurementArray(elbowCM)

                         )),    // new JObject

              new JObject(new JProperty("@id", "Forearm"),
                          new JProperty("tr", MeasurementArray(ForearmCM)
                         )),

               new JObject(new JProperty("@id", "Wrist"),
                          new JProperty("tr", MeasurementArray(WristCM)

                                      )),

                new JObject(new JProperty("@id", "Hip"),
                          new JProperty("tr", MeasurementArray(HipCM)



                          )),
                new JObject(new JProperty("@id", "Knee"),
                          new JProperty("tr", MeasurementArray(KneeCM)

                                      )),
                new JObject(new JProperty("@id", "Ankle"),
                          new JProperty("tr", MeasurementArray(AnkleCM)

                                      )),
                new JObject(new JProperty("@id", "Cervical"),
                          new JProperty("tr", MeasurementArray(CervicalCM)

                                      )),
                  new JObject(new JProperty("@id", "Lumbar"),
                          new JProperty("tr",

                           MeasurementArray(LumbarCM)
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
                          MeasurementArray(shoulderGoal)
                          ) ),

              new JObject(new JProperty("@id", "Shoulder-Last Measurement"),
                          new JProperty("tr", MeasurementArray(shoulderLM)


                          ) ),

               new JObject(new JProperty("@id", "Shoulder-Initial Measurement"),
                          new JProperty("tr", MeasurementArray(shoulderIE)
                          )        //tr 
                          ),

                new JObject(new JProperty("@id", "Elbow-Goal"),
                          new JProperty("tr", MeasurementArray(elbowGoal)

                                      )),    // new JObject

                new JObject(new JProperty("@id", "Elbow-Last Measurement"),
                          new JProperty("tr",
                        MeasurementArray(elbowLM)
                                      )),    // new JObject

                new JObject(new JProperty("@id", "Elbow-Initial Measurement"),
                          new JProperty("tr", MeasurementArray(elbowIE)

                                      )),    // new JObject

              new JObject(new JProperty("@id", "Forearm-Goal"),
                          new JProperty("tr", MeasurementArray(ForearmGoal)
                                      )),    // new JObject

                new JObject(new JProperty("@id", "Forearm-Last Measurement"),
                          new JProperty("tr",
                         MeasurementArray(ForearmLM)
                                      )),    // new JObject

                new JObject(new JProperty("@id", "Forearm-Initial Measurement"),
                          new JProperty("tr",
                           MeasurementArray(ForearmIE)
                                      )),    // new JObject

               new JObject(new JProperty("@id", "Wrist-Goal"),
                          new JProperty("tr",
                          MeasurementArray(WristGoal)
                                      )),
                 new JObject(new JProperty("@id", "Wrist-Last Measurement"),
                          new JProperty("tr",
                          MeasurementArray(WristLM)
                                      )),
                 new JObject(new JProperty("@id", "Wrist-Initial Measurement"),
                          new JProperty("tr",
                          MeasurementArray(WristIE)
                                      )),

                new JObject(new JProperty("@id", "Hip-Goal"),
                          new JProperty("tr",
                            MeasurementArray(HipGoal)
                            )),
                 new JObject(new JProperty("@id", "Hip-Last Measurement"),
                          new JProperty("tr",
                          MeasurementArray(HipLM)
                                      )),
                 new JObject(new JProperty("@id", "Hip-Initial Measurement"),
                          new JProperty("tr",
                          MeasurementArray(HipIE)
                          )  ),


                 
                new JObject(new JProperty("@id", "Knee-Goal"),
                          new JProperty("tr",
                        MeasurementArray(KneeGoal)
                                      )),
                 new JObject(new JProperty("@id", "Knee-Last Measurement"),
                          new JProperty("tr",
                      MeasurementArray(KneeLM)
                                      )),
                    new JObject(new JProperty("@id", "Knee-Initial Measurement"),
                          new JProperty("tr",
                          MeasurementArray(KneeIE)
                                      )),

                new JObject(new JProperty("@id", "Ankle-Goal"),
                          new JProperty("tr",
                            MeasurementArray(AnkleGoal)
                                      )),
                  new JObject(new JProperty("@id", "Ankle-Last Measurement"),
                          new JProperty("tr",
                      MeasurementArray(AnkleLM)
                                      )),
                    new JObject(new JProperty("@id", "Ankle-Initial Measurement"),
                          new JProperty("tr",
                          MeasurementArray(AnkleIE)
                                      )),


                new JObject(new JProperty("@id", "Cervical-Goal"),
                          new JProperty("tr",
                           MeasurementArray(CervicalGoal)
                                      )),
                  new JObject(new JProperty("@id", "Cervical-Last Measurement"),
                          new JProperty("tr",
                      MeasurementArray(CervicalLM)
                                      )),
                    new JObject(new JProperty("@id", "Cervical-Initial Measurement"),
                          new JProperty("tr",
                          MeasurementArray(CervicalIE)
                                      )),

                 new JObject(new JProperty("@id", "Lumbar-Goal"),
                          new JProperty("tr",
                          MeasurementArray(LumbarGoal)
                                      )),
                   new JObject(new JProperty("@id", "Lumbar-Last Measurement"),
                          new JProperty("tr",
                      MeasurementArray(LumbarLM)
                                      )),
                    new JObject(new JProperty("@id", "Lumbar-Initial Measurement"),
                          new JProperty("tr",
                          MeasurementArray(LumbarIE)
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



        public List<MSFunctionalCharEntity> GetFunctChar(int patientId, int NoteId, string NoteType)
        {
            List<MSFunctionalCharEntity> MSFunctionalCharEntity = new List<MSFunctionalCharEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    MSFunctionalCharEntity = context.Database.SqlQuery<MSFunctionalCharEntity>("SP_GetDocFuncCharac @Docrowid,@patientid,@NoteType ",
                  new SqlParameter("Docrowid", SqlDbType.BigInt) { Value = NoteId },
                   new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId },
                    new SqlParameter("NoteType", SqlDbType.VarChar) { Value = NoteType }
                  ).ToList();

                  
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return MSFunctionalCharEntity;
        }

        public MSDateOfServiceEntity GetMSDateOfService(int patientId, int NoteId, string NoteType)
        {
            Team.Rehab.DataModel.RehabEntities rehab = new RehabEntities();
            List<MSDateOfServiceEntity> msDateOfServiceEntity = new List<MSDateOfServiceEntity>();
            try
            {
                //================ Date Of Service ====================================================

                msDateOfServiceEntity = (from r in rehab.tblDocMasters
                                         where r.Docrowid == NoteId

                                         select new MSDateOfServiceEntity
                                         {
                                             DateOfService = r.DateOfService,
                                             NoteType = r.NoteType,
                                             ThType = r.ThType
                                         }).ToList();

                //================END ====================================================================
            }
            catch (Exception ex)
            {

                throw;
            }
            return msDateOfServiceEntity.FirstOrDefault();
        }

        public MSHints GetHints(int patientId, int NoteId, string NoteType)
        {
            Team.Rehab.DataModel.RehabEntities rehab = new RehabEntities();
            MSHints msHints = new MSHints();
            try
            {
                msHints = (from r in rehab.tblHintsNotes

                           select new MSHints
                           {
                               hnrowid = r.hnrowid,
                               FuncCharacHint = r.FuncCharacHint,
                               FuncCharRem = r.FuncCharRem,
                               DescRem = r.DescRem
                           }).ToList().FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }

            return msHints;


        }

        public List<MSShoulderEntity> GetMeasurements(int patientId, int NoteId, string bodyPart, string level, string NoteType)
        {
            string storedProc = string.Empty;
            if (NoteType == "PPOC2")
            { storedProc = "SP_GetPhyMeasureIE2"; }
            else { storedProc = "SP_GetPhyMeasure"; }

            List<MSShoulderEntity> MSFunctionalCharEntity = new List<MSShoulderEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    MSFunctionalCharEntity = context.Database.SqlQuery<MSShoulderEntity>(storedProc + " @Docrowid,@bodypart,@level,@patientid ",
                   new SqlParameter("Docrowid", SqlDbType.BigInt) { Value = NoteId },
                    new SqlParameter("bodypart", SqlDbType.VarChar) { Value = bodyPart },
                     new SqlParameter("level", SqlDbType.VarChar) { Value = level },
                     new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId }

                   ).ToList();


                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return MSFunctionalCharEntity;
        }

        public string GetOthMeasurements(int patientId, int NoteId, string bodyPart, string level, string NoteType)
        {
            string otherMeasurment = string.Empty;
            string storedProc = string.Empty;
            if (NoteType == "PPOC2")
            { storedProc = "SP_GetOthMeasureIE2"; }
            else { storedProc = "SP_GetOthMeasure"; }

            List<OtherMeasurementEntity> MSFunctionalCharEntity = new List<OtherMeasurementEntity>();
            OtherMeasurementEntity MSFunctionalCharEntity1 = new OtherMeasurementEntity();
            try
            {
                using (var context = new RehabEntities())
                {
                    MSFunctionalCharEntity = context.Database.SqlQuery<OtherMeasurementEntity>(storedProc + " @Docrowid,@bodypart,@level,@patientid ",
                   new SqlParameter("Docrowid", SqlDbType.BigInt) { Value = NoteId },
                    new SqlParameter("bodypart", SqlDbType.VarChar) { Value = bodyPart },
                    new SqlParameter("level", SqlDbType.VarChar) { Value = level },
                     new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId }

                   ).ToList();
                    if (MSFunctionalCharEntity.Count>0)
                    {
                        MSFunctionalCharEntity1 = MSFunctionalCharEntity.FirstOrDefault();
                        otherMeasurment = MSFunctionalCharEntity1.Measurement.ToString();
                    }
                }
            }
            catch (Exception ex) 
            {

                throw;
            }
            return otherMeasurment;
        }
        public List<MSIntervensionsEntity> GetIntervensions(int patientId, int NoteId, string type)
        {
            string storedProc = string.Empty;
            if (type == "B")
            { storedProc = "SP_GetDocNoteInterventions"; }
            else { storedProc = "SP_GetDocBillInterventions"; }


                List<MSIntervensionsEntity> mSIntervensionsEntity = new List<MSIntervensionsEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    mSIntervensionsEntity = context.Database.SqlQuery<MSIntervensionsEntity>(storedProc + " @patientid,@docrowid,@type ",
                   new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId },
                    new SqlParameter("docrowid", SqlDbType.BigInt) { Value = NoteId },
                     new SqlParameter("type", SqlDbType.VarChar) { Value = type }
                    

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
            return mSIntervensionsEntity;
        }

        public List<MSIntervensionsReposrtEntity> GetIntervensionsReports(int patientId, int NoteId, string type)
        {
            string storedProc = string.Empty;
            if (type == "B")
            { storedProc = "SP_GetDocNoteInterventions"; }
            else { storedProc = "SP_GetDocBillInterventions"; }


            List<MSIntervensionsReposrtEntity> mSIntervensionsEntity = new List<MSIntervensionsReposrtEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    mSIntervensionsEntity = context.Database.SqlQuery<MSIntervensionsReposrtEntity>(storedProc + " @patientid,@docrowid,@type ",
                   new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId },
                    new SqlParameter("docrowid", SqlDbType.BigInt) { Value = NoteId },
                     new SqlParameter("type", SqlDbType.VarChar) { Value = type }


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
            return mSIntervensionsEntity;
        }

        public List<MSIntervensionsSumEntity> GetDocSumInterventions(int NoteId)
        {
           

            List<MSIntervensionsSumEntity> mSIntervensionsSumEntity = new List<MSIntervensionsSumEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    mSIntervensionsSumEntity = context.Database.SqlQuery<MSIntervensionsSumEntity>("SP_GetDocSumInterventions @Docrowid ",
                  
                    new SqlParameter("Docrowid", SqlDbType.BigInt) { Value = NoteId }
                    


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
            return mSIntervensionsSumEntity;
        }

        public List<MSProgExerciseEntity> GetProgExer(int patientId, int NoteId)
        {
            
            

            List<MSProgExerciseEntity> mSProgExerciseEntity = new List<MSProgExerciseEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    mSProgExerciseEntity = context.Database.SqlQuery<MSProgExerciseEntity>("SP_GetDocProgExer @docrowid,@patientid,@report ",
                   new SqlParameter("docrowid", SqlDbType.BigInt) { Value = NoteId },
                    new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId },
                     new SqlParameter("report", SqlDbType.VarChar) { Value = "N" }


                   ).ToList();
                  
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return mSProgExerciseEntity;
        }

        public List<MSDecHints> GetTreatDesc(int patientId, int NoteId)
        {



            List<MSDecHints> mSDecHints = new List<MSDecHints>();
            try
            {
                using (var context = new RehabEntities())
                {
                    mSDecHints = context.Database.SqlQuery<MSDecHints>("SP_GetDocTreatDesc @docrowid,@patientid ",
                   new SqlParameter("docrowid", SqlDbType.BigInt) { Value = NoteId },
                    new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId }
                   


                   ).ToList();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return mSDecHints;
        }

        public List<MSGoalsEntity> GetGoals(int patientId, string GoalType)
        {



            List<MSGoalsEntity> mSGoalsEntity = new List<MSGoalsEntity>();
            try
            {
                using (var context = new RehabEntities())
                {
                    mSGoalsEntity = context.Database.SqlQuery<MSGoalsEntity>("SP_GetPatGoals @patientid,@GoalType ",
                   new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId },
                               new SqlParameter("GoalType", SqlDbType.VarChar) { Value = GoalType }


                   ).ToList();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return mSGoalsEntity;
        }
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
                                                      obj. PrevMinuts
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

        public JArray IntervensionReportArray(List<MSIntervensionsReposrtEntity> MSIntervensions)
        {
            JObject jsonObject = null;
            JObject jsonObject1 = null;
            JArray jsonArray = new JArray();
            try
            {
                foreach (MSIntervensionsReposrtEntity obj in MSIntervensions)
                {

                 

                    jsonObject=     new JObject(new JProperty("td",
             new JObject(new JProperty(obj.Rcode)
             )));
             jsonArray.Add(jsonObject);
                    jsonObject1=   new JObject(new JProperty("td",
              new JObject(new JProperty(obj.Modifier))));
                    jsonArray.Add(jsonObject1);
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

        public JArray GoalsArray(List<MSGoalsEntity> mSGoals)
        {
            JObject jsonObject = null;

            JArray jsonArray = new JArray();
            try
            {
                foreach (MSGoalsEntity obj in mSGoals)
                {

                    jsonObject = new JObject(new JProperty("td",
                                   new JObject(new JProperty(obj.Description,
                                                       obj.GoalNMet,
                                                       obj.GoalPMet,
                                                      obj.GoalMet
                                                    

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

        public JArray MeasurementArray(List<MSShoulderEntity> MSShoulderEntity)
        {
            JObject jsonObject = null;
            JObject jsonObject1 = null;
            JArray jsonArray = new JArray();
            try
            {
                foreach (MSShoulderEntity obj in MSShoulderEntity)
                {
                    jsonObject = new JObject(new JProperty("th",
                                      new JObject(new JProperty("@scope", "row"),
                                                  new JProperty("#text", obj.MeasurementType)
                                      )),
                                       new JProperty("td", obj.SL, obj.SR, obj.AL, obj.AR, obj.PR)
                                    
                                      );
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
}
