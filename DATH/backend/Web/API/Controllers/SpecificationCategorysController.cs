using Bussiness.Dto;
using Bussiness.Interface.SpecificationCategoryInterface;
using Bussiness.Services.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    public class SpecificationCategorysController : AdminBaseController
    {
        private readonly ISpecificationCategoryAppService _specificationCategoryAppService;

        public SpecificationCategorysController(
            ISpecificationCategoryAppService specificationCategoryAppService
            )
        {
            _specificationCategoryAppService = specificationCategoryAppService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            object res = await _specificationCategoryAppService.GetSpecificationCategorys(input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            SpecificationCategoryForViewDto? res = await _specificationCategoryAppService.GetSpecificationCategory(id);
            if(res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SpecificationCategoryInput input)
        {
            SpecificationCategoryForViewDto? res = await _specificationCategoryAppService.CreateOrUpdate(null, input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SpecificationCategoryInput input)
        {
            SpecificationCategoryForViewDto? res = await _specificationCategoryAppService.CreateOrUpdate(id, input);
            if (res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _specificationCategoryAppService.Delete(id);
            return CustomResult(id, HttpStatusCode.OK);
        }

        [AllowAnonymous]
        [HttpGet("{code}/specifications")]
        public async Task<IActionResult> GetColorByCode(string code)
        {
            ICollection<GetColorByCodeForViewDto>? res = await _specificationCategoryAppService.GetColorByCode(code);
            if (res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }
    }
}
