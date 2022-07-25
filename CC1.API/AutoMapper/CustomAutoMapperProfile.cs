using AutoMapper;
using CC1.Model;
using CC1.Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CC1.API.AutoMapper
{
    public class CustomAutoMapperProfile : Profile
    {
        public CustomAutoMapperProfile()
        {
            //base.CreateMap<User, UserDTO>();
            //映射的在前面，被映射的在后面
            base.CreateMap<UserDTO, User>();
            //base.CreateMap<BlogNews, BlogNewsDTO>()
            // .ForMember(dest => dest.TypeName, sourse => sourse.MapFrom(src => src.TypeInfo.Name))
            // .ForMember(dest => dest.WriterName, sourse => sourse.MapFrom(src => src.WriterInfo.Name));
        }
    }
}
