using Bussiness.Dto;
using Bussiness.Services.Core;

namespace Bussiness.Interface.ShippingInterface
{
    public interface IShippingAppService
    {
        Task<object> GetShippings(PaginationInput input);
        Task<ShippingForViewDto?> GetShipping(int id);
        Task<ShippingForViewDto?> CreateOrUpdate(int? id, ShippingInput input);
        Task Delete(int id);
    }
}
