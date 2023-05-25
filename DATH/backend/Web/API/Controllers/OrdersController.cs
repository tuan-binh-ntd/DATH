using Bussiness.Interface.OrderInterface;
using Bussiness.Interface.OrderInterface.Dto;
using CoreApiResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : BaseController
    {
        private readonly IOrderAppService _orderAppService;

        public OrdersController(
            IOrderAppService orderAppService
            )
        {
            _orderAppService = orderAppService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(OrderInput input)
        {
            object res = await _orderAppService.CreateOrder(input);
            return CustomResult(res, System.Net.HttpStatusCode.OK);
        }
    }
}
