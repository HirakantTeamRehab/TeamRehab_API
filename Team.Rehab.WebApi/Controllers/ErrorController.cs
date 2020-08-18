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
namespace Team.Rehab.WebApi.Controllers
{
    public class ErrorController : ApiController
    {
        ICommonRepository _commonRepository;

        public ErrorController(ICommonRepository commonRepository)
        {

            this._commonRepository = commonRepository;
        }
        [HttpPost]
        [Route("~/api/Error/LogAppError")]
        public void LogAppError(tblAppError AppErrors)
        {

            _commonRepository.LogAppError(AppErrors);
        }
    }
}
