using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.BackgroundServices.Interfaces
{
    public interface IHolidayEventBackgroundService
    {
        Task CheckAndSendNotificationsForUpcomingHolidayEvents();
    }

}
