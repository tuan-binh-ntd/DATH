using API.Dto;
using API.Entities;
using AutoMapper;

namespace API.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Demo, DemoDto>().ReverseMap();
        }
    }
}
