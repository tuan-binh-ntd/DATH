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
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
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

            if (input.PageNum != null && input.PageSize != null) return CustomResult(await query.Pagination(input), HttpStatusCode.OK);
            else return CustomResult(await query.ToListAsync(), HttpStatusCode.OK);
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
            if (data == null) return CustomResult(HttpStatusCode.NoContent);

            return CustomResult(data, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SpecificationInput input)
        {
            Specification? specification = new();
            _mapper.Map(input, specification);
            await _specRepo.InsertAsync(specification);

            SpecificationForViewDto? res = new();
            _mapper.Map(specification, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, SpecificationInput input)
        {
            Specification? specification = await _specRepo.GetAsync(id);
            if (specification == null) return CustomResult(HttpStatusCode.NoContent);
            _mapper.Map(input, specification);

            await _specRepo.UpdateAsync(specification!);
            SpecificationForViewDto? res = new();
            _mapper.Map(specification, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _specRepo.DeleteAsync(id);
            return CustomResult(id, HttpStatusCode.OK);
        }
    }
}
