using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
    public class PatientEntity
    {
        public int Prowid { get; set; }
        public short ClinicNo { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string MiddleInitial { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }
        public string ReferralDate { get; set; }
        public string InjuryDate { get; set; }
        public string DischargeDate { get; set; }
        public string ReferralSource { get; set; }
        public Nullable<bool> PostOp { get; set; }
        public string Note { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string HomePh { get; set; }
        public bool HomePhO { get; set; }
        public bool HomePhB { get; set; }
        public string WorkPh { get; set; }
        public bool WorkPhO { get; set; }
        public bool WorkPhB { get; set; }
        public string CellPh { get; set; }
        public bool CellPhO { get; set; }
        public bool CellPhB { get; set; }
        public string OtherPh { get; set; }
        public bool OtherPhO { get; set; }
        public bool OtherPhB { get; set; }
        public string Email { get; set; }
        public bool NoEmail { get; set; }
        public bool NoMarketingEmail { get; set; }
        public Nullable<int> TherapistID { get; set; }
        public string firstvisitdate { get; set; }
        public Nullable<bool> RenumberPatientID { get; set; }
        public string Race { get; set; }
        public string createdby { get; set; }
        public System.DateTime createdts { get; set; }
        public string updatedby { get; set; }
        public System.DateTime updatedts { get; set; }
        public string Discipline { get; set; }
        public Nullable<bool> CapMet { get; set; }
        public Nullable<bool> OTCapMet { get; set; }
        public string ReferralSource2 { get; set; }
        public string ReferralSource3 { get; set; }
        public Nullable<bool> ApptRemind { get; set; }
        public Nullable<bool> ApptRemindEval { get; set; }
    }
}
