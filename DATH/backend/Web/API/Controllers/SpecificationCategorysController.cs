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
            List<SpecificationCategory>? specificationCategories = await _specCateRepo.GetAll().AsNoTracking().ToListAsync();

            if (specificationCategories == null) return CustomResult(HttpStatusCode.NotFound);

            List<SpecificationCategoryForViewDto> res = new();

            _mapper.Map(specificationCategories, res);

            return CustomResult(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            SpecificationCategory? specificationCategory = await _specCateRepo.GetAsync(id);

            if (specificationCategory == null) return CustomResult(HttpStatusCode.NotFound);

            SpecificationCategoryForViewDto res = new();

            _mapper.Map(specificationCategory, res);

            return CustomResult(res);
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
