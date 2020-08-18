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
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Web;


namespace Team.Rehab.Repository.Data
{
  public  class UnSignNotes
    {
        public string ValidateDocs(int Patientid, int type)
        {
            string NoteType = Convert.ToString(type);
            DocManager objDocManager = new Data.DocManager();
            string message = string.Empty;
            DataSet ds;
            string noteType = string.Empty;
            try
            {
                ds = objDocManager.GetDocMaster(Patientid);
                string[] noteflag = new string[8];
                for (var i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                {
                    noteType = ds.Tables[0].Rows[i]["NoteDet"].ToStringOrEmpty();
                    switch (noteType)
                    {
                        case "Initial Eval":
                            {
                                noteflag[0] = "1";
                                break;
                            }

                        case "Initial Eval2":
                            {
                                noteflag[1] = "1";
                                break;
                            }

                        case "Daily Note":
                            {
                                noteflag[2] = "1";
                                break;
                            }

                        case "FCE":
                            {
                                noteflag[3] = "1";
                                break;
                            }
                    }

                    if (NoteType == "PPOC")
                    {
                        if (noteflag[3] == "1")
                        {
                            //message = "This is a Work Conditioning Patient - Cannot open Eval."
                            return message = objDocManager.GetErrorMsg("WCPEval");

                        }
                    }

                    if (NoteType == "PFCE")
                    {
                        if (noteflag[0] == "1" | noteflag[1] == "1")
                        {
                            //message = "Eval exists for the Patient, Open a New Chart for FCE."
                            return message = objDocManager.GetErrorMsg("WCPEvalExist");
                            HttpContext.Current.Response.Redirect("DocMaster.aspx?PatId=" + Patientid, false);
                        }
                    }

                    if (NoteType == "PTREAT")
                    {
                        if (ds.Tables[0].Rows[i]["NoteDet"].ToStringOrEmpty() == "Initial Eval" | ds.Tables[0].Rows[i]["NoteDet"].ToStringOrEmpty() == "Initial Eval2" | ds.Tables[0].Rows[i]["NoteDet"].ToStringOrEmpty() == "FCE" | ds.Tables[0].Rows[i]["NoteDet"].ToStringOrEmpty() == "Medical Necessity")
                        {
                            if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Signed"]) == false)
                            {
                                //message = "Initial Evaluation or Initial Evaluation 2 or FCE or Medical Necessity Notes are not signed, hence Daily Note will not be created."
                                return message = objDocManager.GetErrorMsg("SignNoDaily");

                            }
                        }
                    }

                    // If NoteType = "PPOC2" Then
                    // If ds.Tables[0].Rows[i].Item("NoteDet") = "Re-Eval" Or _
                    // ds.Tables[0].Rows[i].Item("NoteDet") = "Daily Note" Or _
                    // ds.Tables[0].Rows[i].Item("NoteDet") = "Discharge" Or _
                    // ds.Tables[0].Rows[i].Item("NoteDet") = "Medical Necessity" Then
                    // If HttpContext.Current.Session("Docrowid") = Nothing Or HttpContext.Current.Session("Docrowid") = 0 Then
                    //message = "Initial Evaluation 2 cannot be created as Re-Eval, Daily, Medical Necessity notes are found"
                    // HttpContext.Current.Response.Redirect("DocMaster.aspx?patientid=" & Patientid, False)
                    // Else
                    // End If
                    // End If
                    // End If

                    if (NoteType == "PMN")
                    {
                        if (ds.Tables[0].Rows[i]["NoteDet"].ToStringOrEmpty() == "Initial Eval" | ds.Tables[0].Rows[i]["NoteDet"].ToStringOrEmpty() == "Initial Eval2" | ds.Tables[0].Rows[i]["NoteDet"].ToStringOrEmpty() == "FCE")
                        {
                            // ds.Tables[0].Rows[i].Item("NoteDet") = "Daily Note" Or _ or Daily 
                            if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Signed"]) == false)
                            {
                                //message = "Initial Evaluation or Initial Evaluation 2 or FCE Notes are not signed, hence Medical Necessity Note will not be created or updated."
                                return message = objDocManager.GetErrorMsg("SignNoMed");

                            }
                        }
                    }

                    if (NoteType == "PDIS")
                    {
                        if (ds.Tables[0].Rows[i]["NoteDet"].ToStringOrEmpty() == "Initial Eval" | ds.Tables[0].Rows[i]["NoteDet"].ToStringOrEmpty() == "Initial Eval2" | ds.Tables[0].Rows[i]["NoteDet"].ToStringOrEmpty() == "FCE" | ds.Tables[0].Rows[i]["NoteDet"].ToStringOrEmpty() == "Daily Note" | ds.Tables[0].Rows[i]["NoteDet"].ToStringOrEmpty() == "Medical Necessity")
                        {
                            if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Signed"]) == false)
                            {
                                //message = "Initial Evaluation or Initial Evaluation 2 or FCE or Daily Notes are not signed, hence Discharge Note will not be created or updated."
                                return message = objDocManager.GetErrorMsg("SignNoDis");

                            }
                        }
                    }

                    if (ds.Tables[0].Rows[i]["NoteDet"].ToStringOrEmpty() == "Discharge")
                    {
                        if (NoteType == "PDIS")
                        {
                        }
                        else if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Signed"]) == true)
                        {
                            //message = "Patient is discharged, hence other notes can't be created or updated."
                            return message = objDocManager.GetErrorMsg("PatDischarge");

                        }
                    }
                }

                if (NoteType == "PTREAT")
                {
                    if (noteflag[0] == "1" | noteflag[1] == "1" | noteflag[3] == "1")
                    {
                    }
                    else
                    {
                        //message = "Initial Evaluation or Initial Evaluation 2 or PFC Notes not found, hence Daily Note will not be created."
                        return message = objDocManager.GetErrorMsg("NoDailyCre");

                    }
                }

                // If NoteType = "PMN" Then
                // If (noteflag(0) = "1" Or noteflag(1) = "1") And noteflag(2) = "1" Then
                // Else
                //message = "Initial Evaluation or Daily not found, hence Medical Necessity Note will not be created."
                // HttpContext.Current.Response.Redirect("DocMaster.aspx?patientid=" & Patientid)
                // End If
                // End If

                int j = 0;
                if (NoteType == "PTREAT")
                {
                    for (var i = ds.Tables[0].Rows.Count - 1; i >= 0; i += -1)
                    {
                        noteType = ds.Tables[0].Rows[i]["NoteDet"].ToStringOrEmpty();
                        switch (noteType)
                        {
                            case "Discharge":
                                {
                                    if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Signed"]) == true)
                                    {
                                        //message = "Patient is discharged, hence note cannot not be created."
                                        return message = objDocManager.GetErrorMsg("PatDischarge");

                                    }

                                    break;
                                }

                            case "Daily":
                                {
                                    if (ds.Tables[0].Rows[i - 1]["NoteDet"].ToStringOrEmpty() == "Daily")
                                    {
                                        //message = "2 Daily Notes found, hence pl create Medical Necessity Note."
                                        return message = objDocManager.GetErrorMsg("2DailyFound");

                                    }

                                    break;
                                }
                        }


                        ds.Tables[0].Rows[i]["NoteDet"] = "Daily Note";

                    }
                }
            }
            catch (Exception ex)
            {
                CustomLogger.SendExcepToDB(ex, "Data/UnSignNotes/ValidateDocs");
                throw;
            }
         
                return message;
            
        }
    }
}
