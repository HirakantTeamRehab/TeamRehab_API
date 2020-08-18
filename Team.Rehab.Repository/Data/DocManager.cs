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
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;


namespace Team.Rehab.Repository.Data
{
    public class DocManager
    {
        Team.Rehab.DataModel.RehabEntities rehab = new RehabEntities();
        public List<MSFunctionalCharEntity> GetDocFuncCharacR(int docrowid)
        {
            List<MSFunctionalCharEntity> msFunctionalCharEntity = new List<MSFunctionalCharEntity>();
            try
            {

                using (var context = new RehabEntities())
                {
                    msFunctionalCharEntity = context.Database.SqlQuery<MSFunctionalCharEntity>("SP_GetDocFuncCharacR @Docrowid ",
                   new SqlParameter("Docrowid", SqlDbType.BigInt) { Value = docrowid }



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
            return msFunctionalCharEntity;
        }
    

        public DataSet GetDocFuncCharacR(int docrowid, bool DatasetVersion)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocFuncCharacR";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@Docrowid";
                        param.Value = docrowid;
                        cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public string GetErrorMsg(string error)
        {
            var filteredResult = from s in rehab.tblErrorCodes
                                 where s.ErrorCode == error
                                 select s;
            string errorMsg = filteredResult.ToList().LastOrDefault().ErrDes.ToStringOrEmpty();
            return errorMsg;
            // return filteredResult.ToString();
        }

        public List<IniEvalNoteCheckEntity> chkIECNote(int patientId, int docrowid)
        {
            List<IniEvalNoteCheckEntity> chkIECNoteEntity = new List<IniEvalNoteCheckEntity>();
            try
            {

                using (var context = new RehabEntities())
                {
                    chkIECNoteEntity = context.Database.SqlQuery<IniEvalNoteCheckEntity>("SP_chkIECNote @patientid,@docrowid ",
                   new SqlParameter("patientid", SqlDbType.BigInt) { Value = patientId },
                   new SqlParameter("docrowid", SqlDbType.BigInt) { Value = docrowid }



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
            return chkIECNoteEntity;

        }
        public DataTable GetTableByNoteID(int docrowid, string SPname)
        {
            DataTable dt = new DataTable();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    System.Data.Common.DbDataReader sqlReader;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        //cmd.CommandText = "SP_GetDocDateOfService";
                        cmd.CommandText = SPname;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@docrowid";
                        param.Value = docrowid;
                        cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return dt;
        }

        public DataTable DTByDataAdapter(string SQL)
        {
            DataTable dt = new DataTable();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    DataSet ds = new DataSet();
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);

                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        //cmd.CommandText = "SP_GetDocDateOfService";
                        SqlCommand com = new SqlCommand(SQL, con);
                        SqlDataAdapter da = new SqlDataAdapter(com);

                        da.Fill(ds);
                        dt = ds.Tables[0];

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return dt;
        }

        public int ExecuteScalar(string strSql)
        {
            int count;
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(strSql, con))
                    {

                        count = Convert.ToInt32(cmd.ExecuteScalar());

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return count;
        }
        public DataTable GetDocDateOfService(int docrowid)
        {
            DataTable dt = new DataTable();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    System.Data.Common.DbDataReader sqlReader;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocDateOfService";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@docrowid";
                        param.Value = docrowid;
                        cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return dt;
        }

        //ITextManager Database calls//////////////////////////////
        public DataTable GetPrintNameAndNPI(string CommandText)
        {
            DataTable dt = new DataTable();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    System.Data.Common.DbDataReader sqlReader;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = CommandText;
                        cmd.CommandType = System.Data.CommandType.Text;
                        //SqlParameter param = new SqlParameter();
                        //param.ParameterName = "@docrowid";
                        //param.Value = docrowid;
                        //cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return dt;
        }

        public DataTable GetPatient(int PatientID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    System.Data.Common.DbDataReader sqlReader;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetPatient";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@PTrowid";
                        param.Value = PatientID;
                        cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return dt;
        }

        public DataTable GetPatVisits(int PatientID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    System.Data.Common.DbDataReader sqlReader;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetPatVisits";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@PTrowid";
                        param.Value = PatientID;
                        cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return dt;
        }

        public DataTable GetPatInsuranceClm(int PatientID, int DocID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    System.Data.Common.DbDataReader sqlReader;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetPatInsuranceClm";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter();
                        param1.ParameterName = "@PTrowid";
                        param1.Value = PatientID;
                        SqlParameter param2 = new SqlParameter();
                        param2.ParameterName = "@docrowid";
                        param2.Value = DocID;
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.CommandTimeout = 0;
                        sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();
                        dt.Load(sqlReader);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return dt;
        }

        public DataTable GetDocTherapist(int UserID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    System.Data.Common.DbDataReader sqlReader;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetPatTherapist";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@UserId";
                        param.Value = UserID;
                        cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return dt;
        }

        public DataTable GetDocTherapistRpt(int DocId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    System.Data.Common.DbDataReader sqlReader;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetPatTherapistRpt";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@docrowid";
                        param.Value = DocId;
                        cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return dt;
        }

        public DataSet GetPatDiagnosis(int PatientId)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetPatDiagnosis";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@PTrowid";
                        param.Value = PatientId;
                        cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetDocRInterventions(int Docrowid)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocRInterventions";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@Docrowid";
                        param.Value = Docrowid;
                        cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetDocSumInterventions(int Docrowid)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocSumInterventions";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@Docrowid";
                        if (Docrowid == null)
                        {
                            param.Value = 0;
                        }
                        else
                        {
                            param.Value = Docrowid;
                        }
                        cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetDocProgExer(int Docrowid, int PatientID, string Report)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocProgExer";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter();
                        SqlParameter param2 = new SqlParameter();
                        SqlParameter param3 = new SqlParameter();
                        param1.ParameterName = "@Docrowid";
                        param1.Value = Docrowid;
                        param2.ParameterName = "@patientid";
                        param2.Value = PatientID;
                        param3.ParameterName = "@report";
                        param3.Value = Report;
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);

                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetDocPainR(int Docrowid)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocPainR";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@Docrowid";
                        param.Value = Docrowid;
                        cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetDocBodyPart(int Docrowid, string Level)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_DocGetBodyPart";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        SqlParameter param1 = new SqlParameter();
                        param.ParameterName = "@Docrowid";
                        param.Value = Docrowid;
                        param1.ParameterName = "@level";
                        param1.Value = Level;
                        cmd.Parameters.Add(param);
                        cmd.Parameters.Add(param1);
                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetMeasurementsR(int PatientId, int Docrowid, string BodyPart, string Level)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {

                        SqlParameter param = new SqlParameter();
                        SqlParameter param1 = new SqlParameter();
                        SqlParameter param2 = new SqlParameter();

                        param.ParameterName = "@Docrowid";
                        param.Value = Docrowid;
                        param1.ParameterName = "@bodypart";
                        param1.Value = Level;
                        param2.ParameterName = "@level";
                        param2.Value = Level;
                        if (BodyPart == "Cervical" || BodyPart == "Lumbar")
                        {
                            if (Level == "Goal")
                            {
                                SqlParameter par = new SqlParameter();
                                par.ParameterName = "@patientid";
                                par.Value = PatientId;
                                cmd.Parameters.Add(par);
                                cmd.Parameters.Add(param1);
                                cmd.Parameters.Add(param2);
                                cmd.CommandText = "SP_GetPhyMeasureRGCerLum";
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            }
                            else
                            {
                                cmd.Parameters.Add(param);
                                cmd.Parameters.Add(param1);
                                cmd.Parameters.Add(param2);
                                cmd.CommandText = "SP_GetPhyMeasureRCerLum";
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            }
                        }
                        else
                        {
                            if (Level == "Goal")
                            {
                                cmd.Parameters.Add(param);
                                cmd.Parameters.Add(param1);
                                cmd.Parameters.Add(param2);
                                cmd.CommandText = "SP_GetPhyMeasureRIEG";
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            }
                            else
                            {
                                cmd.Parameters.Add(param);
                                cmd.Parameters.Add(param1);
                                cmd.Parameters.Add(param2);
                                cmd.CommandText = "SP_GetPhyMeasureR";
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            }
                        }


                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetMeasurementsRG(int PatientId, string BodyPart, string Level)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {

                        SqlParameter param = new SqlParameter();
                        SqlParameter param1 = new SqlParameter();
                        SqlParameter param2 = new SqlParameter();

                        param.ParameterName = "@patientid";
                        param.Value = PatientId;
                        param1.ParameterName = "@bodypart";
                        param1.Value = Level;
                        param2.ParameterName = "@level";
                        param2.Value = Level;
                        if (BodyPart == "Cervical" || BodyPart == "Lumbar")
                        {
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param1);
                            cmd.Parameters.Add(param2);
                            cmd.CommandText = "SP_GetPhyMeasureRGCerLum";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        }
                        else
                        {

                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param1);
                            cmd.Parameters.Add(param2);
                            cmd.CommandText = "SP_GetPhyMeasureRG";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        }


                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetDocExtremityTests(string Type, int DocId, int PatientId)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocExtremityTests";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter();
                        SqlParameter param2 = new SqlParameter(); SqlParameter param3 = new SqlParameter();
                        param1.ParameterName = "@Type";
                        param1.Value = Type;
                        param2.ParameterName = "@Docrowid";
                        param2.Value = DocId;
                        param3.ParameterName = "@patientid";
                        param3.Value = PatientId;
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);

                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetDocSpinalTests(string Type, int DocId, int PatientId)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocSpinalTests";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter();
                        SqlParameter param2 = new SqlParameter(); SqlParameter param3 = new SqlParameter();
                        param1.ParameterName = "@Type";
                        param1.Value = Type;
                        param2.ParameterName = "@Docrowid";
                        param2.Value = DocId;
                        param3.ParameterName = "@patientid";
                        param3.Value = PatientId;
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);

                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetDocOMPT(string Type, int DocId, int PatientId)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocOMPTTests";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter();
                        SqlParameter param2 = new SqlParameter(); SqlParameter param3 = new SqlParameter();
                        param1.ParameterName = "@Type";
                        param1.Value = Type;
                        param2.ParameterName = "@Docrowid";
                        param2.Value = DocId;
                        param3.ParameterName = "@patientid";
                        param3.Value = PatientId;
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);

                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetDocFuncCharac(int DocId, int PatientId, string NoteType)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocFuncCharac";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter();
                        SqlParameter param2 = new SqlParameter(); SqlParameter param3 = new SqlParameter();

                        param1.ParameterName = "@Docrowid";
                        param1.Value = DocId;
                        param2.ParameterName = "@patientid";
                        param2.Value = PatientId;
                        param3.ParameterName = "@NoteType";
                        param3.Value = NoteType;
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);

                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetPatGoals(int PatientId, string GoalType)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetPatGoals";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter();
                        SqlParameter param2 = new SqlParameter(); SqlParameter param3 = new SqlParameter();

