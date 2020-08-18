using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Team.Rehab.Repository.Data
{

    public class MyHeaderFooterNoteEvent : PdfPageEventHelper
    {
        private Font titleFont = FontFactory.GetFont("Roboto", 18, Font.BOLD);
        private Font subTitleFont = FontFactory.GetFont("Roboto", 12, Font.BOLD);
        private Font boldTableFont = FontFactory.GetFont("Roboto", 10, Font.BOLD);
        private Font endingMessageFont = FontFactory.GetFont("Roboto", 10, Font.ITALIC);
        private Font bodyFont = FontFactory.GetFont("Roboto", 10, Font.NORMAL);
        public String text = "";
        DocManager db = new DocManager();

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            int hdrStartLeft, hdrStartTop;
            base.OnEndPage(writer, document);
            PdfContentByte canvas = writer.DirectContent;
            text = "Page " + writer.PageNumber.ToString() + " of ";
            iTextSharp.text.Rectangle pageSize = document.PageSize;

            // hdrStartLeft = CInt(ConfigurationManager.AppSettings("hdrStartLeft"))
            // hdrStartTop = CInt(ConfigurationManager.AppSettings("hdrStartTop"))

            DataTable dtP = new DataTable(); // Patient Details
            dtP = db.GetPatient(Convert.ToInt32(HttpContext.Current.Session["patientid"]));

            hdrStartLeft = 140;
            hdrStartTop = 720;

            if (Convert.ToBoolean(dtP.Rows[0]["UseLogo"]) == false)
            {
                var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/team-rehab-logo-prnt.jpg"));
                logo.SetAbsolutePosition(150, 750);
                document.Add(logo);
            }
            else
            {
                Byte[] bytes = (Byte[])dtP.Rows[0]["LogoFile"];

                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(bytes);

                image.SetAbsolutePosition(140, 750);
                document.Add(image);
            }

            if (HttpContext.Current.Session["NoteType"].ToString() == "PPOC" | HttpContext.Current.Session["NoteType"].ToString() == "PPOC2")
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase("Plan Of Care - Initial Evaluation", titleFont), 320,
                    hdrStartTop + 5, 0);
            else if (HttpContext.Current.Session["NoteType"].ToString() == "PTREAT")
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase("Daily Note", titleFont), 330, hdrStartTop + 5, 0);
            else if (HttpContext.Current.Session["NoteType"].ToString() == "PMN")
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase("Medical Necessity", titleFont), 330, hdrStartTop + 5, 0);
            else if (HttpContext.Current.Session["NoteType"].ToString() == "PPOCRE")
            {
                DataTable dtPV = new DataTable();
                dtPV = db.GetPatVisits(Convert.ToInt32(HttpContext.Current.Session["patientid"]));
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase("Visits " + dtPV.Rows[0]["Visits"].ToString(), bodyFont), 40, hdrStartTop + 5, 0);
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase("POC - Re-Evaluation", titleFont), 350, hdrStartTop + 5, 0);
            }
            else if (HttpContext.Current.Session["NoteType"].ToString() == "PMV")
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase("Missed Visit", titleFont), 330, hdrStartTop + 5, 0);
            else if (HttpContext.Current.Session["NoteType"].ToString() == "PCOMM")
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase("Physicians Communication", titleFont), 340, hdrStartTop + 5, 0);
            else if (HttpContext.Current.Session["NoteType"].ToString() == "PDIS")
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase("Discharge", titleFont), 350, hdrStartTop + 5, 0);
            else if (HttpContext.Current.Session["NoteType"].ToString() == "PFCE")
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase("Functional Capacity Evaluation", titleFont), 320, hdrStartTop + 5, 0);


            string strSQL;
           // DBHelper db = new DBHelper();
            DataTable dtD = new DataTable(); // Document Details   
            DataTable dtPIC = new DataTable(); // Pat Insurance Claim Num
            DataTable dtDoS = new DataTable(); // DoS
            DataTable dtPatVisits = new DataTable(); // Patient Visits
            DataTable dtTher = new DataTable(); // Treating Therapist

            dtD = db.GetDocDateOfService(Convert.ToInt32( HttpContext.Current.Session["docrowid"]));
            dtPIC = db.GetPatInsuranceClm(Convert.ToInt32(HttpContext.Current.Session["patientid"]),
                Convert.ToInt32(HttpContext.Current.Session["docrowid"]));
            dtPatVisits = db.GetPatVisits(Convert.ToInt32(HttpContext.Current.Session["patientid"]));
            if (HttpContext.Current.Session["UserRole"].ToString().Trim() == "Therapist")
                dtTher = db.GetDocTherapist(Convert.ToInt32(HttpContext.Current.Session["user"]));
            else
                dtTher = db.GetDocTherapistRpt(Convert.ToInt32(HttpContext.Current.Session["docrowid"]));
            dtPIC.Columns[0].MaxLength = 50;
            if (dtPIC.Rows.Count > 0)
            {
                dtPIC.Rows[0]["ClmNoTxt"] = "Claim Number";
                dtPIC.AcceptChanges();
            }

            canvas.MoveTo(10, hdrStartTop);
            canvas.LineTo(580, hdrStartTop);
            canvas.Stroke();

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase("Patient's Name", boldTableFont), hdrStartLeft, hdrStartTop - 15, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(dtP.Rows[0]["PatientName"].ToString(), bodyFont), hdrStartLeft + 10, hdrStartTop - 15, 0);

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase("Date Of Service", boldTableFont), hdrStartLeft + 300, hdrStartTop - 15, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(dtD.Rows[0]["DateOfService"].ToString(), bodyFont), hdrStartLeft + 310, hdrStartTop - 15, 0);

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase("DOB", boldTableFont), hdrStartLeft, hdrStartTop - 30, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(dtP.Rows[0]["BirthDate"].ToString(), bodyFont), hdrStartLeft + 10, hdrStartTop - 30, 0);

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase("Medical Record", boldTableFont), hdrStartLeft, hdrStartTop - 45, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(dtP.Rows[0]["MedicalRecordNum"].ToString(), bodyFont), hdrStartLeft + 10, hdrStartTop - 45, 0);

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase("SOC Date", boldTableFont), hdrStartLeft + 300, hdrStartTop - 45, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(dtP.Rows[0]["firstvisitdate"].ToString(), bodyFont), hdrStartLeft + 310, hdrStartTop - 45, 0);

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase("Treating Therapist", boldTableFont), hdrStartLeft, hdrStartTop - 60, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(dtTher.Rows[0]["Name"].ToString(), bodyFont), hdrStartLeft + 10, hdrStartTop - 60, 0);

             ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase("Clinic Phone", boldTableFont), hdrStartLeft + 300, hdrStartTop - 60, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(dtP.Rows[0]["ClinicPhone"].ToString(), bodyFont), hdrStartLeft + 310, hdrStartTop - 60, 0);

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase("Referring Physican", boldTableFont), hdrStartLeft, hdrStartTop - 75, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(HttpContext.Current.Session["PrintName"].ToString(), bodyFont), hdrStartLeft + 10, hdrStartTop - 75, 0);

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase("Clinic Fax", boldTableFont), hdrStartLeft + 300, hdrStartTop - 75, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(dtP.Rows[0]["Fax"].ToString(), bodyFont), hdrStartLeft + 310, hdrStartTop - 75, 0);

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase("Treating Clinic", boldTableFont), hdrStartLeft, hdrStartTop - 90, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(dtP.Rows[0]["MarketingclinicName"].ToString(), bodyFont), hdrStartLeft + 10, hdrStartTop - 90, 0);

            if (dtP.Rows[0]["PatientCondition"].ToString() == "")
            {
            }
            else
            {
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase("Claim Number", boldTableFont), hdrStartLeft + 300, hdrStartTop - 90, 0);
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(dtPIC.Rows[0]["ClaimNo"].ToString(), bodyFont), hdrStartLeft + 310, hdrStartTop - 90, 0);
            }

            canvas.MoveTo(10, hdrStartTop - 100);
            canvas.LineTo(580, hdrStartTop - 100);
            canvas.Stroke();
        }

        public void AbbrMeasureType(string dt)
        {
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            int pageN = writer.PageNumber;
            // Dim text As [String] = "Page " + pageN.ToString() + "/"

            PdfContentByte canvas = writer.DirectContent;
            iTextSharp.text.Rectangle pageSize = document.PageSize;
        }
    }
}