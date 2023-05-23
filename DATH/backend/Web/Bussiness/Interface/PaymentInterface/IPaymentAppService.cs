using Bussiness.Dto;
using Bussiness.Services.Core;

namespace Bussiness.Interface.PaymentInterface
{
    public interface IPaymentAppService
    {
        Task<object> GetPayments(PaginationInput input);
        Task<PaymentForViewDto?> GetPayment(int id);
        Task<PaymentForViewDto?> CreateOrUpdate(int? id, PaymentInput input);
        Task Delete(int id);
    }
}
