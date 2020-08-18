using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data;
using System.Web;
using System.Configuration;

namespace Team.Rehab.Repository.Data
{
    public class ITextMgrNotes
    {
        DocManager db = new DocManager();
        public string PrintiText(string Type, string SignPrint, int NoteCnt, int RS, int PatientId, int docrowid, string NoteType, string UserRole, string User)
        {
            DataTable dtD = new DataTable();
            DataTable dt = new DataTable();
            DataSet dsSpTests = new DataSet();

            //DBHelper db = new DBHelper();
            string rptSavePath1;

            HttpContext.Current.Session["patientid"] = PatientId;
            HttpContext.Current.Session["docrowid"] = docrowid;
            HttpContext.Current.Session["NoteType"] = NoteType;
            HttpContext.Current.Session["UserRole"] = UserRole;
            HttpContext.Current.Session["user"] = User;
            if (RS == 0)
            {
                HttpContext.Current.Session["PrintName"] = "";
                HttpContext.Current.Session["NPI"] = "";
            }
            else
            {
                string strSQL1;
                strSQL1 = " Select PrintName, NPINumber from tblreferrer where Rrowid = " + RS;
                dt = db.GetPrintNameAndNPI(strSQL1);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Session["PrintName"] = dt.Rows[0]["PrintName"].ToString();
                    HttpContext.Current.Session["NPI"] = dt.Rows[0]["NPINumber"].ToString();
                }
            }

            dtD = db.GetDocDateOfService(docrowid);
            string rptName;
            string rptSavePath;
            // Dim rptSavePath1  As String


            dt = db.GetPatient(PatientId);

            if (NoteCnt == 0)
                rptName = dt.Rows[0][0].ToString().PadLeft(3, '0') + "-" + PatientId.ToString().PadLeft(6, '0') + "_" + dt.Rows[0]["FirstName"]
                    + "_" + dt.Rows[0]["LastName"] + "_" + NoteType + "_" + dtD.Rows[0]["DateOfService"].ToString().Replace("/", "-")
                    + "_" + DateTime.Now.ToString().Replace(":", "-").Replace("/", "-").Replace(" ", "-");
            else
                rptName = dt.Rows[0][0].ToString().PadLeft(3, '0') + "-" + PatientId.ToString().PadLeft(6, '0')
                    + "_" + dt.Rows[0]["FirstName"] + "_" + dt.Rows[0]["LastName"] + "_" + NoteType + "_" + NoteCnt.ToString() + "_"
                    + dtD.Rows[0]["DateOfService"].ToString().Replace("/", "-") + "_" +
                    DateTime.Now.ToString().Replace(":", "-").Replace("/", "-").Replace(" ", "-");
            //  rptSavePath = ConfigurationManager.AppSettings["folderpath"] + dt.Rows[0]["Folder"].ToString();
            string path = Convert.ToString(dt.Rows[0]["Folder"]);
            rptSavePath = ConfigurationManager.AppSettings["folderpath"];
            rptSavePath1 = rptSavePath + @"\" + rptName + ".pdf";

            //if (!Directory.Exists(rptSavePath))
            //    Directory.CreateDirectory(rptSavePath);
            MemoryStream ms = new MemoryStream();
            CloudeStoreg.CreateDirectory(path, rptSavePath);
            float ht;
            DataTable tblMeasurement = new DataTable();
            DataRow nRowMeasurement;
            DataColumn Col = new DataColumn();
            var document = new Document(PageSize.A4, 40, 15, 230, 20);
            string output = rptSavePath1;

            var writer = PdfWriter.GetInstance(document, ms);

            // Open the Document for writing
            document.Open();
            writer.PageEvent = new MyHeaderFooterNoteEvent();

            var titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
            var subTitleFont = FontFactory.GetFont("Arial", 14, Font.BOLD);
            var boldTableFont = FontFactory.GetFont("Arial", 11, Font.BOLD);
            var boldTableFontU = FontFactory.GetFont("Arial", 12, Font.BOLD | Font.UNDERLINE);
            // boldTableFontU.SetStyle(Font.UNDERLINE)

            var endingMessageFont = FontFactory.GetFont("Arial", 10, Font.ITALIC);
            var bodyFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);
            var bodyFontMed = FontFactory.GetFont("Arial", 12, Font.NORMAL);
            var bodyFontSmall = FontFactory.GetFont("Arial", 8, Font.NORMAL);
            var boldTableFontSmallU = FontFactory.GetFont("Arial", 10, Font.BOLD | Font.UNDERLINE);
            var bodyFontC = FontFactory.GetFont("Courier", 9, Font.NORMAL);
            // Dim bodyFontCM = FontFactory.GetFont("Courier", 8.5, Font.NORMAL, BaseColor.BLACK)
            var bodyFontCM = FontFactory.GetFont("Courier", 8, Font.BOLD);

            PdfPCell cell;
            PdfContentByte canvas = writer.DirectContent;

            float[] widths;
            iTextSharp.text.pdf.draw.LineSeparator Singleline;
            Paragraph p;

