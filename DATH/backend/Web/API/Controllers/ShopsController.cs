using AutoMapper;
using Bussiness.Dto;
using Bussiness.Repository;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopsController : AdminBaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Shop> _shopRepo;

        public ShopsController(
            IMapper mapper,
            IRepository<Shop> shopRepo
            )
        {
            _mapper = mapper;
            _shopRepo = shopRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IQueryable<ShopForViewDto> query = from s in _shopRepo.GetAll().AsNoTracking()
                                               select new ShopForViewDto()
                                               {
                                                   Id = s.Id,
                                                   Name = s.Name,
                                                   Address = s.Address
                                               };
            List<ShopForViewDto>? data = await query.ToListAsync();
            if (data == null) return CustomResult(HttpStatusCode.NotFound);

            return CustomResult(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            IQueryable<ShopForViewDto> query = from s in _shopRepo.GetAll().AsNoTracking()
                                               where s.Id == id
                                               select new ShopForViewDto()
                                               {
                                                   Id = id,
                                                   Name = s.Name,
                                                   Address = s.Address
                                               };
            ShopForViewDto? data = await query.FirstOrDefaultAsync();
            if (data == null) return CustomResult(null, HttpStatusCode.NotFound);

            return CustomResult(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShopInput input)
        {
            Shop data = new();
            _mapper.Map(input, data);

            await _shopRepo.InsertAsync(data);
            return CustomResult(data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ShopInput input)
        {
            Shop? data = await _shopRepo.GetAsync(id);
            if (data == null) return CustomResult(HttpStatusCode.NotFound);
            _mapper.Map(input, data);

            await _shopRepo.UpdateAsync(data);
            return CustomResult(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _shopRepo.DeleteAsync(id);
            return CustomResult(data);
        }
    }
}
