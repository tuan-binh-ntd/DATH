using Bussiness.Dto;
using Bussiness.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Interface.ShopInterface
{
    public interface IShopAppService
    {
        Task<object> GetShops(PaginationInput input);
        Task<ShopForViewDto?> GetShop(int id);
        Task<ShopForViewDto?> CreateOrUpdate(int? id, ShopInput input);
        Task Delete(int id);
        Task<GetWarehouseForViewDto?> GetWarehouse(int id);
    }
}
