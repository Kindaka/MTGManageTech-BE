using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.AccountDTOs;
using MartyrGraveManagement_BAL.ModelViews.AreaDTOs;
using MartyrGraveManagement_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegisterDtoRequest, Account>()
           .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
           .ForMember(dest => dest.HashedPassword, opt => opt.MapFrom(src => src.Password));
            CreateMap<UserAuthenticatingDtoResponse, Account>();
            CreateMap<Area, AreaDTOResponse>().ReverseMap();
        }
    }
}
