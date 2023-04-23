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

            CreateMap<Shipping, ShippingInput>().ReverseMap();

            CreateMap<Promotion, PromotionInput>().ReverseMap();

            CreateMap<Shop, ShopForViewDto>().ReverseMap();

            CreateMap<Specification, SpecificationForViewDto>().ReverseMap();

            CreateMap<SpecificationCategory, SpecificationCategoryForViewDto>().ReverseMap();

            CreateMap<Product, ProductForViewDto>().ReverseMap();

            CreateMap<Promotion, PromotionForViewDto>().ReverseMap();

            CreateMap<ProductCategory, ProductCategoryForViewDto>().ReverseMap();

            CreateMap<Photo, PhotoDto>().ReverseMap();

            CreateMap<Employee, EmployeeInput>().ReverseMap();

            CreateMap<Customer, CustomerForViewDto>().ReverseMap();
        }
    }
}
