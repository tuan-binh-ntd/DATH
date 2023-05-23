using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface.ShippingInterface;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Services.ShippingService
{
    public class ShippingAppService : BaseService, IShippingAppService
    {
        private readonly IRepository<Shipping> _shippingRepo;

        public ShippingAppService(
            IMapper mapper,
            IRepository<Shipping> shippingRepo
            )
        {
            ObjectMapper = mapper;
            _shippingRepo = shippingRepo;
        }

        #region CreateOrUpdate
        public async Task<ShippingForViewDto?> CreateOrUpdate(int? id, ShippingInput input)
        {
            if (id is null)
            {
                Shipping data = new();
                ObjectMapper!.Map(input, data);

                await _shippingRepo.InsertAsync(data);
                ShippingForViewDto? res = new();
                ObjectMapper!.Map(data, res);
                return res;
            }
            else
            {
                Shipping? data = await _shippingRepo.GetAsync((int)id);
                if (data == null) return null;
                ObjectMapper!.Map(input, data);

                await _shippingRepo.UpdateAsync(data);
                ShippingForViewDto? res = new();
                ObjectMapper!.Map(data, res);
                return res;
            }
        }
        #endregion

        #region MyRegion
        public async Task Delete(int id)
        {
            await _shippingRepo.DeleteAsync(id);
        }
        #endregion

        #region GetShipping
        public async Task<ShippingForViewDto?> GetShipping(int id)
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
            if (data == null) return null;

            return data;
        }
        #endregion

        #region GetShippings
        public async Task<object> GetShippings(PaginationInput input)
        {
            IQueryable<ShippingForViewDto> query = from s in _shippingRepo.GetAll().AsNoTracking()
                                                   select new ShippingForViewDto()
                                                   {
                                                       Id = s.Id,
                                                       Name = s.Name,
                                                       Cost = s.Cost,
                                                   };

            if (input.PageNum != null && input.PageSize != null) return await query.Pagination(input);
            else return await query.ToListAsync();
        }
        #endregion
    }
}
