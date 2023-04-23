using AutoMapper;
using Bussiness.Dto;
using Bussiness.Repository;
using CoreApiResponse;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Customer, long> _customerRepo;

        public CustomersController(
            IMapper mapper,
            IRepository<Customer, long> customerRepo
            )
        {
            _mapper = mapper;
            _customerRepo = customerRepo;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            Customer? customer = await _customerRepo.GetAsync(id);
            if (customer == null) return CustomResult(HttpStatusCode.NoContent);
            CustomerForViewDto res = new();
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, CustomerInput input)
        {
            Customer? customer = await _customerRepo.GetAsync(id);
            if (customer == null) return CustomResult(HttpStatusCode.NoContent);
            _mapper.Map(input, customer);
            await _customerRepo.UpdateAsync(customer!);
            CustomerForViewDto? res = new();
            _mapper.Map(customer, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPost("{id}/addresses")]
        public async Task<IActionResult> Update(long id, AddressInput input)
        {
            Customer? customer = await _customerRepo.GetAsync(id);
            if (customer == null) return CustomResult(HttpStatusCode.NoContent);

            if (input.Addresses!.Count != 0) customer!.Address = string.Join(",", input.Addresses!.ToList());
            await _customerRepo.UpdateAsync(customer!);
            CustomerForViewDto? res = new();
            _mapper.Map(customer, res);
            return CustomResult(res, HttpStatusCode.OK);
        }
    }
}
