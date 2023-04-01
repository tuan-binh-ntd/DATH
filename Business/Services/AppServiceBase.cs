using AutoMapper;
using Microsoft.AspNetCore.Components;

namespace API.Services
{
    public abstract class AppServiceBase
    {
        [Inject]
        public IMapper Mapper { get; set; }
    }
}
