using AutoMapper;

namespace API.Controllers
{
    public class EmployeesController : AdminBaseController
    {
        private readonly IMapper _mapper;

        public EmployeesController(
            IMapper mapper
            )
        {
            _mapper = mapper;
        }
    }
}
