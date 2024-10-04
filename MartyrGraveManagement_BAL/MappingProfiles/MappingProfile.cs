using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.AccountDTOs;
using MartyrGraveManagement_BAL.ModelViews.AreaDTOs;
using MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveInformationDTOs;
using MartyrGraveManagement_BAL.ModelViews.OrdersDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceCategoryDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using MartyrGraveManagement_BAL.Services.Implements;
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
            // Account mappings
            CreateMap<UserRegisterDtoRequest, Account>()
               .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
               .ForMember(dest => dest.HashedPassword, opt => opt.MapFrom(src => src.Password));

            CreateMap<UserAuthenticatingDtoResponse, Account>();

            // Area mappings
            CreateMap<Area, AreaDTOResponse>().ReverseMap();

            // MartyrGrave mappings
            CreateMap<MartyrGrave, MartyrGraveDtoRequest>().ReverseMap();
            CreateMap<MartyrGrave, MartyrGraveDtoResponse>().ReverseMap();

            // MartyrGraveInformation mappings
            CreateMap<MartyrGraveInformation, MartyrGraveInformationDtoRequest>().ReverseMap();
            CreateMap<MartyrGraveInformation, MartyrGraveInformationDtoResponse>().ReverseMap();

            //ServiceCategory mappings
            CreateMap<ServiceCategory, ServiceCategoryDto>().ReverseMap();
            CreateMap<ServiceCategory, ServiceCategoryDtoResponse>().ReverseMap();

            CreateMap<Service, ServiceDtoRequest>().ReverseMap();
            CreateMap<Service, ServiceDtoResponse>().ReverseMap();

            //Cart mappings
            CreateMap<CartItem, CartItemsDTORequest>().ReverseMap();
            CreateMap<CartItem, CartItemsDTOResponse>().ReverseMap();

            //Order mappings
            CreateMap<Order, OrdersDTORequest>().ReverseMap();
            CreateMap<Order, OrdersDTOResponse>().ReverseMap();

        }
    }
}
 