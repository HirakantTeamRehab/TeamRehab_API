using System.Linq;
using System.Transactions;
using Team.Rehab.InterfaceRepository;
using Team.Rehab.DataModel;
using Team.Rehab.Repository.UnitOfwork;
using Team.Rehab.BusinessEntities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Data.Entity;

using System.IO;
using System.Xml;
using System.Net;
using System.Xml.Linq;

namespace Team.Rehab.Repository.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IUnitOfwork _unitOfWork;

        private RehabEntities _ctx;
        public AuthRepository()
        {
            this._unitOfWork = new UnitOfWork();
            _ctx = new RehabEntities();
        }

        public tblDevice TblDevice { get; private set; }

        public tblAppUser RegisterUser(DevicesEntity deviceModel)
        {
            bool flag = false;
            List<tblAppUser> user = new List<tblAppUser>();
            List<tblDevice> tblDevice = new List<tblDevice>();
           
            try
            {
                user = _unitOfWork.AppUserRepo.Get(c =>  c.PhoneNumber.Equals(deviceModel.PhoneNumber)).ToList();
                if (user.Count() > 0)
                {
                    
                    tblDevice = _unitOfWork.DeviceRepo.Get(c => c.EmailId.Equals(deviceModel.EmailId) && c.PhoneNumber.Equals(deviceModel.PhoneNumber)).ToList();
                    if (tblDevice.Count() == 0)
                    {
                        using (var scope = new TransactionScope())
                        {
                            var TblDevice = new tblDevice
                            {
                                //Drowid = 1,
                                DeviceId = deviceModel.DeviceId,
                                EmailId = deviceModel.EmailId,
                                PhoneNumber = deviceModel.PhoneNumber,
                                SignPath = deviceModel.SignPath,


                            };

                            _unitOfWork.DeviceRepo.Insert(TblDevice);
                            _unitOfWork.Save();
                            scope.Complete();
                           
                        }
                    }

                    flag = true;

                }
                if (flag)
                {
                    return user.FirstOrDefault();
                }
                else
                {
                    tblAppUser emptyUser = new tblAppUser();
                    return emptyUser;
                }

            }
            catch (Exception ex)
            {

                CustomLogger.LogError("AuthRepository in RegisterUser " + Convert.ToString(ex.Message));
                CustomLogger.SendExcepToDB(ex, "AuthRepository");

                throw;
            }
            finally
            {
            }
            
        }

        public Client FindClient(string clientId)
        {

            var modelClient = _unitOfWork.ClientRepo.Get(o => o.Id.Equals(clientId)).FirstOrDefault();
            var client = new Client
            {
                Id = modelClient.Id,
                ApplicationType = (ApplicationTypes)modelClient.ApplicationType,
                Name = modelClient.Name,
                AllowedOrigin = modelClient.AllowedOrigin,
                Active = modelClient.Active,
                RefreshTokenLifeTime = modelClient.RefreshTokenLifeTime,
                Secret = modelClient.Secret
            };
            return client;
        }

        public async Task<bool> AddRefreshToken(tblRefreshTokens token)
        {

            var existingToken = _ctx.tblRefreshTokens.Where(r => r.DeviceId == token.DeviceId && r.ClientId == token.ClientId).Select(x=>x.RefreshToken).SingleOrDefault();

            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }

            _ctx.tblRefreshTokens.Add(token);

            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.tblRefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                _ctx.tblRefreshTokens.Remove(refreshToken);
                return await _ctx.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(tblRefreshTokens refreshToken)
        {
            _ctx.tblRefreshTokens.Remove(refreshToken);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<tblRefreshTokens> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.tblRefreshTokens.FindAsync(refreshTokenId); 

            return refreshToken;
        }

        public List<tblRefreshTokens> GetAllRefreshTokens()
        {
            return _ctx.tblRefreshTokens.ToList();
        }

        /// <summary>
        /// Updates a product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="productEntity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRefreshToken(RefreshToken refreshToken)
        {
            var success = false;
            try
            {
                if (refreshToken != null)
                {
                    //  using (var scope = new TransactionScope())
                    // {
                    var rToken = _ctx.tblRefreshTokens.Find(refreshToken.Id);
                    //   rToken.VerificationCode = refreshToken.VarificationCode;
                    if (rToken != null)
                    {
                        rToken.Id = refreshToken.Id;
                        rToken.Subject = refreshToken.Subject;
                        rToken.ClientId = refreshToken.ClientId;
                        rToken.IssuedUtc = refreshToken.IssuedUtc;
                        rToken.ExpiresUtc = refreshToken.ExpiresUtc;
                        rToken.ProtectedTicket = refreshToken.ProtectedTicket;                    
      
       
                          _ctx.tblRefreshTokens.Attach(rToken);
                        _ctx.Entry(rToken).State = EntityState.Modified;
                        success = await _ctx.SaveChangesAsync() > 0;
                        //  scope.Complete();
                        // success = true;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
               
             
                CustomLogger.LogError("AuthRepository in UpdateRefreshToken " + Convert.ToString(ex.Message));
                CustomLogger.SendExcepToDB(ex, "AuthRepository");

                throw;
            }
            return success;
        }
        public bool UpdateDevices(tblOTP devices)
        {
            devices.IssuedUtc = DateTime.UtcNow;
            tblOTP otpEntity = new tblOTP();
            var success = false;
            try
            {
                if (devices != null)
                {
                    //  using (var scope = new TransactionScope())
                    // {
                    //var rToken = _ctx.tblDevices.Find(devices.PhoneNumber);
                    var rToken = _ctx.tblOTPs.Where(x => x.PhoneNumber == devices.PhoneNumber).FirstOrDefault();

                    // rToken.OTPCode = devices.OTPCode;
                    if (rToken != null)
                    {

                        rToken.OTPCode = devices.OTPCode;
                        rToken.IssuedUtc = DateTime.UtcNow;
                        _ctx.Entry(rToken).State = EntityState.Modified;
                        _ctx.SaveChanges();

                       
                    }
                    else
                    {
                        _ctx.tblOTPs.Add(devices);
                        _ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
              
                CustomLogger.LogError("AuthRepository in UpdateDevices "+Convert.ToString(ex.Message));              
                CustomLogger.SendExcepToDB(ex, "AuthRepository");
                throw;
            }
          
            return success;
        }


        public UserDevicesEntity GetUserByPhoneNo(string phoneNumber)
        {
            UserDevicesEntity patient = new UserDevicesEntity();
            try
            {
               
                //UserDevicesEntity
                //  patient = _unitOfWork.DeviceRepo.Get(o => o.DeviceId.Equals(phoneNumber)).FirstOrDefault();

                //patient = (from _devices in _unitOfWork.DeviceRepo.Get(o => o.PhoneNumber.Equals(phoneNumber))
                //                            join _user in _unitOfWork.UserEntityRepo.Get() on _devices.UserId equals _user.UserId
                //           join _refreshtokens in _unitOfWork.RefreshTokenRepo.Get() on _devices.DeviceId equals _refreshtokens.DeviceId
                //           where _devices.PhoneNumber == phoneNumber
                //                            select new UserDevicesEntity
                //                            {
                //                                UserId = _user.UserId,
                //                                Password = _user.UserPassword,
                //                                RefreshToken = _refreshtokens.RefreshToken,
                //                                ExpiresUTC= _refreshtokens.ExpiresUtc
                //                            }).FirstOrDefault();

                patient = (from _devices in _unitOfWork.DeviceRepo.Get(o => o.PhoneNumber.Equals(phoneNumber))
                           join _refreshtokens in _unitOfWork.RefreshTokenRepo.Get() on _devices.DeviceId equals _refreshtokens.DeviceId
                           where _devices.PhoneNumber == phoneNumber
                           select new UserDevicesEntity
                           {
                               UserId = _devices.EmailId,
                               Password = _devices.PhoneNumber,
                               RefreshToken = _refreshtokens.RefreshToken,
                               ExpiresUTC = _refreshtokens.ExpiresUtc
                           }).FirstOrDefault();

            }
            catch (Exception ex)
            {

                throw;
            }
         
            ////===============Mapper===========================================
            //var config = new MapperConfiguration(cfg => {
            //    cfg.CreateMap<tblDevice, DevicesEntity>();
            //});

            //IMapper mapper = config.CreateMapper();
            //deviceEntity = mapper.Map<tblDevice, DevicesEntity>(patient);
            ////===============mapper end==========================================
            return patient;
        }
        public tblAppUser GetAppUserByPhoneNo(string phoneNumber)
        {
            tblAppUser patient = new tblAppUser();
            try
            {

                //UserDevicesEntity
                // var patient = _unitOfWork.DeviceRepo.Get(o => o.DeviceId.Equals(phoneNumber)).FirstOrDefault();

                patient =  _unitOfWork.AppUserRepo.Get(o => o.PhoneNumber.Equals(phoneNumber)).FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw;
            }

            ////===============Mapper===========================================
            //var config = new MapperConfiguration(cfg => {
            //    cfg.CreateMap<tblDevice, DevicesEntity>();
            //});

            //IMapper mapper = config.CreateMapper();
            //deviceEntity = mapper.Map<tblDevice, DevicesEntity>(patient);
            ////===============mapper end==========================================
            return patient;
        }
        public tblDevice GetMobileNo(string deviceId)
        {
            tblDevice tblDevice   = new tblDevice();
            try
            {

                //UserDevicesEntity
                tblDevice = _unitOfWork.DeviceRepo.Get(o => o.DeviceId.Equals(deviceId)).FirstOrDefault();

            
                

            }
            catch (Exception ex)
            {
                CustomLogger.SendExcepToDB(ex, "AuthRepository");
                CustomLogger.LogError(Convert.ToString("GetMobileNo in AuthRepository- " + ex.Message));

                throw;
            }

            ////===============Mapper===========================================
            //var config = new MapperConfiguration(cfg => {
            //    cfg.CreateMap<tblDevice, DevicesEntity>();
            //});

            //IMapper mapper = config.CreateMapper();
            //deviceEntity = mapper.Map<tblDevice, DevicesEntity>(patient);
            ////===============mapper end==========================================
            return tblDevice;
        }
        public tblRefreshTokens GetRefreshTokenByDeviceID(string deviceId)
        {
            tblRefreshTokens tblDevice = new tblRefreshTokens();
            try
            {

                //UserDevicesEntity
                tblDevice = _unitOfWork.RefreshTokenRepo.Get(o => o.DeviceId.Equals(deviceId)).FirstOrDefault();

            }
            catch (Exception ex)
            {
                CustomLogger.LogError("Error in GetRefreshTokenByDeviceID  " + Convert.ToString(ex.Message));
                CustomLogger.SendExcepToDB(ex, "AuthRepository");
                throw;
            }

            ////===============Mapper===========================================
            //var config = new MapperConfiguration(cfg => {
            //    cfg.CreateMap<tblDevice, DevicesEntity>();
            //});

            //IMapper mapper = config.CreateMapper();
            //deviceEntity = mapper.Map<tblDevice, DevicesEntity>(patient);
            ////===============mapper end==========================================
            return tblDevice;
        }
        public bool VerifyOTP(string phoneNumber, string OTPCode)
        {
            tblOTP OTP = new tblOTP();
            bool verifiedStatus = false;
            try
            {
                OTP = _unitOfWork.OTPRepo.Get(o => o.PhoneNumber.Equals(phoneNumber)).FirstOrDefault();
                //OTP = (from _OTP in _unitOfWork.OTPRepo.Get(o => o.PhoneNumber.Equals(phoneNumber))
                //                    //join _refreshtokens in _unitOfWork.RefreshTokenRepo.Get() on _devices.DeviceId equals _refreshtokens.DeviceId
                //                    //where _devices.PhoneNumber == phoneNumber
                //                select new tblOTP
                //                {
                //                    OTPCode = _OTP.OTPCode                                   
                //                }).FirstOrDefault();

                if (OTPCode == Convert.ToString(OTP.OTPCode))
                {
                    verifiedStatus = true;
                }
            }
            catch (Exception ex)
            {
                CustomLogger.SendExcepToDB(ex, "AuthRepository");
                CustomLogger.LogError(Convert.ToString("VerifyOTP in AuthRepository- " + ex.Message));
                throw;
            }
            return verifiedStatus;
                
        }

        public bool SendOTP(string StrPhone,string strTimeZone)
        {

            string responseStr = "";
            // string StrPhone = "9049991221";
            string cultureName = "en-US";

            string fulldate = "12-08-2018 19:08:54";
            string[] time = strTimeZone.Split(' ');

            string datepart = time[0];
            string[] tt = datepart.Split('-');
            string finaldate = tt[1] + "/" + tt[0] + "/" + tt[2] + " " + time[1];

            try
            {
                //  Get list of Patients to send a note
                Random rnd = new Random();
                int myRandomNo = rnd.Next(100000, 999999); // creates a 8 digit random no.
                //var culture = new CultureInfo(cultureName);
                //object Tday = DateTime.Now.ToString(culture);
                DateTime mobileDate = Convert.ToDateTime(finaldate);
                object Tday = mobileDate.ToString("yyyy-MM-dd HH:mm");
                int mm = Convert.ToInt32(mobileDate.ToString("mm"));
                int ss = Convert.ToInt32(mobileDate.ToString("ss"));
                int hh = Convert.ToInt32(mobileDate.ToString("HH"));
                //ss = ss + 30;
                //if (ss < 60)
                //{
                //    Tday = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                //    Tday = (Tday + ":"+ Convert.ToString(ss));
                //}
                //else
                //{
                //    Tday = DateTime.Now.ToString("yyyy-MM-dd HH");
                //    ss = ss - 60;
                //    mm = mm + 1;
                //    Tday = (Tday + ":" + Convert.ToString(mm));
                //    //Tday = (Tday + ":" + Convert.ToString(ss));
                //}
                Tday = mobileDate.ToString("yyyy-MM-dd ");
                ss = ss - 60;
                mm = mm + 1;
                string minute = string.Empty;
                if (mm < 10)
                {
                    minute = "0" + mm;
                }
                else
                {
                    minute = Convert.ToString(mm);
                }
                hh = hh - 1;
                string hours = string.Empty;
                if (hh < 10)
                {
                    hours = "0" + hh;
                }
                else
                {
                    hours = Convert.ToString(hh);
                }
                Tday = (Tday + hours + ":" + minute);
                Tday = (Tday + "-05");

                //string msg = "OTP is" + myRandomNo;111
                //HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create("http://sms.mahabeautyplus.com/sendsms/sendsms.php?username=memahatra&password=mes1231&type=TEXT&sender=Alerts&mobile="+ StrPhone + "&message=" + msg + "");

                //// HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create("http://sms.mahabeautyplus.com/sendsms/sendsms.php?username=memesthn&password=mes2014&type=TEXT&sender=Alerts&mobile=8879333715&message=hello");

                //HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
                //System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                //string responseString = respStreamReader.ReadToEnd();
                //respStreamReader.Close();
                //myResp.Close();

               

                //===========================USA=========================================
                string url1 = "https://api.remindercall.net/v3.2/dispatch.xml";
                // Enter code for sending text message 
                //00b4def6b655ce6026fbdc66014f779b6789da1321b9a43cad42f1804269134d
                // -----------    FORM XML    START       ------------------------------------------------------------------------------------------
                XDocument doc = new XDocument(
                                                new XElement("request",
                                                    new XElement("key", "cb5aba79e96ccfd59c5b3265e0786bb74c15598b2d2fc35fbef99ffed62a4825"),
                                                    new XElement("text", new XAttribute("action", "create"),
                                                        new XElement("id", myRandomNo),
                                                        new XElement("delivery", Tday.ToString()),
                                                        new XElement("number", StrPhone),
                                                        new XElement("message", "OTP is " + myRandomNo))));


                // -----------    FORM XML    END       ------------------------------------------------------------------------------------------
                HttpWebRequest request = ((HttpWebRequest)(WebRequest.Create(url1)));
                byte[] bytes;
                string xmlString = doc.ToString();
                bytes = System.Text.Encoding.ASCII.GetBytes(xmlString);
                request.ContentType = "text/xml; encoding=\'utf-8\'";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse resp;
                resp = ((HttpWebResponse)(request.GetResponse()));
                if ((resp.StatusCode == HttpStatusCode.OK))
                {
                    Stream responseStream = resp.GetResponseStream();
                    responseStr = new StreamReader(responseStream).ReadToEnd();
                }
                // TODO: Warning!!!! NULL EXPRESSION DETECTED...


                // responseStr = "<?xml version=""1.0"" encoding=""UTF-8""?>
                //               <response>
                //                  <status>
                //                     <text>
                //                         <id>2</id>
                //                         <result>success</result>
                //                         <status>new</status>
                //                     </text>
                //                 </status>
                //               </response>"
                string genre = string.Empty;
                using (XmlReader reader = XmlReader.Create(new StringReader(responseStr)))
                {
                    reader.ReadToFollowing("text");
                    reader.MoveToFirstAttribute();
                    reader.ReadToFollowing("result");
                    genre = reader.ReadElementContentAsString();
                }
                //=============================USA END
                tblOTP otpEntity = new tblOTP();
                otpEntity.PhoneNumber = StrPhone;
                otpEntity.OTPCode = Convert.ToString(myRandomNo);
                
                var result = UpdateDevices(otpEntity);
            }
            catch (Exception ex)
            {
               
                CustomLogger.LogMessage(ex.Message, "");
                CustomLogger.SendExcepToDB(ex, "AuthRepository");

                throw;
            }




            return true;
        }

        public tblAppUser GetNotifications(string phoneNumber)
        {
            tblAppUser patient = new tblAppUser();

            int ? NPINumber =null;
            try
            {

                //UserDevicesEntity
                // var patient = _unitOfWork.DeviceRepo.Get(o => o.DeviceId.Equals(phoneNumber)).FirstOrDefault();

                patient = _unitOfWork.AppUserRepo.Get(o => o.PhoneNumber.Equals(phoneNumber)).FirstOrDefault();
                NPINumber = patient.NPINumber;
            }
            catch (Exception ex)
            {

                throw;
            }

            ////===============Mapper===========================================
            //var config = new MapperConfiguration(cfg => {
            //    cfg.CreateMap<tblDevice, DevicesEntity>();
            //});

            //IMapper mapper = config.CreateMapper();
            //deviceEntity = mapper.Map<tblDevice, DevicesEntity>(patient);
            ////===============mapper end==========================================
            return patient;
        }
    }
}
