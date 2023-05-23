using Bussiness.Dto;
using Bussiness.Services.Core;

namespace Bussiness.Interface.EmployeeInterface
{
    public interface IEmployeeAppService
    {
        Task<object> GetEmployees(PaginationInput input);
        Task<EmployeeForViewDto?> Update(long id, EmployeeInput input);
        Task Delete(long id);
    }
}
