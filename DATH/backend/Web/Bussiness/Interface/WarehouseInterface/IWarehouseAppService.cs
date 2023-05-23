using Bussiness.Dto;
using Bussiness.Services.Core;

namespace Bussiness.Interface.WarehouseInterface
{
    public interface IWarehouseAppService
    {
        Task<object> GetWarehouses(PaginationInput input);
        Task<WarehouseForViewDto?> GetWarehouse(int id);
        Task<object?> CreateOrUpdate(int? id, WarehouseInput input);
        Task Delete(int id);
        Task<object> GetProductsForWarehouse(int id, PaginationInput input);
        Task<object> AddProductToParentWarehouse(int id, AddProductToWarehouseInput input);
    }
}
