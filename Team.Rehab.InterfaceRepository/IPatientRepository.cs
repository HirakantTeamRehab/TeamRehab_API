using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.BusinessEntities;
using Team.Rehab.DataModel;

namespace Team.Rehab.InterfaceRepository
{
    public interface IPatientRepository
    {
        List<tblPatients>GetPatients();
        List<SP_PatientNotes_Result> GetPatientNotes(int patientId);
        tblPatients GetPatientbyID(int patientId);
        List<PatientEntity> GetPatientsbyNPINumber(string NPINumber);
        SP_PatientNotes_Result GetPatientNotesByID(int patientId, int noteId);
        ViewModelPatientTherapist GetPatientsTherapist(string NPINumber);
        List<NotesSummaryEntity> GetNotesSummary(int patientId);
        //List<NotificationsEntity> GetNotifications(int NPINumber);
        List<PatientSchedulesEntity> GetPatientsSchedules(int patientId);

        string GetPdf(int patientId, int noteId);

        string SignNotes(int patientId, int noteId,int NPINumber);
        string UnSignNotes(int patientId, int noteId, int NPINumber);
        ReferralSignDetailsEntity GetRefSignDetails(int patientId, int noteId, string NPINumber);

        IniEvalSummaryEntity GetNotesSummary(int patientId, int noteId);
        List<MSGoalsEntity> GetPatientGoals(int patientId);
        List<PatTherapistEntity> GetDocTherapist(string NPINumber);
        bool SendRefEMail(string recipient, int npinum, int patientId, int noteId);
    }
}
