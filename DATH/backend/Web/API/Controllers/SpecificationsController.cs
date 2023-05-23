using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface.SpecificationInterface;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Controllers
{
    public class SpecificationsController : AdminBaseController
    {
        private readonly ISpecificationAppService _specificationAppService;

        public SpecificationsController(
            ISpecificationAppService specificationAppService
            )
        {
            _specificationAppService = specificationAppService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            object res = await _specificationAppService.GetSpecifications(input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            SpecificationForViewDto? res = await _specificationAppService.GetSpecification(id);
            if(res == null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SpecificationInput input)
        {
            SpecificationForViewDto? res = await _specificationAppService.CreateOrUpdate(null, input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, SpecificationInput input)
        {
            SpecificationForViewDto? res = await _specificationAppService.CreateOrUpdate(id, input);
            if (res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _specificationAppService.Delete(id);
            return CustomResult(id, HttpStatusCode.OK);
        }
    }
}
