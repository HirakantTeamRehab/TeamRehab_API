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
   public class TPatientRespository : ITPatientRepository
    {
        private readonly IUnitOfwork _unitOfWork;
        // private readonly UnitOfWork _unitOfWork;
        private readonly IProcedureManagement _procedureManagement;

        public TPatientRespository(IUnitOfwork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            //this._procedureManagement = new ProcedureManagement();
        }
        public JArray GetPatientList(string firstName, string lastName, string dateOfBirth, string NPINumber)
        {
            try
            {
                List<PatientEntity> patientEntity = new List<PatientEntity>();
                List<tblPatients> tblPatientEntity = new List<tblPatients>();
                Team.Rehab.DataModel.RehabEntities rehab = new RehabEntities();
                string ReferralSource = string.Empty;
                if (!string.IsNullOrEmpty(NPINumber))
                {
                    tblReferrer referrer = _unitOfWork.ReferrerRepo.Get(o => o.NPINumber == NPINumber).FirstOrDefault();
                    if (referrer != null)
                    {
                        ReferralSource = Convert.ToString(referrer.Rrowid);


                    }
                }
                var query = rehab.tblPatients.AsQueryable();
                if (!string.IsNullOrEmpty(NPINumber))
                    query = query.Where(iv => iv.ReferralSource.Contains(ReferralSource));
                if (!string.IsNullOrEmpty(firstName))
                    query = query.Where(iv => iv.FirstName.Contains(firstName));

                if (!string.IsNullOrEmpty(lastName))
                    query = query.Where(iv => iv.LastName.Contains(lastName));
                if (!string.IsNullOrEmpty(dateOfBirth))
                    query = query.Where(iv => iv.BirthDate.Equals(dateOfBirth));

                patientEntity = (from iv in query
                                 select new PatientEntity {
                                     Prowid=iv.Prowid,
                                     FirstName = iv.FirstName,
                                     LastName = iv.LastName,
                                     MiddleInitial = iv.MiddleInitial,
                                     Gender = iv.Gender,
                                     BirthDate = iv.BirthDate,
                                     Address1 = iv.Address1,
                                     Address2 = iv.Address2,
                                     City = iv.City,
                                     State = iv.State,
                                     ZipCode = iv.ZipCode,
                                     HomePh = iv.HomePh,
                                     WorkPh = iv.WorkPh,
                                     CellPh = iv.CellPh,
                                    }).ToList();


                //===============mapper end==========================================
                JArray jResponse = new JArray();
                if (patientEntity.Count() > 0)
                {
                    CCDAGeneration.PatientDemographics _patientDemographics = new CCDAGeneration.PatientDemographics();
                

                    jResponse = _patientDemographics.ConvertPatientInfo(patientEntity);
                    return jResponse;
                }
                else
                {
                    JArray jsonArray = new JArray();
                    JObject rss =
               new JObject(
                new JProperty("No record found"
                   ));
                    jsonArray.Add(rss);
                    return jsonArray;  
                }
            }
            catch (Exception ex)
            {
                throw;

            }
        }
    }
}
