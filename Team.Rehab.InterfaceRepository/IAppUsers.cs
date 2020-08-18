using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.DataModel;
using Team.Rehab.BusinessEntities;

namespace Team.Rehab.InterfaceRepository
{
   public interface IAppUsers
    {
        AppUsersEntity GetAppUserById(int AUrowid);
         List<AppUsersEntity> GetAllAppUsers();
        int CreateAppUser(AppUsersEntity appUserEntity);
        bool UpdateAppUser(AppUsersEntity appUserEntity);
        bool DeleteAppUser(int auRowId);
        tblAppAdmins GetAdmins(tblAppAdmins admin);
    }
}
