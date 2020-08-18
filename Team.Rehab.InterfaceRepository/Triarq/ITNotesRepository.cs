using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.BusinessEntities;

namespace Team.Rehab.InterfaceRepository.Triarq
{
    public interface ITNotesRepository
    {
        JArray GetMedicalNcessityNotes(int patientId,int NoteId);
        string GetCCDAInXMLFormat(int patientId, int NoteId);
        List<TriPatientNotesEntity> GetPatientNotes(int patientId);
    }
}
