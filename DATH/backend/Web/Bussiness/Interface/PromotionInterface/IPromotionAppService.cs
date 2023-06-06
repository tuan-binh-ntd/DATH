using Bussiness.Dto;
using Bussiness.Services.Core;

namespace Bussiness.Interface.PromotionInterface
{
    public interface IPromotionAppService
    {
        Task<object> GetPromotions(PaginationInput input);
        Task<PromotionForViewDto?> GetPromotion(int id);
        Task<PromotionForViewDto?> GetPromotion(string code);
        Task<PromotionForViewDto?> CreateOrUpdate(int? id, PromotionInput input);
        Task Delete(int id);
    }
}
