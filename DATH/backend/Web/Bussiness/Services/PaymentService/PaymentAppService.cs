using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface.Core;
using Bussiness.Interface.PaymentInterface;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Services.PaymentService
{
    public class PaymentAppService : BaseService, IPaymentAppService
    {
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IPhotoService _photoService;

        public PaymentAppService(
            IRepository<Payment> paymentRepo,
            IMapper mapper,
            IPhotoService photoService
            )
        {
            ObjectMapper = mapper;
            _paymentRepo = paymentRepo;
            _photoService = photoService;
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
                                                      Url = p.Url,
                                                      PublicId = p.PublicId,
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
                                                      Url = p.Url,
                                                      PublicId = p.PublicId,
                                                  };

            if (input.PageNum != null && input.PageSize != null) return await query.Pagination(input);
            else return await query.ToListAsync();
        }
        #endregion

        #region AddPhoto
        public async Task<object> AddPhoto(int id, IFormFile file)
        {
            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return result.Error.Message;

            Payment? payment = await _paymentRepo.GetAsync(id);

            payment!.Url = result.SecureUrl.AbsoluteUri;
            payment!.PublicId = result.PublicId;

            if(await _paymentRepo.UpdateAsync(payment) != null)
            {
                PaymentForViewDto res = new();

                ObjectMapper!.Map(payment, res);
                return res;
            }
            return "Problem adding photo";
        }
        #endregion

        #region DeletePhoto
        public async Task<object> DeletePhoto(int id)
        {
            Payment? payment = await _paymentRepo.GetAll().Where(p => p.Id == id).SingleOrDefaultAsync();

            var result = await _photoService.DeletePhotoAsync(payment!.PublicId!);
            if (result.Error != null) return result.Error.Message;
            payment.Url = null;
            payment.PublicId = null;

            if (await _paymentRepo.UpdateAsync(payment) != null)
            {
                PaymentForViewDto res = new();

                ObjectMapper!.Map(payment, res);
                return res;
            }
            return "Failed to delete your photo";
        }
        #endregion

    }
}
