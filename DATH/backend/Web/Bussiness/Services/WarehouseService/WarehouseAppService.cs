using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface.WarehouseInterface;
using Bussiness.Interface.WarehouseInterface.Dto;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Entities.Enum.Warehouse;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Services.WarehouseService
{
    public class WarehouseAppService : BaseService, IWarehouseAppService
    {
        private readonly IRepository<Warehouse> _warehouseRepo;
        private readonly IRepository<Shop> _shopRepo;
        private readonly IRepository<WarehouseDetail, long> _warehouseDetailRepo;
        private readonly IRepository<Product, long> _productRepo;
        private readonly IRepository<Order, long> _orderRepo;

        public WarehouseAppService(
            IRepository<Warehouse> warehouseRepo,
            IRepository<Shop> shopRepo,
            IMapper mapper,
            IRepository<WarehouseDetail, long> warehouseDetailRepo,
            IRepository<Product, long> productRepo,
            IRepository<Order, long> orderRepo
            )
        {
            ObjectMapper = mapper;
            _warehouseRepo = warehouseRepo;
            _shopRepo = shopRepo;
            _warehouseDetailRepo = warehouseDetailRepo;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
        }

        #region AddProductToParentWarehouse
        public async Task<object> AddProductToParentWarehouse(int id, AddProductToWarehouseInput input)
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
                    return "The product is not in warehouse";
                }
                else if (data!.Quantity < input.Quantity)
                {   
                    return "Not enough quantity";
                }
            }

            WarehouseDetail warehouseDetail = new()
            {
                WarehouseId = id,
                ActualDate = DateTime.Now,
            };

            ObjectMapper!.Map(input, warehouseDetail);

            await _warehouseDetailRepo.InsertAsync(warehouseDetail);

            IQueryable<AddProductToParentWarehouseForViewDto> query = HandleWarehouse(id);

            return await query.ToListAsync();
        }
        #endregion

        #region CreateOrUpdate
        public async Task<object?> CreateOrUpdate(int? id, WarehouseInput input)
        {
            if (id is null)
            {
                Shop? shop = await _shopRepo.GetAsync((int)input.ShopId!);

                Warehouse? warehouse = await _warehouseRepo.GetAll().AsNoTracking().Where(w => w.ShopId == input.ShopId).FirstOrDefaultAsync();
                if (warehouse != null) return "Shop had warehouse";

                Warehouse? parentWarehouse = await _warehouseRepo.GetAll().AsNoTracking().Where(w => w.ParentId == null && w.ShopId == null).SingleOrDefaultAsync();
                if (parentWarehouse == null) return "No Parent Warehouse";
                Warehouse data = new()
                {
                    ParentId = parentWarehouse!.Id
                };
                ObjectMapper!.Map(input, data);

                await _warehouseRepo.InsertAsync(data);
                WarehouseForViewDto? res = new();
                ObjectMapper!.Map(data, res);
                res.ShopName = shop!.Name;
                return res;
            }
            else
            {
                Warehouse? warehouse = await _warehouseRepo.GetAll().AsNoTracking().Where(w => w.Id == id).SingleOrDefaultAsync();
                if (warehouse != null && warehouse.ShopId == input.ShopId) return "Shop had warehouse";

                Shop? shop = await _shopRepo.GetAsync((int)input.ShopId!);

                Warehouse? data = await _warehouseRepo.GetAsync((int)id);
                if (data == null) return null;

                ObjectMapper!.Map(input, data);

                await _warehouseRepo.UpdateAsync(data);
                WarehouseForViewDto? res = new();
                ObjectMapper!.Map(data, res);
                res.ShopName = shop!.Name;
                return res;
            }
        }
        #endregion

        #region Delete
        public async Task Delete(int id)
        {
            await _warehouseRepo.DeleteAsync(id);
        }
        #endregion

        #region ExportToOrder
        public async Task<ICollection<WarehouseForViewDto>> ExportToOrder(ExportToOrderInput input)
        {
            Order? order = await _orderRepo.GetAll().AsNoTracking().Where(o => o.Code == input.OrderCode).SingleOrDefaultAsync();
            int warehouseId = await _warehouseRepo.GetAll().AsNoTracking().Where(w => w.ShopId == (int)order!.ShopId!).Select(w => w.Id).SingleOrDefaultAsync();

            ICollection<WarehouseDetail> warehouseDetails = new List<WarehouseDetail>();
            foreach(ExportToOrderDetailInput item in input.ExportToOrderDetailInputs!)
            {
                WarehouseDetail warehouseDetail = new()
                {
                    ObjectName = input.OrderCode,
                    ActualDate = DateTime.Now,
                    Type = EventType.Export,
                    ProductId = item.ProductId,
                    Color = item.Color,
                    WarehouseId = warehouseId
                };
                warehouseDetails.Add(warehouseDetail);
            }

            await _warehouseDetailRepo.AddRangeAsync(warehouseDetails);

            ICollection<WarehouseForViewDto> res = new List<WarehouseForViewDto>();

            foreach(WarehouseDetail item in warehouseDetails)
            {
                res.Add(ObjectMapper!.Map<WarehouseForViewDto>(item));
            }
            return res;
        }
        #endregion

        #region GetProductsForWarehouse
        public async Task<object> GetProductsForWarehouse(int id, PaginationInput input)
        {
            var query = HandleWarehouse(id);
            if (input.PageNum != null && input.PageSize != null) return await query.Pagination(input);
            else return await query.ToListAsync();
        }
        #endregion

        #region GetWarehouse
        public async Task<WarehouseForViewDto?> GetWarehouse(int id)
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

                if (data == null) return null;

                return data;
            }

            Warehouse? warehouse = await _warehouseRepo.GetAll().Where(w => w.ShopId == null && w.ParentId == null).SingleOrDefaultAsync();
            if (warehouse == null) return null;
            WarehouseForViewDto res = new();
            ObjectMapper!.Map(warehouse, res);

            if (warehouse == null) return null;

            return res;
        }
        #endregion

        #region GetWarehouses
        public async Task<object> GetWarehouses(PaginationInput input)
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

            if (input.PageNum != null && input.PageSize != null) return await query.Pagination(input);
            else return await query.ToListAsync();
        }
        #endregion

        #region Private method
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
        #endregion
    }
}
