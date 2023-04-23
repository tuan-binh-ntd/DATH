using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalRHub
{
    [Authorize]
    public class NotifyHub : Hub
    {
    }
}
