using Bussiness.Dto;
using Bussiness.Services.Core;

namespace Bussiness.Interface.SpecificationCategoryInterface
{
    public interface ISpecificationCategoryAppService
    {
        Task<object> GetSpecificationCategorys(PaginationInput input);
        Task<SpecificationCategoryForViewDto?> GetSpecificationCategory(int id);
        Task<SpecificationCategoryForViewDto?> CreateOrUpdate(int? id, SpecificationCategoryInput input);
        Task Delete(int id);
        Task<ICollection<GetColorByCodeForViewDto>?> GetColorByCode(string code);
    }
}
