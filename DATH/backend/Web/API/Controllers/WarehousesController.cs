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
    public class WarehousesController : AdminBaseController
    {
        private readonly IRepository<Warehouse> _warehouseRepo;
        private readonly IRepository<Shop> _shopRepo;
        private readonly IMapper _mapper;

        public WarehousesController(
            IRepository<Warehouse> warehouseRepo,
            IRepository<Shop> shopRepo,
            IMapper mapper
            )
        {
            _warehouseRepo = warehouseRepo;
            _shopRepo = shopRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            IQueryable<WarehouseForViewDto> query = from w in _warehouseRepo.GetAll().AsNoTracking()
                                                    join s in _shopRepo.GetAll().AsNoTracking() on w.ShopId equals s.Id
                                                    select new WarehouseForViewDto()
                                                    {
                                                        Id = w.Id,
                                                        Name = w.Name,
                                                        ShopName = s.Name,
                                                        ShopId = w.ShopId,
                                                    };

            if (input.PageNum != null && input.PageSize != null) return CustomResult(await query.Pagination(input), HttpStatusCode.OK);
            else return CustomResult(await query.ToListAsync(), HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            IQueryable<WarehouseForViewDto> query = from w in _warehouseRepo.GetAll().AsNoTracking()
                                                    join s in _shopRepo.GetAll().AsNoTracking() on w.ShopId equals s.Id
                                                    where w.Id == id
                                                    select new WarehouseForViewDto()
                                                    {
                                                        Id = w.Id,
                                                        Name = w.Name,
                                                        ShopName = s.Name,
                                                        ShopId = w.ShopId,
                                                    };
            WarehouseForViewDto? data = await query.SingleOrDefaultAsync();
            if (data == null) return CustomResult(null, HttpStatusCode.NoContent);

            return CustomResult(data, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(WarehouseInput input)
        {
            Shop? shop = await _shopRepo.GetAsync((int)input.ShopId!);

            Warehouse? warehouse = await _warehouseRepo.GetAll().AsNoTracking().Where(w => w.ShopId == input.ShopId).FirstOrDefaultAsync();
            if (warehouse != null) return CustomResult("Shop had warehouse", HttpStatusCode.BadRequest);

            Warehouse? parentWarehouse = await _warehouseRepo.GetAll().AsNoTracking().Where(w => w.ParentId == null && w.ShopId == null).SingleOrDefaultAsync();
            if (parentWarehouse == null) return CustomResult("No Parent Warehouse", HttpStatusCode.NoContent);
            Warehouse data = new()
            {
                ParentId = parentWarehouse!.Id
            };
            _mapper.Map(input, data);

            await _warehouseRepo.InsertAsync(data);
            WarehouseForViewDto? res = new();
            _mapper.Map(data, res);
            res.ShopName = shop!.Name;
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, WarehouseInput input)
        {
            Shop? shop = await _shopRepo.GetAsync((int)input.ShopId!);

            Warehouse? warehouse = await _warehouseRepo.GetAll().AsNoTracking().Where(w => w.ShopId == input.ShopId).FirstOrDefaultAsync();
            if (warehouse != null) return CustomResult("Shop had warehouse", HttpStatusCode.BadRequest);

            Warehouse? data = await _warehouseRepo.GetAsync(id);
            if (data == null) return CustomResult(HttpStatusCode.NoContent);

            _mapper.Map(input, data);

            await _warehouseRepo.UpdateAsync(data);
            WarehouseForViewDto? res = new();
            _mapper.Map(data, res);
            res.ShopName = shop!.Name;
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _warehouseRepo.DeleteAsync(id);
            return CustomResult(id, HttpStatusCode.OK);
        }
    }
}
