using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Team.Rehab.InterfaceRepository;
using Team.Rehab.Repository.Repository;
using Team.Rehab.DataModel;
using Team.Rehab.BusinessEntities;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using Team.Rehab.WebApi.ErrorHelper;
using System.Xml;
using System.Xml.Linq;
using Team.Rehab.WebApi.Helpers;

namespace Team.Rehab.WebApi.Controllers
{

    public class MobAppRegistrationController : ApiController
    {
        IAuthRepository _authRepository;
        public MobAppRegistrationController()
        {

            this._authRepository = new AuthRepository();
        }

        [HttpPost]
        [Route("~/api/Register")]
        //public tblAppUser Register()
        //{
        //    string sPath = "";
        //    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");
        //    DevicesEntity deviceModel = new DevicesEntity();
        //    tblAppUser result = new tblAppUser();
        //    System.Web.HttpPostedFile hpf = HttpContext.Current.Request.Files["Image"];
        //    deviceModel.DeviceId = Convert.ToString(HttpContext.Current.Request.Form["DeviceId"]);
        //    deviceModel.EmailId = Convert.ToString(HttpContext.Current.Request.Form["EmailId"]);
        //    deviceModel.PhoneNumber = Convert.ToString(HttpContext.Current.Request.Form["PhoneNumber"]);
        //    //deviceModel.SignPath = System.Text.Encoding.ASCII.GetBytes(HttpContext.Current.Request.Form["Image"]);
        //    deviceModel.SignPath = null;
        //    deviceModel.OTP = "999999";
        //    if (hpf != null)
        //    {
        //        try
        //    {

        //        // deviceModel.UserId = Convert.ToString(HttpContext.Current.Request.Form["UserId"]);

        //        if (hpf.ContentLength > 0)
        //        {

        //            string fileType = hpf.ContentType;
        //            Stream file_Strm = hpf.InputStream;
        //            string file_Name = Path.GetFileName(hpf.FileName);
        //            int fileSize = hpf.ContentLength;
        //            byte[] fileRcrd = new byte[fileSize];
        //            file_Strm.Read(fileRcrd, 0, fileSize);


        //            Stream fs = hpf.InputStream;
        //            BinaryReader br = new BinaryReader(fs);
        //            Byte[] bytes = br.ReadBytes((Int32)fs.Length);
        //            deviceModel.SignPath = bytes;
        //        }




        //        result = _authRepository.RegisterUser(deviceModel);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new ApiDataException(2000, "Technical error occured, Please contact administrator", HttpStatusCode.InternalServerError);

        //    }
        //    }
        //    else
        //    {
        //        throw new ApiDataException(1004, "Image is not selected", HttpStatusCode.InternalServerError);
        //    }

