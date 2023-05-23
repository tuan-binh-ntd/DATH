using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface.PaymentInterface;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Services.PaymentService
{
    public class PaymentAppService : BaseService, IPaymentAppService
    {
        private readonly IRepository<Payment> _paymentRepo;

        public PaymentAppService(
            IRepository<Payment> paymentRepo,
            IMapper mapper
            )
        {
            ObjectMapper = mapper;
            _paymentRepo = paymentRepo;
        }

        #region CreateOrUpdate
        public async Task<PaymentForViewDto?> CreateOrUpdate(int? id, PaymentInput input)
        {
            if(id is null)
            {
                Payment? data = new();
                ObjectMapper!.Map(input, data);
                await _paymentRepo.InsertAsync(data);

                PaymentForViewDto? res = new();
                ObjectMapper!.Map(data, res);
                return res;
            }
            else
            {
                Payment? payment = await _paymentRepo.GetAsync((int)id);
                if (payment == null) return null;
                ObjectMapper!.Map(input, payment);

                await _paymentRepo.UpdateAsync(payment);
                PaymentForViewDto? res = new();
                ObjectMapper!.Map(payment, res);
                return res;
            }
        }
        #endregion

        #region Delete
        public async Task Delete(int id)
        {
            await _paymentRepo.DeleteAsync(id);
        }
        #endregion

        #region GetPayment
        public async Task<PaymentForViewDto?> GetPayment(int id)
        {
            IQueryable<PaymentForViewDto> query = from p in _paymentRepo.GetAll().AsNoTracking()
                                                  where p.Id == id
                                                  select new PaymentForViewDto()
                                                  {
                                                      Id = p.Id,
                                                      Name = p.Name,
                                                  };
            PaymentForViewDto? data = await query.SingleOrDefaultAsync();
            if (data == null) return null;

            return data;
        }
        #endregion

        #region GetPayments
        public async Task<object> GetPayments(PaginationInput input)
        {
            IQueryable<PaymentForViewDto> query = from p in _paymentRepo.GetAll().AsNoTracking()
                                                  select new PaymentForViewDto()
                                                  {
                                                      Id = p.Id,
                                                      Name = p.Name,
                                                  };

            if (input.PageNum != null && input.PageSize != null) return await query.Pagination(input);
            else return await query.ToListAsync();
        }
        #endregion
    }
}
