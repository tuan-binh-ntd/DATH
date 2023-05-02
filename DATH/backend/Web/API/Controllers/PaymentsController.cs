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
    public class PaymentsController : AdminBaseController
    {
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IMapper _mapper;

        public PaymentsController(
            IRepository<Payment> paymentRepo,
            IMapper mapper
            )
        {
            _paymentRepo = paymentRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            IQueryable<PaymentForViewDto> query = from p in _paymentRepo.GetAll().AsNoTracking()
                                                        select new PaymentForViewDto()
                                                        {
                                                            Id = p.Id,
                                                            Name = p.Name,
                                                        };

            if (input.PageNum != null && input.PageSize != null) return CustomResult(await query.Pagination(input), HttpStatusCode.OK);
            else return CustomResult(await query.ToListAsync(), HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            IQueryable<PaymentForViewDto> query = from p in _paymentRepo.GetAll().AsNoTracking()
                                                        where p.Id == id
                                                        select new PaymentForViewDto()
                                                        {
                                                            Id = p.Id,
                                                            Name = p.Name,
                                                        };
            PaymentForViewDto? data = await query.SingleOrDefaultAsync();
            if (data == null) return CustomResult(HttpStatusCode.NoContent);

            return CustomResult(data, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PaymentInput input)
        {
            Payment? data = new();
            _mapper.Map(input, data);
            await _paymentRepo.InsertAsync(data);

            PaymentForViewDto? res = new();
            _mapper.Map(data, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PaymentInput input)
        {
            Payment? payment = await _paymentRepo.GetAsync(id);
            if (payment == null) return CustomResult(HttpStatusCode.NoContent);
            _mapper.Map(input, payment);

            await _paymentRepo.UpdateAsync(payment);
            PaymentForViewDto? res = new();
            _mapper.Map(payment, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _paymentRepo.DeleteAsync(id);
            return CustomResult(id, HttpStatusCode.OK);
        }
    }
}
