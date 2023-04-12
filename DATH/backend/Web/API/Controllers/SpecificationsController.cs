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
    public class SpecificationsController : AdminBaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Specification, long> _specRepo;

        public SpecificationsController(
            IMapper mapper,
            IRepository<Specification, long> specRepo
            )
        {
            _mapper = mapper;
            _specRepo = specRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<Specification>? specifications = await _specRepo.GetAll().AsNoTracking().ToListAsync();

            if (specifications == null) return CustomResult(HttpStatusCode.NotFound);
            List<SpecificationCategoryForViewDto> res = new();

            _mapper.Map(specifications, res);

            return CustomResult(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            Specification? specification = await _specRepo.GetAsync(id);

            if (specification == null) return CustomResult(HttpStatusCode.NotFound);
            SpecificationCategoryForViewDto res = new();

            _mapper.Map(specification, res);

            return CustomResult(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SpecificationInput input)
        {
            Specification? specification = new();
            specification.CreatorUserId = User.GetUserId();
            _mapper.Map(input, specification);

            await _specRepo.InsertAsync(specification);

            return CustomResult(HttpStatusCode.Created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, SpecificationInput input)
        {
            Specification? specification = await _specRepo.GetAsync(id);
            if (specification == null) return CustomResult(HttpStatusCode.NotFound);
            specification.LastModifierUserId = User.GetUserId();
            _mapper.Map(input, specification);
           
            await _specRepo.UpdateAsync(specification!);

            return CustomResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            Specification? specification = await _specRepo.GetAsync(id);
            specification!.DeleteUserId = User.GetUserId();
            await _specRepo.DeleteAsync(id);

            return CustomResult();
        }
    }
}
