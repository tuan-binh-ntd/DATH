using Bussiness.Dto;
using Bussiness.Interface.InstallmentInterface;
using Bussiness.Services.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    public class InstallmentsController : AdminBaseController
    {
        private readonly IInstallmentAppService _installmentAppService;

        public InstallmentsController(
           IInstallmentAppService installmentAppService
            )
        {
            _installmentAppService = installmentAppService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            object res = await _installmentAppService.GetInstallments(input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            InstallmentForViewDto? res = await _installmentAppService.GetInstallment(id);
            if(res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(InstallmentInput input)
        {
            InstallmentForViewDto? res = await _installmentAppService.CreateOrUpdate(null, input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, InstallmentInput input)
        {
            InstallmentForViewDto? res = await _installmentAppService.CreateOrUpdate(id, input);
            if (res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _installmentAppService.Delete(id);
            return CustomResult(id, HttpStatusCode.OK);
        }
    }
}
