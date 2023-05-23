using Bussiness.Dto;

namespace Bussiness.Interface.CustomerInterface
{
    public interface ICustomerAppService
    {
        Task<CustomerForViewDto?> GetCustomer(long id);
        Task<CustomerForViewDto?> Update(long id, CustomerInput input);
        Task<CustomerForViewDto?> CreateAddress(long id, AddressInput input);
    }
}
