using AutoMapper;

namespace Bussiness.Services.Core
{
    public abstract class BaseService
    {
        protected IMapper? ObjectMapper { get; set; }
    }
}
