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
            CreateMap<ShopDto, Shop>().ReverseMap();
            CreateMap<RegisterDto, Customer>().ReverseMap();
            CreateMap<RegisterDto, Employee>().ReverseMap();
            CreateMap<ShopDto, Shop>().ReverseMap();
            CreateMap<Customer, UserDto>().ReverseMap();
            CreateMap<Employee, UserDto>().ReverseMap();
        }
    }
}