        //}
        public tblAppUser Register()
        {
            string sPath = "";
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");
            DevicesEntity deviceModel = new DevicesEntity();
            tblAppUser result = new tblAppUser();
            // System.Web.HttpPostedFile hpf = HttpContext.Current.Request.Files["Image"];
            deviceModel.DeviceId = Convert.ToString(HttpContext.Current.Request.Form["DeviceId"]);
            deviceModel.EmailId = Convert.ToString(HttpContext.Current.Request.Form["EmailId"]);
            deviceModel.PhoneNumber = Convert.ToString(HttpContext.Current.Request.Form["PhoneNumber"]);
            deviceModel.SignPath = Convert.ToString(HttpContext.Current.Request.Form["Image"]);
           // deviceModel.SignPath = null;
            deviceModel.OTP = "999999";
            //  if (hpf != null)
            //  {
            try
            {

                // deviceModel.UserId = Convert.ToString(HttpContext.Current.Request.Form["UserId"]);

                //  if (hpf.ContentLength > 0)
                // {

                //string fileType = hpf.ContentType;
                //Stream file_Strm = hpf.InputStream;
                //string file_Name = Path.GetFileName(hpf.FileName);
                //int fileSize = hpf.ContentLength;
                //byte[] fileRcrd = new byte[fileSize];
                //file_Strm.Read(fileRcrd, 0, fileSize);


                //   Stream fs = hpf.InputStream;
                // BinaryReader br = new BinaryReader(fs);
                //  Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                // deviceModel.SignPath = bytes;
                //  }




                result = _authRepository.RegisterUser(deviceModel);
                return result;
            }
            catch (Exception ex)
            {

                throw new ApiDataException(2000, "Technical error occured, Please contact administrator", HttpStatusCode.InternalServerError);

            }
            // }
            //else
            // {
            // throw new ApiDataException(1004, "Image is not selected", HttpStatusCode.InternalServerError);
            // }

        }
        [HttpGet]
        [Route("~/api/CheckUser")]
        public IHttpActionResult CheckUser(string phoneNumber)
        {
            UserDevicesEntity result = new UserDevicesEntity();
            try
            {
                //
                result = _authRepository.GetUserByPhoneNo(phoneNumber);
            }
            catch (Exception ex)
            {
               
                throw new ApiDataException(2000, "Technical error occured, Please contact administrator", HttpStatusCode.InternalServerError);

            }

            finally
            {

                if (result == null)
                {
                    throw new ApiDataException(1001, "The user is not registerd", HttpStatusCode.InternalServerError);

                }
                //  return result;

            }
            return Ok(result);
        }
        [HttpGet]
        [Route("~/api/CheckAccess")]
        public IHttpActionResult CheckAccess(string phoneNumber)
        {

            tblAppUser result = new tblAppUser();
            try
            {
                result = _authRepository.GetAppUserByPhoneNo(phoneNumber);
            }
            catch (Exception ex)
            {
               
                throw new ApiDataException(2000, "Technical error occured, Please contact administrator", HttpStatusCode.InternalServerError);

            }

            finally
            {

                if (result == null)
                {
                    throw new ApiDataException(1004, "Access not provided", HttpStatusCode.InternalServerError);

                }

                //  return result;

            }

            return Ok(result);
        }
        [HttpGet]
        [Route("~/api/VerifyOTP")]
        public IHttpActionResult VerifyOTP(string phoneNumber, string OTP)
        {
            bool result = false;
            try
            {
                result = _authRepository.VerifyOTP(phoneNumber, OTP);
            }
            catch (Exception ex)
            {
               
                throw new ApiDataException(2000, "Technical error occured, Please contact administrator", HttpStatusCode.InternalServerError);

            }

            finally
            {

                if (result == false)
                {
                    throw new ApiDataException(1002, "The OTP is incorrect", HttpStatusCode.InternalServerError);

                }
                //  return result;

            }
            return Ok(result);
        }
        [HttpGet]
        [Route("~/api/GetMobileNumber")]
        public IHttpActionResult GetMobileNumber(string DeviceId)
        {

            tblDevice result = new tblDevice();
            try
            {
                result = _authRepository.GetMobileNo(DeviceId);
            }
            catch (Exception ex)
            {
              
                throw new ApiDataException(2000, "Technical error occured, Please contact administrator", HttpStatusCode.InternalServerError);

            }

            finally
            {

                if (result == null)
                {
                    throw new ApiDataException(1003, "Mobile number is not present", HttpStatusCode.InternalServerError);

                }

                //  return result;

            }

            return Ok(result.PhoneNumber);
        }
        [Route("~/api/GetRefreshToken")]
        public IHttpActionResult GetRefreshTokenByDeviceID(string DeviceId)
        {

            tblRefreshTokens result = new tblRefreshTokens();
            try
            {
                result = _authRepository.GetRefreshTokenByDeviceID(DeviceId);
            }
            catch (Exception ex)
            {
               
                throw new ApiDataException(2000, "Technical error occured, Please contact administrator", HttpStatusCode.InternalServerError);

            }

            finally
            {

                if (result == null)
                {
                    throw new ApiDataException(1003, "Refresh token not found", HttpStatusCode.InternalServerError);

                }

                //  return result;

            }

            return Ok(result.RefreshToken);
        }

        [HttpPost]
        [Route("~/api/SendOTP")]
        public IHttpActionResult SendMessage(string StrPhone,string strTimeZone)
        {
            bool result = false;

            try
            {
                result = _authRepository.SendOTP(StrPhone,strTimeZone);
            }
            catch (Exception ex)
            {
              
                throw new ApiDataException(2000, "Technical error occured, Please contact administrator", HttpStatusCode.InternalServerError);

            }
             

            return Ok("OTP Sent");


        }

        public IHttpActionResult GetNotifications(string StrPhone)
        {
            bool result = false;

            try
            {
                //result = _authRepository.SendOTP(StrPhone);
            }
            catch (Exception ex)
            {

                throw new ApiDataException(2000, "Technical error occured, Please contact administrator", HttpStatusCode.InternalServerError);

            }


            return Ok("OTP Sent");


        }
    }
}
