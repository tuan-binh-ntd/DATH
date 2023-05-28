using Bussiness.Interface.OrderInterface;
using Bussiness.Interface.OrderInterface.Dto;
using Bussiness.Services.Core;
using CoreApiResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
            return CustomResult(res, HttpStatusCode.OK);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ForwardToTheStore(long id, ForwardToTheStoreInput input)
        {
            object res = await _orderAppService.ForwardToTheStore(id, input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            object res = await _orderAppService.GetOrdersForAdmin(input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        //[Authorize(Policy = "RequireEmployeeRole")]
        //[HttpGet("{shopId}")]
        //public async Task<IActionResult> Get(int shopId, [FromQuery] PaginationInput input)
        //{
        //    object res = await _orderAppService.GetOrdersForShop(shopId, input);
        //    return CustomResult(res, HttpStatusCode.OK);
        //}

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            OrderForViewDto? res = await _orderAppService.GetOrder(id);
            if (res is null) return CustomResult(null, HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [AllowAnonymous]
        [HttpGet("by-code/{code}")]
        public async Task<IActionResult> Get(string code)
        {
            OrderForViewDto? res = await _orderAppService.GetOrder(code);
            if (res is null) return CustomResult(null, HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }
    }
}
