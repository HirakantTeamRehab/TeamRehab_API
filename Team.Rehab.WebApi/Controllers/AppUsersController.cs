using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Team.Rehab.InterfaceRepository;
using Team.Rehab.Repository.Repository;
using Team.Rehab.DataModel;
using Team.Rehab.WebApi.ErrorHelper;
using Team.Rehab.WebApi.Helpers;
using Team.Rehab.BusinessEntities;
using AttributeRouting.Web.Http;

namespace Team.Rehab.WebApi.Controllers
{
    public class AppUsersController : ApiController
    {
        private readonly IAppUsers _appUserRepository;




        /// <summary>
        /// Public constructor to initialize product service instance
        /// </summary>
        public AppUsersController(IAppUsers appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }
        // GET api/product
        [GET("allAppUsers")]
        [GET("all")]
        public HttpResponseMessage Get()
        {
            //int b = 0;
            //int a = 100 / b;
            var products = _appUserRepository.GetAllAppUsers();
            var appUserEntities = products as List<AppUsersEntity> ?? products.ToList();
            if (appUserEntities.Any())
                return Request.CreateResponse(HttpStatusCode.OK, appUserEntities);
            throw new ApiDataException(1000, "user not found", HttpStatusCode.NotFound);
        }

        // GET api/product/5
        [GET("AppUser/{id?}")]
        
        public HttpResponseMessage Get(int id)
        {
            if (id != null)
            {
                var user = _appUserRepository.GetAppUserById(id);
                if (user != null)
                    return Request.CreateResponse(HttpStatusCode.OK, user);

                throw new ApiDataException(1001, "No user found for this id.", HttpStatusCode.NotFound);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }

        // POST api/product
        [POST("Create")]
        [POST("Register")]
        public int Post([FromBody] AppUsersEntity appUserEntity)
        {
            return _appUserRepository.CreateAppUser(appUserEntity);
        }

        // PUT api/product/5
        [PUT("Update/AppUserid/{id}")]
        [PUT("Modify/AppUserid/{id}")]
        public bool Put([FromBody] AppUsersEntity appUserEntity)
        {
            if (appUserEntity.AUrowid > 0)
            {
                return _appUserRepository.UpdateAppUser(appUserEntity);
            }
            return false;
        }

        // DELETE api/product/5
        [DELETE("remove/AppUserid/{id}")]
        [PUT("delete/AppUserid/{id}")]
        public bool Delete(int id)
        {
            if (id != null && id > 0)
            {
                var isSuccess = _appUserRepository.DeleteAppUser(id);
                if (isSuccess)
                {
                    return isSuccess;
                }
                throw new ApiDataException(1002, "User is already deleted or not exist in system.", HttpStatusCode.NoContent);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }
    }
}
