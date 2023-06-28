using Bussiness.Dto;
using Bussiness.Interface.InstallmentInterface.Dto;
using Bussiness.Services.Core;
using Microsoft.AspNetCore.Http;

namespace Bussiness.Interface.ProductInterface
{
    public interface IProductAppService
    {
        Task<object> GetProducts(PaginationInput input, ProductFilterDto filter);
        Task<ProductForViewDto?> GetProduct(long id);
        Task<ProductForViewDto?> CreateOrUpdate(long? id, ProductInput input);
        Task<long> Delete(long id);
        Task<object?> AddPhoto(long id, bool isMain, long? specificationId, IFormFile file);
        Task<object?> RemovePhoto(long id, int photoId);
        Task<ProductForViewDto?> GetProductBySpecificationId(long id, long specificationId);
        Task<IEnumerable<GetInstallmentProductForCustomerForView>?> GetInstallmentProductForCustomer(long customerId);
    }
}
