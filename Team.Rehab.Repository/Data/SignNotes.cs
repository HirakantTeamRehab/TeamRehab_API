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

    public class SignNotes
    {
        string rptSavePath1;
        Team.Rehab.DataModel.RehabEntities rehab = new RehabEntities();

        public string SignIE(string Type, int docrowid, int patientId, string noteType)
        {
            rptSavePath1 = string.Empty;
            string userName = string.Empty;
            ITextMgrNotes objITextMgrNotes = new Data.ITextMgrNotes();
            if (HttpContext.Current != null && HttpContext.Current.User != null
                    && HttpContext.Current.User.Identity.Name != null)
            {
                userName = HttpContext.Current.User.Identity.Name;
                //  userId = HttpContext.Current.User.Identity.GetUserId();
            }
            //   userName = "A123";
            // string userRole = "System Administrator";
            string userRole = string.Empty;
            string SignPrint = "btnSign";
            DocManager objDocManager = new DocManager();
            try
            {


                DataSet dschk;

                if (Type == "Preview")
                {
                }
                else
                {
                    List<MSFunctionalCharEntity> msFunctionalCharEntity = new List<MSFunctionalCharEntity>();
                    msFunctionalCharEntity = objDocManager.GetDocFuncCharacR(docrowid);
                    if (msFunctionalCharEntity.Count == 0)
                    {
                        return objDocManager.GetErrorMsg("FuncChar");
                    }

                }
                if (docrowid < 0)
                {
                    return objDocManager.GetErrorMsg("SaveSec");
                }
                List<IniEvalNoteCheckEntity> chkIECNoteEntity = new List<IniEvalNoteCheckEntity>();
                chkIECNoteEntity = objDocManager.chkIECNote(patientId, docrowid);
                if (chkIECNoteEntity.Count() == 1)
                {

                }
                else { return objDocManager.GetErrorMsg("SetGol"); }
                string strfvd = string.Empty;

                strfvd = "		Update tblPatients Set firstvisitdate = DM.DateOfService " +
                         " from tblPatients P " +
                        " Inner Join tbldocmaster DM on P.Prowid = DM.PTrowid " +
                        " where firstvisitdate is NULL or firstvisitdate = '' " +
                        " and DM.NoteType in ( 'PPOC', 'PFCE' ) " +
                        " and P.Prowid = " + patientId;

                int rowAffected = objDocManager.ExecuteNonQuery(strfvd);
                if (docrowid < 0)
                {
                    return objDocManager.GetErrorMsg("Seldoc");
                }
                DataTable dschkNote = new DataTable();
                dschkNote = objDocManager.GetPatReferral(docrowid, patientId);
                if (dschkNote.Rows.Count == 0)
                {
                    return objDocManager.GetErrorMsg("NoRef");
                }
                string strSQL = string.Empty;
                string tmpstr = string.Empty;

                strSQL = " select Coalesce(ReferralSource, '') as ReferralSource, Coalesce(ReferralSource2, '') as ReferralSource2 " +
                    " , Coalesce(ReferralSource3, '') as ReferralSource3 from tblDocMaster " +
                    " where PTrowid = " + patientId +
                    " and docrowid = " + docrowid;
                DataTable dt = new DataTable();
                dt = objDocManager.DTByDataAdapter(strSQL);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["ReferralSource"].ToString() != "")
                    {

                        rptSavePath1 = objITextMgrNotes.PrintiText(Type, SignPrint, 1, Convert.ToInt32(dt.Rows[0]["ReferralSource"]), patientId, docrowid, noteType, userName, userRole);

                    }
                    if (dt.Rows[0]["ReferralSource2"].ToString() != "")
                    {

                        rptSavePath1 = objITextMgrNotes.PrintiText(Type, SignPrint, 2, Convert.ToInt32(dt.Rows[0]["ReferralSource2"]), patientId, docrowid, noteType, userName, userRole);

                    }
                    if (dt.Rows[0]["ReferralSource3"].ToString() != "")
                    {

                        rptSavePath1 = objITextMgrNotes.PrintiText(Type, SignPrint, 3, Convert.ToInt32(dt.Rows[0]["ReferralSource3"]), patientId, docrowid, noteType, userName, userRole);

                    }
                    else
                    {
                        rptSavePath1 = objITextMgrNotes.PrintiText(Type, SignPrint, 1, Convert.ToInt32(dt.Rows[0]["ReferralSource"]), patientId, docrowid, noteType, userName, userRole);

                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                CustomLogger.SendExcepToDB(ex, "Data/SignNotes/SignIE");
            }
            return rptSavePath1;
        }

        public string SignFCE(string Type, int docrowid, int patientId, string noteType)
        {
            DocManager objDocManager = new DocManager(); string userName = string.Empty;
            ITextMgrNotes objITextMgrNotes = new Data.ITextMgrNotes();
            if (HttpContext.Current != null && HttpContext.Current.User != null
                    && HttpContext.Current.User.Identity.Name != null)
            {
                userName = HttpContext.Current.User.Identity.Name;
                //  userId = HttpContext.Current.User.Identity.GetUserId();
            }
            userName = "A123";
            string userRole = "System Administrator";
            string SignPrint = "btnSign";
            rptSavePath1 = string.Empty;
            try
            {
                if (docrowid < 0)
                {
                    return "please provide the note id to sign";
                }
                DataTable dschkNote = new DataTable();
                dschkNote = objDocManager.GetPatReferral(docrowid, patientId);
                if (dschkNote.Rows.Count == 0)
                {
                    return objDocManager.GetErrorMsg("NoRef");
                }
                string strfvd = string.Empty;

                strfvd = "		Update tblPatients Set firstvisitdate = DM.DateOfService " +
                         " from tblPatients P " +
                        " Inner Join tbldocmaster DM on P.Prowid = DM.PTrowid " +
                        " where firstvisitdate is NULL or firstvisitdate = '' " +
                        " and DM.NoteType in ( 'PPOC', 'PFCE' ) " +
                        " and P.Prowid = " + patientId;

                int rowAffected = objDocManager.ExecuteNonQuery(strfvd);
                if (docrowid < 0)
                {
                    return objDocManager.GetErrorMsg("Seldoc");
                }

                string strSQL = string.Empty;
                string tmpstr = string.Empty;

                strSQL = " select Coalesce(ReferralSource, '') as ReferralSource, Coalesce(ReferralSource2, '') as ReferralSource2 " +
                    " , Coalesce(ReferralSource3, '') as ReferralSource3 from tblDocMaster " +
                    " where PTrowid = " + patientId +
                    " and docrowid = " + docrowid;
                DataTable dt = new DataTable();
                dt = objDocManager.DTByDataAdapter(strSQL);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["ReferralSource"].ToString() != "")
                    {

                        rptSavePath1 = objITextMgrNotes.PrintiText(Type, SignPrint, 1, Convert.ToInt32(dt.Rows[0]["ReferralSource"]), patientId, docrowid, noteType, userName, userRole);

                    }
                    if (dt.Rows[0]["ReferralSource2"].ToString() != "")
                    {

                        rptSavePath1 = objITextMgrNotes.PrintiText(Type, SignPrint, 2, Convert.ToInt32(dt.Rows[0]["ReferralSource2"]), patientId, docrowid, noteType, userName, userRole);

                    }
                    if (dt.Rows[0]["ReferralSource3"].ToString() != "")
                    {

                        rptSavePath1 = objITextMgrNotes.PrintiText(Type, SignPrint, 3, Convert.ToInt32(dt.Rows[0]["ReferralSource3"]), patientId, docrowid, noteType, userName, userRole);

                    }
                    else
                    {
                        rptSavePath1 = objITextMgrNotes.PrintiText(Type, SignPrint, 1, Convert.ToInt32(dt.Rows[0]["ReferralSource"]), patientId, docrowid, noteType, userName, userRole);

                    }
                }
            }
            catch (Exception ex)
            {
                CustomLogger.SendExcepToDB(ex, "Data/SignNotes/SignFCE");
                throw;
            }
            return rptSavePath1;
        }

        public string SignDaily(string Type, int docRowId, int PatientId, string NoteType)
        {
            string SignPrint = "btnSign";
            try
            {
                DataSet dschk = new DataSet();
                // DBHelper db = new DBHelper();
                DocManager db = new DocManager();

                ITextMgrNotes iTextMgrNotes = new ITextMgrNotes();

                if (docRowId.ToStringOrEmpty() == "")
                {
                    // lblMErrorMsg.Text = "Note not created, pl save a section to create a note"
                    return db.GetErrorMsg("SaveSec");
                }

                if (Type == "Preview")
                {
                }
                else
                {
                    dschk = db.GetDocSumInterventions(docRowId);
                    if (dschk.Tables[0].Rows.Count == 0)
                    {
                        // lblMErrorMsg.Text = "Functional Characteristics required for signing this note."
                        return db.GetErrorMsg("FuncChar");
                    }
                    dschk = db.GetDocSumInterventions(docRowId);
                    if (dschk.Tables[0].Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dschk.Tables[0].Rows[0]["totalunits"]) == 0)
                        {
                            // lblMErrorMsg.Text = "Charges / Interventions required for signing this note."
                            return db.GetErrorMsg("ReqChgIntv");
                        }
                    }
                    else
                    {
                        // lblMErrorMsg.Text = "Charges / Interventions required for signing this note."
                        return db.GetErrorMsg("ReqChgIntv");
                    }
                }

                DataTable dtPI = new DataTable();
                dtPI = db.GetPatInsuranceNote(PatientId, docRowId).Tables[0];
                if (dtPI.Rows.Count == 0)
                    HttpContext.Current.Session["instype"] = "";
                else
                    HttpContext.Current.Session["instype"] = dtPI.Rows[0]["AccountTypeCode"].ToStringOrEmpty();

                DataTable dt = new DataTable();
                dt = db.GetDocRInterventions(docRowId).Tables[0];
                DataTable dtFR = new DataTable();
                if (HttpContext.Current.Session["instype"].ToStringOrEmpty() == "MC" | HttpContext.Current.Session["instype"].ToStringOrEmpty() == "MI"
                    | HttpContext.Current.Session["instype"].ToStringOrEmpty() == "MP" | HttpContext.Current.Session["instype"].ToStringOrEmpty() == "MO"
                    | HttpContext.Current.Session["instype"].ToStringOrEmpty() == "MN")
                {
                    for (var i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        if ((dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97001" &
                            System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) ||
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97161" &
                            System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) ||
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97162" &
                            System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) ||
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97163" &
                            System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) ||
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97003" &
                            System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) ||
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97164" &
                            System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) |
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97362" & System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) |
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97002" & System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) |
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97165" & System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) |
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97166" & System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) |
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97167" & System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) |
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97168" & System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) |
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97004" & System.Convert.ToInt32(dt.Rows[i]["units"]) > 0))
                        {
                            string strSQL;
                            DataSet FRcomp = new DataSet();
                            strSQL = " Select ISNULL(FRComp, 'N') as FRComp from tblDocMaster where docrowid =  " +
                                docRowId;
                            FRcomp = db.ExecuteDataset(strSQL);
                            if (FRcomp.Tables.Count > 0 && FRcomp.Tables[0].Rows.Count > 0 && FRcomp.Tables[0].Rows[0]["FRComp"].ToStringOrEmpty() == "N")
                            {
                                // lblMErrorMsg.Text = "Functional Reporting incomplete, hence unable to sign the note."
                                return db.GetErrorMsg("SelFnRep");
                            }
                        }
                    }
                }

                string strSQL1;
                string tmpstr = "";
                strSQL1 = " select Coalesce(ReferralSource, '') as ReferralSource, Coalesce(ReferralSource2, '') " +
                    "as ReferralSource2 " + " , Coalesce(ReferralSource3, '') as ReferralSource3 from tblDocMaster " + " " +
                    "where PTrowid = " + PatientId.ToStringOrEmpty() + " and docrowid = " + docRowId.ToStringOrEmpty();

                DataTable dt1 = new DataTable();
                dt1 = db.ExecuteDataset(strSQL1).Tables[0];
                if (dt1.Rows.Count > 0)
                {
                    if (dt1.Rows[0]["ReferralSource"].ToStringOrEmpty() != "")
                    {
                        rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 1, Convert.ToInt32(dt1.Rows[0]["ReferralSource"]),
                           PatientId, docRowId, NoteType, "", "");
                        if (SignPrint == "btnSignPrint" | Type == "Preview")
                            // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                            tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                    }
                }
                else
                {
                    rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 1, 0,
                        PatientId, docRowId, NoteType, "", "");
                    if (SignPrint == "btnSignPrint" | Type == "Preview")
                        // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                        tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                }

                if (Type == "Preview")
                {
                    tmpstr = tmpstr.TrimEnd('|');
                    //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "shownotecall", "PrintNote('" + tmpstr + "','');", true);
                }
                else if (tmpstr == "") { }

                // lblMSuccessMsg.Text = "Note created successfully"
                //Response.Redirect("DocMaster.aspx?PatId=" + this.Master.FindControl("ctl00$ctl00$hdnpatientid") as HiddenField.Value);
                else
                {
                    tmpstr = tmpstr.TrimEnd('|');
                    //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "shownotecall", "PrintNote('" + tmpstr + "'," + "\"DocMaster.aspx?PatId=" + this.Master.FindControl("ctl00$ctl00$hdnpatientid") as HiddenField.Value + "\");", true);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
                CustomLogger.SendExcepToDB(ex, "Data/SignNotes/Daily");
            }
            return rptSavePath1;
        }

        public string SignReEval(string Type, string SignPrint, int DocRowId, int PatientId, string DocNote)
        {
            try
            {
                DocManager db = new DocManager();
                DataSet dschk = new DataSet();
                ITextMgrNotes iTextMgrNotes = new ITextMgrNotes();
                if (Type == "Preview")
                {
                }
                else
                {
                    dschk = db.GetDocFuncCharacR(DocRowId, true);
                    if (dschk.Tables[0].Rows.Count == 0)
                    {
                        // lblMErrorMsg.Text = "Functional Characteristics required for signing this note."
                        return db.GetErrorMsg("FuncChar");
                    }
                }
                if (DocRowId.ToStringOrEmpty() == "")
                {
                    // lblMErrorMsg.Text = "Note not created, pl save a section to create a note"
                    return db.GetErrorMsg("SaveSec");
                }

                string strSQL;
                string tmpstr = "";
                strSQL = " select Coalesce(ReferralSource, '') as ReferralSource, Coalesce(ReferralSource2, '') as ReferralSource2 " + " , " +
                    "Coalesce(ReferralSource3, '') as ReferralSource3, CreateWCPDF from tblDocMaster DM " + "" +
                    " Left Outer Join tbldNoteSummary NS on NS.Docrowid = DM.Docrowid " + " where PTrowid = " +
                    PatientId + " " +
                    "and DM.docrowid = " + DocRowId;
                //DBHelper db = new DBHelper();
                DataTable dt = new DataTable();
                dt = db.ExecuteDataset(strSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["ReferralSource"].ToStringOrEmpty() != "")
                    {
                        if (dt.Rows[0]["CreateWCPDF"].ToStringOrEmpty() != "")
                        {
                            if (Convert.ToBoolean(dt.Rows[0]["CreateWCPDF"]) == true)
                                rptSavePath1 =
                                    iTextMgrNotes.PrintiTextWC(Type, SignPrint, 1, Convert.ToInt32(dt.Rows[0]["ReferralSource"]),
                                    PatientId, DocRowId,
                                    DocNote);
                            else
                                rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 1,
                                    Convert.ToInt32(dt.Rows[0]["ReferralSource"]), PatientId, DocRowId, DocNote, "", "");
                        }
                        else
                        {
                            rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 1,
                                  Convert.ToInt32(dt.Rows[0]["ReferralSource"]), PatientId, DocRowId, DocNote, "", "");

                        }


                        if (SignPrint == "btnSignPrint" | Type == "Preview")
                            // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                            tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                    }

                    if (dt.Rows[0]["ReferralSource2"].ToStringOrEmpty() != "")
                    {
                        if (Convert.ToBoolean(dt.Rows[0]["CreateWCPDF"]) == true)
                            rptSavePath1 = iTextMgrNotes.PrintiTextWC(Type, SignPrint, 2,
                                Convert.ToInt32(dt.Rows[0]["ReferralSource2"]), PatientId, DocRowId, DocNote);
                        else
                            rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 2, Convert.ToInt32(dt.Rows[0]["ReferralSource2"]),
                                PatientId, DocRowId, DocNote, "", "");

                        if (SignPrint == "btnSignPrint" | Type == "Preview")
                            // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                            tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                    }

                    if (dt.Rows[0]["ReferralSource3"].ToStringOrEmpty() != "")
                    {
                        if (Convert.ToBoolean(dt.Rows[0]["CreateWCPDF"]) == true)
                            rptSavePath1 = iTextMgrNotes.PrintiTextWC(Type, SignPrint, 3,
                                Convert.ToInt32(dt.Rows[0]["ReferralSource3"]), PatientId, DocRowId, DocNote);
                        else
                            rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 3,
                                Convert.ToInt32(dt.Rows[0]["ReferralSource3"]), PatientId, DocRowId, DocNote, "", "");

                        if (SignPrint == "btnSignPrint" | Type == "Preview")
                            // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                            tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                    }
                }
                else
                {
                    rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 1, 0,
                        PatientId, DocRowId, DocNote, "", "");
                    if (SignPrint == "btnSignPrint")
                        // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                        tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                }

                if (Type == "Preview")
                {
                    tmpstr = tmpstr.TrimEnd('|');
                    // ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "shownotecall", "PrintNote('" + tmpstr + "','');", true);
                }
                else if (tmpstr == "") { }
                // lblMSuccessMsg.Text = "Note created successfully"
                // Response.Redirect("DocMaster.aspx?PatId=" + this.Master.FindControl("ctl00$ctl00$hdnpatientid") as HiddenField.Value);
                else
                {
                    tmpstr = tmpstr.TrimEnd('|');
                    // ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "shownotecall", "PrintNote('" + tmpstr + "'," + "\"DocMaster.aspx?PatId=" + this.Master.FindControl("ctl00$ctl00$hdnpatientid") as HiddenField.Value + "\");", true);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                CustomLogger.SendExcepToDB(ex, "Data/SignNotes/SignReEval");
            }
            return rptSavePath1;
        }
        public string SignPhyComm(string Type, string SignPrint, int DocRowId, int PatientId, string DocNote)
        {
            try
            {
                DocManager db = new DocManager();
                ITextMgrNotes iTextMgrNotes = new ITextMgrNotes();
                if (DocRowId.ToStringOrEmpty() == "")
                {
                    // lblMErrorMsg.Text = "Note not created, pl save a section to create a note"
                    return db.GetErrorMsg("SaveSec");
                }

                string strSQL;
                string tmpstr = "";
                strSQL = " select Coalesce(ReferralSource, '') as ReferralSource, Coalesce(ReferralSource2, '') as ReferralSource2 " + " ," +
                    " Coalesce(ReferralSource3, '') as ReferralSource3 from tblDocMaster " + " where PTrowid = " +
                    PatientId + " and docrowid = " +
                    DocRowId;
                // DBHelper db = new DBHelper();
                DataTable dt = new DataTable();
                dt = db.ExecuteDataset(strSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["ReferralSource"].ToStringOrEmpty() != "")
                    {
                        rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 1,
                            Convert.ToInt32(dt.Rows[0]["ReferralSource"]), PatientId,
                            DocRowId, DocNote, "", "");
                        if (SignPrint == "btnSignPrint" | Type == "Preview")
                            // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                            tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                    }
                    if (dt.Rows[0]["ReferralSource2"].ToStringOrEmpty() != "")
                    {
                        rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 2, Convert.ToInt32(dt.Rows[0]["ReferralSource2"])
                            , PatientId, DocRowId, DocNote, "", "");
                        if (SignPrint == "btnSignPrint" | Type == "Preview")
                            // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                            tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                    }
                    if (dt.Rows[0]["ReferralSource3"].ToStringOrEmpty() != "")
                    {
                        rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 3,
                            Convert.ToInt32(dt.Rows[0]["ReferralSource3"]), PatientId, DocRowId, DocNote, "", "");
                        if (SignPrint == "btnSignPrint" | Type == "Preview")
                            // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                            tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                    }
                }
                else
                {
                    rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 1, 0,
                        PatientId, DocRowId, DocNote, "", "");
                    if (SignPrint == "btnSignPrint" | Type == "Preview")
                        // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                        tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                }

                if (Type == "Preview")
                {
                    tmpstr = tmpstr.TrimEnd('|');
                    //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "shownotecall", "PrintNote('" + tmpstr + "','');", true);
                }
                else if (tmpstr == "") { }
                // lblMSuccessMsg.Text = "Note created successfully"
                //Response.Redirect("DocMaster.aspx?PatId=" + this.Master.FindControl("ctl00$ctl00$hdnpatientid") as HiddenField.Value);
                else
                {
                    tmpstr = tmpstr.TrimEnd('|');
                    //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "shownotecall", "PrintNote('" + tmpstr + "'," + "\"DocMaster.aspx?PatId=" + this.Master.FindControl("ctl00$ctl00$hdnpatientid") as HiddenField.Value + "\");", true);
                }
            }
            catch (Exception ex)
            {
                CustomLogger.SendExcepToDB(ex, "Data/SignNotes/SignComm");
            }
            return rptSavePath1;
        }

        public string SignMissed(string Type, string SignPrint, int DocRowId, int PatientId, string DocNote)
        {

            try
            {
                DocManager db = new DocManager();

                ITextMgrNotes iTextMgrNotes = new ITextMgrNotes();
                if (DocRowId.ToStringOrEmpty() == "")
                {
                    // lblMErrorMsg.Text = "Note not created, pl save a section to create a note"
                    return db.GetErrorMsg("SaveSec");
                }

                string strSQL;
                string tmpstr = "";
                strSQL = " select Coalesce(ReferralSource, '') as ReferralSource, Coalesce(ReferralSource2, '') as ReferralSource2 " + " , " +
                    "Coalesce(ReferralSource3, '') as ReferralSource3 from tblDocMaster " + " where PTrowid = " + PatientId + " and docrowid = " + DocRowId;
                // DBHelper db = new DBHelper();
                DataTable dt = new DataTable();
                dt = db.ExecuteDataset(strSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["ReferralSource"].ToStringOrEmpty() != "")
                    {
                        rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 1,
                            Convert.ToInt32(dt.Rows[0]["ReferralSource"]), PatientId, DocRowId, DocNote, "", "");
                        if (SignPrint == "btnSignPrint" | Type == "Preview")
                            // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                            tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                    }
                }
                else
                {
                    rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 1, 0,
                        PatientId,
                        DocRowId, DocNote, "", "");
                    if (SignPrint == "btnSignPrint" | Type == "Preview")
                        // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                        tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                }

                if (Type == "Preview")
                {
                    tmpstr = tmpstr.TrimEnd('|');
                    //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "shownotecall", "PrintNote('" + tmpstr + "','');", true);
                }
                else if (tmpstr == "") { }
                // lblMSuccessMsg.Text = "Note created successfully"
                //Response.Redirect("DocMaster.aspx?PatId=" + this.Master.FindControl("ctl00$ctl00$hdnpatientid") as HiddenField.Value);
                else
                {
                    tmpstr = tmpstr.TrimEnd('|');
                    //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "shownotecall", "PrintNote('" + tmpstr + "'," + "\"DocMaster.aspx?PatId=" + this.Master.FindControl("ctl00$ctl00$hdnpatientid") as HiddenField.Value + "\");", true);
                }
            }
            catch (Exception ex)
            {
                CustomLogger.SendExcepToDB(ex, "Data/SignNotes/SignMissed");
            }
            return rptSavePath1;

        }

        public string SignMedNecessity(string Type, string SignPrint, int docRowId, int PatientId, string NoteType)
        {
            try
            {
                DataSet dschk = new DataSet();
                // DBHelper db = new DBHelper();
                DocManager db = new DocManager();

                ITextMgrNotes iTextMgrNotes = new ITextMgrNotes();

                if (docRowId.ToStringOrEmpty() == "")
                {
                    // lblMErrorMsg.Text = "Note not created, pl save a section to create a note"
                    return db.GetErrorMsg("SaveSec");
                }

                if (Type == "Preview")
                {
                }
                else
                {
                    dschk = db.GetDocFuncCharacR(docRowId, true);
                    if (dschk.Tables[0].Rows.Count == 0)
                    {
                        // lblMErrorMsg.Text = "Functional Characteristics required for signing this note."
                        return db.GetErrorMsg("FuncChar");
                    }
                    dschk = db.GetDocSumInterventions(docRowId);
                    if (dschk.Tables[0].Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dschk.Tables[0].Rows[0]["totalunits"]) == 0)
                        {
                            // lblMErrorMsg.Text = "Charges / Interventions required for signing this note."
                            return db.GetErrorMsg("ReqChgIntv");
                        }
                    }
                    else
                    {
                        // lblMErrorMsg.Text = "Charges / Interventions required for signing this note."
                        return db.GetErrorMsg("ReqChgIntv");
                    }
                }

                DataTable dtPI = new DataTable();
                dtPI = db.GetPatInsuranceNote(PatientId, docRowId).Tables[0];
                if (dtPI.Rows.Count == 0)
                    HttpContext.Current.Session["instype"] = "";
                else
                    HttpContext.Current.Session["instype"] = dtPI.Rows[0]["AccountTypeCode"].ToStringOrEmpty();

                DataTable dt = new DataTable();
                dt = db.GetDocRInterventions(docRowId).Tables[0];
                DataTable dtFR = new DataTable();
                if (HttpContext.Current.Session["instype"].ToStringOrEmpty() == "MC" | HttpContext.Current.Session["instype"].ToStringOrEmpty() == "MI"
                    | HttpContext.Current.Session["instype"].ToStringOrEmpty() == "MP" | HttpContext.Current.Session["instype"].ToStringOrEmpty() == "MO"
                    | HttpContext.Current.Session["instype"].ToStringOrEmpty() == "MN")
                {
                    for (var i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        if ((dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97001" &
                            System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) ||
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97161" &
                            System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) ||
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97162" &
                            System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) ||
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97163" &
                            System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) ||
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97003" &
                            System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) ||
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97164" &
                            System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) |
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97362" & System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) |
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97002" & System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) |
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97165" & System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) |
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97166" & System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) |
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97167" & System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) |
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97168" & System.Convert.ToInt32(dt.Rows[i]["units"]) > 0) |
                            (dt.Rows[i]["CPTCode"].ToStringOrEmpty() == "97004" & System.Convert.ToInt32(dt.Rows[i]["units"]) > 0))
                        {
                            string strSQL;
                            DataSet FRcomp = new DataSet();
                            strSQL = " Select ISNULL(FRComp, 'N') as FRComp from tblDocMaster where docrowid =  " +
                                docRowId;
                            FRcomp = db.ExecuteDataset(strSQL);
                            if (FRcomp.Tables.Count > 0 && FRcomp.Tables[0].Rows.Count > 0 && FRcomp.Tables[0].Rows[0]["FRComp"].ToStringOrEmpty() == "N")
                            {
                                // lblMErrorMsg.Text = "Functional Reporting incomplete, hence unable to sign the note."
                                return db.GetErrorMsg("SelFnRep");
                            }
                        }
                    }
                }

                string strSQL1;
                string tmpstr = "";
                strSQL1 = " select Coalesce(ReferralSource, '') as ReferralSource, Coalesce(ReferralSource2, '') " +
                    "as ReferralSource2 " + " , Coalesce(ReferralSource3, '') as ReferralSource3 from tblDocMaster " + " " +
                    "where PTrowid = " + PatientId.ToStringOrEmpty() + " and docrowid = " + docRowId.ToStringOrEmpty();

                DataTable dt1 = new DataTable();
                dt1 = db.ExecuteDataset(strSQL1).Tables[0];
                if (dt1.Rows.Count > 0)
                {
                    if (dt1.Rows[0]["ReferralSource"].ToStringOrEmpty() != "")
                    {
                        rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 1, Convert.ToInt32(dt1.Rows[0]["ReferralSource"]),
                           PatientId, docRowId, NoteType, "", "");
                        if (SignPrint == "btnSignPrint" | Type == "Preview")
                            // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                            tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                    }
                }
                else
                {
                    rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 1, 0,
                        PatientId, docRowId, NoteType, "", "");
                    if (SignPrint == "btnSignPrint" | Type == "Preview")
                        // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                        tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                }

                if (Type == "Preview")
                {
                    tmpstr = tmpstr.TrimEnd('|');
                    //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "shownotecall", "PrintNote('" + tmpstr + "','');", true);
                }
                else if (tmpstr == "") { }

                // lblMSuccessMsg.Text = "Note created successfully"
                //Response.Redirect("DocMaster.aspx?PatId=" + this.Master.FindControl("ctl00$ctl00$hdnpatientid") as HiddenField.Value);
                else
                {
                    tmpstr = tmpstr.TrimEnd('|');
                    //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "shownotecall", "PrintNote('" + tmpstr + "'," + "\"DocMaster.aspx?PatId=" + this.Master.FindControl("ctl00$ctl00$hdnpatientid") as HiddenField.Value + "\");", true);
                }
            }
            catch (Exception ex)
            {
                CustomLogger.SendExcepToDB(ex, "Data/SignNotes/SignMedNecessity");
            }
            return rptSavePath1;
        }

        public string SignDischarge(string Type, string SignPrint, int DocRowId, int PatientId, string DocNote)
        {
            try
            {
                DataSet dschk = new DataSet();
                DocManager db = new DocManager();
                ITextMgrNotes iTextMgrNotes = new ITextMgrNotes();

                if (Type == "Preview")
                {
                }
                else
                {
                    dschk = db.GetDocFuncCharacR(DocRowId, true);
                    if (dschk.Tables[0].Rows.Count == 0)
                    {
                        // lblMErrorMsg.Text = "Functional Characteristics required for signing this note."
                        return db.GetErrorMsg("FuncChar");
                    }
                }

                if (DocRowId.ToStringOrEmpty() == "")
                {
                    // lblMErrorMsg.Text = "Note not created, pl save a section to create a note"
                    return db.GetErrorMsg("SaveSec");
                }

                if (Type == "Preview")
                {
                }
                else
                {
                    DataTable dtDos = new DataTable();
                    dtDos = db.GetDocDateOfService(DocRowId);

                    db.IUDPatient("S", PatientId, "", "", "", "", "", "", "", "", "", dtDos.Rows[0][0].ToStringOrEmpty(), "", false, "", "", "", "", "", ""
                                           , "", false, false, "", false, false, "", false, false, "", false, false, "", false, false, HttpContext.Current.Session["User"].ToStringOrEmpty(), "", "", "", "", "",
                                           false, false, "0", "0", false);
                }

                string strSQL;
                string tmpstr = "";
                strSQL = " select Coalesce(ReferralSource, '') as ReferralSource, Coalesce(ReferralSource2, '') as ReferralSource2 " + " ," +
                    " Coalesce(ReferralSource3, '') as ReferralSource3 from tblDocMaster " + "" +
                    " where PTrowid = " + PatientId.ToStringOrEmpty() + " " +
                    "and docrowid = " + DocRowId.ToStringOrEmpty();
                //DBHelper db = new DBHelper();
                DataTable dt = new DataTable();
                dt = db.ExecuteDataset(strSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["ReferralSource"].ToStringOrEmpty() != "")
                    {
                        rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 1, Convert.ToInt32(dt.Rows[0]["ReferralSource"]),
                            PatientId, DocRowId,
                            DocNote, "", "");
                        if (SignPrint == "btnSignPrint" | Type == "Preview")
                            // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                            tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" +
                                rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                    }

                    if (dt.Rows[0]["ReferralSource2"].ToStringOrEmpty() != "")
                    {
                        rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 2,
                            Convert.ToInt32(dt.Rows[0]["ReferralSource2"]), PatientId, DocRowId, DocNote, "", "");
                        if (SignPrint == "btnSignPrint" | Type == "Preview")
                            // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                            tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                    }

                    if (dt.Rows[0]["ReferralSource3"].ToStringOrEmpty() != "")
                    {
                        rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 3, Convert.ToInt32(dt.Rows[0]["ReferralSource3"]),
                            PatientId, DocRowId, DocNote, "", "");
                        if (SignPrint == "btnSignPrint" | Type == "Preview")
                            // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                            tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                    }
                }
                else
                {
                    rptSavePath1 = iTextMgrNotes.PrintiText(Type, SignPrint, 1, 0,
                        PatientId, DocRowId,
                        DocNote, "", "");
                    if (SignPrint == "btnSignPrint" | Type == "Preview")
                        // tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("""shownote.aspx?filetype=PDF&pdfurl=" & ITextMgrNotes.rptSavePath1.Replace("\\","\").Replace("\","\\") & """") + "|"
                        tmpstr = tmpstr + System.Web.HttpUtility.UrlDecode("\"shownote.aspx?filetype=PDF&pdfurl=" + rptSavePath1.Replace(@"\", @"\\") + "\"") + "|";
                }

                if (Type == "Preview")
                {
                    tmpstr = tmpstr.TrimEnd('|');
                    //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "shownotecall", "PrintNote('" + tmpstr + "','');", true);
                }
                else if (tmpstr == "") { }
                // lblMSuccessMsg.Text = "Note created successfully"
                //Response.Redirect("DocMaster.aspx?PatId=" + this.Master.FindControl("ctl00$ctl00$hdnpatientid") as HiddenField.Value);
                else
                {
                    tmpstr = tmpstr.TrimEnd('|');
                    // ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "shownotecall", "PrintNote('" + tmpstr + "'," + "\"DocMaster.aspx?PatId=" + this.Master.FindControl("ctl00$ctl00$hdnpatientid") as HiddenField.Value + "\");", true);
                }
            }
            catch (Exception ex)
            {
                CustomLogger.SendExcepToDB(ex, "Data/SignNotes/SignDischarge");
            }
            return rptSavePath1;
        }
    }
}
