using AutoMapper;
using Entities;
using Service.DemoService.Dto;

namespace Service.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Demo, DemoDto>().ReverseMap();
        }
    }
}
