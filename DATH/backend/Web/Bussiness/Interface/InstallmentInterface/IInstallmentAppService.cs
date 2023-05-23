using Bussiness.Dto;
using Bussiness.Services.Core;

namespace Bussiness.Interface.InstallmentInterface
{
    public interface IInstallmentAppService
    {
        Task<object> GetInstallments(PaginationInput input);
        Task<InstallmentForViewDto?> GetInstallment(int id);
        Task<InstallmentForViewDto?> CreateOrUpdate(int? id, InstallmentInput input);
        Task Delete(int id);
    }
}
