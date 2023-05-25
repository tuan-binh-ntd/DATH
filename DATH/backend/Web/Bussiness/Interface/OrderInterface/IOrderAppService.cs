using Bussiness.Interface.OrderInterface.Dto;
using Bussiness.Services.Core;

namespace Bussiness.Interface.OrderInterface
{
    public interface IOrderAppService
    {
        Task<object> GetOrders(PaginationInput input);
        Task<object> CreateOrder(OrderInput input);
        Task<object> UpdateOrder();
    }
}
