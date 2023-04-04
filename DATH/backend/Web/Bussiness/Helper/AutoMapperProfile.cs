using AutoMapper;
using Bussiness.Dto;
using Entities;

namespace Bussiness.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser, RegisterDto>().ReverseMap();
        }
    }
}
