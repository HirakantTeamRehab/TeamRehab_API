using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.InterfaceRepository;
using Team.Rehab.Repository.Repository;
using Team.Rehab.DataModel;
using Team.Rehab.Repository.UnitOfwork;
using System.Data;
using System.Data.SqlClient;
using Team.Rehab.BusinessEntities;
using System.Net;
using System.IO;
using System.Configuration;
using System.Transactions;

namespace Team.Rehab.Repository.Repository
{
   public class NotificationsRepository : INotificationsRepository
    {
        private readonly IUnitOfwork _unitOfWork;
       
        public NotificationsRepository()
        {
            this._unitOfWork = new UnitOfWork();
            
        }
        public bool UpdateNotification(string NotificationID)
        {
           
             var success = false;
            if (!string.IsNullOrEmpty(NotificationID))
            {
                using (var scope = new TransactionScope())
                {
                 tblAppNotification notificationEntity = new tblAppNotification();
                //  var  notificationEntity = _unitOfWork.NotificationRepo.Get(o => o.Prowid.Equals(NotificationID)).FirstOrDefault(); 
                    if (!string.IsNullOrEmpty(NotificationID))
                    {
                        //product.ProductName = productEntity.ProductName;
                        _unitOfWork.NotificationRepo.Update(notificationEntity);
                        _unitOfWork.Save();
                        scope.Complete();
                         success = true;
                    }
                }
            }
            return success;
        }

        public List<NotificationsEntity> GetNotifications(int NPINumber)
        {
            Team.Rehab.DataModel.RehabEntities rehab = new RehabEntities();
            List<NotificationsEntity> notificationsEntity = new List<NotificationsEntity>();
            notificationsEntity = (from notifications in rehab.tblAppNotifications.Where(a => a.IsViewed == false && a.NPINumber == NPINumber)
                                   join ptint in rehab.tblPatients on notifications.Prowid equals ptint.Prowid

                                   //join refer in rehab.tblReferrers on ptint.ReferralSource equals refer.Rrowid
                                   //join referrer in rehab.tblReferrers on ptint.ReferralSource equals referrer.Rrowid
                                   //where referrer.NPINumber = NPINumber
                                   select new NotificationsEntity
                                   {
                                       NotificationId = notifications.Nrowid,
                                       FirstName = ptint.FirstName,
                                       LastName = ptint.LastName,
                                       AlertMessage = notifications.AlertMsg,
                                       ModifiedDate = notifications.UpdatedTs.ToString()

                                   }
            ).ToList();
            return notificationsEntity;
        }
    }
}
