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
    [Route("api/[controller]")]
    [ApiController]
    public class ShopsController : AdminBaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Shop> _shopRepo;
        private readonly IRepository<Warehouse> _warehouseRepo;

        public ShopsController(
            IMapper mapper,
            IRepository<Shop> shopRepo,
            IRepository<Warehouse> warehouseRepo
            )
        {
            _mapper = mapper;
            _shopRepo = shopRepo;
            _warehouseRepo = warehouseRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            IQueryable<ShopForViewDto> query = from s in _shopRepo.GetAll().AsNoTracking()
                                               select new ShopForViewDto()
                                               {
                                                   Id = s.Id,
                                                   Name = s.Name,
                                                   Address = s.Address
                                               };

            if (input.PageNum != null && input.PageSize != null) return CustomResult(await query.Pagination(input), HttpStatusCode.OK);
            else return CustomResult(await query.ToListAsync(), HttpStatusCode.OK);
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
            if (data == null) return CustomResult(null, HttpStatusCode.NoContent);

            return CustomResult(data, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShopInput input)
        {
            Shop data = new();
            _mapper.Map(input, data);

            await _shopRepo.InsertAsync(data);
            ShopForViewDto? res = new();
            _mapper.Map(data, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ShopInput input)
        {
            Shop? data = await _shopRepo.GetAsync(id);
            if (data == null) return CustomResult(HttpStatusCode.NoContent);
            _mapper.Map(input, data);

            await _shopRepo.UpdateAsync(data);
            ShopForViewDto? res = new();
            _mapper.Map(data, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _shopRepo.DeleteAsync(id);
            return CustomResult(id, HttpStatusCode.OK);
        }

        [HttpGet("{id}/warehouse")]
        public async Task<IActionResult> GetWarehouse(int id)
        {
            IQueryable<GetWarehouseForViewDto> query = from s in _shopRepo.GetAll().AsNoTracking()
                                                       join w in _warehouseRepo.GetAll().AsNoTracking() on s.Id equals w.ShopId
                                                       where s.Id == id
                                                       select new GetWarehouseForViewDto
                                                       {
                                                           Id = s.Id,
                                                           Name = s.Name,
                                                           Address = s.Address,
                                                           WarehouseId = w.Id,
                                                           WarehouseName = w.Name,
                                                       };
            GetWarehouseForViewDto? data = await query.SingleOrDefaultAsync();
            if(data == null) return CustomResult("Not found warehouse" ,HttpStatusCode.NoContent);
            return CustomResult(data, HttpStatusCode.OK);
        }
    }
}
