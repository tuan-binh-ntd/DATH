using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface.ShopInterface;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopsController : AdminBaseController
    {
        private readonly IShopAppService _shopAppService;

        public ShopsController(
            IShopAppService shopAppService
            )
        {
            _shopAppService = shopAppService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            object res = await _shopAppService.GetShops(input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            ShopForViewDto? res = await _shopAppService.GetShop(id);
            if (res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShopInput input)
        {
            ShopForViewDto? res = await _shopAppService.CreateOrUpdate(null, input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ShopInput input)
        {
            ShopForViewDto? res = await _shopAppService.CreateOrUpdate(null, input);
            if (res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _shopAppService.Delete(id);
            return CustomResult(id, HttpStatusCode.OK);
        }

        [HttpGet("{id}/warehouse")]
        public async Task<IActionResult> GetWarehouse(int id)
        {
            GetWarehouseForViewDto? res = await _shopAppService.GetWarehouse(id);
            if (res is null) return CustomResult("Not found warehouse", HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }
    }
}
