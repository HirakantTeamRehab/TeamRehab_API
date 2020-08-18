using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.DataModel;


namespace Team.Rehab.InterfaceRepository
{
    public interface ICommonRepository
    {
        tblBlockedIP GetIpsByID(int ipID);
        tblBlockedIP GetIps();
        int DeleteUser(int ipID);
        int CreateFailureEntry(tblBlockedIP lstFailureInfo);
        void UpdateFailureInfo(tblBlockedIP lstFailureInfo);
        void LogAppError(tblAppError AppError);
    }
}
