using Bussiness.Dto;
using Bussiness.Services.Core;

namespace Bussiness.Interface.SpecificationInterface
{
    public interface ISpecificationAppService
    {
        Task<object> GetSpecifications(PaginationInput input);
        Task<SpecificationForViewDto?> GetSpecification(long id);
        Task<SpecificationForViewDto?> CreateOrUpdate(long? id, SpecificationInput input);
        Task Delete(long id);
    }
}
