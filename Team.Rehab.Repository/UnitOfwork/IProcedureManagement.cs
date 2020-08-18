using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.DataModel;
using Team.Rehab.InterfaceRepository;

namespace Team.Rehab.Repository.UnitOfwork
{
    public interface IProcedureManagement
    {
        void Save();
       
        List<SP_ValidateUser_Result> GetUser(string UserName, string Password);

        
    }
}
