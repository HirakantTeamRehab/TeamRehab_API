using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualBasic;

namespace Team.Rehab.Repository.Data
{
    class MyHeaderFooterNoteEventWC : PdfPageEventHelper
    {
        private Font titleFont = FontFactory.GetFont("Roboto", 10, Font.BOLD);
        private Font bodyFont = FontFactory.GetFont("Roboto", 8, Font.NORMAL);

        private Font subTitleFont = FontFactory.GetFont("Roboto", 12, Font.BOLD);
        private Font boldTableFont = FontFactory.GetFont("Roboto", 10, Font.BOLD);
        private Font endingMessageFont = FontFactory.GetFont("Roboto", 10, Font.ITALIC);
        DocManager db = new DocManager();

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            int hdrStartLeft, hdrStartTop;
            base.OnEndPage(writer, document);
            PdfContentByte canvas = writer.DirectContent;
            String text = "Page " + writer.PageNumber.ToString() + " of ";
            // hdrStartLeft = CInt(ConfigurationManager.AppSettings("hdrStartLeft"))
            // hdrStartTop = CInt(ConfigurationManager.AppSettings("hdrStartTop"))

            hdrStartLeft = 140;
            hdrStartTop = 820;


            string strSQL;
            //DBHelper db = new DBHelper();
            DataTable dtP = new DataTable(); // Patient Details
            DataTable dtD = new DataTable(); // Document Details   
            DataTable dtPIC = new DataTable(); // Pat Insurance Claim Num
            DataTable dtDoS = new DataTable(); // DoS
            DataTable dtPatVisits = new DataTable(); // Patient Visits
            DataTable dtTher = new DataTable(); // Treating Therapist

            dtP = db.GetPatient(Convert.ToInt32(HttpContext.Current.Session["patientid"]));
            dtD = db.GetDocDateOfService(Convert.ToInt32(HttpContext.Current.Session["docrowid"]));
            dtPIC = db.GetPatInsuranceClm(Convert.ToInt32(HttpContext.Current.Session["patientid"]), Convert.ToInt32(HttpContext.Current.Session["docrowid"]));
            dtPatVisits = db.GetPatVisits(Convert.ToInt32(HttpContext.Current.Session["patientid"]));
            dtTher = db.GetDocTherapistRpt(Convert.ToInt32(HttpContext.Current.Session["docrowid"]));
            dtPIC.Columns[0].MaxLength = 50;
            if (dtPIC.Rows.Count > 0)
            {
                dtPIC.Rows[0]["ClmNoTxt"] = "Claim Number";
                dtPIC.AcceptChanges();
            }

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase(dtP.Rows[0]["MarketingclinicName"].ToString(), bodyFont), 580, 830, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase(dtP.Rows[0]["PhyStreetAddress"].ToString(), bodyFont), 580, 820, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase(dtP.Rows[0]["CityStateZip"].ToString(), bodyFont), 580, 810, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase("Phone: " + dtP.Rows[0]["ClinicPhone"].ToString(), bodyFont), 580, 800, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase("Progress Note - " + dtD.Rows[0]["DateOfService"].ToString(), titleFont), 300, 800, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase("Fax: " + dtP.Rows[0]["Fax"].ToString(), bodyFont), 580, 790, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase(dtP.Rows[0]["LastName"].ToString() + ", " + dtP.Rows[0]["FirstName"].ToString(), bodyFont), 305, 780, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase("Claim Number: " + dtPIC.Rows[0]["ClaimNo"].ToString(), bodyFont), 580, 775, 0);

            canvas.MoveTo(10, 770);
            canvas.LineTo(600, 770);
            canvas.Stroke();

            if (Convert.ToBoolean(dtP.Rows[0]["UseLogo"]) == false)
            {
                var logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Images/TrWclogo.jpg"));
                logo.SetAbsolutePosition(1, 780);
                document.Add(logo);
            }
            else
            {
                Byte[] bytes = (Byte[])dtP.Rows[0]["LogoFile"];
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(bytes);
                image.SetAbsolutePosition(20, 790);
                image.ScaleToFit(120F, 240F);
                document.Add(image);
            }
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            int pageN = writer.PageNumber;
            String text = "Page " + pageN.ToString() + "/";

            PdfContentByte canvas = writer.DirectContent;
            iTextSharp.text.Rectangle pageSize = document.PageSize;

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(text + pageSize.ToString(), titleFont), 450, 10, 0);
        }
    }
}
