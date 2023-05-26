using Bussiness.Interface.OrderInterface;
using Bussiness.Interface.OrderInterface.Dto;
using Bussiness.Services.Core;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalRHub
{
    public class OrderHub : Hub
    {
        private readonly IOrderAppService _orderAppService;
        private readonly UserManager<AppUser> _userManager;

        public OrderHub(
            IOrderAppService orderAppService,
            UserManager<AppUser> userManager
            )
        {
            _orderAppService = orderAppService;
            _userManager = userManager;
        }

        public async Task CreateOrder(OrderInput input)
        {
            OrderForViewDto? res = await _orderAppService.CreateOrder(input);

            AppUser? admin = await _userManager.FindByNameAsync("admin");

            await Clients.User(admin!.Id.ToString()).SendAsync("NewOrder", res);
        }

        public async Task GetOrderForAdmin()
        {
            HttpContext httpContext = Context.GetHttpContext()!;
            var pageNum = httpContext.Request.Query["PageNum"].ToString();
            var pageSize = httpContext.Request.Query["PageSize"].ToString();
            PaginationInput input = new()
            {
                PageNum = int.Parse(pageNum),
                PageSize = int.Parse(pageSize)
            };

            object res = await _orderAppService.GetOrdersForAdmin(input);

            await Clients.Caller.SendAsync("GetOrderForAdmin", res);
        }

        public async Task GetOrderForShop()
        {
            HttpContext httpContext = Context.GetHttpContext()!;
            var pageNum = httpContext.Request.Query["PageNum"].ToString();
            var pageSize = httpContext.Request.Query["PageSize"].ToString();
            var shopId = httpContext.Request.Query["ShopId"].ToString();
            PaginationInput input = new()
            {
                PageNum = int.Parse(pageNum),
                PageSize = int.Parse(pageSize)
            };

            object res = await _orderAppService.GetOrdersForShop(int.Parse(shopId), input);

            await Clients.Caller.SendAsync("GetOrderForShop", res);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
