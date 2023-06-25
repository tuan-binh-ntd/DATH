using Bussiness.Dto;
using Bussiness.Interface.CustomerInterface;
using Bussiness.Interface.OrderInterface;
using Bussiness.Interface.OrderInterface.Dto;
using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : BaseController
    {
        private readonly ICustomerAppService _customerAppService;
        private readonly IOrderAppService _orderAppService;

        public CustomersController(
            ICustomerAppService customerAppService,
            IOrderAppService orderAppService
            )
        {
            _customerAppService = customerAppService;
            _orderAppService = orderAppService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            CustomerForViewDto? res = await _customerAppService.GetCustomer(id);
            if(res == null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, CustomerInput input)
        {
            CustomerForViewDto? res = await _customerAppService.Update(id, input);
            if (res == null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPost("{id}/addresses")]
        public async Task<IActionResult> Update(long id, AddressInput input)
        {
            CustomerForViewDto? res = await _customerAppService.CreateAddress(id, input);
            if (res == null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpGet("{id}/orders")]
        public async Task<IActionResult> GetOrderForCustomer(long id)
        {
            IEnumerable<OrderForViewDto> res = await _orderAppService.GetOrdersForCustomer(id);

            return CustomResult(res, HttpStatusCode.OK);
        }
    }
}
