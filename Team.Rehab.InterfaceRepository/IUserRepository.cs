using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.BusinessEntities;
using Team.Rehab.DataModel;

namespace Team.Rehab.InterfaceRepository
{
    public interface IUserRepository
    {
        List<tblAppUser> GetUser(string UserName,string Password);
        List<ValidateUserEntity> AuthenticateUser(string UserName, string Password);

        // tblUser GetUser(Guid userId);

    }
}
