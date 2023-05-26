using Bussiness.Interface.OrderInterface.Dto;
using Bussiness.Services.Core;

namespace Bussiness.Interface.OrderInterface
{
    public interface IOrderAppService
    {
        Task<object> GetOrdersForAdmin(PaginationInput input);
        Task<object> GetOrdersForShop(int shopId, PaginationInput input);
        Task<OrderForViewDto> CreateOrder(OrderInput input);
        Task<object> UpdateOrder();
        Task<object> ForwardToTheStore(long id, ForwardToTheStoreInput input);
    }
}
