using AutoMapper;
using Bussiness.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMapper _mapper;

        public ProductsController(
            IMapper mapper
            )
        {
            _mapper = mapper;
        }
    }
}
