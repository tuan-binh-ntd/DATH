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
            IQueryable<SpecificationForViewDto> query = from s in _specRepo.GetAll().AsNoTracking()
                                                        select new SpecificationForViewDto()
                                                        {
                                                            Id = s.Id,
                                                            Code = s.Code,
                                                            Value = s.Value,
                                                            Description = s.Description,
                                                            SpecificationCategoryId = s.SpecificationCategoryId,
                                                        };
            List<SpecificationForViewDto>? data = await query.ToListAsync();
            if (data == null) return CustomResult(HttpStatusCode.NotFound);

            return CustomResult(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            IQueryable<SpecificationForViewDto> query = from s in _specRepo.GetAll().AsNoTracking()
                                                        where s.Id == id
                                                        select new SpecificationForViewDto()
                                                        {
                                                            Id = s.Id,
                                                            Code = s.Code,
                                                            Value = s.Value,
                                                            Description = s.Description,
                                                            SpecificationCategoryId = s.SpecificationCategoryId,
                                                        };
            List<SpecificationForViewDto>? data = await query.ToListAsync();
            if (data == null) return CustomResult(HttpStatusCode.NotFound);

            return CustomResult(data);
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
