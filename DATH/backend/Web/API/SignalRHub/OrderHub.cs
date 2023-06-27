using Bussiness.Dto;
using Bussiness.Interface.EmployeeInterface;
using Bussiness.Interface.NotificationInterface;
using Bussiness.Interface.NotificationInterface.Dto;
using Bussiness.Interface.OrderInterface;
using Bussiness.Interface.OrderInterface.Dto;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Net;

namespace API.SignalRHub
{
    public class OrderHub : Hub
    {
        private readonly IOrderAppService _orderAppService;
        private readonly UserManager<AppUser> _userManager;
        private readonly INotificationAppService _notificationAppService;
        private readonly IHubContext<NotifyHub> _notifyHub;
        private readonly IEmployeeAppService _employeeAppService;

        public OrderHub(
            IOrderAppService orderAppService,
            UserManager<AppUser> userManager,
            INotificationAppService notificationAppService,
            IHubContext<NotifyHub> notifyHub,
            IEmployeeAppService employeeAppService
            )
        {
            _orderAppService = orderAppService;
            _userManager = userManager;
            _notificationAppService = notificationAppService;
            _notifyHub = notifyHub;
            _employeeAppService = employeeAppService;
        }

        public async Task<object> CreateOrder(OrderInput input)
        {
            OrderForViewDto? res = await _orderAppService.CreateOrder(input);

            AppUser? admin = await _userManager.FindByNameAsync("admin");

            await Clients.User(admin!.Id.ToString()).SendAsync("NewOrder", res);

            NotificationInput notificationInput = new()
            {
                Content = $"New Order: {res.Code}",
                IsRead = false,
                UserId = admin.Id,
            };

            await CreateNotification(notificationInput);

            return new { StatusCode = HttpStatusCode.OK, Data = res };
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task CreateNotification(NotificationInput input)
        {
            NotificationForViewDto res = await _notificationAppService.CreateOrUpdate(null, input);

            await _notifyHub.Clients.User(input.UserId.ToString()).SendAsync("NewNotification", res);
        }

        public async Task ForwardOrderForStores(long id, UpdateOrderInput input)
        {
            OrderForViewDto? res = await _orderAppService.ForwardToTheStore(id, input);

            EmployeeForViewDto? orderEmployee = await _employeeAppService.GetOrderEmployeeByShop((int)input.ShopId!);

            if (orderEmployee is null)
            {
                throw new HubException($"{orderEmployee!.ShopName} not have order employee. Please add order employee for the shop");
            }

            NotificationInput notificationInput = new()
            {
                Content = $"New Order: {res.Code} for shop",
                IsRead = false,
                UserId = orderEmployee.Id,
            };

            await CreateNotification(notificationInput);
        }
    }
}
