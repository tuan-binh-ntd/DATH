using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface.PromotionInterface;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Services.PromotionService
{
    public class PromotionAppService : BaseService, IPromotionAppService
    {
        private readonly IRepository<Promotion> _promotionRepo;

        public PromotionAppService(
            IMapper mapper,
            IRepository<Promotion> promotionRepo
            )
        {
            ObjectMapper = mapper;
            _promotionRepo = promotionRepo;
        }

        #region CreateOrUpdate
        public async Task<PromotionForViewDto?> CreateOrUpdate(int? id, PromotionInput input)
        {
            if(id is null)
            {
                Promotion data = new();
                ObjectMapper!.Map(input, data);

                await _promotionRepo.InsertAsync(data);

                PromotionForViewDto? res = new();
                ObjectMapper!.Map(data, res);
                return res;
            }
            else
            {
                Promotion? data = await _promotionRepo.GetAsync((int)id);
                if (data == null) return null;
                ObjectMapper!.Map(input, data);

                await _promotionRepo.UpdateAsync(data);
                PromotionForViewDto? res = new();
                ObjectMapper!.Map(data, res);
                return res;
            }
        }
        #endregion

        #region Delete
        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetPromotion
        public async Task<PromotionForViewDto?> GetPromotion(int id)
        {
            IQueryable<PromotionForViewDto> query = from p in _promotionRepo.GetAll().AsNoTracking()
                                                    where p.Id == id
                                                    select new PromotionForViewDto()
                                                    {
                                                        Id = p.Id,
                                                        Name = p.Name,
                                                        Code = p.Code,
                                                        StartDate = p.StartDate,
                                                        EndDate = p.EndDate,
                                                        Discount = p.Discount,
                                                    };
            PromotionForViewDto? data = await query.FirstOrDefaultAsync();
            if (data == null) return null;

            return data;
        }

        public async Task<PromotionForViewDto?> GetPromotion(string code)
        {
            IQueryable<PromotionForViewDto> query = from p in _promotionRepo.GetAll().AsNoTracking()
                                                    where p.Code == code
                                                    select new PromotionForViewDto()
                                                    {
                                                        Id = p.Id,
                                                        Name = p.Name,
                                                        Code = p.Code,
                                                        StartDate = p.StartDate,
                                                        EndDate = p.EndDate,
                                                        Discount = p.Discount,
                                                    };
            PromotionForViewDto? data = await query.FirstOrDefaultAsync();
            if (data == null) return null;

            return data;
        }

        #endregion

        #region GetPromotions
        public async Task<object> GetPromotions(PaginationInput input)
        {
            IQueryable<PromotionForViewDto> query = from p in _promotionRepo.GetAll().AsNoTracking()
                                                    select new PromotionForViewDto()
                                                    {
                                                        Id = p.Id,
                                                        Name = p.Name,
                                                        Code = p.Code,
                                                        StartDate = p.StartDate,
                                                        EndDate = p.EndDate,
                                                        Discount = p.Discount,
                                                    };

            if (input.PageNum != null && input.PageSize != null) return await query.Pagination(input);
            else return await query.ToListAsync();
        }
        #endregion

    }
}
