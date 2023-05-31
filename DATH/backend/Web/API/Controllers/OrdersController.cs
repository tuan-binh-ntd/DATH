using Bussiness.Interface.OrderInterface;
using Bussiness.Interface.OrderInterface.Dto;
using Bussiness.Interface.WarehouseInterface;
using Bussiness.Interface.WarehouseInterface.Dto;
using Bussiness.Services.Core;
using CoreApiResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : BaseController
    {
        private readonly IOrderAppService _orderAppService;
        private readonly IWarehouseAppService _warehouseAppService;

        public OrdersController(
            IOrderAppService orderAppService,
            IWarehouseAppService warehouseAppService
            )
        {
            _orderAppService = orderAppService;
            _warehouseAppService = warehouseAppService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(OrderInput input)
        {
            object res = await _orderAppService.CreateOrder(input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [Authorize(Policy = "RequireEmployeeRole")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> ForwardToTheStore(long id, UpdateOrderInput input)
        {
            if(input.ShopId is not null)
            {
                OrderForViewDto res = await _orderAppService.ForwardToTheStore(id, input);
                return CustomResult(res, HttpStatusCode.OK);
            }
            else
            {
                OrderForViewDto res = await _orderAppService.UpdateOrder(id, input);
                return CustomResult(res, HttpStatusCode.OK);
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            object res = await _orderAppService.GetOrdersForAdmin(input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            OrderForViewDto? res = await _orderAppService.GetOrder(id);
            if (res is null) return CustomResult(null, HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("{id}/warehouse")]
        public async Task<IActionResult> ExportToOrder(ExportToOrderInput input)
        {
            object res = await _warehouseAppService.ExportToOrder(input);
            if (res is string)
            {
                return CustomResult(res, HttpStatusCode.BadRequest);
            }
            else
            {
                return CustomResult(res, HttpStatusCode.OK);
            }
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
