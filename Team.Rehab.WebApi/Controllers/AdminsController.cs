using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Team.Rehab.DataModel;
using Team.Rehab.InterfaceRepository;

namespace Team.Rehab.WebApi.Controllers
{
    public class AdminsController : ApiController
    {
        private readonly IAppUsers _appUserRepository;




        /// <summary>
        /// Public constructor to initialize product service instance
        /// </summary>
        public AdminsController(IAppUsers appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }
        // GET: api/Admins
        // POST: api/Admins
        [ResponseType(typeof(tblAppAdmins))]
        public IHttpActionResult PostAdmin(tblAppAdmins admin)
        {
            _appUserRepository.GetAdmins(admin);

            if (_appUserRepository.GetAdmins(admin)!=null)
            {
                return Ok(admin);
            }
            else
            {
                return NotFound();
            }

        }

    }
}
