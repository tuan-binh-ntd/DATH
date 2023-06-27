using Bussiness.Dto;
using Bussiness.Interface.OrderInterface;
using Bussiness.Services.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net;

namespace API.SignalRHub
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        private readonly IOrderAppService _orderAppService;

        public PresenceHub(
            PresenceTracker tracker,
            IOrderAppService orderAppService
            )
        {
            _tracker = tracker;
            _orderAppService = orderAppService;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext!.Request.Query["userId"].ToString();

            var isOnline = await _tracker.UserConnected(Context!.User!.Identity!.Name!, Context.ConnectionId);

            if (isOnline)
            {
                await Clients.Others.SendAsync("UserIsOnline", Context!.User!.Identity!.Name);
            }

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {

            var isOffline = await _tracker.UserDisconnected(Context!.User!.Identity!.Name!, Context.ConnectionId);
            if (isOffline)
            {
                await Clients.Others.SendAsync("UserIsOffline", Context!.User!.Identity!.Name);
            }

            await base.OnDisconnectedAsync(exception);
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        public async Task<object> GetOrderForAdmin(PaginationInput input)
        {
            object res = await _orderAppService.GetOrdersForAdmin(input);

            await Clients.Caller.SendAsync("GetOrderForAdmin", res);

            return new { StatusCode = HttpStatusCode.OK, Data = res };
        }

        public async Task<object> GetOrderForShop(GetOrderForShopInput input)
        {
            object res = await _orderAppService.GetOrdersForShop(input.ShopId, input);

            await Clients.Caller.SendAsync("GetOrderForShop", res);

            return new { StatusCode = HttpStatusCode.OK, Data = res };
        }
    }
}
