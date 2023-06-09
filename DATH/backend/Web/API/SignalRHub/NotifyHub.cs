using Bussiness.Interface.NotificationInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalRHub
{
    [Authorize]
    public class NotifyHub : Hub
    {
        private readonly INotificationAppService _notificationAppService;

        public NotifyHub(
            INotificationAppService notificationAppService
            )
        {
            _notificationAppService = notificationAppService;
        }


    }
}
