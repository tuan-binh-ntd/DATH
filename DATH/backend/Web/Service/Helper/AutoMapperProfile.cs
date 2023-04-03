using AutoMapper;
using Entities;
using Service.AccountService.Dto;

namespace Service.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser, RegisterDto>().ReverseMap();
        }
    }
}
