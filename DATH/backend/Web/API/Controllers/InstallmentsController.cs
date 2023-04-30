using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Repository;
using Bussiness.Services;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Controllers
{
    public class InstallmentsController : AdminBaseController
    {
        private readonly IRepository<Installment> _installmentRepo;
        private readonly IMapper _mapper;

        public InstallmentsController(
            IRepository<Installment> installmentRepo,
            IMapper mapper
            )
        {
            _installmentRepo = installmentRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            IQueryable<InstallmentForViewDto> query = from i in _installmentRepo.GetAll().AsNoTracking()
                                                    select new InstallmentForViewDto()
                                                    {
                                                        Id = i.Id,
                                                        Balance = i.Balance,
                                                        Term = i.Term
                                                    };

            if (input.PageNum != null && input.PageSize != null) return CustomResult(await query.Pagination(input), HttpStatusCode.OK);
            else return CustomResult(await query.ToListAsync(), HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            IQueryable<InstallmentForViewDto> query = from i in _installmentRepo.GetAll().AsNoTracking()
                                                      where i.Id == id
                                                      select new InstallmentForViewDto()
                                                      {
                                                          Id = i.Id,
                                                          Balance = i.Balance,
                                                          Term = i.Term
                                                      };
            InstallmentForViewDto? data = await query.FirstOrDefaultAsync();
            if (data == null) return CustomResult(null, HttpStatusCode.NoContent);

            return CustomResult(data, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(InstallmentInput input)
        {
            Installment? data = new();
            _mapper.Map(input, data);
            await _installmentRepo.InsertAsync(data);

            InstallmentForViewDto? res = new();
            _mapper.Map(data, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, InstallmentInput input)
        {
            Installment? data = await _installmentRepo.GetAsync(id);
            if (data == null) return CustomResult(HttpStatusCode.NoContent);
            _mapper.Map(input, data);

            await _installmentRepo.UpdateAsync(data);
            InstallmentForViewDto? res = new();
            _mapper.Map(data, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _installmentRepo.DeleteAsync(id);
            return CustomResult(id, HttpStatusCode.OK);
        }
    }
}
