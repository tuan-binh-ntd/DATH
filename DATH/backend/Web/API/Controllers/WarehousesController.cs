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
        private readonly IMapper _mapper;

        public WarehousesController(
            IRepository<Warehouse> warehouseRepo,
            IMapper mapper
            )
        {
            _warehouseRepo = warehouseRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            IQueryable<WarehouseForViewDto> query = from s in _warehouseRepo.GetAll().AsNoTracking()
                                                   select new WarehouseForViewDto()
                                                   {
                                                       Id = s.Id,
                                                       Name = s.Name
                                                   };

            if (input.PageNum != null && input.PageSize != null) return CustomResult(await query.Pagination(input), HttpStatusCode.OK);
            else return CustomResult(await query.ToListAsync(), HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            IQueryable<WarehouseForViewDto> query = from s in _warehouseRepo.GetAll().AsNoTracking()
                                                   where s.Id == id
                                                   select new WarehouseForViewDto()
                                                   {
                                                       Id = id,
                                                       Name = s.Name
                                                   };
            WarehouseForViewDto? data = await query.FirstOrDefaultAsync();
            if (data == null) return CustomResult(null, HttpStatusCode.NoContent);

            return CustomResult(data, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(WarehouseInput input)
        {
            Warehouse? warehouse = await _warehouseRepo.GetAll().AsNoTracking().Where(w => w.ShopId == input.ShopId).FirstOrDefaultAsync();
            if (warehouse != null) return CustomResult("Shop had warehouse", HttpStatusCode.BadRequest);
            Warehouse data = new();
            _mapper.Map(input, data);

            await _warehouseRepo.InsertAsync(data);
            WarehouseForViewDto? res = new();
            _mapper.Map(data, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, WarehouseInput input)
        {
            Warehouse? data = await _warehouseRepo.GetAsync(id);
            if (data == null) return CustomResult(HttpStatusCode.NoContent);
            _mapper.Map(input, data);

            await _warehouseRepo.UpdateAsync(data);
            ShippingForViewDto? res = new();
            _mapper.Map(data, res);
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
