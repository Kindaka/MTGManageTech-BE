using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.AccountDTOs;
using MartyrGraveManagement_BAL.ModelViews.AreaDTOs;
using MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs;
using MartyrGraveManagement_BAL.ModelViews.FeedbackDTOs;
using MartyrGraveManagement_BAL.ModelViews.JobDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveInformationDTOs;
using MartyrGraveManagement_BAL.ModelViews.OrdersDTOs;
using MartyrGraveManagement_BAL.ModelViews.PaymentDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceCategoryDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
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
               .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.AccountName))
               .ForMember(dest => dest.HashedPassword, opt => opt.MapFrom(src => src.Password));

            CreateMap<UserAuthenticatingDtoResponse, Account>();

            // Area mappings
            CreateMap<Area, AreaDTOResponse>().ReverseMap();

            // MartyrGrave mappings
            CreateMap<MartyrGrave, MartyrGraveDtoRequest>().ReverseMap();
            CreateMap<MartyrGrave, MartyrGraveDtoResponse>().ReverseMap();
            CreateMap<GraveImage, GraveImageDtoRequest>().ReverseMap();
            CreateMap<MartyrGrave, MartyrGraveUpdateDtoRequest>().ReverseMap();

            // MartyrGraveInformation mappings
            CreateMap<MartyrGraveInformation, MartyrGraveInformationDtoRequest>().ReverseMap();
            CreateMap<MartyrGraveInformation, MartyrGraveInformationDtoResponse>().ReverseMap();

            //ServiceCategory mappings
            CreateMap<ServiceCategory, ServiceCategoryDto>().ReverseMap();
            CreateMap<ServiceCategory, ServiceCategoryDtoResponse>().ReverseMap();

            CreateMap<Service, ServiceDtoRequest>().ReverseMap();
            CreateMap<Service, ServiceDtoResponse>().ReverseMap();
            CreateMap<Service, ServiceDetailDtoResponse>().ReverseMap();
            //Cart mappings
            CreateMap<CartItem, CartItemsDTORequest>().ReverseMap();
            CreateMap<CartItem, CartItemsDTOResponse>().ReverseMap();
            CreateMap<CartItem, CartItemGetByCustomerDTOResponse>().ReverseMap();

            //Order mappings
            CreateMap<Order, OrdersDTORequest>().ReverseMap();
            CreateMap<Order, OrdersDTOResponse>().ReverseMap();

            // Task mapping
            CreateMap<StaffTask, TaskDtoRequest>().ReverseMap();
            CreateMap<StaffTask, TaskDtoResponse>().ReverseMap();

            // Job Mapping
            CreateMap<StaffJob, JobDtoRequest>().ReverseMap();
            CreateMap<StaffJob, JobDtoResponse>().ReverseMap();

            //Payment mapping
            CreateMap<Payment, PaymentDTORequest>().ReverseMap();
            CreateMap<Payment, PaymentDTOResponse>().ReverseMap();

            //Feeback mapping
            CreateMap<FeedbackDtoRequest, Feedback>();
            CreateMap<Feedback, FeedbackDtoResponse>();


        }
    }
}
 