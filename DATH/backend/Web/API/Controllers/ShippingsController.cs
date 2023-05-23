using Bussiness.Dto;
using Bussiness.Interface.ShippingInterface;
using Bussiness.Services.Core;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    public class ShippingsController : AdminBaseController
    {
        private readonly IShippingAppService _shippingAppService;

        public ShippingsController(
            IShippingAppService shippingAppService
            )
        {
            _shippingAppService = shippingAppService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            object res = await _shippingAppService.GetShippings(input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            ShippingForViewDto? res = await _shippingAppService.GetShipping(id);
            if (res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShippingInput input)
        {
            ShippingForViewDto? res = await _shippingAppService.CreateOrUpdate(null, input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ShippingInput input)
        {
            ShippingForViewDto? res = await _shippingAppService.CreateOrUpdate(id, input);
            if (res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _shippingAppService.Delete(id);
            return CustomResult(id, HttpStatusCode.OK);
        }
    }
}
