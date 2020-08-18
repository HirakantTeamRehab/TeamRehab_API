using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.BusinessEntities;

namespace Team.Rehab.InterfaceRepository.Data
{
  public  interface IMedicalNecessityNotes
    {

         MSDateOfServiceEntity GetMSDateOfService(int patientId, int NoteId, string NoteType);

         MSHints GetHints(int patientId, int NoteId, string NoteType);

         List<MSFunctionalCharEntity> GetFunctChar(int patientId, int NoteId, string NoteType);
         //List<MSShoulderEntity> GetShoulder(int patientId, int NoteId, string NoteType);
        
    }
}
