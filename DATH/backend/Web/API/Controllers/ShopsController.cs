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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            IQueryable<ShopForViewDto> query = from s in _shopRepo.GetAll().AsNoTracking()
                                               where id == null || s.Id == id
                                               select new ShopForViewDto()
                                               {
                                                   Id = id,
                                                   Name = s.Name,
                                                   Address = s.Address
                                               };

            return CustomResult(await query.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShopDto input)
        {
            Shop data = new();
            _mapper.Map(input, data);

            await _shopRepo.InsertAsync(data);
            return CustomResult(data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(ShopDto input)
        {
            Shop? data = await _shopRepo.GetAsync((int)input.Id!);
            if (data == null)
            {
                return CustomResult("Failed", HttpStatusCode.InternalServerError);
            }
            _mapper.Map(input, data);

            await _shopRepo.UpdateAsync(data);
            return CustomResult(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Shop? data = await _shopRepo.GetAsync(id);
            await _shopRepo.DeleteAsync(id);
            return CustomResult(data);
        }
    }
}
