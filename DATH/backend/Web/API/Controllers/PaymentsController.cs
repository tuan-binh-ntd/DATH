using Bussiness.Dto;
using Bussiness.Interface.PaymentInterface;
using Bussiness.Services.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    public class PaymentsController : AdminBaseController
    {
        private readonly IPaymentAppService _paymentAppService;

        public PaymentsController(
            IPaymentAppService paymentAppService
            )
        {
            _paymentAppService = paymentAppService;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            object res = await _paymentAppService.GetPayments(input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            PaymentForViewDto? res = await _paymentAppService.GetPayment(id);
            if (res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PaymentInput input)
        {
            PaymentForViewDto? res = await _paymentAppService.CreateOrUpdate(null, input);
            if (res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PaymentInput input)
        {
            PaymentForViewDto? res = await _paymentAppService.CreateOrUpdate(id, input);
            if (res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _paymentAppService.Delete(id);
            return CustomResult(id, HttpStatusCode.OK);
        }

        [HttpPost("{id}/photos")]
        public async Task<IActionResult> AddPhoto(int id, IFormFile file)
        {
            object res = await _paymentAppService.AddPhoto(id, file);
            if (res is string) return CustomResult(res.ToString(), HttpStatusCode.BadRequest);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}/photos")]
        public async Task<IActionResult> RemovePhoto(int id)
        {
            object res = await _paymentAppService.DeletePhoto(id);
            if (res is string) return CustomResult(res.ToString(), HttpStatusCode.BadRequest);
            return CustomResult(res, HttpStatusCode.OK);
        }
    }
}
