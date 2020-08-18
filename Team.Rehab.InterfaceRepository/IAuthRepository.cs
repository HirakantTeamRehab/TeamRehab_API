using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.DataModel;
using Team.Rehab.BusinessEntities;
namespace Team.Rehab.InterfaceRepository
{
    public interface IAuthRepository
    {
        tblAppUser RegisterUser(DevicesEntity deviceModel);

        Client FindClient(string clientId);
        Task<bool> AddRefreshToken(tblRefreshTokens token);
        Task<bool> RemoveRefreshToken(string refreshTokenId);
        Task<bool> RemoveRefreshToken(tblRefreshTokens refreshToken);
        Task<tblRefreshTokens> FindRefreshToken(string refreshTokenId);
        List<tblRefreshTokens> GetAllRefreshTokens();
        Task<bool> UpdateRefreshToken(RefreshToken refreshToken);
       bool UpdateDevices(tblOTP devices);
        UserDevicesEntity GetUserByPhoneNo(string phoneNumber);
        tblAppUser GetAppUserByPhoneNo(string phoneNumber);
        bool VerifyOTP(string phoneNumber, string OTP);
        bool SendOTP(string StrPhone, string strTimeZone);
        tblDevice GetMobileNo(string deviceId);
        tblRefreshTokens GetRefreshTokenByDeviceID(string deviceId);
    }
}
