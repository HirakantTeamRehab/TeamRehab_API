using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.BusinessEntities;
using Team.Rehab.DataModel;

namespace Team.Rehab.InterfaceRepository
{
    public interface INotificationsRepository
    {
        bool UpdateNotification(string NotificationID);
        List<NotificationsEntity> GetNotifications(int NPINumber);
    }
}
