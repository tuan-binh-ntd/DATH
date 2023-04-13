using AutoMapper;
using Bussiness.Dto;
using Bussiness.Extensions;
using Bussiness.Repository;
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
        public async Task<IActionResult> Get()
        {
            IQueryable<SpecificationCategoryForViewDto> query = from s in _specCateRepo.GetAll().AsNoTracking()
                                                                select new SpecificationCategoryForViewDto()
                                                                {
                                                                    Id = s.Id,
                                                                    Code = s.Code,
                                                                    Value = s.Value
                                                                };
            List<SpecificationCategoryForViewDto>? data = await query.ToListAsync();
            if (data == null) return CustomResult(HttpStatusCode.NotFound);

            return CustomResult(data);
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
            if (data == null) return CustomResult(HttpStatusCode.NotFound);

            return CustomResult(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SpecificationCategoryInput input)
        {
            SpecificationCategory specificationCategory = new();
            specificationCategory.CreatorUserId = User.GetUserId();
            _mapper.Map(input, specificationCategory);
            await _specCateRepo.InsertAsync(specificationCategory);
            return CustomResult(HttpStatusCode.Created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SpecificationCategory input)
        {
            SpecificationCategory? specificationCategory = await _specCateRepo.GetAsync(id);
            specificationCategory!.LastModifierUserId = User.GetUserId();
            if (specificationCategory == null) return CustomResult(HttpStatusCode.NotFound);
            _mapper.Map(input, specificationCategory);
            await _specCateRepo.UpdateAsync(specificationCategory!);
            return CustomResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            SpecificationCategory? specificationCategory = await _specCateRepo.GetAsync(id);
            specificationCategory!.DeleteUserId = User.GetUserId();
            await _specCateRepo.DeleteAsync(id);
            return CustomResult();
        }
    }
}
