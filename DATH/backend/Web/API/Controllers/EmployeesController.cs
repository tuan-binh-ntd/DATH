using Bussiness.Dto;
using Bussiness.Interface.EmployeeInterface;
using Bussiness.Services.Core;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    public class EmployeesController : EmployeeBaseController
    {
        private readonly IEmployeeAppService _employeeAppService;

        public EmployeesController(
            IEmployeeAppService employeeAppService
            )
        {
            _employeeAppService = employeeAppService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            object res = await _employeeAppService.GetEmployees(input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, EmployeeInput input)
        {
            EmployeeForViewDto? res = await _employeeAppService.Update(id, input);
            if(res == null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _employeeAppService.Delete(id);
            return CustomResult(id, HttpStatusCode.OK);
        }
    }
}
