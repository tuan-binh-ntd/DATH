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

            CreateMap<ShopInput, Shop>().ReverseMap();

            CreateMap<RegisterDto, Customer>().ReverseMap();

            CreateMap<RegisterDto, Employee>().ReverseMap();

            CreateMap<ShopInput, Shop>().ReverseMap();

            CreateMap<Customer, UserDto>().ReverseMap();

            CreateMap<Employee, UserDto>().ReverseMap();

            CreateMap<Customer, CustomerInput>().ReverseMap();

            CreateMap<SpecificationCategoryInput, SpecificationCategory>().ReverseMap();

            CreateMap<SpecificationInput, Specification>().ReverseMap();

            CreateMap<ProductCategory, ProductCategoryInput>().ReverseMap();

            CreateMap<Product, ProductInput>().ReverseMap();

        }
    }
}
