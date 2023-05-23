using Bussiness.Dto;
using Bussiness.Interface.WarehouseInterface;
using Bussiness.Services.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    public class WarehousesController : AdminBaseController
    {
        private readonly IWarehouseAppService _warehouseAppService;

        public WarehousesController(
            IWarehouseAppService warehouseAppService
            )
        {
            _warehouseAppService = warehouseAppService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            object res = await _warehouseAppService.GetWarehouses(input);
            return CustomResult(res, HttpStatusCode.OK);    
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            WarehouseForViewDto? res = await _warehouseAppService.GetWarehouse(id);
            if(res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(WarehouseInput input)
        {
            object? res = await _warehouseAppService.CreateOrUpdate(null, input);
            if(res is string) return CustomResult(res.ToString(), HttpStatusCode.BadRequest);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, WarehouseInput input)
        {
            object? res = await _warehouseAppService.CreateOrUpdate(id, input);
            if (res is string) return CustomResult(res.ToString(), HttpStatusCode.BadRequest);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _warehouseAppService.Delete(id);
            return CustomResult(id, HttpStatusCode.OK);
        }

        [HttpPost("{id}/products")]
        public async Task<IActionResult> AddProductToParentWarehouse(int id, AddProductToWarehouseInput input)
        {
            object res = await _warehouseAppService.AddProductToParentWarehouse(id, input);
            if (res is string) return CustomResult(res.ToString(), HttpStatusCode.BadRequest);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [AllowAnonymous]
        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetProductsForWarehouse(int id, [FromQuery] PaginationInput input)
        {
            object res = await _warehouseAppService.GetProductsForWarehouse(id, input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        
    }
}