            // ********************************************
            // Start Diagnosis section  
            // ********************************************

            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC" | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2" | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PTREAT" | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PMN" | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE" | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PFCE" | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PMV" | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PCOMM"))
            {
                DataTable dtPD = new DataTable(); // Patient Diagnosis details
                dtPD = db.GetPatDiagnosis(Convert.ToInt32(HttpContext.Current.Session["patientid"])).Tables[0];

                if (dtPD.Rows.Count > 0)
                {
                    PdfPTable rdiag = new PdfPTable(3);
                    rdiag.TotalWidth = 550.0F;
                    rdiag.LockedWidth = true;

                    // Start  new Page if very small space left on the page
                    ht = writer.GetVerticalPosition(true);
                    if (ht < 250)
                        document.NewPage();

                    // relative col widths in proportions - 1:1:6
                    widths = new float[] { 1.0F, 0.8F, 6.0F };
                    rdiag.SetWidths(widths);
                    rdiag.HorizontalAlignment = 0;

                    p = new Paragraph(new Phrase("Diagnoses", boldTableFontU));
                    p.IndentationLeft = 230;
                    document.Add(p);

                    cell = new PdfPCell(new Phrase("Onset Date", boldTableFont));
                    cell.Border = 0;
                    cell.PaddingBottom = 5;
                    rdiag.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Code", boldTableFont));
                    cell.Border = 0;
                    cell.PaddingBottom = 5;
                    rdiag.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Description", boldTableFont));
                    cell.Border = 0;
                    cell.PaddingBottom = 5;
                    rdiag.AddCell(cell);

                    for (var i = 0; i <= dtPD.Rows.Count - 1; i++)
                    {
                        cell = new PdfPCell(new Phrase(dtPD.Rows[i]["OnsetDate"].ToString(), bodyFont));
                        cell.Border = 0;
                        cell.Padding = 2;
                        rdiag.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtPD.Rows[i]["DiagnosisCode"].ToString(), bodyFont));
                        cell.Border = 0;
                        cell.Padding = 2;
                        rdiag.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtPD.Rows[i]["Description"].ToString(), bodyFont));
                        cell.Border = 0;
                        cell.Padding = 2;
                        rdiag.AddCell(cell);
                    }

                    rdiag.SpacingBefore = 10.0F;
                    // rdiag.SpacingAfter = 10.0F
                    document.Add(rdiag);

                    Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                    document.Add(new Chunk(Singleline));
                }
            }
            // ********************************************
            // End Diagnosis section  
            // ********************************************


            // ********************************************
            // Start PT Interventions & Summary
            // ********************************************

            if (Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PTREAT"
                | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PMN" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PDIS")
            {
                DataTable dtDRI = new DataTable(); // Doc R Interventions - PT Interventions Table 2 
                dtDRI = db.GetDocRInterventions(Convert.ToInt32(HttpContext.Current.Session["docrowid"])).Tables[0];

                if (dtDRI.Rows.Count > 0)
                {

                    // Start  new Page if very small space left on the page
                    ht = writer.GetVerticalPosition(true);
                    if (ht < 250)
                        document.NewPage();

                    PdfPTable rinterv = new PdfPTable(5);
                    rinterv.TotalWidth = 550.0F;
                    rinterv.LockedWidth = true;

                    // relative col widths in proportions - 5:1
                    float[] widthsrTot = new float[] { 5.0F, 1.1F, 1.0F, 1.0F, 1.0F };
                    rinterv.SetWidths(widthsrTot);

                    p = new Paragraph(new Phrase("Interventions", boldTableFontU));
                    p.IndentationLeft = 225;
                    document.Add(p);

                    cell = new PdfPCell(new Phrase("Description", boldTableFont));
                    cell.Border = 0;
                    cell.PaddingBottom = 5;
                    rinterv.AddCell(cell);

                    cell = new PdfPCell(new Phrase("CPT Code", boldTableFont));
                    cell.Border = 0;
                    cell.PaddingBottom = 5;
                    rinterv.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Modifiers", boldTableFont));
                    cell.Border = 0;
                    cell.PaddingBottom = 5;
                    rinterv.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Minutes", boldTableFont));
                    cell.PaddingBottom = 5;
                    cell.Border = 0;
                    rinterv.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Units", boldTableFont));
                    cell.Border = 0;
                    cell.PaddingBottom = 5;
                    rinterv.AddCell(cell);

                    for (var i = 0; i <= dtDRI.Rows.Count - 1; i++)
                    {
                        cell = new PdfPCell(new Phrase(dtDRI.Rows[i]["CPTDescription"].ToString(), bodyFont));
                        cell.Border = 0;
                        cell.Padding = 2;
                        rinterv.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDRI.Rows[i]["CPTCode"].ToString(), bodyFont));
                        cell.Padding = 2;
                        cell.Border = 0;
                        cell.HorizontalAlignment = 3;
                        rinterv.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDRI.Rows[i]["Modifiers"].ToString(), bodyFont));
                        cell.Border = 0;
                        cell.Padding = 2;
                        cell.HorizontalAlignment = 3;
                        rinterv.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDRI.Rows[i]["Minutes"].ToString(), bodyFont));
                        cell.Border = 0;
                        cell.Padding = 2;
                        cell.HorizontalAlignment = 3;
                        rinterv.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDRI.Rows[i]["units"].ToString(), bodyFont));
                        cell.Border = 0;
                        cell.Padding = 2;
                        cell.HorizontalAlignment = 3;
                        rinterv.AddCell(cell);
                    }

                    rinterv.SpacingBefore = 10.0F;
                    document.Add(rinterv);

                    p = new Paragraph(new Phrase("Interventions Summary", boldTableFontU));
                    p.IndentationLeft = 200;
                    document.Add(p);

                    PdfPTable rintervS = new PdfPTable(6);

                    DataTable dtDSI = new DataTable(); // Doc R Interventions - PT Interventions Table 2

                    dtDSI = db.GetDocSumInterventions(Convert.ToInt32(HttpContext.Current.Session["docrowid"])).Tables[0];

                    if (dtDSI.Rows.Count > 0)
                    {
                        widths = new float[] { 2.0F, 1.0F, 2.0F, 1.0F, 2.0F, 1.0F };
                        rintervS.SetWidths(widths);
                        rintervS.HorizontalAlignment = 0;
                        rintervS.TotalWidth = 500.0F;
                        rintervS.LockedWidth = true;

                        cell = new PdfPCell(new Phrase("Total Minutes", boldTableFont));
                        cell.Border = 0;
                        rintervS.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDSI.Rows[0]["totalminutes"].ToString(), bodyFont));
                        cell.Border = 0;
                        rintervS.AddCell(cell);

                        cell = new PdfPCell(new Phrase("Timed Minutes", boldTableFont));
                        cell.Border = 0;
                        rintervS.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDSI.Rows[0]["totaltimedminutes"].ToString(), bodyFont));
                        cell.Border = 0;
                        rintervS.AddCell(cell);

                        cell = new PdfPCell(new Phrase("UnTimed Minutes", boldTableFont));
                        cell.Border = 0;
                        rintervS.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDSI.Rows[0]["totaluntimedminutes"].ToString(), bodyFont));
                        cell.Border = 0;
                        rintervS.AddCell(cell);

                        cell = new PdfPCell(new Phrase("Total Units", boldTableFont));
                        cell.Border = 0;
                        rintervS.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDSI.Rows[0]["totalunits"].ToString(), bodyFont));
                        cell.Border = 0;
                        rintervS.AddCell(cell);

                        cell = new PdfPCell(new Phrase("Timed Units", boldTableFont));
                        cell.Border = 0;
                        rintervS.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDSI.Rows[0]["totaltimedunits"].ToString(), bodyFont));
                        cell.Border = 0;
                        rintervS.AddCell(cell);

                        cell = new PdfPCell(new Phrase("UnTimed Units", boldTableFont));
                        cell.Border = 0;
                        rintervS.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDSI.Rows[0]["totaluntimedunits"].ToString(), bodyFont));
                        cell.Border = 0;
                        rintervS.AddCell(cell);

                        rintervS.SpacingBefore = 10.0F;

                        Paragraph p1 = new Paragraph();
                        p1.IndentationLeft = 50;
                        p1.Add(rintervS);
                        document.Add(p1);

                        Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                        document.Add(new Chunk(Singleline));
                    }
                }
            }
            // ********************************************
            // End PT Interventions & Summary
            // ********************************************


            // ********************************************
            // Start progressive Exercises
            // ********************************************
            // 
            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PTREAT" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PMN" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PDIS"))
            {
                DataTable dtDPE = new DataTable(); // Progressive Exercises Table 3
                dtDPE = db.GetDocProgExer(docrowid, PatientId, "Y").Tables[0];
                if (dtDPE.Rows.Count > 0)
                {

                    // Start  new Page if very small space left on the page
                    ht = writer.GetVerticalPosition(true);
                    if (ht < 250)
                        document.NewPage();

                    p = new Paragraph(new Phrase("Progressive Exercises", boldTableFontU));
                    p.IndentationLeft = 200;
                    document.Add(p);

                    PdfPTable prgexr = new PdfPTable(5);
                    prgexr.TotalWidth = 550.0F;
                    prgexr.LockedWidth = true;
                    // relative col widths in proportions - 10:1

                    float[] widthprgexr = new float[] { 5.5F, 1.0F, 1.0F, 1.5F, 3.0F };
                    prgexr.SetWidths(widthprgexr);
                    prgexr.HorizontalAlignment = 0;

                    cell = new PdfPCell(new Phrase("Exercise", boldTableFont));
                    cell.PaddingBottom = 5;
                    cell.Border = 0;
                    prgexr.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Sets", boldTableFont));
                    cell.PaddingBottom = 5;
                    cell.Border = 0;
                    prgexr.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Reps", boldTableFont));
                    cell.PaddingBottom = 5;
                    cell.Border = 0;
                    prgexr.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Time", boldTableFont));
                    cell.PaddingBottom = 5;
                    cell.Border = 0;
                    prgexr.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Wt/BaseColor/Level", boldTableFont));
                    cell.Border = 0;
                    prgexr.AddCell(cell);

                    for (var i = 0; i <= dtDPE.Rows.Count - 1; i++)
                    {
                        cell = new PdfPCell(new Phrase(dtDPE.Rows[i]["Exercise"].ToString(), bodyFont));
                        cell.Padding = 2;
                        cell.Border = 0;
                        prgexr.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDPE.Rows[i]["Sets"].ToString(), bodyFont));
                        cell.Padding = 2;
                        cell.Border = 0;
                        prgexr.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDPE.Rows[i]["Reps"].ToString(), bodyFont));
                        cell.Padding = 2;
                        cell.Border = 0;
                        prgexr.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDPE.Rows[i]["Qty"].ToString(), bodyFont));
                        cell.Padding = 2;
                        cell.Border = 0;
                        prgexr.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDPE.Rows[i]["Weight"].ToString(), bodyFont));
                        cell.Padding = 2;
                        cell.Border = 0;
                        prgexr.AddCell(cell);
                    }

                    prgexr.SpacingBefore = 10.0F;
                    document.Add(prgexr);

                    Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                    document.Add(new Chunk(Singleline));
                }
            }
            // ********************************************
            // End progressive Exercises
            // ********************************************


            // ********************************************
            // Start Pain
            // ********************************************
            // 
            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2" |
               Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE" |
               Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PDIS"))
            {
                DataTable dtDP = new DataTable(); // Pain Table

                dtDP = db.GetDocPainR(docrowid).Tables[0];

                if (dtDP.Rows.Count > 0)
                {

                    // Start  new Page if very small space left on the page
                    ht = writer.GetVerticalPosition(true);
                    if (ht < 250)
                        document.NewPage();

                    PdfPTable rpain = new PdfPTable(3);

                    rpain.TotalWidth = 500.0F;
                    rpain.LockedWidth = true;

                    // relative col widths in proportions - 2:1:1
                    widths = new float[] { 3.0F, 1.0F, 1.0F };
                    rpain.SetWidths(widths);
                    rpain.HorizontalAlignment = 0;

                    // leave a gap before and after the table
                    rpain.SpacingBefore = 10.0F;

                    p = new Paragraph(new Phrase("Pain", boldTableFontU));
                    p.IndentationLeft = 250;
                    document.Add(p);

                    cell = new PdfPCell(new Phrase("Location", boldTableFont));
                    cell.Border = 0;
                    rpain.AddCell(cell);

                    cell = new PdfPCell(new Phrase("With Rest", boldTableFont));
                    cell.Border = 0;
                    rpain.AddCell(cell);

                    cell = new PdfPCell(new Phrase("With Activity", boldTableFont));
                    cell.Border = 0;
                    rpain.AddCell(cell);

                    for (var i = 0; i <= dtDP.Rows.Count - 1; i++)
                    {
                        cell = new PdfPCell(new Phrase(dtDP.Rows[i]["PainSite"].ToString(), bodyFont));
                        cell.Border = 0;
                        rpain.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDP.Rows[i]["PainscaleAR"].ToString(), bodyFont));
                        cell.Border = 0;
                        rpain.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDP.Rows[i]["PainscaleWA"].ToString(), bodyFont));
                        cell.Border = 0;
                        rpain.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDP.Rows[i]["EXDescription"].ToString(), bodyFont));
                        cell.Border = 0;
                        cell.Colspan = 3;
                        cell.PaddingLeft = 10;
                        rpain.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dtDP.Rows[i]["REDescription"].ToString(), bodyFont));
                        cell.Border = 0;
                        cell.Colspan = 3;
                        cell.PaddingLeft = 10;
                        rpain.AddCell(cell);
                    }

                    document.Add(rpain);

                    Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                    document.Add(new Chunk(Singleline));
                }
            }
            // ********************************************
            // End Pain
            // ********************************************

            // ********************************************
            // Start of Joint Measurements section
            // ********************************************
            // 
            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PMN" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PDIS"))
            {
                DataTable dtG = new DataTable();
                DataSet ds1;
                string strMeasureType;

                if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC" | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2"))
                    strMeasureType = "IE";
                else
                    strMeasureType = "LM";

                ds1 = db.GetDocBodyPart(docrowid, strMeasureType);

                DataTable dt2;
                DataTable dtIE = new DataTable();
                string temp;
                string temp1;

                if (ds1.Tables[0].Rows.Count > 0)
                {

                    // Start  new Page if very small space left on the page
                    ht = writer.GetVerticalPosition(true);
                    if (ht < 250)
                        document.NewPage();

                    for (var i = 0; i <= ds1.Tables[0].Rows.Count - 1; i++)
                    {
                        if (i == 0)
                        {
                            dtIE = db.GetMeasurementsR(PatientId, docrowid,
                                ds1.Tables[0].Rows[i][0].ToString(), strMeasureType).Tables[0];
                            if (ds1.Tables[0].Rows[i]["bodypart"].ToString() == "Other")
                            {
                                dtIE.Rows[0].Delete();
                                dtIE.Rows[1].Delete();
                                dtIE.AcceptChanges();
                            }
                        }
                        else
                        {
                            dt2 = db.GetMeasurementsR(PatientId, docrowid, ds1.Tables[0].Rows[i][0].ToString(),
                                strMeasureType).Tables[0];
                            if (ds1.Tables[0].Rows[i]["bodypart"].ToString() == "Other")
                            {
                                dt2.Rows[0].Delete();
                                dt2.Rows[1].Delete();
                                dt2.AcceptChanges();
                            }
                            dtIE.Merge(dt2);
                        }
                    }
                }
                else
                {

                    temp = "AA";
                    dtIE = db.GetMeasurementsR(PatientId, docrowid, temp, "IE").Tables[0];
                }

                // ds1 = DocManager.GetDocBodyPartG(Request.QueryString("PatId"), "Goal")
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    for (var i = 0; i <= ds1.Tables[0].Rows.Count - 1; i++)
                    {
                        if (i == 0)
                        {
                            dtG = db.GetMeasurementsRG(PatientId,
                                ds1.Tables[0].Rows[i][0].ToString(), "Goal").Tables[0];
                            if (ds1.Tables[0].Rows[i]["bodypart"].ToString() == "Other")
                            {
                                dtG.Rows[0].Delete();
                                dtG.Rows[1].Delete();
                                dtG.AcceptChanges();
                            }
                        }
                        else
                        {
                            dt2 = db.GetMeasurementsRG(PatientId, ds1.Tables[0].Rows[i][0].ToString(), "Goal").Tables[0];
                            if (ds1.Tables[0].Rows[i]["bodypart"].ToString() == "Other")
                            {
                                dt2.Rows[0].Delete();
                                dt2.Rows[1].Delete();
                                dt2.Rows[2]["MeasurementType"] = "";
                                dt2.Rows[2]["Measurement"] = "";
                                if (dt2.Rows.Count == 4)
                                {
                                    dt2.Rows[3]["MeasurementType"] = "";
                                    dt2.Rows[3]["Measurement"] = "";
                                }
                                dt2.AcceptChanges();
                            }
                            dtG.Merge(dt2);
                        }
                    }
                }
                else
                {
                    temp1 = "AB";
                    dtG = db.GetMeasurementsRG(docrowid, temp1, "Goal").Tables[0];
                }

                Col = new DataColumn("MeasurementTypeIE");
                tblMeasurement.Columns.Add(Col);
                Col = new DataColumn("MeasurementIE");
                tblMeasurement.Columns.Add(Col);
                Col = new DataColumn("MeasurementTypeG");
                tblMeasurement.Columns.Add(Col);
                Col = new DataColumn("MeasurementG");
                tblMeasurement.Columns.Add(Col);

                for (var i = 0; i <= dtIE.Rows.Count - 1; i++)
                {
                    nRowMeasurement = tblMeasurement.NewRow();
                    if (dtIE.Rows[i]["MeasurementType"].ToString().Trim() == "Other")
                        nRowMeasurement["MeasurementIE"] = dtIE.Rows[i]["MeasurementType"].ToString().PadRight(17, ' ') + dtIE.Rows[i]["Measurement"].ToString();
                    else
                        nRowMeasurement["MeasurementIE"] = dtIE.Rows[i]["MeasurementType"].ToString().PadRight(17, ' ') + dtIE.Rows[i]["Measurement"].ToString().PadRight(35, ' ') +
                            dtG.Rows[i]["MeasurementType"].ToString().PadRight(17, ' ') + dtG.Rows[i]["Measurement"].ToString().PadRight(30, ' ');
                    tblMeasurement.Rows.Add(nRowMeasurement);
                }


                if (tblMeasurement.Rows.Count == 3)
                {
                }
                else if (tblMeasurement.Rows.Count > 3
        )
                {
                    PdfPTable rjmeasure = new PdfPTable(2);

                    rjmeasure.TotalWidth = 550.0F;
                    rjmeasure.LockedWidth = true;

                    // relative col widths in proportions - 1:1
                    widths = new float[] { 1.2F, 1.0F };
                    rjmeasure.SetWidths(widths);
                    rjmeasure.HorizontalAlignment = 0;

                    p = new Paragraph(new Phrase("Joint Measurements", boldTableFontU));
                    p.IndentationLeft = 205;
                    document.Add(p);

                    cell = new PdfPCell(new Phrase("Current Level", boldTableFont));
                    cell.Border = 0;
                    rjmeasure.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Goal", boldTableFont));
                    cell.Border = 0;
                    rjmeasure.AddCell(cell);

                    for (var i = 0; i <= tblMeasurement.Rows.Count - 1; i++)
                    {
                        cell = new PdfPCell(new Phrase(tblMeasurement.Rows[i]["MeasurementIE"].ToString(), bodyFontCM));
                        cell.Border = 0;
                        cell.Colspan = 2;
                        rjmeasure.AddCell(cell);
                    }

                    rjmeasure.SpacingBefore = 10.0F;
                    document.Add(rjmeasure);

                    Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                    document.Add(new Chunk(Singleline));
                }
                else
                {
                    PdfPTable rjmeasure = new PdfPTable(1);

                    rjmeasure.TotalWidth = 550.0F;
                    rjmeasure.LockedWidth = true;

                    // relative col widths in proportions - 1:1
                    widths = new float[] { 1.0F };
                    rjmeasure.SetWidths(widths);
                    rjmeasure.HorizontalAlignment = 0;

                    p = new Paragraph(new Phrase("Joint Measurements", boldTableFontU));
                    p.IndentationLeft = 205;
                    document.Add(p);

                    cell = new PdfPCell(new Phrase("Current Level", boldTableFont));
                    cell.Border = 0;
                    rjmeasure.AddCell(cell);

                    cell = new PdfPCell(new Phrase(tblMeasurement.Rows[0]["MeasurementIE"].ToString(), bodyFontCM));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    rjmeasure.AddCell(cell);

                    rjmeasure.SpacingBefore = 10.0F;
                    document.Add(rjmeasure);

                    Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                    document.Add(new Chunk(Singleline));
                }
            }
            // ********************************************
            // End of Joint Measurements section
            // ********************************************

            // ********************************************
            // Start of Extremity Tests section
            // ********************************************
            // 
            DataSet dsExTests = new DataSet();
            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC" |
               Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2" |
               Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE"
                | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PDIS"))
            {
                ht = writer.GetVerticalPosition(true);
                if (ht < 250)
                    document.NewPage();

                dsExTests = db.GetDocExtremityTests("D", docrowid, 0);

                if (dsExTests.Tables[0].Rows.Count > 0)
                {
                    PdfPTable rexttest = new PdfPTable(2);

                    rexttest.TotalWidth = 600.0F;
                    rexttest.LockedWidth = true;

                    // relative col widths in proportions - 1:1
                    widths = new float[] { 3.0F, 1.0F };
                    rexttest.SetWidths(widths);
                    rexttest.HorizontalAlignment = 0;

                    // leave a gap before and after the table
                    rexttest.SpacingBefore = 10.0F;

                    p = new Paragraph(new Phrase("Extremity Tests", boldTableFontU));
                    p.IndentationLeft = 220;
                    document.Add(p);

                    cell = new PdfPCell(new Phrase("Test", boldTableFont));
                    cell.Border = 0;
                    rexttest.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Result", boldTableFont));
                    cell.Border = 0;
                    rexttest.AddCell(cell);

                    for (var i = 0; i <= dsExTests.Tables[0].Rows.Count - 1; i++)
                    {
                        cell = new PdfPCell(new Phrase(dsExTests.Tables[0].Rows[i]["Description"].ToString(), bodyFont));
                        cell.Border = 0;
                        rexttest.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dsExTests.Tables[0].Rows[i]["TestPN"].ToString(), bodyFont));
                        cell.Border = 0;
                        rexttest.AddCell(cell);
                    }

                    document.Add(rexttest);

                    Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                    document.Add(new Chunk(Singleline));
                }
            }
            // ********************************************
            // End of Extremity Tests section
            // ********************************************


            // ********************************************
            // Start of Extremity Assessment section
            // ********************************************
            // 
            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC"
                | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2"
                | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE"
                | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PDIS"))
            {
                if (dsExTests.Tables[1].Rows.Count > 0)
                {
                    ht = writer.GetVerticalPosition(true);
                    if (ht <= 250)
                        document.NewPage();

                    p = new Paragraph(new Phrase("Extremity Assessment", boldTableFontU));
                    p.IndentationLeft = 205;
                    document.Add(p);

                    var p1 = new Paragraph(new Phrase(dsExTests.Tables[1].Rows[0]["assessment"].ToStringOrEmpty(), bodyFont));
                    p1.SpacingBefore = 10.0F;
                    document.Add(p1);

                    Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                    document.Add(new Chunk(Singleline));
                }
            }
            // ********************************************
            // End of Extremity Assessment section
            // ********************************************


            // ********************************************
            // Start of Spinal Tests section
            // ********************************************
            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC"
                | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2" |
               Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE"
               | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PDIS"))
            {
                dsSpTests = db.GetDocSpinalTests("D", docrowid, 0);

                if (dsSpTests.Tables[0].Rows.Count > 0)
                {
                    ht = writer.GetVerticalPosition(true);
                    if (ht < 250)
                        document.NewPage();

                    PdfPTable rexttest = new PdfPTable(2);

                    rexttest.TotalWidth = 600.0F;
                    rexttest.LockedWidth = true;

                    // relative col widths in proportions - 1:1
                    widths = new float[] { 3.0F, 1.0F };
                    rexttest.SetWidths(widths);
                    rexttest.HorizontalAlignment = 0;

                    // leave a gap before and after the table
                    rexttest.SpacingBefore = 10.0F;

                    p = new Paragraph(new Phrase("Spinal Tests", boldTableFontU));
                    p.IndentationLeft = 220;
                    document.Add(p);

                    cell = new PdfPCell(new Phrase("Test", boldTableFont));
                    cell.Border = 0;
                    rexttest.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Result", boldTableFont));
                    cell.Border = 0;
                    rexttest.AddCell(cell);

                    for (var i = 0; i <= dsSpTests.Tables[0].Rows.Count - 1; i++)
                    {
                        cell = new PdfPCell(new Phrase(dsSpTests.Tables[0].Rows[i]["Description"].ToString(), bodyFont));
                        cell.Border = 0;
                        rexttest.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dsSpTests.Tables[0].Rows[i]["TestPN"].ToString(), bodyFont));
                        cell.Border = 0;
                        rexttest.AddCell(cell);
                    }

                    document.Add(rexttest);

                    Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                    document.Add(new Chunk(Singleline));
                }
            }
            // ********************************************
            // End of Spinal Tests section
            // ********************************************

            // ********************************************
            // Start of Spinal Assessment section
            // ********************************************
            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE"
                | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PDIS"))
            {
                if (dsSpTests.Tables[1].Rows.Count > 0)
                {
                    if (dsSpTests.Tables[1].Rows[0]["assessment"].ToString().Trim() == "")
                    {
                    }
                    else
                    {
                        // Start  new Page if very small space left on the page                
                        ht = writer.GetVerticalPosition(true);
                        if (ht < 250)
                            document.NewPage();

                        p = new Paragraph(new Phrase("Spinal Assessment", boldTableFontU));
                        p.IndentationLeft = 205;
                        document.Add(p);

                        var p1 = new Paragraph(new Phrase(dsSpTests.Tables[1].Rows[0]["assessment"].ToStringOrEmpty(), bodyFont));
                        p1.SpacingBefore = 10.0F;
                        document.Add(p1);

                        Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                        document.Add(new Chunk(Singleline));
                    }
                }
            }
            // ********************************************
            // End of Spinal Assessment section
            // ********************************************

            // ********************************************
            // Start of OMPT section
            // ********************************************
            // 
            DataSet dsOmTests = new DataSet();
            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2"
                | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PDIS"))
            {
                dsOmTests = db.GetDocOMPT("D", docrowid, 0);

                if (dsOmTests.Tables[0].Rows.Count > 0)
                {
                    ht = writer.GetVerticalPosition(true);
                    if (ht < 250)
                        document.NewPage();

                    PdfPTable rexttest = new PdfPTable(5);

                    rexttest.TotalWidth = 550.0F;
                    rexttest.LockedWidth = true;

                    // relative col widths in proportions - 1:1
                    widths = new float[] { 3.5F, 1.0F, 1.0F, 1.0F, 1.2F };
                    rexttest.SetWidths(widths);
                    rexttest.HorizontalAlignment = 0;

                    // leave a gap before and after the table
                    rexttest.SpacingBefore = 10.0F;

                    p = new Paragraph(new Phrase("OMPT Spinal Tests", boldTableFontU));
                    p.IndentationLeft = 220;
                    document.Add(p);

                    cell = new PdfPCell(new Phrase("Test", boldTableFont));
                    cell.Border = 0;
                    rexttest.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Result", boldTableFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.PaddingLeft = 50;
                    rexttest.AddCell(cell);

                    for (var i = 0; i <= dsOmTests.Tables[0].Rows.Count - 1; i++)
                    {
                        cell = new PdfPCell(new Phrase(dsOmTests.Tables[0].Rows[i]["Description"].ToString(), bodyFont));
                        cell.Border = 0;
                        rexttest.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dsOmTests.Tables[0].Rows[i]["TestPAN"].ToString(), bodyFont));
                        cell.Border = 0;
                        rexttest.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dsOmTests.Tables[0].Rows[i]["TestID"].ToString(), bodyFont));
                        cell.Border = 0;
                        rexttest.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dsOmTests.Tables[0].Rows[i]["TestWB"].ToString(), bodyFont));
                        cell.Border = 0;
                        rexttest.AddCell(cell);

                        cell = new PdfPCell(new Phrase(dsOmTests.Tables[0].Rows[i]["TestCP"].ToString(), bodyFont));
                        cell.Border = 0;
                        rexttest.AddCell(cell);
                    }

                    document.Add(rexttest);

                    Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                    document.Add(new Chunk(Singleline));
                }
            }
            // ********************************************
            // End of OMPT section
            // ********************************************
            // 

            // ********************************************
            // Start of OMPT Assessment section
            // ********************************************
            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC"
                | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2"
                | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE"
                | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PDIS"))
            {
                if (dsOmTests.Tables[1].Rows.Count > 0)
                {
                    ht = writer.GetVerticalPosition(true);
                    if (ht < 250)
                        document.NewPage();

                    p = new Paragraph(new Phrase("OMPT Spinal Assessment", boldTableFontU));
                    p.IndentationLeft = 205;
                    document.Add(p);

                    var p1 = new Paragraph(new Phrase(dsOmTests.Tables[1].Rows[0]["assessment"].ToString(), bodyFont));
                    p1.SpacingBefore = 10.0F;
                    document.Add(p1);

                    Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                    document.Add(new Chunk(Singleline));
                }
            }
            // ********************************************
            // End of OMPT Assessment section
            // ********************************************

            // ********************************************
            // Start of Functional Analysis section
            // ********************************************
            // 
            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC"
                | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2"
                | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PMN")
                | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE" |
               Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PFCE"
               | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PDIS")
            {
                ht = writer.GetVerticalPosition(true);
                if (ht < 180)
                    document.NewPage();

                DataTable dtFR = new DataTable();
                // dtFR = DocManager.GetDocFuncCharac(TryCast(Me.Master.FindControl("ctl00$ctl00$hdndocrowid"), HiddenField).Value, TryCast(Me.Master.FindControl("ctl00$ctl00$hdnpatientid"), HiddenField).Value, Request.QueryString("DocNote")).Tables[0]
                dtFR = db.GetDocFuncCharac(docrowid, 0, NoteType).Tables[0];
                // dtFR = DocManager.GetDocFuncCharacR(HttpContext.Current.Session["docrowid"), 0, HttpContext.Current.Session["NoteType")).Tables[0] for Reeval check diff

                if (dtFR.Rows.Count > 0)
                {
                    p = new Paragraph(new Phrase("Functional Analysis", boldTableFontU));
                    p.IndentationLeft = 205;
                    document.Add(p);

                    var p1 = new Paragraph(new Phrase(dtFR.Rows[0]["funccharac"].ToString(), bodyFont));
                    p1.SpacingBefore = 10.0F;
                    document.Add(p1);

                    Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                    document.Add(new Chunk(Singleline));
                }
            }
            // ********************************************
            // End of Functional Analysis section
            // ********************************************

            // ********************************************
            // Start of IE Goals 
            // ********************************************
            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPFCE"))
            {
                PdfPTable rgoal = new PdfPTable(1);
                rgoal.TotalWidth = 600.0F;
                rgoal.LockedWidth = true;
                // relative col widths in proportions - 5:1
                rgoal.HorizontalAlignment = 0;
                rgoal.SpacingBefore = 10.0F;

                DataTable dtSGoals = new DataTable();
                DataTable dtFGoals = new DataTable();
                DataTable dtLGoals = new DataTable();
                dtSGoals = db.GetPatGoals(PatientId, "S").Tables[0];

                if (dtSGoals.Rows.Count > 0)
                {
                    ht = writer.GetVerticalPosition(true);
                    if (ht < 250)
                        document.NewPage();

                    p = new Paragraph(new Phrase("Short Term Goals", boldTableFontU));
                    p.IndentationLeft = 205;
                    document.Add(p);

                    for (var i = 0; i <= dtSGoals.Rows.Count - 1; i++)
                    {
                        cell = new PdfPCell(new Phrase(dtSGoals.Rows[i]["Description"].ToString(), bodyFont));
                        cell.Border = 0;
                        rgoal.AddCell(cell);
                    }
                    document.Add(rgoal);
                    Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                    document.Add(new Chunk(Singleline));
                }

                ht = writer.GetVerticalPosition(true);
                if (ht < 80)
                    document.NewPage();

                p = new Paragraph(new Phrase("Long Term Goals", boldTableFontU));
                p.IndentationLeft = 205;
                document.Add(p);

                dtLGoals = db.GetPatGoals(PatientId, "L").Tables[0];
                dtFGoals = db.GetPatGoals(PatientId, "F").Tables[0];
                dtLGoals.Merge(dtFGoals);

                for (var i = 0; i <= dtLGoals.Rows.Count - 1; i++)
                {
                    cell = new PdfPCell(new Phrase(dtLGoals.Rows[i]["Description"].ToString(), bodyFont));
                    cell.Border = 0;
                    rgoal.AddCell(cell);
                }

                document.Add(rgoal);

                Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                document.Add(new Chunk(Singleline));
            }
            // ********************************************
            // End of IE Goals
            // ********************************************

            // ********************************************
            // Start of Goals Re-Eval
            // ********************************************
            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PMN" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PDIS"))
            {
                ht = writer.GetVerticalPosition(true);
                if (ht < 180)
                    document.NewPage();

                DataTable dtSGoals = new DataTable();
                DataTable dtFGoals = new DataTable();
                DataTable dtLGoals = new DataTable();
                dtSGoals = db.GetPatGoals(PatientId, "S").Tables[0];
                dtLGoals = db.GetPatGoals(PatientId, "L").Tables[0];
                dtFGoals = db.GetPatGoals(PatientId, "F").Tables[0];
                dtLGoals.Merge(dtFGoals);

                PdfPTable rgoal = new PdfPTable(4);
                rgoal.TotalWidth = 550.0F;
                rgoal.LockedWidth = true;
                float[] widthsg = new float[] { 5.0F, 1.0F, 1.5F, 1.0F };
                rgoal.SetWidths(widthsg);
                rgoal.HorizontalAlignment = 0;
                rgoal.SpacingBefore = 10.0F;

                if (dtSGoals.Rows.Count > 0)
                {
                    ht = writer.GetVerticalPosition(true);
                    if (ht < 250)
                        document.NewPage();

                    p = new Paragraph(new Phrase("Short Term Goals", boldTableFontU));
                    p.IndentationLeft = 205;
                    document.Add(p);

                    cell = new PdfPCell(new Phrase("", boldTableFont));
                    cell.Border = 0;
                    rgoal.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Met", boldTableFontSmallU));
                    cell.Border = 0;
                    rgoal.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Partially Met", boldTableFontSmallU));
                    cell.Border = 0;
                    rgoal.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Not Met", boldTableFontSmallU));
                    cell.Border = 0;
                    rgoal.AddCell(cell);

                    for (var i = 0; i <= dtSGoals.Rows.Count - 1; i++)
                    {
                        cell = new PdfPCell(new Phrase(dtSGoals.Rows[i]["Description"].ToString(), bodyFont));
                        cell.Border = 0;
                        rgoal.AddCell(cell);

                        if (dtSGoals.Rows[i]["GoalMet"].ToString() == "1")
                        {
                            var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/tick.png"));
                            cell = new PdfPCell(logo);
                        }
                        else
                        {
                            var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/untick.png"));
                            cell = new PdfPCell(logo);
                        }
                        cell.Border = 0;
                        cell.Padding = 4;
                        cell.Left = 4;
                        rgoal.AddCell(cell);

                        if (dtSGoals.Rows[i]["GoalPMet"].ToString() == "1")
                        {
                            var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/tick.png"));
                            cell = new PdfPCell(logo);
                        }
                        else
                        {
                            var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/untick.png"));
                            cell = new PdfPCell(logo);
                        }
                        cell.Border = 0;
                        cell.Padding = 4;
                        cell.Left = 4;
                        rgoal.AddCell(cell);

                        if (dtSGoals.Rows[i]["GoalNMet"].ToString() == "1")
                        {
                            var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/tick.png"));
                            cell = new PdfPCell(logo);
                        }
                        else
                        {
                            var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/untick.png"));
                            cell = new PdfPCell(logo);
                        }
                        cell.Border = 0;
                        cell.Padding = 4;
                        cell.Left = 4;
                        rgoal.AddCell(cell);
                    }
                    document.Add(rgoal);
                    Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                    document.Add(new Chunk(Singleline));
                }

                ht = writer.GetVerticalPosition(true);
                if (ht < 80)
                    document.NewPage();

                PdfPTable rgoall = new PdfPTable(4);
                rgoall.TotalWidth = 550.0F;
                rgoall.LockedWidth = true;
                float[] widthsl = new float[] { 5.0F, 1.0F, 1.5F, 1.0F };
                rgoall.SetWidths(widthsg);
                rgoall.HorizontalAlignment = 0;
                rgoall.SpacingBefore = 10.0F;

                if (dtLGoals.Rows.Count > 0)
                {
                    p = new Paragraph(new Phrase("Long Term Goals", boldTableFontU));
                    p.IndentationLeft = 205;
                    document.Add(p);

                    cell = new PdfPCell(new Phrase("", boldTableFont));
                    cell.Border = 0;
                    rgoall.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Met", boldTableFontSmallU));
                    cell.Border = 0;
                    rgoall.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Partially Met", boldTableFontSmallU));
                    cell.Border = 0;
                    rgoall.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Not Met", boldTableFontSmallU));
                    cell.Border = 0;
                    rgoall.AddCell(cell);

                    for (var i = 0; i <= dtLGoals.Rows.Count - 1; i++)
                    {
                        cell = new PdfPCell(new Phrase(dtLGoals.Rows[i]["Description"].ToString(), bodyFont));
                        cell.Border = 0;
                        rgoall.AddCell(cell);

                        if (dtLGoals.Rows[i]["GoalMet"].ToString() == "1")
                        {
                            var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/tick.png"));
                            cell = new PdfPCell(logo);
                        }
                        else
                        {
                            var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/untick.png"));
                            cell = new PdfPCell(logo);
                        }
                        cell.Border = 0;
                        cell.Padding = 4;
                        cell.Left = 4;
                        rgoall.AddCell(cell);

                        if (dtLGoals.Rows[i]["GoalPMet"].ToString() == "1")
                        {
                            var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/tick.png"));
                            cell = new PdfPCell(logo);
                        }
                        else
                        {
                            var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/untick.png"));
                            cell = new PdfPCell(logo);
                        }
                        cell.Border = 0;
                        cell.Padding = 4;
                        cell.Left = 4;
                        rgoall.AddCell(cell);

                        if (dtLGoals.Rows[i]["GoalNMet"].ToString() == "1")
                        {
                            var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/tick.png"));
                            cell = new PdfPCell(logo);
                        }
                        else
                        {
                            var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/untick.png"));
                            cell = new PdfPCell(logo);
                        }
                        cell.Border = 0;
                        cell.Padding = 4;
                        cell.Left = 4;
                        rgoall.AddCell(cell);
                    }
                    document.Add(rgoall);
                }


                Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                document.Add(new Chunk(Singleline));
            }
            // ********************************************
            // End of IE Goals
            // ********************************************

            // ********************************************
            // Start of Planned Interventions section
            // ********************************************
            // 
            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2" | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE"))
            {
                ht = writer.GetVerticalPosition(true);
                if (ht < 250)
                    document.NewPage();

                DataTable dtInt = new DataTable(), dtIntFD = new DataTable();
                dtInt = db.GetDocProInterventions(docrowid).Tables[0];

                dtIntFD = db.GetDocProInterventionsFD(docrowid).Tables[0];

                PdfPTable rinterFD = new PdfPTable(4);
                rinterFD.TotalWidth = 300.0F;
                rinterFD.LockedWidth = true;

                widths = new float[] { 4.0F, 1.0F, 4.0F, 2.0F };
                rinterFD.SetWidths(widths);
                rinterFD.HorizontalAlignment = 0;
                rinterFD.SpacingBefore = 10.0F;

                p = new Paragraph(new Phrase("Interventions", boldTableFontU));
                p.IndentationLeft = 230;
                document.Add(p);

                cell = new PdfPCell(new Phrase("Frequency", boldTableFont));
                cell.Border = 0;

                rinterFD.AddCell(cell);

                cell = new PdfPCell(new Phrase(dtIntFD.Rows[0]["Frequency"].ToString(), bodyFont));
                cell.Border = 0;
                rinterFD.AddCell(cell);

                cell = new PdfPCell(new Phrase("Duration", boldTableFont));
                cell.Border = 0;
                rinterFD.AddCell(cell);

                cell = new PdfPCell(new Phrase(dtIntFD.Rows[0]["Duration"].ToString(), bodyFont));
                cell.Border = 0;
                rinterFD.AddCell(cell);

                document.Add(rinterFD);

                PdfPTable rinter = new PdfPTable(4);
                rinter.TotalWidth = 600.0F;
                rinter.LockedWidth = true;

                widths = new float[] { 3.0F, 1.0F, 3.0F, 1.0F };
                rinter.SetWidths(widths);
                rinter.HorizontalAlignment = 0;
                rinter.SpacingBefore = 10.0F;

                cell = new PdfPCell(new Phrase("Intervention", boldTableFont));
                cell.Border = 0;
                cell.Colspan = 2;
                rinter.AddCell(cell);

                cell = new PdfPCell(new Phrase("CPT Code", boldTableFont));
                cell.Border = 0;
                cell.Colspan = 2;
                rinter.AddCell(cell);

                for (var i = 0; i <= dtInt.Rows.Count - 1; i++)
                {
                    cell = new PdfPCell(new Phrase(dtInt.Rows[i]["CPTDescription"].ToString(), bodyFont));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    rinter.AddCell(cell);

                    cell = new PdfPCell(new Phrase(dtInt.Rows[i]["CPTCode"].ToString(), bodyFont));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    rinter.AddCell(cell);
                }

                document.Add(rinter);

                Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                document.Add(new Chunk(Singleline));
            }
            // ********************************************
            // End of Planned Interventions section
            // ********************************************

            // '********************************************
            // ' Start of Treatment Description
            // '********************************************
            // '
            // If (  HttpContext.Current.Session["NoteType") = "PPOC" Or HttpContext.Current.Session["NoteType") = "PPOC2"   ) Then

            // Dim rtreatdes As New PdfPTable(1)
            // rtreatdes.TotalWidth = 600.0F
            // rtreatdes.LockedWidth = True

            // 'leave a gap before and after the table
            // rtreatdes.SpacingBefore = 5.0F
            // rtreatdes.SpacingAfter = 5.0F

            // p = New Paragraph(New Phrase("Treatment Description", boldTableFontU))
            // p.IndentationLeft = 205
            // document.Add(p)
            // Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.BLACK , Element.ALIGN_LEFT, 1)
            // document.Add(New Chunk(Singleline))

            // cell = New PdfPCell(New Phrase("treat desc", boldTableFont))
            // cell.Border = 0
            // rtreatdes.AddCell(cell)

            // document.Add(rtreatdes)

            // Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.BLACK , Element.ALIGN_LEFT, 1)
            // document.Add(New Chunk(Singleline))
            // End If
            // '********************************************
            // ' End of Treatment Description
            // '********************************************

            // ********************************************
            // Start of Reasons for Missed Visit
            // ********************************************
            // 
            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PMV"))
            {
                p = new Paragraph(new Phrase("Reasons for Missed Visit", boldTableFontU));
                p.IndentationLeft = 200;
                document.Add(p);

                PdfPTable rmissvi = new PdfPTable(2);
                rmissvi.TotalWidth = 500.0F;
                rmissvi.LockedWidth = true;

                // relative col widths in proportions - 1:1
                widths = new float[] { 1F, 2.0F };
                rmissvi.SetWidths(widths);
                rmissvi.HorizontalAlignment = 0;

                // leave a gap before and after the table
                rmissvi.SpacingBefore = 10.0F;

                // Start  new Page if very small space left on the page
                ht = writer.GetVerticalPosition(true);
                if (ht < 250)
                    document.NewPage();

                DataTable dtM = new DataTable();
                dtM = db.GetDocMissed(docrowid,
                    PatientId).Tables[0];

                cell = new PdfPCell(new Phrase("Patient was not treated due to", boldTableFont));
                cell.Border = 0;
                rmissvi.AddCell(cell);

                if (dtM.Rows.Count > 0)
                    cell = new PdfPCell(new Phrase(dtM.Rows[0]["reason"].ToString(), bodyFont));
                else
                    cell = new PdfPCell(new Phrase("", boldTableFont));
                cell.Border = 0;
                rmissvi.AddCell(cell);

                document.Add(rmissvi);

                PdfPTable rmissvi2 = new PdfPTable(2);
                rmissvi2.TotalWidth = 500.0F;
                rmissvi2.LockedWidth = true;

                // relative col widths in proportions - 1:1        
                widths = new float[] { 1.0F, 8.0F };
                rmissvi2.SetWidths(widths);
                rmissvi2.HorizontalAlignment = 0;

                // leave a gap before and after the table
                rmissvi2.SpacingBefore = 10.0F;

                cell = new PdfPCell(new Phrase("Plan", boldTableFont));
                cell.Border = 0;
                rmissvi2.AddCell(cell);

                if (dtM.Rows.Count > 0)
                    cell = new PdfPCell(new Phrase(dtM.Rows[0]["plandetails"].ToString(), bodyFont));
                else
                    cell = new PdfPCell(new Phrase("", boldTableFont));
                cell.Border = 0;
                rmissvi2.AddCell(cell);

                document.Add(rmissvi2);

                PdfPTable rmissvi3 = new PdfPTable(2);
                rmissvi3.TotalWidth = 500.0F;
                rmissvi3.LockedWidth = true;

                // relative col widths in proportions - 1:1        
                widths = new float[] { 0.7F, 2.0F };
                rmissvi3.SetWidths(widths);
                rmissvi3.HorizontalAlignment = 0;

                // leave a gap before and after the table
                rmissvi3.SpacingBefore = 10.0F;

                cell = new PdfPCell(new Phrase("Missed Treatment Note", boldTableFont));
                cell.Border = 0;
                rmissvi3.AddCell(cell);

                if (dtM.Rows.Count > 0)
                    cell = new PdfPCell(new Phrase(dtM.Rows[0]["note"].ToString(), bodyFont));
                else
                    cell = new PdfPCell(new Phrase("", boldTableFont));
                cell.Border = 0;
                rmissvi3.AddCell(cell);

                document.Add(rmissvi3);

                Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                document.Add(new Chunk(Singleline));
            }
            // ********************************************
            // End of Reasons for Missed Visit
            // ********************************************


            // ********************************************
            // Start of Note Summary
            // ********************************************
            // 
            // If (  HttpContext.Current.Session["NoteType") = "PPOC" Or HttpContext.Current.Session["NoteType") = "PPOC2" Or HttpContext.Current.Session["NoteType") = "PPOCRE" Or HttpContext.Current.Session["NoteType") = "PDIS"   ) Then

            // ht = writer.GetVerticalPosition(true)
            // If ht < 80 Then
            // document.NewPage()
            // End If

            // p = New Paragraph(New Phrase("Note Summary", boldTableFontU))
            // p.IndentationLeft = 205
            // document.Add(p)

            // Dim dsTDR As New DataSet 'Treatment Notes Table 4
            // dsTDR = DocManager.GetDocNoteSummaryIER(HttpContext.Current.Session["docrowid"))

            // Dim p1 = New Paragraph (New Phrase(dsTDR.Tables[0].Rows[0)["notesummary"].ToString(), bodyFont))
            // p1.SpacingBefore = 10.0f
            // document.Add(p1)      

            // Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.BLACK , Element.ALIGN_LEFT, 1)                    
            // document.Add(New Chunk(Singleline))

            // End If
            // ********************************************
            // End of Note Summary
            // ********************************************


            // ********************************************
            // Start Treatment Notes
            // ********************************************
            // 

            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC" |
               Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2" |
               Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PTREAT" |
               Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PMN" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE" |
                Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PCOMM"))
            {
                DataSet dsTDR = new DataSet(); // Treatment Notes Table 4
                if (Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PCOMM" |
                    Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC" |
                    Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2" |
                    Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2" |
                    Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE")
                    dsTDR = db.GetDocNoteSummary(docrowid);
                else
                    dsTDR = db.GetDocTreatDescR(docrowid);

                if (dsTDR.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PCOMM")
                    {
                    }
                    else
                    {
                        ht = writer.GetVerticalPosition(true);
                        if (ht < 250)
                            document.NewPage();
                    }

                    if (Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PCOMM")
                    {
                        p = new Paragraph(new Phrase("Therapy Comments", boldTableFontU));
                        p.IndentationLeft = 205;
                        document.Add(p);
                    }
                    else if (Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PTREAT"
                     )
                    {
                        p = new Paragraph(new Phrase("Treatment Notes", boldTableFontU));
                        p.IndentationLeft = 205;
                        document.Add(p);
                    }
                    else if (Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC"
                     )
                    {
                        p = new Paragraph(new Phrase("Note Summary", boldTableFontU));
                        p.IndentationLeft = 205;
                        document.Add(p);
                    }
                    else if (Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2"
                     )
                    {
                        p = new Paragraph(new Phrase("Note Summary", boldTableFontU));
                        p.IndentationLeft = 205;
                        document.Add(p);
                    }
                    else if (Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE"
                     )
                    {
                        p = new Paragraph(new Phrase("Note Summary", boldTableFontU));
                        p.IndentationLeft = 205;
                        document.Add(p);
                    }
                    else if (Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PMN")
                    {
                        p = new Paragraph(new Phrase("Treatment Description", boldTableFontU));
                        p.IndentationLeft = 205;
                        document.Add(p);
                    }



                    if (Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PCOMM"
                        | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC" |
                        Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2" |
                        Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE")
                    {
                        var p1 = new Paragraph(new Phrase(dsTDR.Tables[0].Rows[0]["notesummary"].ToString(), bodyFont));
                        p1.SpacingBefore = 10.0F;
                        document.Add(p1);
                    }
                    else
                    {
                        var p1 = new Paragraph(new Phrase(dsTDR.Tables[0].Rows[0]["treatdesc"].ToString(), bodyFont));
                        p1.SpacingBefore = 10.0F;
                        document.Add(p1);
                    }


                    Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                    document.Add(new Chunk(Singleline));
                }
            }
            // ********************************************
            // End Treatment Notes
            // ********************************************

            // ********************************************
            // Start of Discharge Summary
            // ********************************************
            // 
            if ((Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PDIS"))
            {
                dt = db.GetDocDisSummary(docrowid).Tables[0];

                if (dt.Rows.Count > 0)
                {

                    // Start  new Page if very small space left on the page
                    ht = writer.GetVerticalPosition(true);
                    if (ht < 250)
                        document.NewPage();

                    PdfPTable rdissum = new PdfPTable(2);
                    rdissum.TotalWidth = 580.0F;
                    rdissum.LockedWidth = true;

                    p = new Paragraph(new Phrase("Discharge Summary", boldTableFontU));
                    p.IndentationLeft = 205;
                    document.Add(p);

                    // relative col widths in proportions - 2:1:2:1
                    widths = new float[] { 2.5F, 1.0F };
                    rdissum.SetWidths(widths);
                    rdissum.HorizontalAlignment = 0;
                    rdissum.HorizontalAlignment = 0;

                    // leave a gap before and after the table
                    rdissum.SpacingBefore = 10.0F;

                    cell = new PdfPCell(new Phrase("Has the patient / caregiver been given proper notification of discharge ?", boldTableFont));
                    cell.Border = 0;
                    rdissum.AddCell(cell);

                    RadioCheckField chkbox;
                    Rectangle rect = new Rectangle(180, 180, 180, 180);


                    // cell = New PdfPCell(New Phrase(dt.Rows[0)["notification"].ToString(), bodyFont))
                    // cell.Border = 0
                    // rdissum.AddCell(cell)

                    if (dt.Rows[0]["notification"].ToString() == "True")
                    {
                        var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/tick.png"));
                        cell = new PdfPCell(logo);
                    }
                    else
                    {
                        var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/untick.png"));
                        cell = new PdfPCell(logo);
                    }
                    cell.Border = 0;
                    rdissum.AddCell(cell);

                    document.Add(rdissum);

                    PdfPTable rdissum2 = new PdfPTable(2);
                    rdissum2.TotalWidth = 580.0F;
                    rdissum2.LockedWidth = true;

                    // relative col widths in proportions - 1:1
                    widths = new float[] { 1.2F, 1.0F };
                    rdissum2.SetWidths(widths);
                    rdissum2.HorizontalAlignment = 0;

                    // leave a gap before and after the table
                    rdissum2.SpacingBefore = 10.0F;

                    cell = new PdfPCell(new Phrase("No further therapeutic intervention is indicated at this time.", boldTableFont));
                    cell.Border = 0;
                    rdissum2.AddCell(cell);

                    if (dt.Rows[0]["further"].ToString() == "True")
                    {
                        var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/tick.png"));
                        cell = new PdfPCell(logo);
                    }
                    else
                    {
                        var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/untick.png"));
                        cell = new PdfPCell(logo);
                    }
                    cell.Border = 0;
                    rdissum2.AddCell(cell);

                    document.Add(rdissum2);

                    PdfPTable rdissum3 = new PdfPTable(2);
                    rdissum3.TotalWidth = 580.0F;
                    rdissum3.LockedWidth = true;

                    // relative col widths in proportions - 1:1
                    widths = new float[] { 1.0F, 3.0F };
                    rdissum3.SetWidths(widths);
                    rdissum3.HorizontalAlignment = 0;

                    // leave a gap before and after the table
                    rdissum3.SpacingBefore = 10.0F;

                    cell = new PdfPCell(new Phrase("Reason for Discharge", boldTableFont));
                    cell.Border = 0;
                    rdissum3.AddCell(cell);

                    cell = new PdfPCell(new Phrase(dt.Rows[0]["reasons"].ToString(), bodyFont));
                    cell.Border = 0;
                    rdissum3.AddCell(cell);

                    document.Add(rdissum3);

                    PdfPTable rdissum4 = new PdfPTable(2);
                    rdissum4.TotalWidth = 550.0F;
                    rdissum4.LockedWidth = true;

                    // relative col widths in proportions - 1:1
                    widths = new float[] { 1.0F, 2.0F };
                    rdissum4.SetWidths(widths);
                    rdissum4.HorizontalAlignment = 0;

                    // leave a gap before and after the table
                    rdissum4.SpacingBefore = 10.0F;

                    cell = new PdfPCell(new Phrase("Further instructions for patient", boldTableFont));
                    cell.Border = 0;
                    rdissum4.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(new Phrase(dt.Rows[0]["patins"].ToString(), bodyFont)));
                    cell.Border = 0;
                    rdissum4.AddCell(cell);

                    document.Add(rdissum4);

                    PdfPTable rdissum5 = new PdfPTable(2);
                    rdissum5.TotalWidth = 550.0F;
                    rdissum5.LockedWidth = true;

                    // relative col widths in proportions - 1:1
                    widths = new float[] { 1.0F, 2.0F };
                    rdissum5.SetWidths(widths);
                    rdissum5.HorizontalAlignment = 0;

                    // leave a gap before and after the table
                    rdissum5.SpacingBefore = 10.0F;

                    cell = new PdfPCell(new Phrase("Additional Information", boldTableFont));
                    cell.Border = 0;
                    rdissum5.AddCell(cell);

                    var p1 = new Paragraph(new Phrase(dt.Rows[0]["addlinfo"].ToString(), bodyFont));
                    p1.SpacingBefore = 10.0F;

                    cell = new PdfPCell(p1);
                    cell.Border = 0;
                    rdissum5.AddCell(cell);

                    document.Add(rdissum5);

                    Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1F, 100F, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                    document.Add(new Chunk(Singleline));
                }
            }
            // ********************************************
            // End of Discharge Summary
            // ********************************************

            // ********************************************
            // Start of Sign box
            // ********************************************
            // ***** signbox2 for Doctor

            PdfPTable signbox2 = new PdfPTable(2);
            // signbox.SplitLate = false
            signbox2.TotalWidth = 250.0F;
            signbox2.LockedWidth = true;

            // relative col widths in proportions - 1:1        
            widths = new float[] { 2.2F, 1.0F };
            signbox2.SetWidths(widths);
            signbox2.HorizontalAlignment = 0;

            cell = new PdfPCell(new Phrase(" ", bodyFont));
            cell.Colspan = 2;
            cell.Border = 0;
            // cell.Padding = 8
            cell.Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
            signbox2.AddCell(cell);

            cell = new PdfPCell(new Phrase(" ", bodyFont));
            cell.Border = 0;
            cell.Rowspan = 2;
            cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
            // Cell.Padding = 8

            signbox2.AddCell(cell);

            cell = new PdfPCell(new Phrase(" ", bodyFont));
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER;
            // cell.Padding = 8
            cell.HorizontalAlignment = 2;
            signbox2.AddCell(cell);

            cell = new PdfPCell(new Phrase(" ", bodyFont));
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
            // cell.Padding = 8
            cell.HorizontalAlignment = 2;
            signbox2.AddCell(cell);

            cell = new PdfPCell(new Phrase(Convert.ToString(HttpContext.Current.Session["PrintName"]) + "             Date/Time", bodyFont));
            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
            // cell.Padding = 8
            cell.Colspan = 2;
            signbox2.AddCell(cell);

            cell = new PdfPCell(new Phrase("NPI " + Convert.ToString(HttpContext.Current.Session["NPI"]), bodyFont));
            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
            // cell.Padding = 8
            cell.Colspan = 2;
            signbox2.AddCell(cell);

            cell = new PdfPCell(new Phrase("I certify the need for these services furnished under  this plan of treatment  while under my care.", bodyFontSmall));
            cell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
            cell.Colspan = 2;
            // cell.Padding = 8
            signbox2.AddCell(cell);

            // ***** signbox for Therapist
            if (Type == "Preview")
            {
            }
            else
            {
                if (Convert.ToString(HttpContext.Current.Session["UserRole"]).Trim() == "Therapist")
                    dt = db.GetDocTherapist(Convert.ToInt32(HttpContext.Current.Session["user"]));
                else
                    // dt = DocManager.GetDocTherapistRpt(TryCast(Me.Master.FindControl("ctl00$ctl00$hdndocrowid"), HiddenField).Value).Tables[0]
                    dt = db.GetDocTherapistRpt(Convert.ToInt32(HttpContext.Current.Session["docrowid"]));

                PdfPTable signbox = new PdfPTable(2);
                // signbox.SplitLate = false
                signbox.TotalWidth = 250.0F;
                signbox.LockedWidth = true;

                // relative col widths in proportions - 1:1        
                widths = new float[] { 2.2F, 1.0F };
                signbox.SetWidths(widths);
                signbox.HorizontalAlignment = 0;

                cell = new PdfPCell(new Phrase("Electronically Signed By", bodyFont));
                cell.Colspan = 2;
                cell.Border = 0;
                // cell.Padding = 8
                cell.Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                signbox.AddCell(cell);

                DataTable dtSig = new DataTable();
                if (HttpContext.Current.Session["UserRole"].ToStringOrEmpty() == "Therapist")
                    dtSig = db.GetDocTherapist(Convert.ToInt32(HttpContext.Current.Session["user"]));
                else
                    dtSig = db.GetDocTherapistRpt(Convert.ToInt32(HttpContext.Current.Session["docrowid"]));
                Byte[] bytes = (Byte[])dtSig.Rows[0]["signaturefile"];

                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(bytes);
                // Do something special with the empty ones
                if (bytes.Length == 0)
                {
                }
                else
                {
                    image.ScaleToFit(100F, 200F);
                    Chunk imageChunk = new Chunk(image, 0, 0);
                }

                PdfPCell imageCell = new PdfPCell(image);
                imageCell.Border = 0;
                imageCell.Rowspan = 2;
                imageCell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                imageCell.Padding = 2;

                signbox.AddCell(imageCell);


                cell = new PdfPCell(new Phrase(DateTime.Today.ToString("MM/dd/yyyy"), bodyFont));
                cell.Border = 0;
                cell.Border = Rectangle.RIGHT_BORDER;
                // cell.Padding = 8
                cell.HorizontalAlignment = 2;
                signbox.AddCell(cell);

                cell = new PdfPCell(new Phrase(DateTime.Now.ToString("hh:mm:ss tt"), bodyFont));
                cell.Border = 0;
                cell.Border = Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                // cell.Padding = 8
                cell.HorizontalAlignment = 2;
                signbox.AddCell(cell);

                cell = new PdfPCell(new Phrase(dtSig.Rows[0]["Name"].ToStringOrEmpty(), bodyFont));
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                // cell.Padding = 8
                cell.Colspan = 2;
                signbox.AddCell(cell);

                cell = new PdfPCell(new Phrase("NPI " + dtSig.Rows[0]["NPI"].ToStringOrEmpty(), bodyFont));
                cell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.Colspan = 2;
                // cell.Padding = 8
                signbox.AddCell(cell);

                cell = new PdfPCell(new Phrase("State License #  " + dtSig.Rows[0]["LicenseNumber"], bodyFont));
                cell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                cell.Colspan = 2;
                // cell.Padding = 8
                signbox.AddCell(cell);

                PdfPTable MainSignbox = new PdfPTable(2);
                MainSignbox.TotalWidth = 540F;
                MainSignbox.LockedWidth = true;

                // relative col widths in proportions - 1:1        
                widths = new float[] { 1.0F, 1.0F };
                MainSignbox.SetWidths(widths);
                MainSignbox.HorizontalAlignment = 0;

                if (Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PTREAT" |
                   Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PMV" |
                    Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PMN" |
                    Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PCOMM" |
                    Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PDIS")
                {
                    cell = new PdfPCell(new Phrase("", bodyFont));
                    cell.Border = 0;
                    MainSignbox.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.AddElement(signbox2);
                    MainSignbox.AddCell(cell);
                }

                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(signbox);
                MainSignbox.AddCell(cell);

                document.Add(MainSignbox);

                // ********************************************
                // Clinic Phone and Fax
                // ********************************************

                dt = db.GetPatient(PatientId);

                if (Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC"
                    | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOC2"
                    | Convert.ToString(HttpContext.Current.Session["NoteType"]) == "PPOCRE")
                {
                    var p1 = new Paragraph(new Phrase("Clinic Phone Number : " + dt.Rows[0]["ClinicPhone"].ToString(), bodyFontMed));
                    p1.SpacingBefore = 10.0F;
                    document.Add(p1);
                    p1 = new Paragraph(new Phrase("  Clinic Fax Number : " + dt.Rows[0]["Fax"].ToString(), bodyFontMed));
                    document.Add(p1);
                }
            }



            document.Close();

            //  byte[] bytes1 = File.ReadAllBytes(rptSavePath1);
            byte[] bytes1 = ms.ToArray();
            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(bytes1);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    // stamper.SetEncryption(PdfWriter.ENCRYPTION_AES_128, "", "", PdfWriter.ALLOW_ASSEMBLY)
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i),
                            Element.ALIGN_CENTER, new Phrase(i + " of " + pages, bodyFont), 510, 725, 0);
                }
                bytes1 = stream.ToArray();
            }
            //appdatafiles\EasyDoc\MOS
            string containerName = rptSavePath + "\\" + path;
            CloudeStoreg.createFileFromBytes(containerName, rptName, bytes1);
            // File.WriteAllBytes("D:\PDFs\Test_1.pdf", bytes1)
            //   File.WriteAllBytes(rptSavePath1, bytes1);
            rptSavePath1 = containerName + "\\" + rptName + ".pdf";
            string strSQL;

            if (Type == "Preview")
            {
            }
            else if (NoteCnt == 0)
            {
                if (HttpContext.Current.Session["UserRole"].ToStringOrEmpty() == "Therapist")
                    db.IUDDocMaster("U", docrowid,
                        PatientId, "", 0, "", "", "1", "",
                        rptSavePath1, "0", HttpContext.Current.Session["User"].ToStringOrEmpty());
                else
                    db.DocSigned(docrowid, rptSavePath1);
            }
            else if (NoteCnt == 1)
            {
                if (HttpContext.Current.Session["UserRole"].ToStringOrEmpty() == "Therapist")
                    db.IUDDocMaster("U", docrowid,
                        PatientId, "", 0, "", "", "1", "",
                        rptSavePath1, "0", HttpContext.Current.Session["User"].ToStringOrEmpty());
                else
                    db.DocSigned(docrowid, rptSavePath1);
            }
            else if (NoteCnt == 2
            )
            {
                strSQL = "Update tblDocMaster set PDFName2 = '" + rptSavePath1 + "' where Docrowid = " +
                    docrowid;
                db.ExecuteScalar(strSQL);
            }
            else if (NoteCnt == 3
            )
            {
                strSQL = "Update tblDocMaster set PDFName3 = '" + rptSavePath1 + "' where Docrowid = " +
                    docrowid;
                db.ExecuteScalar(strSQL);
            }

            return rptSavePath1;
        }

        public string PrintiTextWC(string Type, string SignPrint, int NoteCnt, int RS, int PatientId, int docrowid, string NoteType)
        {
            DataTable dtD = new DataTable();
            DataTable dt = new DataTable();
            DataTable dtv = new DataTable();
            DataTable dtPatVisits = new DataTable();
            // Patient Visits
            DataTable dtTher = new DataTable();
            DocManager db = new DocManager();
            //  Treating Therapist
            DataSet ds;
            // DBHelper db = new DBHelper();
            string rptName;
            string rptSavePath;
            string rptSavePath1;
            string strSQL;
            HttpContext.Current.Session["patientid"] = PatientId;
            HttpContext.Current.Session["docrowid"] = docrowid;
            HttpContext.Current.Session["NoteType"] = NoteType;
            if ((RS == 0))
            {
                HttpContext.Current.Session["PrintName"] = "";
                HttpContext.Current.Session["NPI"] = "";
            }
            else
            {
                string strSQL1;
                strSQL1 = (" Select PrintName, NPINumber from tblreferrer where Rrowid = " + RS);
                dt = db.ExecuteDataset(strSQL1).Tables[0];
                if ((dt.Rows.Count > 0))
                {
                    HttpContext.Current.Session["PrintName"] = dt.Rows[0]["PrintName"].ToStringOrEmpty();
                    HttpContext.Current.Session["NPI"] = dt.Rows[0]["NPINumber"].ToStringOrEmpty();
                }

            }

            dtD = db.GetDocDateOfService(docrowid);
            dt = db.GetPatient(PatientId);
            if ((NoteCnt == 0))
            {
                rptName = ((dt.Rows[0][0].ToStringOrEmpty().PadLeft(3, '0') + ("-" + HttpContext.Current.Session["patientid"].ToStringOrEmpty().PadLeft(6, '0')))
                    + ("_"
                            + (dt.Rows[0]["FirstName"] + ("_"
                            + (dt.Rows[0]["LastName"] + ("_"
                            + (HttpContext.Current.Session["NoteType"] + ("WC" + ("_"
                            + (dtD.Rows[0]["DateOfService"].ToStringOrEmpty().Replace("/", "-") +
                            ("_" + DateTime.Now.ToStringOrEmpty().Replace(":", "-").Replace("/", "-").Replace(" ", "-"))))))))))));
            }
            else
            {
                rptName = ((dt.Rows[0][0].ToStringOrEmpty().PadLeft(3, '0') + ("-" + HttpContext.Current.Session["patientid"].ToStringOrEmpty().
                    PadLeft(6, '0'))) + ("_"
                            + (dt.Rows[0]["FirstName"] + ("_"
                            + (dt.Rows[0]["LastName"] + ("_"
                            + (HttpContext.Current.Session["NoteType"] + ("WC" + ("_"
                            + (NoteCnt.ToStringOrEmpty() + ("_"
                            + (dtD.Rows[0]["DateOfService"].ToStringOrEmpty().Replace("/", "-")
                            + ("_" + DateTime.Now.ToStringOrEmpty().Replace(":", "-").Replace("/", "-").Replace(" ", "-"))))))))))))));
            }
            string path = Convert.ToString(dt.Rows[0]["Folder"]);
            rptSavePath = (ConfigurationManager.AppSettings["folderpath"] + dt.Rows[0]["Folder"].ToStringOrEmpty());
            rptSavePath1 = (rptSavePath + ("\\"
                        + (rptName + ".pdf")));
            if (!Directory.Exists(rptSavePath))
            {
                Directory.CreateDirectory(rptSavePath);
            }


            float ht;
            MemoryStream ms = new MemoryStream();
            DataTable tblMeasurement = new DataTable();
            DataRow nRowMeasurement;
            DataColumn Col = new DataColumn();
            Document document = new Document(PageSize.A4, 10, 10, 90, 10);
            string output = rptSavePath1;
            //  PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(output, FileMode.CreateNew));
            PdfWriter writer = PdfWriter.GetInstance(document, ms);

            //  Open the Document for writing
            document.Open();
            writer.PageEvent = new MyHeaderFooterNoteEventWC();
            var titleFont = FontFactory.GetFont("Roboto", 18, Font.BOLD);
            var subTitleFont = FontFactory.GetFont("Roboto", 14, Font.BOLD);
            var boldTableFont = FontFactory.GetFont("Roboto", 11, Font.BOLD);
            var boldTableFontU = FontFactory.GetFont("Roboto", 12, (Font.BOLD | Font.UNDERLINE));
            var boldTableFontSmallU = FontFactory.GetFont("Roboto", 10, (Font.BOLD | Font.UNDERLINE));
            // boldTableFontU.SetStyle(Font.UNDERLINE)
            var endingMessageFont = FontFactory.GetFont("Arial", 10, Font.ITALIC);
            var bodyFont = FontFactory.GetFont("Roboto", 10, Font.NORMAL);
            var bodyFontSmall = FontFactory.GetFont("Roboto", 8, Font.NORMAL);
            var bodyFontSmallU = FontFactory.GetFont("Roboto", 8, (Font.NORMAL | Font.UNDERLINE));
            var bodyFontSmallBold = FontFactory.GetFont("Roboto", 8, Font.BOLD);
            var bodyFontC = FontFactory.GetFont("Courier", 7, Font.BOLD);
            PdfPCell cell;
            PdfContentByte canvas = writer.DirectContent;
            float[] widths;
            iTextSharp.text.pdf.draw.LineSeparator Singleline;
            Paragraph p;
            // ********************************************
            //  Start Diagnosis section  
            // ********************************************
            DataTable dtPD = new DataTable();
            //  Patient Diagnosis details
            dtPD = db.GetPatDiagnosis(PatientId).Tables[0];
            PdfPTable rdiag = new PdfPTable(2);
            if ((dtPD.Rows.Count > 0))
            {
                rdiag.TotalWidth = 180.0F;
                rdiag.LockedWidth = true;
                widths = new float[] {
                    1.4F,
                    4.0F};
                rdiag.SetWidths(widths);
                rdiag.HorizontalAlignment = 0;
                for (int i = 0; i <= (dtPD.Rows.Count - 1); i++)
                {
                    if ((i == 0))
                    {
                        cell = new PdfPCell(new Phrase("Diagnosis", bodyFontSmall));
                        cell.Border = 0;
                        cell.Padding = 2;
                        rdiag.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Phrase(" ", bodyFontSmall));
                        cell.Border = 0;
                        cell.Padding = 2;
                        rdiag.AddCell(cell);
                    }

                    cell = new PdfPCell(new Phrase(dtPD.Rows[i]["DiagnosisCode"].ToStringOrEmpty()
                        + " " + dtPD.Rows[i]["Description"].ToStringOrEmpty(), bodyFontSmall));
                    cell.Border = 0;
                    cell.Padding = 2;
                    rdiag.AddCell(cell);
                }

            }


            // ********************************************
            //  End Diagnosis section  
            // ********************************************
            PdfPTable tblPat = new PdfPTable(1);
            tblPat.TotalWidth = 220.0F;
            tblPat.LockedWidth = true;
            widths = new float[] {
                1.0F};
            tblPat.SetWidths(widths);
            tblPat.HorizontalAlignment = 0;
            // cell = New PdfPCell(New Phrase("Name : Abhijeet Karve", bodyFontSmall))
            cell = new PdfPCell(new Phrase("Name : "
                                + dt.Rows[0]["LastName"] + ", " + dt.Rows[0]["FirstName"], bodyFontSmall));
            cell.Border = 0;
            cell.Padding = 2;
            tblPat.AddCell(cell);
            // cell = New PdfPCell(New Phrase("MR# : 123-456789 DOB: 2/2/1975 Sex:M", bodyFontSmall))
            cell = new PdfPCell(new Phrase("MR# : "
                                + dt.Rows[0][0].ToStringOrEmpty().PadLeft(3, '0') +
                                "-" + HttpContext.Current.Session["patientid"].ToStringOrEmpty().PadLeft(6, '0') + "   DOB: "
                                + dt.Rows[0]["BirthDate"] + "   Sex : " + dt.Rows[0]["Gender"], bodyFontSmall));
            cell.Border = 0;
            cell.Padding = 2;
            tblPat.AddCell(cell);
            // cell = New PdfPCell(New Phrase("Date Of Injury: 2/2/1975", bodyFontSmall))
            cell = new PdfPCell(new Phrase(("Date Of Injury : " + dt.Rows[0]["InjuryDate"]), bodyFontSmall));
            cell.Border = 0;
            cell.Padding = 2;
            tblPat.AddCell(cell);

            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(rdiag);
            tblPat.AddCell(cell);

            PdfPTable tblRef = new PdfPTable(1);
            tblRef.TotalWidth = 180.0F;
            tblRef.LockedWidth = true;
            widths = new float[] {
                1.0F};
            tblRef.SetWidths(widths);
            tblRef.HorizontalAlignment = 0;
            // cell = New PdfPCell(New Phrase("Physician : Abhijeet Karve", bodyFontSmall))
            cell = new PdfPCell(new Phrase("Physician : " + HttpContext.Current.Session["PrintName"].ToStringOrEmpty(), bodyFontSmall));
            cell.Border = 0;
            cell.Padding = 2;
            tblRef.AddCell(cell);
            cell = new PdfPCell(new Phrase(" ", bodyFontSmall));
            cell.Border = 0;
            cell.Padding = 2;
            tblRef.AddCell(cell);
            dtPatVisits = db.GetPatVisits(PatientId);
            // cell = New PdfPCell(New Phrase("visit Count : 5", bodyFontSmall))        
            cell = new PdfPCell(new Phrase("Visits to Date : " + dtPatVisits.Rows[0]["Visits"], bodyFontSmall));
            cell.Border = 0;
            cell.Padding = 2;
            tblRef.AddCell(cell);
            strSQL = (" Select Auths = coalesce(sum(NoOfVisits), 0) " + (" FROM tblPatAuths WHERE PTrowid = " + PatientId));
            dtv = db.ExecuteDataset(strSQL).Tables[0];
            cell = new PdfPCell(new Phrase(("Visits Authorized : " + dtv.Rows[0]["Auths"]), bodyFontSmall));
            cell.Border = 0;
            cell.Padding = 2;
            tblRef.AddCell(cell);
            // cell = New PdfPCell(New Phrase("Missed Visit Count : 2", bodyFontSmall))
            strSQL = (" Select Visits = count(*) " + (" FROM dbo.tblDocMaster " + (" WHERE PTrowid = "
                        + (PatientId + " AND (NoteType = \'PMV\')"))));
            dtv = db.ExecuteDataset(strSQL).Tables[0];
            cell = new PdfPCell(new Phrase("Visits Missed : " + dtv.Rows[0]["Visits"], bodyFontSmall));
            cell.Border = 0;
            cell.Padding = 2;
            tblRef.AddCell(cell);

            PdfPTable tblEmpl = new PdfPTable(1);
            tblEmpl.TotalWidth = 180.0F;
            tblEmpl.LockedWidth = true;
            widths = new float[] {
                1.0F};
            tblEmpl.SetWidths(widths);
            tblEmpl.HorizontalAlignment = 0;
            // cell = New PdfPCell(New Phrase("Physician : Abhijeet Karve", bodyFontSmall))
            cell = new PdfPCell(new Phrase("Employer : " + dt.Rows[0]["Employer"], bodyFontSmall));
            cell.Border = 0;
            cell.Padding = 2;
            tblEmpl.AddCell(cell);
            ds = db.GetPatWorkComp(PatientId);
            // cell = New PdfPCell(New Phrase("Surgery Date : 8/8/2017 ", bodyFontSmall))
            if ((ds.Tables[0].Rows.Count > 0))
            {
                cell = new PdfPCell(new Phrase("Surgery Date : " + ds.Tables[0].Rows[0]["SurgeryDate"].ToStringOrEmpty(), bodyFontSmall));
            }
            else
            {
                cell = new PdfPCell(new Phrase("Surgery Date :  ", bodyFontSmall));
            }

            cell.Border = 0;
            cell.Padding = 2;
            tblEmpl.AddCell(cell);
            if ((ds.Tables[0].Rows.Count > 0))
            {
                cell = new PdfPCell(new Phrase("Case Manager : " + ds.Tables[0].Rows[0]["CaseManager"].ToStringOrEmpty(), bodyFontSmall));
            }
            else
            {
                cell = new PdfPCell(new Phrase("Case Manager : ", bodyFontSmall));
            }

            cell.Border = 0;
            cell.Padding = 2;
            tblEmpl.AddCell(cell);
            if ((ds.Tables[0].Rows.Count > 0))
            {
                cell = new PdfPCell(new Phrase("Adjuster : " + ds.Tables[0].Rows[0]["Adjustor"].ToStringOrEmpty(), bodyFontSmall));
            }
            else
            {
                cell = new PdfPCell(new Phrase("Adjuster : ", bodyFontSmall));
            }

            cell.Border = 0;
            cell.Padding = 2;
            tblEmpl.AddCell(cell);
            if ((ds.Tables[0].Rows.Count > 0))
            {
                cell = new PdfPCell(new Phrase("Attorney : " + ds.Tables[0].Rows[0]["Attorney"].ToStringOrEmpty(), bodyFontSmall));
            }
            else
            {
                cell = new PdfPCell(new Phrase("Attorney : ", bodyFontSmall));
            }

            cell.Border = 0;
            cell.Padding = 2;
            tblEmpl.AddCell(cell);
            PdfPTable tbl1 = new PdfPTable(3);
            tbl1.TotalWidth = 580.0F;
            tbl1.LockedWidth = true;
            widths = new float[] {
                1.3F,
                1.0F,
                1.0F};
            tbl1.SetWidths(widths);
            tbl1.HorizontalAlignment = 0;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(tblPat);
            tbl1.AddCell(cell);
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(tblRef);
            tbl1.AddCell(cell);
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(tblEmpl);
            tbl1.AddCell(cell);
            document.Add(tbl1);
            Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
            document.Add(new Chunk(Singleline));
            // ********************************************
            //  Get 3 Recent visits
            // ********************************************
            // 
            DataTable dtLeft = new DataTable();
            DataTable dtMid = new DataTable();
            DataTable dtRight = new DataTable();
            strSQL = ("select Docrowid, DateOfService from tblDocMaster where PTrowid  = "
                        + (PatientId + " And NoteType in (\'PPOC\', \'PFCE\') "));
            dtLeft = db.ExecuteDataset(strSQL).Tables[0];
            strSQL = (" select TOP 1 Docrowid, DateOfService from tblDocMaster where PTrowid  = "
                        + (PatientId + ("  And NoteType in (\'PPOCRE\', \'PMN\') " + (" And CAST(DateOfService as date) < (select DateOfService from tblDocMaster where PTrowid  = "
                        + (PatientId + ("  And Docrowid = "
                        + (docrowid + (" ) " + " order by CAST(DateOfService as date) desc "))))))));
            dtMid = db.ExecuteDataset(strSQL).Tables[0];
            if ((dtMid.Rows.Count == 0))
            {
                strSQL = (" select Docrowid, DateOfService from tblDocMaster where PTrowid  = "
                            + (PatientId + ("  And Docrowid =  " + docrowid)));
                dtMid = db.ExecuteDataset(strSQL).Tables[0];
            }
            else
            {
                strSQL = (" select Docrowid, DateOfService from tblDocMaster where PTrowid  = "
                            + (PatientId + ("  And Docrowid = " + docrowid)));
                dtRight = db.ExecuteDataset(strSQL).Tables[0];
            }

            // ********************************************
            //  Start Treatment Notes
            // ********************************************
            DataSet dsTDR = new DataSet();
            // Treatment Notes Table 4
            dsTDR = db.GetDocNoteSummary(docrowid);
            if ((dsTDR.Tables[0].Rows.Count > 0))
            {
                // Start  new Page if very small space left on the page
                if ((NoteType == "PPOCRE"))
                {
                    if ((dsTDR.Tables[0].Rows[0]["notesummary"].ToStringOrEmpty().Length > 1000))
                    {
                        document.NewPage();
                    }

                }

                DataTable dtIntFD = new DataTable();
                dtIntFD = db.GetDocProInterventionsFD(docrowid).Tables[0];
                if ((dtIntFD.Rows.Count > 0))
                {
                    Phrase pa;
                    Phrase pb;
                    pa = new Phrase(("Summary-  Frequency: " + dtIntFD.Rows[0]["Frequency"].ToStringOrEmpty()), boldTableFontSmallU);
                    pb = new Phrase(("  Duration: " + dtIntFD.Rows[0]["Duration"].ToStringOrEmpty()), bodyFontSmall);
                    pa.Add(pb);
                    p = new Paragraph(pa);
                    document.Add(p);
                }

                Paragraph p1 = new Paragraph(new Phrase(dsTDR.Tables[0].Rows[0]["notesummary"].ToStringOrEmpty(), bodyFontSmall));
                document.Add(p1);
            }

            Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
            document.Add(new Chunk(Singleline));
            // ********************************************
            //  End Treatment Notes
            // ********************************************  
            // ********************************************
            //  Start of Functional Analysis section
            // ********************************************
            // 
            DataTable dtFRL = new DataTable();
            DataTable dtFRM = new DataTable();
            DataTable dtFRR = new DataTable();
            PdfPTable tblFA = new PdfPTable(3);
            tblFA.TotalWidth = 580.0F;
            tblFA.LockedWidth = true;
            widths = new float[] {
                1.0F,
                1.0F,
                1.0F};
            tblFA.SetWidths(widths);
            tblFA.HorizontalAlignment = 0;
            // leave a gap before and after the table
            // tblFA.SpacingBefore = 15.0F
            cell = new PdfPCell(new Phrase(("Evaluation Date: " + dtLeft.Rows[0]["DateOfService"].ToStringOrEmpty()), bodyFontSmall));
            cell.Border = 0;
            cell.Padding = 2;
            tblFA.AddCell(cell);
            cell = new PdfPCell(new Phrase(("Progress Note Date: " + dtMid.Rows[0]["DateOfService"].ToStringOrEmpty()), bodyFontSmall));
            cell.Border = 0;
            cell.Padding = 2;
            tblFA.AddCell(cell);
            if ((dtRight.Rows.Count > 0))
            {
                cell = new PdfPCell(new Phrase(("Progress Note Date: " + dtRight.Rows[0]["DateOfService"].ToStringOrEmpty()), bodyFontSmall));
            }
            else
            {
                cell = new PdfPCell(new Phrase("", bodyFontSmall));
            }

            cell.Border = 0;
            cell.Padding = 2;
            tblFA.AddCell(cell);
            cell = new PdfPCell(new Phrase(" ", bodyFontSmall));
            cell.Border = 0;
            cell.Padding = 2;
            tblFA.AddCell(cell);
            cell = new PdfPCell(new Phrase(" ", bodyFontSmall));
            cell.Border = 0;
            cell.Padding = 2;
            tblFA.AddCell(cell);
            cell = new PdfPCell(new Phrase(" ", bodyFontSmall));
            cell.Border = 0;
            cell.Padding = 2;
            tblFA.AddCell(cell);
            int strlenl = 3000;
            int strlenm = 3000;
            int strlenr = 3000;
            string strfcharac = "";
            dtFRL = db.GetDocFuncCharac(Convert.ToInt32(dtLeft.Rows[0]["Docrowid"]), 0, "PPOC").Tables[0];
            if ((dtFRL.Rows.Count == 0))
            {
                dtFRL = db.GetDocFuncCharac(Convert.ToInt32(dtLeft.Rows[0]["Docrowid"]), 0, "PFCE").Tables[0];
            }

            if ((dtFRL.Rows[0]["funccharac"].ToStringOrEmpty().Length > strlenl))
            {
                strlenl = dtFRL.Rows[0]["funccharac"].ToStringOrEmpty().Substring(0, strlenl).LastIndexOf(Environment.NewLine);
                if ((strlenl == -1))
                {
                    cell = new PdfPCell(new Phrase(dtFRL.Rows[0]["funccharac"].ToStringOrEmpty(), bodyFontSmall));
                }
                else
                {
                    cell = new PdfPCell(new Phrase(dtFRL.Rows[0]["funccharac"].ToStringOrEmpty().Substring(0, strlenl), bodyFontSmall));
                }

            }
            else
            {
                cell = new PdfPCell(new Phrase(dtFRL.Rows[0]["funccharac"].ToStringOrEmpty(), bodyFontSmall));
            }

            cell.Border = 0;
            cell.Padding = 2;
            tblFA.AddCell(cell);
            dtFRM = db.GetDocFuncCharac(Convert.ToInt32(dtMid.Rows[0]["Docrowid"]), 0, "PPOC").Tables[0];
            if ((dtFRM.Rows.Count > 0))
            {
                if ((dtFRM.Rows[0]["funccharac"].ToStringOrEmpty() != ""))
                {
                    if ((dtFRM.Rows[0]["funccharac"].ToStringOrEmpty().Length > strlenm))
                    {
                        strlenm = dtFRM.Rows[0]["funccharac"].ToStringOrEmpty().Substring(0, strlenm).LastIndexOf(Environment.NewLine);
                        if ((strlenm == -1))
                        {
                            cell = new PdfPCell(new Phrase(dtFRM.Rows[0]["funccharac"].ToStringOrEmpty(), bodyFontSmall));
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(dtFRM.Rows[0]["funccharac"].ToStringOrEmpty().Substring(0, strlenm), bodyFontSmall));
                        }

                    }
                    else
                    {
                        cell = new PdfPCell(new Phrase(dtFRM.Rows[0]["funccharac"].ToStringOrEmpty(), bodyFontSmall));
                    }

                }
                else
                {
                    cell = new PdfPCell(new Phrase(" ", bodyFontSmall));
                }

            }
            else
            {
                cell = new PdfPCell(new Phrase(" ", bodyFontSmall));
            }

            cell.Border = 0;
            cell.Padding = 2;
            tblFA.AddCell(cell);
            if ((dtRight.Rows.Count > 0))
            {
                dtFRR = db.GetDocFuncCharac(Convert.ToInt32(dtRight.Rows[0]["Docrowid"]), 0, "PPOC").Tables[0];
            }
            else
            {
                dtFRR = db.GetDocFuncCharac(99999999, 0, "PPOC").Tables[0];
            }

            if ((dtFRR.Rows.Count > 0))
            {
                if ((dtFRR.Rows[0]["funccharac"].ToStringOrEmpty() != ""))
                {
                    if ((dtFRR.Rows[0]["funccharac"].ToStringOrEmpty().Length > strlenr))
                    {
                        strlenr = dtFRR.Rows[0]["funccharac"].ToStringOrEmpty().Substring(0, strlenr).LastIndexOf(Environment.NewLine);
                        if ((strlenr == -1))
                        {
                            cell = new PdfPCell(new Phrase(dtFRR.Rows[0]["funccharac"].ToStringOrEmpty(), bodyFontSmall));
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(dtFRR.Rows[0]["funccharac"].ToStringOrEmpty().Substring(0, strlenr), bodyFontSmall));
                        }

                    }
                    else
                    {
                        cell = new PdfPCell(new Phrase(dtFRR.Rows[0]["funccharac"].ToStringOrEmpty(), bodyFontSmall));
                    }

                }
                else
                {
                    cell = new PdfPCell(new Phrase(" ", bodyFontSmall));
                }

            }
            else
            {
                cell = new PdfPCell(new Phrase(" ", bodyFontSmall));
            }

            cell.Border = 0;
            cell.Padding = 2;
            tblFA.AddCell(cell);
            if (((dtFRL.Rows[0]["funccharac"].ToStringOrEmpty().Length > strlenl)
                        && (strlenl != -1)))
            {
                cell = new PdfPCell(new Phrase(dtFRL.Rows[0]["funccharac"].ToStringOrEmpty().Substring(strlenl), bodyFontSmall));
            }
            else
            {
                cell = new PdfPCell(new Phrase(" ", bodyFontSmall));
            }

            cell.Border = 0;
            cell.Padding = 2;
            tblFA.AddCell(cell);
            if (((dtFRL.Rows[0]["funccharac"].ToStringOrEmpty().Length > strlenm)
                        && (strlenm != -1)))
            {
                cell = new PdfPCell(new Phrase(dtFRL.Rows[0]["funccharac"].ToStringOrEmpty().Substring(strlenm), bodyFontSmall));
            }
            else
            {
                cell = new PdfPCell(new Phrase(" ", bodyFontSmall));
            }

            cell.Border = 0;
            cell.Padding = 2;
            tblFA.AddCell(cell);
            if ((dtFRR.Rows.Count > 0))
            {
                if (((dtFRR.Rows[0]["funccharac"].ToStringOrEmpty().Length > strlenr)
                            && (strlenr != -1)))
                {
                    cell = new PdfPCell(new Phrase(dtFRR.Rows[0]["funccharac"].ToStringOrEmpty().Substring(strlenr), bodyFontSmall));
                }
                else
                {
                    cell = new PdfPCell(new Phrase(" ", bodyFontSmall));
                }

            }
            else
            {
                cell = new PdfPCell(new Phrase(" ", bodyFontSmall));
            }

            cell.Border = 0;
            cell.Padding = 2;
            tblFA.AddCell(cell);
            document.Add(tblFA);
            Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
            document.Add(new Chunk(Singleline));
            // ********************************************
            //  End of Functional Analysis section
            // ********************************************
            // ********************************************
            //  Start Pain
            // ********************************************
            DataTable dtDP = new DataTable();
            //  Pain Table
            PdfPTable tblP1 = new PdfPTable(1);
            tblP1.LockedWidth = true;
            widths = new float[] {
                1.0F};
            tblP1.SetWidths(widths);
            tblP1.HorizontalAlignment = 0;
            dtDP = db.GetDocPainR(Convert.ToInt32(dtLeft.Rows[0]["Docrowid"])).Tables[0];
            if ((dtDP.Rows.Count > 0))
            {
                // Start  new Page if very small space left on the page
                ht = writer.GetVerticalPosition(true);
                if ((ht < 250))
                {
                    document.NewPage();
                }

                if ((dtDP.Rows.Count > 0))
                {
                    cell = new PdfPCell(new Phrase(("Pain Scale: At Rest "
                                        + (dtDP.Rows[0]["PainScaleAR"].ToStringOrEmpty() + ("/10   With Activity "
                                        + (dtDP.Rows[0]["PainScaleWA"].ToStringOrEmpty() + "/10")))), bodyFontSmall));
                }
                else
                {
                    cell = new PdfPCell(new Phrase("", bodyFontSmall));
                }

                cell.Border = 0;
                cell.Padding = 2;
                tblP1.AddCell(cell);
                PdfPTable tblP2 = new PdfPTable(1);
                tblP2.LockedWidth = true;
                widths = new float[] {
                    1.0F};
                tblP2.SetWidths(widths);
                tblP2.HorizontalAlignment = 0;
                dtDP = db.GetDocPainR(Convert.ToInt32(dtMid.Rows[0]["Docrowid"])).Tables[0];
                if ((dtDP.Rows.Count > 0))
                {
                    cell = new PdfPCell(new Phrase(("Pain Scale: At Rest "
                                        + (dtDP.Rows[0]["PainScaleAR"].ToStringOrEmpty() + ("/10   With Activity "
                                        + (dtDP.Rows[0]["PainScaleWA"].ToStringOrEmpty() + "/10")))), bodyFontSmall));
                }
                else
                {
                    cell = new PdfPCell(new Phrase("", bodyFontSmall));
                }

                cell.Border = 0;
                cell.Padding = 2;
                tblFA.AddCell(cell);
                PdfPTable tblP3 = new PdfPTable(1);
                tblP3.LockedWidth = true;
                widths = new float[] {
                    1.0F};
                tblP3.SetWidths(widths);
                tblP3.HorizontalAlignment = 0;
                if ((dtRight.Rows.Count > 0))
                {
                    dtDP = db.GetDocPainR(Convert.ToInt32(dtRight.Rows[0]["Docrowid"].ToStringOrEmpty())).Tables[0];
                }
                else
                {
                    dtDP = db.GetDocPainR(99999999).Tables[0];
                }

                if ((dtDP.Rows.Count > 0))
                {
                    cell = new PdfPCell(new Phrase(("Pain Scale: At Rest "
                                        + (dtDP.Rows[0]["PainScaleAR"].ToStringOrEmpty() + ("/10   With Activity "
                                        + (dtDP.Rows[0]["PainScaleWA"].ToStringOrEmpty() + "/10")))), bodyFontSmall));
                }
                else
                {
                    cell = new PdfPCell(new Phrase("", bodyFontSmall));
                }

                cell.Border = 0;
                cell.Padding = 2;
                tblP3.AddCell(cell);
                PdfPTable tblP = new PdfPTable(3);
                tblP.LockedWidth = true;
                widths = new float[] {
                    1.0F,
                    1.0F,
                    1.0F};
                tblP.SetWidths(widths);
                tblP.HorizontalAlignment = 0;
                // leave a gap before and after the table
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(tblP1);
                tblP.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(tblP2);
                tblP.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(tblP3);
                tblP.AddCell(cell);
                document.Add(tblP);
                Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                document.Add(new Chunk(Singleline));
            }

            // ********************************************
            //  End Pain
            // ********************************************
            // ********************************************
            //  Start of Joint Measurements section
            // ********************************************
            // 
            if (((NoteType == "PPOC")
                        || ((NoteType == "PPOC2")
                        || ((NoteType == "PMN")
                        || ((NoteType == "PPOCRE")
                        || (NoteType == "PDIS"))))))
            {
                DataTable dtG;
                DataSet ds1;
                string strMeasureType;
                if (((NoteType == "PPOC")
                            || (NoteType == "PPOC2")))
                {
                    strMeasureType = "IE";
                }
                else
                {
                    strMeasureType = "LM";
                }

                ds1 = db.GetDocBodyPart(docrowid, strMeasureType);
                PdfPTable tblM = new PdfPTable(3);
                tblM.LockedWidth = true;
                widths = new float[] {
                    1.0F,
                    1.0F,
                    1.0F};
                tblM.SetWidths(widths);
                tblM.HorizontalAlignment = 0;
                // leave a gap before and after the table
                DataTable dtMLeft;
                DataTable dtMMid;
                DataTable dtMRt = new DataTable();
                PdfPTable rjmeasure;
                for (int i = 0; i <= (ds1.Tables[0].Rows.Count - 1); i++)
                {
                    dtMLeft = db.GetMeasurementsR(PatientId, Convert.ToInt32(dtLeft.Rows[0]["Docrowid"].ToStringOrEmpty()), ds1.Tables[0].Rows[i][0].ToStringOrEmpty(), "IE").Tables[0];
                    dtMMid = db.GetMeasurementsR(PatientId, Convert.ToInt32(dtMid.Rows[0]["Docrowid"].ToStringOrEmpty()), ds1.Tables[0].Rows[i][0].ToStringOrEmpty(), "LM").Tables[0];
                    if ((dtRight.Rows.Count > 0))
                    {
                        dtMRt = db.GetMeasurementsR(PatientId, Convert.ToInt32(dtRight.Rows[0]["Docrowid"].ToStringOrEmpty()), ds1.Tables[0].Rows[i][0].ToStringOrEmpty(), "LM").Tables[0];
                    }
                    else
                    {
                        dtMRt = db.GetMeasurementsR(PatientId, 99999999, ds1.Tables[0].Rows[i][0].ToStringOrEmpty(), "LM").Tables[0];
                    }

                    if ((dtMLeft.Rows.Count > 0))
                    {
                        AbbrMeasureType(dtMLeft);
                    }

                    if ((dtMLeft.Rows.Count > 0))
                    {
                        AbbrMeasureType(dtMMid);
                    }

                    if ((dtMLeft.Rows.Count > 0))
                    {
                        AbbrMeasureType(dtMRt);
                    }

                    if ((dtMLeft.Rows.Count > 3))
                    {
                        rjmeasure = new PdfPTable(2);
                        rjmeasure.LockedWidth = true;
                        widths = new float[] {
                            1.0F,
                            1.0F};
                        rjmeasure.SetWidths(widths);
                        rjmeasure.HorizontalAlignment = 0;
                        for (int j = 0; j <= (dtMLeft.Rows.Count - 1); j++)
                        {
                            cell = new PdfPCell(new Phrase((dtMLeft.Rows[j]["MeasurementType"].ToStringOrEmpty().PadRight(10, ' ').ToStringOrEmpty()
                                + dtMLeft.Rows[j]["Measurement"].ToStringOrEmpty()), bodyFontC));
                            cell.Border = 0;
                            cell.Colspan = 2;
                            rjmeasure.AddCell(cell);
                        }

                        cell = new PdfPCell();
                        cell.Border = 0;
                        cell.AddElement(rjmeasure);
                        tblM.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Phrase("", bodyFontSmall));
                        cell.Border = 0;
                        cell.Padding = 2;
                        tblM.AddCell(cell);
                    }

                    if ((dtMMid.Rows.Count > 3))
                    {
                        rjmeasure = new PdfPTable(2);
                        rjmeasure.LockedWidth = true;
                        widths = new float[] {
                            1.0F,
                            1.0F};
                        rjmeasure.SetWidths(widths);
                        rjmeasure.HorizontalAlignment = 0;
                        // For j = 0 To dtMLeft.Rows.Count - 1          
                        for (int j = 0; j <= (dtMMid.Rows.Count - 1); j++)
                        {
                            cell = new PdfPCell(new Phrase((dtMMid.Rows[j]["MeasurementType"].ToStringOrEmpty().PadRight(10, ' ').ToStringOrEmpty()
                                + dtMMid.Rows[j]["Measurement"].ToStringOrEmpty()), bodyFontC));
                            cell.Border = 0;
                            cell.Colspan = 2;
                            rjmeasure.AddCell(cell);
                        }

                        cell = new PdfPCell();
                        cell.Border = 0;
                        cell.AddElement(rjmeasure);
                        tblM.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Phrase("", bodyFontSmall));
                        cell.Border = 0;
                        cell.Padding = 2;
                        tblM.AddCell(cell);
                    }

                    if ((dtMRt.Rows.Count > 3))
                    {
                        rjmeasure = new PdfPTable(2);
                        rjmeasure.LockedWidth = true;
                        widths = new float[] {
                            1.0F,
                            1.0F};
                        rjmeasure.SetWidths(widths);
                        rjmeasure.HorizontalAlignment = 0;
                        for (int j = 0; j <= (dtMRt.Rows.Count - 1); j++)
                        {
                            cell = new PdfPCell(new Phrase((dtMRt.Rows[j]["MeasurementType"].ToStringOrEmpty().PadRight(10, ' ').ToStringOrEmpty() +
                                dtMRt.Rows[j]["Measurement"].ToStringOrEmpty()), bodyFontC));
                            cell.Border = 0;
                            cell.Colspan = 2;
                            rjmeasure.AddCell(cell);
                        }

                        cell = new PdfPCell();
                        cell.Border = 0;
                        cell.AddElement(rjmeasure);
                        tblM.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Phrase("", bodyFontSmall));
                        cell.Border = 0;
                        cell.Padding = 2;
                        tblM.AddCell(cell);
                    }

                }

                tblM.KeepTogether = false;
                document.Add(tblM);
                Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                document.Add(new Chunk(Singleline));
            }

            // ********************************************
            //  End of Joint Measurements section
            // ********************************************
            // ********************************************
            //  Get Extremity, Spinal, OMPT data
            // ********************************************
            DataSet dsELeft;
            DataSet dsEMid;
            DataSet dsERt = new DataSet();
            DataTable dtELeft;
            DataTable dtEAMid;
            DataTable dtEMid;
            DataTable dtERt = new DataTable();
            DataTable dtEALeft;
            DataSet dSEMid = new DataSet();
            DataTable dtEARt = new DataTable();
            dsELeft = db.GetDocExtremityTests("D", Convert.ToInt32(dtLeft.Rows[0]["Docrowid"].ToStringOrEmpty()), 0);
            dtELeft = dsELeft.Tables[0];
            dtEALeft = dsELeft.Tables[1];
            if ((dtMid.Rows.Count > 0))
            {
                dSEMid = db.GetDocExtremityTests("D", Convert.ToInt32(dtMid.Rows[0]["Docrowid"].ToStringOrEmpty()), 0);
            }
            else
            {
                dSEMid = db.GetDocExtremityTests("D", 99999999, 0);
            }

            dtEMid = dSEMid.Tables[0];
            dtEAMid = dSEMid.Tables[1];
            if ((dtRight.Rows.Count > 0))
            {
                dsERt = db.GetDocExtremityTests("D", Convert.ToInt32(dtRight.Rows[0]["Docrowid"].ToStringOrEmpty()), 0);
            }
            else
            {
                dsERt = db.GetDocExtremityTests("D", 99999999, 0);
            }

            dtERt = dsERt.Tables[0];
            dtEARt = dsERt.Tables[1];
            DataSet dsSLeft;
            DataSet dsSMid;
            DataSet dsSRt = new DataSet();
            DataTable dtSLeft;
            DataTable dtSMid;
            DataTable dtSRt = new DataTable();
            DataTable dtSALeft;
            DataTable dtSAMid;
            DataTable dtSARt = new DataTable();
            dsSLeft = db.GetDocSpinalTests("D", Convert.ToInt32(dtLeft.Rows[0]["Docrowid"].ToStringOrEmpty()), 0);
            dtSLeft = dsSLeft.Tables[0];
            dtSALeft = dsSLeft.Tables[1];
            if ((dtMid.Rows.Count > 0))
            {
                dsSMid = db.GetDocSpinalTests("D", Convert.ToInt32(dtMid.Rows[0]["Docrowid"].ToStringOrEmpty()), 0);
            }
            else
            {
                dsSMid = db.GetDocSpinalTests("D", 99999999, 0);
            }

            dtSMid = dsSMid.Tables[0];
            dtSAMid = dsSMid.Tables[1];
            if ((dtRight.Rows.Count > 0))
            {
                dsSRt = db.GetDocSpinalTests("D", Convert.ToInt32(dtRight.Rows[0]["Docrowid"].ToStringOrEmpty()), 0);
            }
            else
            {
                dsSRt = db.GetDocSpinalTests("D", 99999999, 0);
            }

            dtSRt = dsSRt.Tables[0];
            dtSARt = dsSRt.Tables[1];
            DataSet dsOLeft;
            DataSet dsOMid;
            DataSet dsORt = new DataSet();
            DataTable dtOLeft;
            DataTable dtOMid;
            DataTable dtORt = new DataTable();
            DataTable dtOALeft;
            DataTable dtOAMid;
            DataTable dtOARt = new DataTable();
            dsOLeft = db.GetDocOMPT("D", Convert.ToInt32(dtLeft.Rows[0]["Docrowid"].ToStringOrEmpty()), 0);
            dtOLeft = dsOLeft.Tables[0];
            dtOALeft = dsOLeft.Tables[1];
            if ((dtMid.Rows.Count > 0))
            {
                dsOMid = db.GetDocOMPT("D", Convert.ToInt32(dtMid.Rows[0]["Docrowid"].ToStringOrEmpty()), 0);
            }
            else
            {
                dsOMid = db.GetDocOMPT("D", 99999999, 0);
            }

            dtOMid = dsOMid.Tables[0];
            dtOAMid = dsOMid.Tables[1];
            if ((dtRight.Rows.Count > 0))
            {
                dsORt = db.GetDocOMPT("D", Convert.ToInt32(dtRight.Rows[0]["Docrowid"].ToStringOrEmpty()), 0);
            }
            else
            {
                dsORt = db.GetDocOMPT("D", 99999999, 0);
            }

            dtORt = dsORt.Tables[0];
            dtOARt = dsORt.Tables[1];
            string[] Pos = new string[9];
            //object Pos;
            Pos[0] = "";
            Pos[1] = "";
            Pos[2] = "";
            Pos[3] = "";
            Pos[4] = "";
            Pos[5] = "";
            Pos[6] = "";
            Pos[7] = "";
            Pos[8] = "";
            if (((dtEALeft.Rows.Count > 0)
                        || (dtELeft.Rows.Count > 0)))
            {
                Pos[0] = "EL";
            }

            if (((dtSALeft.Rows.Count > 0)
                        || (dtSLeft.Rows.Count > 0)))
            {
                if ((Pos[0] == ""))
                {
                    Pos[0] = "SL";
                }
                else
                {
                    Pos[3] = "SL";
                }

            }

            if (((dtOALeft.Rows.Count > 0)
                        || (dtOLeft.Rows.Count > 0)))
            {
                if ((Pos[0] == ""))
                {
                    Pos[0] = "OL";
                }
                else if ((Pos[3] == ""))
                {
                    Pos[3] = "OL";
                }
                else
                {
                    Pos[6] = "OL";
                }

            }

            if (((dtEAMid.Rows.Count > 0)
                        || (dtEMid.Rows.Count > 0)))
            {
                Pos[1] = "EM";
            }

            if (((dtSAMid.Rows.Count > 0)
                        || (dtSMid.Rows.Count > 0)))
            {
                if ((Pos[1] == ""))
                {
                    Pos[1] = "SM";
                }
                else
                {
                    Pos[4] = "SM";
                }

            }

            if (((dtOAMid.Rows.Count > 0)
                        || (dtOMid.Rows.Count > 0)))
            {
                if ((Pos[1] == ""))
                {
                    Pos[1] = "OM";
                }
                else if ((Pos[4] == ""))
                {
                    Pos[4] = "OM";
                }
                else
                {
                    Pos[7] = "OM";
                }

            }

            if (((dtEARt.Rows.Count > 0)
                        || (dtERt.Rows.Count > 0)))
            {
                Pos[2] = "ER";
            }

            if (((dtSARt.Rows.Count > 0)
                        || (dtSRt.Rows.Count > 0)))
            {
                if ((Pos[2] == ""))
                {
                    Pos[2] = "SR";
                }
                else
                {
                    Pos[5] = "SR";
                }

            }

            if (((dtOARt.Rows.Count > 0)
                        || (dtORt.Rows.Count > 0)))
            {
                if ((Pos[2] == ""))
                {
                    Pos[2] = "OR";
                }
                else if ((Pos[5] == ""))
                {
                    Pos[5] = "OR";
                }
                else
                {
                    Pos[8] = "OR";
                }

            }

            // ********************************************
            //  Start of Extremity section
            // ********************************************
            PdfPTable tblE = new PdfPTable(3);
            tblE.TotalWidth = 580.0F;
            tblE.LockedWidth = true;
            widths = new float[] {
                1.0F,
                1.0F,
                1.0F};
            tblE.SetWidths(widths);
            tblE.HorizontalAlignment = 0;
            // leave a gap before and after the table
            PdfPTable rexttest = new PdfPTable(2);
            PdfPTable rtest = new PdfPTable(1);
            for (int z = 0; (z <= 8); z++)
            {
                rexttest = new PdfPTable(2);
                rexttest.TotalWidth = 190.0F;
                rexttest.LockedWidth = true;
                widths = new float[] {
                    3.0F,
                    1.0F};
                rexttest.SetWidths(widths);
                rexttest.HorizontalAlignment = 0;

                rtest = new PdfPTable(1);
                rtest.TotalWidth = 185.0F;
                rtest.LockedWidth = true;
                widths = new float[] {
                    1.0F};
                rtest.SetWidths(widths);
                rtest.HorizontalAlignment = 0;
                if ((Pos[z] == ""))
                {
                    cell = new PdfPCell(new Phrase("", bodyFontSmall));
                    cell.Border = 0;
                    cell.Padding = 2;
                    tblE.AddCell(cell);
                }
                else
                {
                    switch (Pos[z])
                    {
                        case "EL":
                            cell = new PdfPCell(new Phrase("Extremity test", bodyFontSmallU));
                            break;
                        case "SL":
                            cell = new PdfPCell(new Phrase("Spinal test", bodyFontSmallU));
                            break;
                        case "OL":
                            cell = new PdfPCell(new Phrase("OMPT test", bodyFontSmallU));
                            break;
                        case "EM":
                            cell = new PdfPCell(new Phrase("Extremity test", bodyFontSmallU));
                            break;
                        case "SM":
                            cell = new PdfPCell(new Phrase("Spinal test", bodyFontSmallU));
                            break;
                        case "OM":
                            cell = new PdfPCell(new Phrase("OMPT test", bodyFontSmallU));
                            break;
                        case "ER":
                            cell = new PdfPCell(new Phrase("Extremity test", bodyFontSmallU));
                            break;
                        case "SR":
                            cell = new PdfPCell(new Phrase("Spinal test", bodyFontSmallU));
                            break;
                        case "OR":
                            cell = new PdfPCell(new Phrase("OMPT test", bodyFontSmallU));
                            break;
                    }
                    cell.Border = 0;
                    rexttest.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Result", bodyFontSmallU));
                    cell.Border = 0;
                    rexttest.AddCell(cell);
                    switch (Pos[z])
                    {
                        case "EL":
                            for (int i = 0; i <= (dtELeft.Rows.Count - 1); i++)
                            {
                                cell = new PdfPCell(new Phrase(dtELeft.Rows[i]["Description"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                                cell = new PdfPCell(new Phrase(dtELeft.Rows[i]["TestPN"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                            }

                            if ((dtEALeft.Rows.Count > 0))
                            {
                                cell = new PdfPCell(new Phrase(dtEALeft.Rows[0]["assessment"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rtest.AddCell(cell);
                            }

                            break;
                        case "SL":
                            for (int i = 0; (i
                                        <= (dtSLeft.Rows.Count - 1)); i++)
                            {
                                cell = new PdfPCell(new Phrase(dtSLeft.Rows[i]["Description"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                                cell = new PdfPCell(new Phrase(dtSLeft.Rows[i]["TestPN"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                            }

                            if ((dtSALeft.Rows.Count > 0))
                            {
                                cell = new PdfPCell(new Phrase(dtSALeft.Rows[0]["assessment"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rtest.AddCell(cell);
                            }

                            break;
                        case "OL":
                            for (int i = 0; (i
                                        <= (dtOLeft.Rows.Count - 1)); i++)
                            {
                                cell = new PdfPCell(new Phrase(dtOLeft.Rows[i]["Description"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                                cell = new PdfPCell(new Phrase((dtOLeft.Rows[i]["TestPAN"].ToStringOrEmpty() + (", "
                                                    + (dtOLeft.Rows[i]["TestID"].ToStringOrEmpty() + (", "
                                                    + (dtOLeft.Rows[i]["TestWB"].ToStringOrEmpty() + (", " + dtOLeft.Rows[i]["TestCP"].ToStringOrEmpty())))))),
                                                    bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                            }

                            if ((dtOALeft.Rows.Count > 0))
                            {
                                cell = new PdfPCell(new Phrase(dtOALeft.Rows[0]["assessment"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rtest.AddCell(cell);
                            }

                            break;
                        case "EM":
                            for (int i = 0; (i
                                        <= (dtEMid.Rows.Count - 1)); i++)
                            {
                                cell = new PdfPCell(new Phrase(dtEMid.Rows[i]["Description"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                                cell = new PdfPCell(new Phrase(dtEMid.Rows[i]["TestPN"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                            }

                            if ((dtEAMid.Rows.Count > 0))
                            {
                                cell = new PdfPCell(new Phrase(dtEAMid.Rows[0]["assessment"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rtest.AddCell(cell);
                            }

                            break;
                        case "SM":
                            for (int i = 0; (i
                                        <= (dtSMid.Rows.Count - 1)); i++)
                            {
                                cell = new PdfPCell(new Phrase(dtSMid.Rows[i]["Description"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                                cell = new PdfPCell(new Phrase(dtSMid.Rows[i]["TestPN"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                            }

                            if ((dtSAMid.Rows.Count > 0))
                            {
                                cell = new PdfPCell(new Phrase(dtSAMid.Rows[0]["assessment"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rtest.AddCell(cell);
                            }

                            break;
                        case "OM":
                            for (int i = 0; (i
                                        <= (dtOMid.Rows.Count - 1)); i++)
                            {
                                cell = new PdfPCell(new Phrase(dtOMid.Rows[i]["Description"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                                cell = new PdfPCell(new Phrase((dtOMid.Rows[i]["TestPAN"].ToStringOrEmpty() + (", "
                                                    + (dtOMid.Rows[i]["TestID"].ToStringOrEmpty() + (", "
                                                    + (dtOMid.Rows[i]["TestWB"].ToStringOrEmpty() + (", " + dtOMid.Rows[i]["TestCP"].ToStringOrEmpty())))))), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                            }

                            if ((dtOAMid.Rows.Count > 0))
                            {
                                cell = new PdfPCell(new Phrase(dtOAMid.Rows[0]["assessment"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rtest.AddCell(cell);
                            }

                            break;
                        case "ER":
                            for (int i = 0; (i
                                        <= (dtERt.Rows.Count - 1)); i++)
                            {
                                cell = new PdfPCell(new Phrase(dtERt.Rows[i]["Description"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                                cell = new PdfPCell(new Phrase(dtERt.Rows[i]["TestPN"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                            }

                            if ((dtEARt.Rows.Count > 0))
                            {
                                cell = new PdfPCell(new Phrase(dtEARt.Rows[0]["assessment"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rtest.AddCell(cell);
                            }

                            break;
                        case "SR":
                            for (int i = 0; (i
                                        <= (dtSRt.Rows.Count - 1)); i++)
                            {
                                cell = new PdfPCell(new Phrase(dtSRt.Rows[i]["Description"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                                cell = new PdfPCell(new Phrase(dtSRt.Rows[i]["TestPN"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                            }

                            if ((dtSARt.Rows.Count > 0))
                            {
                                cell = new PdfPCell(new Phrase(dtSARt.Rows[0]["assessment"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rtest.AddCell(cell);
                            }

                            break;
                        case "OR":
                            for (int i = 0; (i
                                        <= (dtORt.Rows.Count - 1)); i++)
                            {
                                cell = new PdfPCell(new Phrase(dtORt.Rows[i]["Description"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                                cell = new PdfPCell(new Phrase((dtORt.Rows[i]["TestPAN"].ToStringOrEmpty() + (", "
                                                    + (dtORt.Rows[i]["TestID"].ToStringOrEmpty() + (", "
                                                    + (dtORt.Rows[i]["TestWB"].ToStringOrEmpty() + (", " + dtORt.Rows[i]["TestCP"].ToStringOrEmpty())))))), bodyFontSmall));
                                cell.Border = 0;
                                rexttest.AddCell(cell);
                            }

                            if ((dtOARt.Rows.Count > 0))
                            {
                                cell = new PdfPCell(new Phrase(dtOARt.Rows[0]["assessment"].ToStringOrEmpty(), bodyFontSmall));
                                cell.Border = 0;
                                rtest.AddCell(cell);
                            }

                            break;
                    }
                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.AddElement(rexttest);
                    cell.AddElement(rtest);
                    tblE.AddCell(cell);
                }

            }

            document.Add(tblE);
            // Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.BLACK , Element.ALIGN_LEFT, 1)
            // document.Add(New Chunk(Singleline))
            // ********************************************
            //  Start of Goals Re-Eval
            // ********************************************
            DataTable dtSGoals = new DataTable();
            DataTable dtFGoals = new DataTable();
            DataTable dtLGoals = new DataTable();
            dtSGoals = db.GetPatGoals(PatientId, "S").Tables[0];
            dtLGoals = db.GetPatGoals(PatientId, "L").Tables[0];
            dtFGoals = db.GetPatGoals(PatientId, "F").Tables[0];
            dtLGoals.Merge(dtFGoals);
            ht = writer.GetVerticalPosition(true);
            if ((ht < 250))
            {
                document.NewPage();
            }

            PdfPTable rgoall = new PdfPTable(4);
            rgoall.TotalWidth = 580.0F;
            rgoall.LockedWidth = true;
            float[] widthsl = new float[] {
                5.0F,
                1.0F,
                1.5F,
                1.0F};
            rgoall.SetWidths(widthsl);
            rgoall.HorizontalAlignment = 0;
            p = new Paragraph(new Phrase("Long Term Goals", boldTableFontSmallU));
            document.Add(p);
            cell = new PdfPCell(new Phrase("", boldTableFontSmallU));
            cell.Border = 0;
            rgoall.AddCell(cell);
            cell = new PdfPCell(new Phrase("Met", boldTableFontSmallU));
            cell.Border = 0;
            rgoall.AddCell(cell);
            cell = new PdfPCell(new Phrase("Partially Met", boldTableFontSmallU));
            cell.Border = 0;
            rgoall.AddCell(cell);
            cell = new PdfPCell(new Phrase("Not Met", boldTableFontSmallU));
            cell.Border = 0;
            rgoall.AddCell(cell);
            for (int i = 0; (i
                        <= (dtLGoals.Rows.Count - 1)); i++)
            {
                cell = new PdfPCell(new Phrase(dtLGoals.Rows[i]["Description"].ToStringOrEmpty(), bodyFontSmall));
                cell.Border = 0;
                cell.Padding = 4;
                rgoall.AddCell(cell);
                if ((dtLGoals.Rows[i]["GoalMet"].ToStringOrEmpty() == "1"))
                {
                    var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/tick.png"));
                    cell = new PdfPCell(logo);
                }
                else
                {
                    var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/untick.png"));
                    cell = new PdfPCell(logo);
                }

                cell.Border = 0;
                cell.Padding = 4;
                cell.Left = 4;
                rgoall.AddCell(cell);
                if ((dtLGoals.Rows[i]["GoalPMet"].ToStringOrEmpty() == "1"))
                {
                    var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/tick.png"));
                    cell = new PdfPCell(logo);
                }
                else
                {
                    var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/untick.png"));
                    cell = new PdfPCell(logo);
                }

                cell.Border = 0;
                cell.Padding = 4;
                cell.Left = 10;
                rgoall.AddCell(cell);
                if ((dtLGoals.Rows[i]["GoalNMet"].ToStringOrEmpty() == "1"))
                {
                    var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/tick.png"));
                    cell = new PdfPCell(logo);
                }
                else
                {
                    var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/untick.png"));
                    cell = new PdfPCell(logo);
                }

                cell.Border = 0;
                cell.Padding = 4;
                cell.Left = 8;
                rgoall.AddCell(cell);
            }

            document.Add(rgoall);
            Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
            document.Add(new Chunk(Singleline));
            // ********************************************
            //  End of IE Goals
            // ********************************************
            DataTable dtProInt = new DataTable();
            dtProInt = db.GetDocProInterventions(docrowid).Tables[0];
            if ((dtProInt.Rows.Count > 0))
            {
                PdfPTable rinterFD = new PdfPTable(4);
                rinterFD.LockedWidth = true;
                widths = new float[] {
                    4.0F,
                    1.0F,
                    4.0F,
                    2.0F};
                rinterFD.SetWidths(widths);
                rinterFD.HorizontalAlignment = 0;
                PdfPTable rinter = new PdfPTable(4);
                rinter.TotalWidth = 600.0F;
                rinter.LockedWidth = true;
                widths = new float[] {
                    3.0F,
                    1.0F,
                    3.0F,
                    1.0F};
                rinter.SetWidths(widths);
                rinter.HorizontalAlignment = 0;
                cell = new PdfPCell(new Phrase("Intervention", boldTableFontSmallU));
                cell.Border = 0;
                cell.Colspan = 2;
                rinter.AddCell(cell);
                cell = new PdfPCell(new Phrase("CPT Code", boldTableFontSmallU));
                cell.Border = 0;
                cell.Colspan = 2;
                rinter.AddCell(cell);
                for (int i = 0; (i
                            <= (dtProInt.Rows.Count - 1)); i++)
                {
                    cell = new PdfPCell(new Phrase(dtProInt.Rows[i]["CPTDescription"].ToStringOrEmpty(), bodyFontSmall));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    rinter.AddCell(cell);
                    cell = new PdfPCell(new Phrase(dtProInt.Rows[i]["CPTCode"].ToStringOrEmpty(), bodyFontSmall));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    rinter.AddCell(cell);
                }

                document.Add(rinter);
            }

            Singleline = new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
            document.Add(new Chunk(Singleline));
            // ********************************************
            //  Start of Sign box
            // ********************************************
            // ***** signbox2 for Doctor
            // Start  new Page if very small space left on the page
            ht = writer.GetVerticalPosition(true);
            if ((ht < 250))
            {
                document.NewPage();
            }

            PdfPTable signbox2 = new PdfPTable(2);
            // signbox.SplitLate = false
            signbox2.TotalWidth = 250.0F;
            signbox2.LockedWidth = true;
            widths = new float[] {
                2.2F,
                1.0F};
            signbox2.SetWidths(widths);
            signbox2.HorizontalAlignment = 0;
            cell = new PdfPCell(new Phrase(" ", bodyFont));
            cell.Colspan = 2;
            cell.Border = 0;
            // cell.Padding = 8
            cell.Border = (Rectangle.TOP_BORDER
                        | (Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER));
            signbox2.AddCell(cell);
            cell = new PdfPCell(new Phrase(" ", bodyFont));
            cell.Border = 0;
            cell.Rowspan = 2;
            cell.Border = (Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER);
            // Cell.Padding = 8
            signbox2.AddCell(cell);
            cell = new PdfPCell(new Phrase(" ", bodyFont));
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER;
            // cell.Padding = 8
            cell.HorizontalAlignment = 2;
            signbox2.AddCell(cell);
            cell = new PdfPCell(new Phrase(" ", bodyFont));
            cell.Border = 0;
            cell.Border = (Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER);
            // cell.Padding = 8
            cell.HorizontalAlignment = 2;
            signbox2.AddCell(cell);
            cell = new PdfPCell(new Phrase(HttpContext.Current.Session["PrintName"].ToStringOrEmpty(), bodyFont));
            cell.Border = (Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER);
            // cell.Padding = 8
            cell.Colspan = 2;
            signbox2.AddCell(cell);
            cell = new PdfPCell(new Phrase(("NPI " + HttpContext.Current.Session["NPI"].ToStringOrEmpty()), bodyFont));
            cell.Border = (Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER);
            // cell.Padding = 8
            cell.Colspan = 2;
            signbox2.AddCell(cell);
            cell = new PdfPCell(new Phrase("I certify the need for these services furnished under  this plan of treatment  while under my care.", bodyFontSmall));
            cell.Border = (Rectangle.RIGHT_BORDER
                        | (Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER));
            cell.Colspan = 2;
            // cell.Padding = 8
            signbox2.AddCell(cell);
            // ***** signbox for Therapist
            if ((Type == "Preview"))
            {

            }
            else
            {
                DataTable dtSig = new DataTable();
                if ((HttpContext.Current.Session["UserRole"].ToStringOrEmpty() == "Therapist"))
                {
                    dtSig = db.GetDocTherapist(Convert.ToInt32(HttpContext.Current.Session["User"]));
                }
                else
                {
                    // dt = DocManager.GetDocTherapistRpt(TryCast(Me.Master.FindControl("ctl00$ctl00$hdndocrowid"), HiddenField).Value).Tables(0)
                    dtSig = db.GetDocTherapistRpt(docrowid);
                }

                PdfPTable signbox = new PdfPTable(2);
                // signbox.SplitLate = false
                signbox.TotalWidth = 250.0F;
                signbox.LockedWidth = true;
                widths = new float[] {
                    2.2F,
                    1.0F};
                signbox.SetWidths(widths);
                signbox.HorizontalAlignment = 0;
                cell = new PdfPCell(new Phrase("Electronically Signed By", bodyFont));
                cell.Colspan = 2;
                cell.Border = 0;
                // cell.Padding = 8
                cell.Border = (Rectangle.TOP_BORDER
                            | (Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER));
                signbox.AddCell(cell);
                // Dim dtSig As New DataTable
                // If Trim(Session["UserRole")) = "Therapist" Then
                //     dtSig  = DocManager.GetDocTherapist(Session["user")).Tables(0)
                // Else
                //     dtSig  = DocManager.GetDocTherapistRpt(TryCast(Me.Master.FindControl("ctl00$ctl00$hdndocrowid"), HiddenField).Value).Tables(0)
                // End If
                Byte[] bytes;
                bytes = (Byte[])dtSig.Rows[0]["signaturefile"];
                // Byte();
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(bytes);
                // Do something special with the empty ones
                if ((bytes.Length == 0))
                {

                }
                else
                {
                    image.ScaleToFit(100F, 200F);
                    Chunk imageChunk = new Chunk(image, 0, 0);
                }

                PdfPCell imageCell = new PdfPCell(image);
                imageCell.Border = 0;
                imageCell.Rowspan = 2;
                imageCell.Border = (Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER);
                imageCell.Padding = 2;
                signbox.AddCell(imageCell);
                // cell = New PdfPCell(New Phrase("8/3/2017", bodyFont))
                cell = new PdfPCell(new Phrase(DateTime.Today.ToString("MM/dd/yyyy"), bodyFont));
                cell.Border = 0;
                cell.Border = Rectangle.RIGHT_BORDER;
                // cell.Padding = 8
                cell.HorizontalAlignment = 2;
                signbox.AddCell(cell);
                // cell = New PdfPCell(New Phrase("5:02:18 PM", bodyFont))
                cell = new PdfPCell(new Phrase(DateTime.Now.ToString("hh:mm:ss tt"), bodyFont));
                cell.Border = 0;
                cell.Border = (Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER);
                // cell.Padding = 8
                cell.HorizontalAlignment = 2;
                signbox.AddCell(cell);
                // cell = New PdfPCell(New Phrase("KARVE ABHIJEET, PT", bodyFont))
                cell = new PdfPCell(new Phrase(dtSig.Rows[0]["Name"].ToString(), bodyFont));
                cell.Border = (Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER);
                // cell.Padding = 8
                cell.Colspan = 2;
                signbox.AddCell(cell);
                cell = new PdfPCell(new Phrase(("NPI :  " + dtSig.Rows[0]["NPI"]), bodyFont));
                cell.Border = (Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER);
                cell.Colspan = 2;
                // cell.Padding = 8
                signbox.AddCell(cell);
                // cell = New PdfPCell(New Phrase("State License #  5501010323", bodyFont))
                cell = new PdfPCell(new Phrase(("State License #  " + dtSig.Rows[0]["LicenseNumber"]), bodyFont));
                cell.Border = (Rectangle.RIGHT_BORDER
                            | (Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER));
                cell.Colspan = 2;
                // cell.Padding = 8
                signbox.AddCell(cell);
                PdfPTable MainSignbox = new PdfPTable(2);
                MainSignbox.TotalWidth = 540F;
                MainSignbox.LockedWidth = true;
                widths = new float[] {
                    1.0F,
                    1.0F};
                MainSignbox.SetWidths(widths);
                MainSignbox.HorizontalAlignment = 0;
                if (((NoteType == "PTREAT")
                            || ((NoteType == "PMV")
                            || ((NoteType == "PMN")
                            || ((NoteType == "PCOMM")
                            || (NoteType == "PDIS"))))))
                {
                    cell = new PdfPCell(new Phrase("", bodyFont));
                    cell.Border = 0;
                    MainSignbox.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.AddElement(signbox2);
                    MainSignbox.AddCell(cell);
                }

                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(signbox);
                MainSignbox.AddCell(cell);
                document.Add(MainSignbox);
                // Dim ps As New Paragraph()
                // ps.IndentationLeft = 250
                // ps.Add(signbox)
                // document.Add(ps)
                // Dim ps1 As New Paragraph()
                // ps1.IndentationLeft = 5
                // ps1.Add(signbox)
                // document.Add(ps1)
                // ********************************************
                //  End of Sign box
                // ********************************************
            }

            document.Close();
            // byte[] bytes1 = File.ReadAllBytes(rptSavePath1);
            byte[] bytes1 = ms.ToArray();
            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(bytes1);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    // stamper.SetEncryption(PdfWriter.ENCRYPTION_AES_128, "", "", PdfWriter.ALLOW_ASSEMBLY)
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i),
                            Element.ALIGN_CENTER, new Phrase(i + " of " + pages, bodyFont), 300, 790, 0);
                }
                bytes1 = stream.ToArray();
            }
            // File.WriteAllBytes("D:\PDFs\Test_1.pdf", bytes1)

            string containerName = rptSavePath + "\\" + path;
            CloudeStoreg.createFileFromBytes(containerName, rptName, bytes1);
            // File.WriteAllBytes(rptSavePath1, bytes1);
            rptSavePath1 = containerName + "\\" + rptName + ".pdf";
            // string strSQL;

            if (Type == "Preview")
            {
            }
            //else if (NoteCnt == 0)
            //{
            //    if (HttpContext.Current.Session["UserRole"].ToStringOrEmpty() == "Therapist")
            //        db.IUDDocMaster("U", docrowid,
            //            PatientId, "", 0, "", "", "1", "",
            //            rptSavePath1, "0", HttpContext.Current.Session["User"].ToStringOrEmpty());
            //    else
            //        db.DocSigned(docrowid, rptSavePath1);
            //}
            else if (NoteCnt == 1)
            {
                if (HttpContext.Current.Session["UserRole"].ToStringOrEmpty() == "Therapist")
                    db.IUDDocMaster("U", docrowid,
                        PatientId, "", 0, "", "", "1", "",
                        rptSavePath1, "0", HttpContext.Current.Session["User"].ToStringOrEmpty());
                else
                    db.DocSigned(docrowid, rptSavePath1);
            }
            else if (NoteCnt == 2
            )
            {
                strSQL = "Update tblDocMaster set PDFName2 = '" + rptSavePath1 + "' where Docrowid = " +
                    docrowid;
                db.ExecuteScalar(strSQL);
            }
            else if (NoteCnt == 3
            )
            {
                strSQL = "Update tblDocMaster set PDFName3 = '" + rptSavePath1 + "' where Docrowid = " +
                    docrowid;
                db.ExecuteScalar(strSQL);
            }

            return rptSavePath1;

        }

        public void AbbrMeasureType(DataTable dt)
        {
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Abduction"))
                {
                    dt.Rows[i]["MeasurementType"] = "Abd";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Adduction"))
                {
                    dt.Rows[i]["MeasurementType"] = "Add";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Dorsiflexion"))
                {
                    dt.Rows[i]["MeasurementType"] = "DF";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Eversion"))
                {
                    dt.Rows[i]["MeasurementType"] = "Evr";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Ext. Rotate"))
                {
                    dt.Rows[i]["MeasurementType"] = "Ext. Rot";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Extension"))
                {
                    dt.Rows[i]["MeasurementType"] = "Ext";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Flexion"))
                {
                    dt.Rows[i]["MeasurementType"] = "Flex";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Horiz. Abd."))
                {
                    dt.Rows[i]["MeasurementType"] = "H Abd";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Horiz. Add."))
                {
                    dt.Rows[i]["MeasurementType"] = "H Add";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Int. Rotate"))
                {
                    dt.Rows[i]["MeasurementType"] = "Int Rot";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Inversion"))
                {
                    dt.Rows[i]["MeasurementType"] = "Inv";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Lat Flexion"))
                {
                    dt.Rows[i]["MeasurementType"] = "Lat Flex";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Plantar Flexion"))
                {
                    dt.Rows[i]["MeasurementType"] = "Pl Flex";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Pronation"))
                {
                    dt.Rows[i]["MeasurementType"] = "Pronation";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Radial D"))
                {
                    dt.Rows[i]["MeasurementType"] = "Radial D";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Rotation"))
                {
                    dt.Rows[i]["MeasurementType"] = "Rot";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Supination"))
                {
                    dt.Rows[i]["MeasurementType"] = "Sup";
                }
                else if ((dt.Rows[i]["MeasurementType"].ToStringOrEmpty() == "Ulnar Dev."))
                {
                    dt.Rows[i]["MeasurementType"] = "Ulnar Dev";
                }

            }

            dt.AcceptChanges();
        }

        public string ReferrerSign(string path, int NPINumber)
        {
            byte[] bytes1;
            var titleFont = FontFactory.GetFont("Roboto", 18, Font.BOLD);
            var subTitleFont = FontFactory.GetFont("Roboto", 14, Font.BOLD);
            var boldTableFont = FontFactory.GetFont("Roboto", 11, Font.BOLD);
            var boldTableFontU = FontFactory.GetFont("Roboto", 12, (Font.BOLD | Font.UNDERLINE));
            var boldTableFontSmallU = FontFactory.GetFont("Roboto", 10, (Font.BOLD | Font.UNDERLINE));
            // boldTableFontU.SetStyle(Font.UNDERLINE)
            var endingMessageFont = FontFactory.GetFont("Arial", 10, Font.ITALIC);
            var bodyFont = FontFactory.GetFont("Roboto", 10, Font.NORMAL);
            var bodyFontSmall = FontFactory.GetFont("Roboto", 8, Font.NORMAL);
            var bodyFontSmallU = FontFactory.GetFont("Roboto", 8, (Font.NORMAL | Font.UNDERLINE));
            var bodyFontSmallBold = FontFactory.GetFont("Roboto", 8, Font.BOLD);
            var bodyFontC = FontFactory.GetFont("Courier", 7, Font.BOLD);
            PdfPCell cell;
            // PdfContentByte canvas = writer.DirectContent;
            float[] widths;
            iTextSharp.text.pdf.draw.LineSeparator Singleline;
            String pathout = @"C:\EasyDoc\test1.pdf";
            float ht;

            using (var inputPDF = CloudeStoreg.MemoryStreamFromFile(path))
            {
                using (var outputPDF = new MemoryStream())
                {
                    //loading existing
                    var reader = new PdfReader(inputPDF.ToArray());
                    Document doc = new Document();
                    PdfWriter writer = PdfWriter.GetInstance(doc, outputPDF);
                    doc.Open();
                    PdfContentByte canvas = writer.DirectContent;
                    PdfImportedPage page;
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {

                        page = writer.GetImportedPage(reader, i);
                        canvas.AddTemplate(page, 1f, 0, 0, 1, 0, 0);

                        //  if (page.PdfWriter.CurrentDocumentSize<250)
                        doc.NewPage();
                    }
                    #region MyClass definition 
                    PdfPTable refSignbox = new PdfPTable(2);
                    // signbox.SplitLate = false
                    refSignbox.TotalWidth = 250.0F;
                    refSignbox.LockedWidth = true;

                    // relative col widths in proportions - 1:1        
                    widths = new float[] { 2.2F, 1.0F };
                    refSignbox.SetWidths(widths);
                    refSignbox.HorizontalAlignment = 0;

                    cell = new PdfPCell(new Phrase("Electronically Signed By", bodyFont));
                    cell.Colspan = 2;
                    cell.Border = 0;
                    // cell.Padding = 8
                    cell.Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    refSignbox.AddCell(cell);

                    DataTable dtRefSig = new DataTable();
                    string strRefSQL = string.Empty;
                    strRefSQL = " select App.FirstName,App.LastName,App.EmailId,Dev.SignPath from tblAppuser App join"
        + " tblDevices Dev on App.PhoneNumber = Dev.PhoneNumber where App.NPINumber  = " + NPINumber;
                    dtRefSig = db.DTByDataAdapter(strRefSQL);
                    //   Byte[] refBytes = System.Text.Encoding.ASCII.GetBytes(dtRefSig.Rows[0].ItemArray[3].ToStringOrEmpty());
                    //Byte[] refBytes = (Byte[])dtRefSig.Rows[0].ItemArray[3];
                    var base64EncodedBytes = System.Convert.FromBase64String(dtRefSig.Rows[0].ItemArray[3].ToStringOrEmpty());
                    // iTextSharp.text.Image refImage = iTextSharp.text.Image.GetInstance(System.Text.Encoding.UTF8.GetString(base64EncodedBytes));
                    //iTextSharp.text.Image refImage = iTextSharp.text.Image.GetInstance(refBytes);
                    MemoryStream mStream = new MemoryStream(Convert.FromBase64String(dtRefSig.Rows[0].ItemArray[3].ToStringOrEmpty()));
                    iTextSharp.text.Image refImage = iTextSharp.text.Image.GetInstance(mStream);

                    // Do something special with the empty ones
                    if (base64EncodedBytes.Length == 0)
                    {
                    }
                    else
                    {
                        refImage.ScaleToFit(100F, 200F);
                        Chunk imageChunk = new Chunk(refImage, 0, 0);
                    }

                    PdfPCell refImageCell = new PdfPCell(refImage);
                    refImageCell.Border = 0;
                    refImageCell.Rowspan = 2;
                    refImageCell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    refImageCell.Padding = 2;

                    refSignbox.AddCell(refImageCell);


                    cell = new PdfPCell(new Phrase(DateTime.Today.ToString("MM/dd/yyyy"), bodyFont));
                    cell.Border = 0;
                    cell.Border = Rectangle.RIGHT_BORDER;
                    // cell.Padding = 8
                    cell.HorizontalAlignment = 2;
                    refSignbox.AddCell(cell);

                    cell = new PdfPCell(new Phrase(DateTime.Now.ToString("hh:mm:ss tt"), bodyFont));
                    cell.Border = 0;
                    cell.Border = Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                    // cell.Padding = 8
                    cell.HorizontalAlignment = 2;
                    refSignbox.AddCell(cell);
                    string Name = dtRefSig.Rows[0]["FirstName"].ToStringOrEmpty() + "," + dtRefSig.Rows[0]["LastName"].ToStringOrEmpty();
                    cell = new PdfPCell(new Phrase(Name.ToStringOrEmpty(), bodyFont));
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    // cell.Padding = 8
                    cell.Colspan = 2;
                    refSignbox.AddCell(cell);

                    cell = new PdfPCell(new Phrase("NPI " + NPINumber.ToStringOrEmpty(), bodyFont));
                    cell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    cell.Colspan = 2;
                    // cell.Padding = 8
                    refSignbox.AddCell(cell);

                    cell = new PdfPCell(new Phrase("", bodyFont));
                    cell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    cell.Colspan = 2;
                    // cell.Padding = 8
                    refSignbox.AddCell(cell);
                    doc.Add(refSignbox);

                    doc.Close();
                    #endregion 
                    bytes1 = outputPDF.ToArray();



                    //using (MemoryStream stream = new MemoryStream())
                    //{
                    //    PdfReader reader12 = new PdfReader(bytes1);
                    //    using (PdfStamper stamper = new PdfStamper(reader12, stream))
                    //    {
                    //        // stamper.SetEncryption(PdfWriter.ENCRYPTION_AES_128, "", "", PdfWriter.ALLOW_ASSEMBLY)
                    //        int pages = reader12.NumberOfPages;
                    //        for (int i = 1; i <= pages; i++)
                    //            ColumnText.ShowTextAligned(stamper.GetUnderContent(i),
                    //                Element.ALIGN_CENTER, new Phrase(i + " of " + pages, bodyFont), 300, 790, 0);
                    //    }
                    //    bytes1 = stream.ToArray();
                    //}
                }
            }
            //===============



            // bytes1 = ms.ToArray();

            string containerName = string.Empty;
            string pdfName = string.Empty;
            string[] words = path.Split('\\');
            int length = 0;
            foreach (string word in words)
            {
               
                if (words.Length - 1 > length)
                {
                    if (length == 0)
                    {
                        containerName = word;
                    }
                    else
                    {
                        containerName = containerName + "\\" + word;
                    }
                }
                else
                {
                    pdfName = word.Substring(0, word.IndexOf('.'));
                  
                    pdfName = pdfName + "_RefSigned";
                }
                length++;
            }


            // File.WriteAllBytes(pathout, bytes1);
            string newPath = string.Empty;
            string res = string.Empty;
            // string containerName = rptSavePath + "\\" + path;
            string response = CloudeStoreg.createFileFromBytes(containerName, pdfName, bytes1);
            if (response == "successful")
            {
                newPath = containerName + "\\" + pdfName + ".pdf";
            }
            else
            {

                newPath = res;
            }
            return newPath;
        }

    }
}
