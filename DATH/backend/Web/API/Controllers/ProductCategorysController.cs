using Bussiness.Dto;
using Bussiness.Interface.ProductCategoryInterface;
using Bussiness.Services.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    public class ProductCategorysController : AdminBaseController
    {
        private readonly IProductCategoryAppService _productCategoryAppService;

        public ProductCategorysController(
            IProductCategoryAppService productCategoryAppService
            )
        {
            _productCategoryAppService = productCategoryAppService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            object res = await _productCategoryAppService.GetProductCategorys(input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            ProductCategoryForViewDto? res = await _productCategoryAppService.GetProductCategory(id);
            if(res == null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategoryInput input)
        {
            ProductCategoryForViewDto? res = await _productCategoryAppService.CreateOrUpdate(null, input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductCategoryInput input)
        {
            ProductCategoryForViewDto? res = await _productCategoryAppService.CreateOrUpdate(id, input);
            if (res == null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productCategoryAppService.Delete(id);
            return CustomResult(id, HttpStatusCode.OK);
        }

        [AllowAnonymous]
        [HttpGet("{id}/specifications")]
        public async Task<IActionResult> GetFilterForProductCategory(int id)
        {
            object res = await _productCategoryAppService.GetFilterForProductCategory(id);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [AllowAnonymous]
        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetProductByCategory(int id, [FromQuery] PaginationInput input, [FromQuery] ProductFilterDto filter)
        {
            object res = await _productCategoryAppService.GetProductByCategory(id, input, filter);
            return CustomResult(res, HttpStatusCode.OK);
        }
    }
}
