using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Repository;
using Bussiness.Services;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Controllers
{
    public class SpecificationCategorysController : AdminBaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<SpecificationCategory> _specCateRepo;

        public SpecificationCategorysController(
            IMapper mapper,
            IRepository<SpecificationCategory> specCateRepo
            )
        {
            _mapper = mapper;
            _specCateRepo = specCateRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            IQueryable<SpecificationCategoryForViewDto> query = from s in _specCateRepo.GetAll().AsNoTracking()
                                                                select new SpecificationCategoryForViewDto()
                                                                {
                                                                    Id = s.Id,
                                                                    Code = s.Code,
                                                                    Value = s.Value
                                                                };

            if (input.PageNum != null && input.PageSize != null) return CustomResult(await query.Pagination(input), HttpStatusCode.OK);
            else return CustomResult(await query.ToListAsync(), HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            IQueryable<SpecificationCategoryForViewDto> query = from s in _specCateRepo.GetAll().AsNoTracking()
                                                                where s.Id == id
                                                                select new SpecificationCategoryForViewDto()
                                                                {
                                                                    Id = s.Id,
                                                                    Code = s.Code,
                                                                    Value = s.Value
                                                                };

            List<SpecificationCategoryForViewDto>? data = await query.ToListAsync();
            if (data == null) return CustomResult(HttpStatusCode.NoContent);

            return CustomResult(data, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SpecificationCategoryInput input)
        {
            SpecificationCategory specificationCategory = new();
            _mapper.Map(input, specificationCategory);
            await _specCateRepo.InsertAsync(specificationCategory);
            SpecificationCategoryForViewDto? res = new();
            _mapper.Map(specificationCategory, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SpecificationCategoryInput input)
        {
            SpecificationCategory? specificationCategory = await _specCateRepo.GetAsync(id);
            if (specificationCategory == null) return CustomResult(HttpStatusCode.NoContent);
            specificationCategory = _mapper.Map(input, specificationCategory);
            await _specCateRepo.UpdateAsync(specificationCategory);
            SpecificationCategoryForViewDto? res = new();
            _mapper.Map(specificationCategory, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _specCateRepo.DeleteAsync(id);
            return CustomResult(id, HttpStatusCode.OK);
        }
    }
}
