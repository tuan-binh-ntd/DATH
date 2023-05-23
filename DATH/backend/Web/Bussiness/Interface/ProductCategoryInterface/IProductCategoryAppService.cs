using Bussiness.Dto;
using Bussiness.Services.Core;

namespace Bussiness.Interface.ProductCategoryInterface
{
    public interface IProductCategoryAppService
    {
        Task<object> GetProductCategorys(PaginationInput input);
        Task<ProductCategoryForViewDto?> GetProductCategory(int id);
        Task<ProductCategoryForViewDto?> CreateOrUpdate(int? id, ProductCategoryInput input);
        Task Delete(int id);
        Task<object> GetFilterForProductCategory(int id);
        Task<object> GetProductByCategory(int id, PaginationInput input, ProductFilterDto filter);
    }
}
