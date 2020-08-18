using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Team.Rehab.BusinessEntities;

namespace CCDAGeneration
{
   public class PatientDemographics
    {

       public JArray ConvertPatientInfo(List<PatientEntity> patientEntity)
        {
           

            JArray jsonArray = new JArray();
            JObject rss = new JObject();
            JObject rss1 = new JObject();
            string CCDAResponse = string.Empty;

            foreach (PatientEntity paient in patientEntity)
            {
                 rss = PatientlistToCCDA(paient);

                jsonArray.Add(rss);
            }
           
            return jsonArray;

        }

        public JObject PatientlistToCCDA(PatientEntity paient)
        {
            DateTime dateOfbirth = Convert.ToDateTime(paient.BirthDate);
            string birthDate = dateOfbirth.ToString("yyyyMMdd");

            JObject rss =
             new JObject(
              new JProperty("ClinicalDocument",
               new JObject(
                new JProperty("realmCode",
                 new JObject(
                  new JProperty("_code", "US"))),
                new JProperty("typeId",
                 new JObject(
                  new JProperty("_root", "2.16.840.1.113883.1.3"),
                  new JProperty("_extension", "POCD_HD000040"))),
                new JProperty("templateId",
                 new JObject(new JProperty("_root", "2.16.840.1.113883.10.20.3")),
                 new JObject(new JProperty("_root", "2.16.840.1.113883.10.20.21.1"))),
                new JProperty("id",
                 new JObject(
                     new JProperty("TeamRehabId", paient.Prowid),
                     new JProperty("TriarqId", ""),
                  new JProperty("_extension", "999021"),
                  new JProperty("_root", "2.16.840.1.113883.19"))),

                new JProperty("code",
                 new JObject(
                  new JProperty("_codeSystem", "2.16.840.1.113883.6.1"),
                  new JProperty("_codeSystemName", "LOINC"),
                  new JProperty("_code", "11506-3"),
                  new JProperty("displayName", "Subsequent evaluation note"))),
                new JProperty("title", "Evaluation Note"),
                new JProperty("effectiveTime",
                 new JObject(new JProperty("_value", "20050329171504+0500"))),
                new JProperty("confidentialityCode",
                 new JObject(new JProperty("_code", "N"),
                  new JProperty("_codeSystem", "2.16.840.1.113883.5.25"))),
                new JProperty("languageCode",
                 new JObject(new JProperty("_code", "en-US"))),
                new JProperty("setId",
                 new JObject(new JProperty("_extension", "111199021"),
                  new JProperty("_root", "2.16.840.1.113883.19"))),
                new JProperty("versionNumber",
                 new JObject(new JProperty("_value", "1"))),


                new JProperty("recordTarget",
                 new JObject(new JProperty("patientRole",
                  new JObject(

                   new JProperty("id",
                    new JObject(new JProperty("_root", "2.16.840.1.113883.3.6132"),
                     new JProperty("_extension", "345678912-0154"))),


                   new JProperty("addr",
                    new JObject(new JProperty(paient.Address1),
                     new JProperty("city", paient.City),
                     new JProperty("postalCode", paient.ZipCode),
                     new JProperty("state", paient.State),
                     new JProperty("country", "US"),
                     new JProperty("_use", "HP"))),

                   new JProperty("telecom",
                    new JObject(new JProperty("_value", paient.WorkPh),
                     new JProperty("_use", "HP"))),

                   new JProperty("patient",
                    new JObject(
                     new JProperty("name",
                      new JObject(new JProperty("given", paient.FirstName),
                       new JProperty("family", paient.LastName))),

                     new JProperty("administrativeGenderCode",
                      new JObject(new JProperty("_code", "M"),
                       new JProperty("_codeSystem", "2.16.840.1.113883.5.1"),
                       new JProperty("_displayName", paient.Gender),
                       new JProperty("_codeSystemName", "AdministrativeGender"))),

                     new JProperty("birthTime",
                      new JObject(new JProperty("_value", birthDate))),

                     new JProperty("maritalStatusCode",
                      new JObject(new JProperty("_code", "M"),
                       new JProperty("_displayName", "Married"),
                       new JProperty("_codeSystem", "2.16.840.1.113883.5.2"),
                       new JProperty("_codeSystemName", "MaritalStatus"))),

                     new JProperty("languageCommunication",
                      new JObject(new JProperty("languageCode",
                        new JObject(new JProperty("_code", "eng"))),


                       new JProperty("methCode",
                        new JObject(new JProperty("_code", "ESP"),
                         new JProperty("_displayName", "Expressed spoken"),
                         new JProperty("_codeSystem", "2.16.840.1.113883.5.60"),
                         new JProperty("_codeSystemName", "LanguageAbilityMode"))),

                       new JProperty("proficiencyLevelCode",
                        new JObject(new JProperty("_code", "E"),
                         new JProperty("_displayName", "Excellent"),
                         new JProperty("_codeSystem", "2.16.840.1.113883.5.61"),
                         new JProperty("_codeSystemName", "LanguageAbilityProficiency"))),

                       new JProperty("preferenceInd",
                        new JObject(new JProperty("_value", "true")))


                      )))))))))));
           // string abc = rss.ToString();
            return rss;
        }
}
}
