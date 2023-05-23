using Bussiness.Dto;
using Bussiness.Services.Core;
using Microsoft.AspNetCore.Http;

namespace Bussiness.Interface.PaymentInterface
{
    public interface IPaymentAppService
    {
        Task<object> GetPayments(PaginationInput input);
        Task<PaymentForViewDto?> GetPayment(int id);
        Task<PaymentForViewDto?> CreateOrUpdate(int? id, PaymentInput input);
        Task Delete(int id);
        Task<object> AddPhoto(int id, IFormFile file);
        Task<object> DeletePhoto(int id);
    }
}
