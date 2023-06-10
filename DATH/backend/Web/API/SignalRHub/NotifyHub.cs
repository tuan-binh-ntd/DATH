using Bussiness.Extensions;
using Bussiness.Interface.NotificationInterface;
using Bussiness.Interface.NotificationInterface.Dto;
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

        public override async Task OnConnectedAsync()
        {
            long? userId = Context.User!.GetUserId();
            if ( userId is not null ) 
            {
                // Return notification list
                IEnumerable<NotificationForViewDto> notifications = await _notificationAppService.Get((long)userId);
                await Clients.Caller.SendAsync("NotificationList", notifications);

                // Return unread notification number
                int unreadNotificationNum = await _notificationAppService.UnreadNotificationNum((long)userId);
                await Clients.Caller.SendAsync("UnreadNotificationNum", unreadNotificationNum);
            }
        }

        public async Task CreateNotification(NotificationInput input)
        {
            NotificationForViewDto res = await _notificationAppService.CreateOrUpdate(null, input);
            await Clients.User(input.UserId.ToString()).SendAsync("CreateNotification", res);
        }

        public async Task ReadNotification(long id, NotificationInput input)
        {
            NotificationForViewDto res = await _notificationAppService.CreateOrUpdate(id, input);
            await Clients.Caller.SendAsync("ReadNotification", res);

            // Return unread notification number
            int unreadNotificationNum = await _notificationAppService.UnreadNotificationNum(input.UserId);
            await Clients.Caller.SendAsync("UnreadNotificationNum", unreadNotificationNum);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {

            await base.OnDisconnectedAsync(exception);
        }
    }
}
