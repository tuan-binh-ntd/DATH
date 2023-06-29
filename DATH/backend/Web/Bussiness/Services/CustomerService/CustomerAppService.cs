using AutoMapper;
using Bussiness.Dto;
using Bussiness.Interface.CustomerInterface;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;

namespace Bussiness.Services.CustomerService
{
    public class CustomerAppService : BaseService, ICustomerAppService
    {
        private readonly IRepository<Customer, long> _customerRepo;

        public CustomerAppService(
            IMapper mapper,
            IRepository<Customer, long> customerRepo
            )
        {
            _customerRepo = customerRepo;
            ObjectMapper = mapper;
        }

        #region CreateAddress
        public async Task<CustomerForViewDto?> CreateAddress(long id, AddressInput input)
        {
            Customer? customer = await _customerRepo.GetAsync(id);
            if (customer == null) return null;

            if (input.Addresses!.Count != 0) customer!.Address = string.Join("|", input.Addresses!.ToList());
            await _customerRepo.UpdateAsync(customer!);
            CustomerForViewDto? res = new();
            ObjectMapper!.Map(customer, res);
            res.UserId = customer.UserId;
            return res;
        }
        #endregion

        #region GetCustomer
        public async Task<CustomerForViewDto?> GetCustomer(long id)
        {
            Customer? customer = await _customerRepo.GetAsync(id);
            if (customer == null) return null;
            CustomerForViewDto res = new();
            ObjectMapper!.Map(customer, res);
            return res;
        }
        #endregion

        #region Update
        public async Task<CustomerForViewDto?> Update(long id, CustomerInput input)
        {
            Customer? customer = await _customerRepo.GetAsync(id);
            if (customer == null) return null;
            ObjectMapper!.Map(input, customer);
            await _customerRepo.UpdateAsync(customer!);
            CustomerForViewDto? res = new();
            ObjectMapper!.Map(customer, res);
            return null;
        }
        #endregion
    }
}
