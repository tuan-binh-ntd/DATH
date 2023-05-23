using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface.ShopInterface;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Services.ShopService
{
    public class ShopAppService : BaseService, IShopAppService
    {
        private readonly IRepository<Shop> _shopRepo;
        private readonly IRepository<Warehouse> _warehouseRepo;

        public ShopAppService(
            IMapper mapper,
            IRepository<Shop> shopRepo,
            IRepository<Warehouse> warehouseRepo
            )
        {
            ObjectMapper = mapper;
            _shopRepo = shopRepo;
            _warehouseRepo = warehouseRepo;
        }

        #region CreateOrUpdate
        public async Task<ShopForViewDto?> CreateOrUpdate(int? id, ShopInput input)
        {
            if (id is null)
            {
                Shop data = new();
                ObjectMapper!.Map(input, data);

                await _shopRepo.InsertAsync(data);
                ShopForViewDto? res = new();
                ObjectMapper!.Map(data, res);
                return res;
            }
            else
            {
                Shop? data = await _shopRepo.GetAsync((int)id);
                if (data == null) return null;
                ObjectMapper!.Map(input, data);

                await _shopRepo.UpdateAsync(data);
                ShopForViewDto? res = new();
                ObjectMapper!.Map(data, res);
                return res;
            }
        }
        #endregion

        #region Delete
        public async Task Delete(int id)
        {
            await _shopRepo.DeleteAsync(id);
        }
        #endregion

        #region GetShop
        public async Task<ShopForViewDto?> GetShop(int id)
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
            if (data == null) return null;

            return data;
        }
        #endregion

        #region GetShops
        public async Task<object> GetShops(PaginationInput input)
        {
            IQueryable<ShopForViewDto> query = from s in _shopRepo.GetAll().AsNoTracking()
                                               select new ShopForViewDto()
                                               {
                                                   Id = s.Id,
                                                   Name = s.Name,
                                                   Address = s.Address
                                               };

            if (input.PageNum != null && input.PageSize != null) return await query.Pagination(input);
            else return await query.ToListAsync();
        }
        #endregion

        #region GetWarehouse
        public async Task<GetWarehouseForViewDto?> GetWarehouse(int id)
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
            if (data == null) return null;
            return data;
        }
        #endregion
    }
}
