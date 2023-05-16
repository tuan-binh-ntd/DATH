using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Repository;
using Bussiness.Services;
using Entities;
using Entities.Enum.Warehouse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace API.Controllers
{
    public class WarehousesController : AdminBaseController
    {
        private readonly IRepository<Warehouse> _warehouseRepo;
        private readonly IRepository<Shop> _shopRepo;
        private readonly IMapper _mapper;
        private readonly IRepository<WarehouseDetail, long> _warehouseDetailRepo;
        private readonly IRepository<Product, long> _productRepo;

        public WarehousesController(
            IRepository<Warehouse> warehouseRepo,
            IRepository<Shop> shopRepo,
            IMapper mapper,
            IRepository<WarehouseDetail, long> warehouseDetailRepo,
            IRepository<Product, long> productRepo
            )
        {
            _warehouseRepo = warehouseRepo;
            _shopRepo = shopRepo;
            _mapper = mapper;
            _warehouseDetailRepo = warehouseDetailRepo;
            _productRepo = productRepo;
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
            if (id != 0)
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

            Warehouse? warehouse = await _warehouseRepo.GetAll().Where(w => w.ShopId == null && w.ParentId == null).SingleOrDefaultAsync();
            if (warehouse == null) return CustomResult(HttpStatusCode.NoContent);
            WarehouseForViewDto res = new();
            _mapper.Map(warehouse, res);

            if (warehouse == null) return CustomResult(null, HttpStatusCode.NoContent);

            return CustomResult(res, HttpStatusCode.OK);
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

            Warehouse? warehouse = await _warehouseRepo.GetAll().AsNoTracking().Where(w => w.Id == id).SingleOrDefaultAsync();
            if (warehouse != null && warehouse.ShopId == input.ShopId) return CustomResult("Shop had warehouse", HttpStatusCode.BadRequest);

            Shop? shop = await _shopRepo.GetAsync((int)input.ShopId!);

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

        [HttpPost("{id}/products")]
        public async Task<IActionResult> AddProductToParentWarehouse(int id, AddProductToWarehouseInput input)
        {
            if (input.Type == EventType.Export)
            {
                WarehouseDetail? data = await (from wd in _warehouseDetailRepo.GetAll().AsNoTracking()
                                               where wd.ProductId == input.ProductId && wd.WarehouseId == id
                                               group wd by wd.ProductId into wad
                                               select new WarehouseDetail
                                               {
                                                   ProductId = wad.Key,
                                                   Quantity = wad.Sum(wad => wad.Quantity)
                                               }).FirstOrDefaultAsync();

                if (data == null)
                {
                    return CustomResult("The product is not in warehouse", HttpStatusCode.BadRequest);
                }
                else if(data!.Quantity < input.Quantity)
                {
                    return CustomResult("Not enough quantity", HttpStatusCode.BadRequest);
                }
            }

            WarehouseDetail warehouseDetail = new()
            {
                WarehouseId = id,
                ActualDate = DateTime.Now,
            };

            _mapper.Map(input, warehouseDetail);

            await _warehouseDetailRepo.InsertAsync(warehouseDetail);

            var query = HandleWarehouse(id);

            return CustomResult(await query.ToListAsync(), HttpStatusCode.OK);
        }

        [AllowAnonymous]
        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetProductsForWarehouse(int id, [FromQuery] PaginationInput input)
        {
            var query = HandleWarehouse(id);
            if (input.PageNum != null && input.PageSize != null) return CustomResult(await query.Pagination(input), HttpStatusCode.OK);
            else return CustomResult(await query.ToListAsync(), HttpStatusCode.OK);
        }

        private IQueryable<AddProductToParentWarehouseForViewDto> HandleWarehouse(int id)
        {
            IQueryable<AddProductToParentWarehouseForViewDto> query = from w in _warehouseRepo.GetAll().AsNoTracking()
                                                                      join wd in _warehouseDetailRepo.GetAll().AsNoTracking() on w.Id equals wd.WarehouseId
                                                                      join p in _productRepo.GetAll().AsNoTracking() on wd.ProductId equals p.Id
                                                                      where w.Id == id
                                                                      orderby wd.ActualDate descending
                                                                      select new AddProductToParentWarehouseForViewDto
                                                                      {
                                                                          ObjectName = wd.ObjectName,
                                                                          ProductId = wd.ProductId,
                                                                          ProductName = p.Name,
                                                                          Quantity = wd.Quantity,
                                                                          Price = p.Price,
                                                                          Type = wd.Type,
                                                                          ActualDate = wd.ActualDate,
                                                                      };
            return query;
        }
    }
}
