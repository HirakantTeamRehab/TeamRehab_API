using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Team.Rehab.BusinessEntities;
using Team.Rehab.InterfaceRepository;
using Team.Rehab.WebApi.ErrorHelper;

namespace Team.Rehab.WebApi.Controllers
{
    public class NotificationsController : ApiController
    {
        INotificationsRepository _notificationsRepository;
        

        public NotificationsController(INotificationsRepository notificationsRepository)
        {

            this._notificationsRepository = notificationsRepository;
        }
        [HttpGet]
        [Route("~/api/Notifications/GetNotifications/{NPINumber}")]
        public List<NotificationsEntity> GetNotifications(int NPINumber)
        {
            try
            {
                //int noteID = 1622602;
               
                List<NotificationsEntity> notifications = new List<NotificationsEntity>();
                notifications = _notificationsRepository.GetNotifications(NPINumber);
                if (notifications.Count < 1)
                {

                    throw new ApiDataException(1001, "No notification found for this id.", HttpStatusCode.NotFound);

                }
                return notifications;
            }
            catch (Exception ex)
            {

                throw new ApiDataException(1001, "No notification found for this id.", HttpStatusCode.NotFound);
                //  throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };

                //throw new Exception("No product found for this id");


            }
        }
    }
   
}
