using CoreApiResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Employee", Policy = "RequireEmployeeRole")]
    public class EmployeeBaseController : BaseController
    {
    }
}