                        param1.ParameterName = "@Patrowid";
                        param1.Value = PatientId;
                        param2.ParameterName = "@GoalType";
                        param2.Value = GoalType;
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);

                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetDocProInterventions(int DocrowId)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocProInterventions";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter();
                        param1.ParameterName = "@Docrowid";
                        param1.Value = DocrowId;

                        cmd.Parameters.Add(param1);

                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetDocProInterventionsFD(int DocrowId)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocProInterventionsFD";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter();
                        param1.ParameterName = "@Docrowid";
                        param1.Value = DocrowId;

                        cmd.Parameters.Add(param1);

                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetDocMissed(int Docrowid, int PatientID)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocMissedNote";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter();
                        SqlParameter param2 = new SqlParameter();
                        param1.ParameterName = "@docrowid";
                        param1.Value = Docrowid;
                        param2.ParameterName = "@patientid";
                        param2.Value = PatientID;

                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);

                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetDocNoteSummary(int Docrowid)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocNoteSummary";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@Docrowid";
                        param.Value = Docrowid;
                        cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetDocTreatDescR(int Docrowid)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocTreatDescR";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@Docrowid";
                        param.Value = Docrowid;
                        cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetDocDisSummary(int Docrowid)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocDisSummary";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@docrowid";
                        param.Value = Docrowid;
                        cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet IUDDocMaster(string dFlag, int docrowid, int PTrowid, string NoteType, int TreatingTherapistId,
            string CreatedDate, string DateOfService, string Signed, string SignedDate, string PDFName, string AuthorizedBy, string User)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("dFlag", dFlag);
                        SqlParameter param2 = new SqlParameter("docrowid", docrowid);
                        SqlParameter param3 = new SqlParameter("PTrowid", PTrowid);
                        SqlParameter param4 = new SqlParameter("NoteType", NoteType);
                        SqlParameter param5 = new SqlParameter("TreatingTherapistId", TreatingTherapistId);
                        SqlParameter param6 = new SqlParameter("CreatedDate", CreatedDate);
                        SqlParameter param7 = new SqlParameter("DateOfService", DateOfService);
                        SqlParameter param8 = new SqlParameter("Signed", Signed);
                        SqlParameter param9 = new SqlParameter("SignedDate", SignedDate);
                        SqlParameter param10 = new SqlParameter("PDFName", PDFName);
                        SqlParameter param11 = new SqlParameter("AuthorizedBy", AuthorizedBy);
                        SqlParameter param12 = new SqlParameter("User", User);
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);
                        cmd.Parameters.Add(param5);
                        cmd.Parameters.Add(param6);
                        cmd.Parameters.Add(param7);
                        cmd.Parameters.Add(param8);
                        cmd.Parameters.Add(param9);
                        cmd.Parameters.Add(param10);
                        cmd.Parameters.Add(param11);
                        cmd.Parameters.Add(param12);
                        cmd.CommandTimeout = 0;
                        if (dFlag == "I")
                        {
                            cmd.CommandText = "SP_IDocMaster";
                            adpt = new SqlDataAdapter(cmd);
                            adpt.Fill(ds);
                        }

                        else
                        {
                            cmd.CommandText = "SP_UDocMaster";
                            adpt = new SqlDataAdapter(cmd);
                            adpt.Fill(ds);
                        }

                    }

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;

        }


        public DataSet DocSigned(int Docrowid, string PDFName)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Update tbldocmaster set Signed = 1, PDFName = '" + PDFName + "' where docrowid = " + Docrowid;
                        cmd.CommandType = System.Data.CommandType.Text;
                        //SqlParameter param = new SqlParameter();
                        //param.ParameterName = "@docrowid";
                        //param.Value = Docrowid;
                        //cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet ExecuteDataset(string strSql)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {

                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    SqlDataAdapter adpt;

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(strSql, con))
                    {
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet GetPatWorkComp(int PatientId)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetPatWorkComp";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@PTrowid";
                        param.Value = PatientId;
                        cmd.Parameters.Add(param);
                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }


        public DataTable GetPatReferral(int docrowid, int patientId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    System.Data.Common.DbDataReader sqlReader;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetPatReferral";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter param1 = new SqlParameter();
                        SqlParameter param2 = new SqlParameter();

                        param1.ParameterName = "@Docrowid";
                        param1.Value = docrowid;

                        param2.ParameterName = "@Patientid";
                        param2.Value = patientId;
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.CommandTimeout = 0;
                        sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return dt;
        }


        public int ExecuteNonQuery(string strSql)
        {
            int rowsAffected;
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(strSql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        // cmd.Parameters.AddWithValue("@Name", name);
                        // cmd.Parameters.AddWithValue("@City", city);
                        // con.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                        con.Close();


                    }
                    // con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return rowsAffected;
        }

        public DataSet GetPatInsuranceNote(int PatientId, int DocRowId)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetPatInsuranceNote";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter();
                        param1.ParameterName = "@PTrowid";
                        param1.Value = PatientId;
                        SqlParameter param2 = new SqlParameter();
                        param2.ParameterName = "@Docrowid";
                        param2.Value = DocRowId;
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);

                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        public DataSet IUDPatient(string dFlag, int Prowid, string ClinicNo, string Title, string FirstName
                                      , string LastName, string NickName, string Gender, string BirthDate, string ReferralDate
                                      , string InjuryDate, string DischargeDate, string ReferralSource, bool PostOp
                                      , string Note, string Address1, string Address2, string City, string State
                                      , string ZipCode, string HomePh, bool HomePhO, bool HomePhB, string WorkPh
                                      , bool WorkPhO, bool WorkPhB, string CellPh, bool CellPhO, bool CellPhB
                                      , string OtherPh, bool OtherPhO, bool OtherPhB, string Email, bool NoEmail
                                      , bool NoMarketingEmail, string User, string TherapistID, string firstvisitdate
                                      , string MiddleInitial, string Race, string Discipline, bool PTSLPCapMet, bool OTCapMet
                                      , string ReferralSource2, string ReferralSource3, bool NoSMS)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param0 = new SqlParameter("dFlag", dFlag);
                        SqlParameter param1 = new SqlParameter("Prowid", Prowid);
                        SqlParameter param2 = new SqlParameter("ClinicNo", ClinicNo);
                        SqlParameter param3 = new SqlParameter("Title", Convert.ToString(Title));
                        SqlParameter param4 = new SqlParameter("FirstName", Convert.ToString(FirstName));
                        SqlParameter param5 = new SqlParameter("LastName", Convert.ToString(LastName));
                        SqlParameter param6 = new SqlParameter("NickName", Convert.ToString(NickName));
                        SqlParameter param7 = new SqlParameter("Gender", Convert.ToString(Gender));
                        SqlParameter param8 = new SqlParameter("BirthDate", Convert.ToString(BirthDate));
                        SqlParameter param9 = new SqlParameter("ReferralDate", Convert.ToString(ReferralDate));
                        SqlParameter param10 = new SqlParameter("InjuryDate", Convert.ToString(InjuryDate));
                        SqlParameter param11 = new SqlParameter("DischargeDate", Convert.ToString(DischargeDate));
                        SqlParameter param12 = new SqlParameter("ReferralSource", Convert.ToString(ReferralSource));
                        SqlParameter param13 = new SqlParameter("PostOp", PostOp);
                        SqlParameter param14 = new SqlParameter("Note", Convert.ToString(Note));
                        SqlParameter param15 = new SqlParameter("Address1", Convert.ToString(Address1));
                        SqlParameter param16 = new SqlParameter("Address2", Convert.ToString(Address2));
                        SqlParameter param17 = new SqlParameter("City", Convert.ToString(City));
                        SqlParameter param18 = new SqlParameter("State", Convert.ToString(State));
                        SqlParameter param19 = new SqlParameter("ZipCode", Convert.ToString(ZipCode.Replace("_", "").Replace("-", "")));
                        SqlParameter param20 = new SqlParameter("HomePh", Convert.ToString(HomePh.Replace("_", "").Replace("-", "")));
                        SqlParameter param21 = new SqlParameter("HomePhO", HomePhO);
                        SqlParameter param22 = new SqlParameter("HomePhB", HomePhB);
                        SqlParameter param23 = new SqlParameter("WorkPh", WorkPh.Replace("_", "").Replace("-", ""));
                        SqlParameter param24 = new SqlParameter("WorkPhO", WorkPhO);
                        SqlParameter param25 = new SqlParameter("WorkPhB", WorkPhB);
                        SqlParameter param26 = new SqlParameter("CellPh", Convert.ToString(CellPh.Replace("_", "").Replace("-", "")));
                        SqlParameter param27 = new SqlParameter("CellPhO", CellPhO);
                        SqlParameter param28 = new SqlParameter("CellPhB", CellPhB);
                        SqlParameter param29 = new SqlParameter("OtherPh", Convert.ToString(OtherPh.Replace("_", "").Replace("-", "")));
                        SqlParameter param30 = new SqlParameter("OtherPhO", OtherPhO);
                        SqlParameter param31 = new SqlParameter("OtherPhB", OtherPhB);
                        SqlParameter param32 = new SqlParameter("Email", Convert.ToString(Email));
                        SqlParameter param33 = new SqlParameter("NoEmail", NoEmail);
                        SqlParameter param34 = new SqlParameter("NoMarketingEmail", NoMarketingEmail);
                        SqlParameter param35 = new SqlParameter("User", Convert.ToString(User));
                        SqlParameter param36 = new SqlParameter("TherapistID", Convert.ToString(TherapistID));
                        SqlParameter param37 = new SqlParameter("firstvisitdate", Convert.ToString(firstvisitdate));
                        SqlParameter param38 = new SqlParameter("MiddleInitial", Convert.ToString(MiddleInitial));
                        SqlParameter param39 = new SqlParameter("Race", Convert.ToString(Race));
                        SqlParameter param40 = new SqlParameter("Discipline", Convert.ToString(Discipline));
                        SqlParameter param41 = new SqlParameter("PTSLPCapMet", PTSLPCapMet);
                        SqlParameter param42 = new SqlParameter("OTCapMet", OTCapMet);
                        SqlParameter param43 = new SqlParameter("ReferralSource2", Convert.ToString(ReferralSource2));
                        SqlParameter param44 = new SqlParameter("ReferralSource3", Convert.ToString(ReferralSource3));

                        SqlParameter param45 = new SqlParameter("ApptRemind", NoSMS);
                        SqlParameter param46 = new SqlParameter("ApptRemindEval", NoSMS);
                        cmd.Parameters.Add(param0);

                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);
                        cmd.Parameters.Add(param5);
                        cmd.Parameters.Add(param6);
                        cmd.Parameters.Add(param7);
                        cmd.Parameters.Add(param8);
                        cmd.Parameters.Add(param9);
                        cmd.Parameters.Add(param10);
                        cmd.Parameters.Add(param11);
                        cmd.Parameters.Add(param12);
                        cmd.Parameters.Add(param13);
                        cmd.Parameters.Add(param14);
                        cmd.Parameters.Add(param15);
                        cmd.Parameters.Add(param16);
                        cmd.Parameters.Add(param18);
                        cmd.Parameters.Add(param19);
                        cmd.Parameters.Add(param20);
                        cmd.Parameters.Add(param21);
                        cmd.Parameters.Add(param22);
                        cmd.Parameters.Add(param23);
                        cmd.Parameters.Add(param24);
                        cmd.Parameters.Add(param25);
                        cmd.Parameters.Add(param26);
                        cmd.Parameters.Add(param27);
                        cmd.Parameters.Add(param28);
                        cmd.Parameters.Add(param29);
                        cmd.Parameters.Add(param30);
                        cmd.Parameters.Add(param31);
                        cmd.Parameters.Add(param32);
                        cmd.Parameters.Add(param33);
                        cmd.Parameters.Add(param34);
                        cmd.Parameters.Add(param35);
                        cmd.Parameters.Add(param36);
                        cmd.Parameters.Add(param37);
                        cmd.Parameters.Add(param38);
                        cmd.Parameters.Add(param39);
                        cmd.Parameters.Add(param40);
                        cmd.Parameters.Add(param41);
                        cmd.Parameters.Add(param42);
                        cmd.Parameters.Add(param43);
                        cmd.Parameters.Add(param44);
                        cmd.Parameters.Add(param45);
                        cmd.Parameters.Add(param46);


                        cmd.CommandTimeout = 0;
                        if (dFlag == "I")
                        {
                            cmd.CommandText = "SP_IDocMaster";
                            adpt = new SqlDataAdapter(cmd);
                            adpt.Fill(ds);
                        }

                        else
                        {
                            cmd.CommandText = "SP_UDocMaster";
                            adpt = new SqlDataAdapter(cmd);
                            adpt.Fill(ds);
                        }

                    }

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;

        }

        public DataSet GetDocMaster(int PatientId)
        {
            DataSet ds = new DataSet();
            try
            {
                using (RehabEntities context = new RehabEntities())
                {
                    string ConnectionString = context.Database.Connection.ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2500;
                    SqlConnection con = new SqlConnection(builder.ConnectionString);
                    // System.Data.Common.DbDataReader sqlReader;
                    SqlDataAdapter adpt;
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SP_GetDocMaster";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter();
                        SqlParameter param2 = new SqlParameter(); SqlParameter param3 = new SqlParameter();

                        param1.ParameterName = "@Patientid";
                        param1.Value = PatientId;

                        cmd.Parameters.Add(param1);


                        cmd.CommandTimeout = 0;
                        adpt = new SqlDataAdapter(cmd);
                        adpt.Fill(ds);
                        //sqlReader = (System.Data.Common.DbDataReader)cmd.ExecuteReader();

                        // dt.Load(sqlReader);

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return ds;
        }

        //public int IUDDocMaster(string dFlag, string docrowid , string PTrowid, string NoteType,
        //                    string TreatingTherapistId, string CreatedDate, string DateOfService,
        //                    string Signed, string SignedDate, string PDFName, string AuthorizedBy,
        //                    string User)
        //{

        //    int count;
        //    try
        //    {
        //        using (RehabEntities context = new RehabEntities())
        //        {
        //            string ConnectionString = context.Database.Connection.ConnectionString;
        //            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
        //            builder.ConnectTimeout = 2500;
        //            SqlConnection con = new SqlConnection(builder.ConnectionString);

        //            con.Open();
        //            using (SqlCommand cmd = con.CreateCommand())
        //            {
        //                cmd.CommandText = "SP_UDocMaster";
        //                cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //                SqlParameter param1 = new SqlParameter();
        //                SqlParameter param2 = new SqlParameter();
        //                SqlParameter param3 = new SqlParameter();
        //                SqlParameter param4 = new SqlParameter();

        //                SqlParameter param5 = new SqlParameter();
        //                SqlParameter param6 = new SqlParameter();
        //                SqlParameter param7 = new SqlParameter();
        //                SqlParameter param8 = new SqlParameter();

        //                SqlParameter param9 = new SqlParameter();
        //                SqlParameter param10 = new SqlParameter();
        //                SqlParameter param11= new SqlParameter();
        //                SqlParameter param12 = new SqlParameter();

        //                param1.ParameterName = "@dFlag";
        //                param1.Value = dFlag;
        //                param2.ParameterName = "@docrowid";
        //                param2.Value = docrowid;

        //                param1.ParameterName = "@PTrowid";
        //                param1.Value = PTrowid;
        //                param2.ParameterName = "@NoteType";
        //                param2.Value = NoteType;

        //                param1.ParameterName = "@TreatingTherapistId";
        //                param1.Value = TreatingTherapistId;
        //                param2.ParameterName = "@CreatedDate";
        //                param2.Value = CreatedDate;

        //                param1.ParameterName = "@DateOfService";
        //                param1.Value = DateOfService;
        //                param2.ParameterName = "@Signed";
        //                param2.Value = Signed;

        //                param1.ParameterName = "@SignedDate";
        //                param1.Value = SignedDate;
        //                param2.ParameterName = "@PDFName";
        //                param2.Value = PDFName;

        //                param1.ParameterName = "@AuthorizedBy";
        //                param1.Value = AuthorizedBy;
        //                param2.ParameterName = "@User";
        //                param2.Value = User;

        //                cmd.Parameters.Add(param1);
        //                cmd.Parameters.Add(param2);
        //                cmd.Parameters.Add(param3);
        //                cmd.Parameters.Add(param4);
        //                cmd.Parameters.Add(param5);
        //                cmd.Parameters.Add(param6);
        //                cmd.Parameters.Add(param7);
        //                cmd.Parameters.Add(param8);
        //                cmd.Parameters.Add(param9);
        //                cmd.Parameters.Add(param10);
        //                cmd.Parameters.Add(param11);
        //                cmd.Parameters.Add(param12);

        //                count = Convert.ToInt32(cmd.ExecuteScalar());

        //            }
        //            con.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //    return count;
        //}

      
        }

   

}

