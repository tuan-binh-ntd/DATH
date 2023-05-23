using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface.PaymentInterface;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            if(res is null) return CustomResult(HttpStatusCode.NoContent);
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
    }
}
