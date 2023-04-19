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
    public class ShippingsController : AdminBaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Shipping> _shippingRepo;

        public ShippingsController(
            IMapper mapper,
            IRepository<Shipping> shippingRepo
            )
        {
            _mapper = mapper;
            _shippingRepo = shippingRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            IQueryable<ShippingForViewDto> query = from s in _shippingRepo.GetAll().AsNoTracking()
                                               select new ShippingForViewDto()
                                               {
                                                   Id = s.Id,
                                                   Name = s.Name,
                                                   Cost = s.Cost,
                                               };

            PaginationResult<ShippingForViewDto> data = await query.Pagination(input);

            if (data == null) return CustomResult(HttpStatusCode.NoContent);

            return CustomResult(data, HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            IQueryable<ShippingForViewDto> query = from s in _shippingRepo.GetAll().AsNoTracking()
                                               where s.Id == id
                                               select new ShippingForViewDto()
                                               {
                                                   Id = id,
                                                   Name = s.Name,
                                                   Cost = s.Cost
                                               };
            ShippingForViewDto? data = await query.FirstOrDefaultAsync();
            if (data == null) return CustomResult(null, HttpStatusCode.NoContent);

            return CustomResult(data, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShippingInput input)
        {
            Shipping data = new();
            _mapper.Map(input, data);

            await _shippingRepo.InsertAsync(data);
            ShippingForViewDto? res = new();
            _mapper.Map(data, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ShippingInput input)
        {
            Shipping? data = await _shippingRepo.GetAsync(id);
            if (data == null) return CustomResult(HttpStatusCode.NoContent);
            _mapper.Map(input, data);

            await _shippingRepo.UpdateAsync(data);
            ShippingForViewDto? res = new();
            _mapper.Map(data, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _shippingRepo.DeleteAsync(id);
            return CustomResult(id, HttpStatusCode.OK);
        }
    }
}
