using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.AccountDTOs;
using MartyrGraveManagement_BAL.ModelViews.AreaDTOs;
using MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs;
using MartyrGraveManagement_BAL.ModelViews.CustomerDTOs;
using MartyrGraveManagement_BAL.ModelViews.FeedbackDTOs;
using MartyrGraveManagement_BAL.ModelViews.HistoricalEventDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveInformationDTOs;
using MartyrGraveManagement_BAL.ModelViews.OrdersDTOs;
using MartyrGraveManagement_BAL.ModelViews.PaymentDTOs;
using MartyrGraveManagement_BAL.ModelViews.ScheduleDTOs;
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
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
               .ForMember(dest => dest.HashedPassword, opt => opt.MapFrom(src => src.Password));

            CreateMap<UserAuthenticatingDtoResponse, Account>().ReverseMap();
            CreateMap<Account, AccountDtoResponse>().ReverseMap();
            CreateMap<Account, CustomerRegisterDtoRequest>().ReverseMap();
            CreateMap<Account, UpdateProfileDtoRequest>().ReverseMap();
            CreateMap<Account, UpdateProfileStaffOrManagerDtoRequest>().ReverseMap();

            // Area mappings
            CreateMap<Area, AreaDTOResponse>().ReverseMap();

            // MartyrGrave mappings
            CreateMap<MartyrGrave, MartyrGraveDtoRequest>().ReverseMap();
            CreateMap<MartyrGrave, MartyrGraveDtoResponse>().ReverseMap();
            CreateMap<GraveImage, GraveImageDtoRequest>().ReverseMap();
            CreateMap<MartyrGrave, MartyrGraveUpdateDtoRequest>().ReverseMap();
            CreateMap<MartyrGrave, MartyrGraveSearchDtoResponse>().ReverseMap();
            CreateMap<MartyrGrave, MartyrGraveSearchDtoRequest>().ReverseMap();
            CreateMap<MartyrGrave, MartyrGraveGetAllDtoResponse>().ReverseMap();

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
            CreateMap<CartItemCustomer, CartItemsDTORequest>().ReverseMap();
            CreateMap<CartItemCustomer, CartItemsDTOResponse>().ReverseMap();
            CreateMap<CartItemCustomer, CartItemGetByCustomerDTOResponse>().ReverseMap();

            //Order mappings
            CreateMap<Order, OrdersDTORequest>().ReverseMap();
            CreateMap<Order, OrdersDTOResponse>().ReverseMap();

            // Task mapping
            CreateMap<StaffTask, TaskDtoRequest>().ReverseMap();
            CreateMap<StaffTask, TaskDtoResponse>().ReverseMap();
            CreateMap<StaffTask, UpdateTaskStatusRequest>().ReverseMap();
            CreateMap<StaffTask, ReassignTaskRequest>().ReverseMap();
            CreateMap<StaffTask, AssignTaskDTORequest>().ReverseMap();
            CreateMap<StaffTask, TaskImageUpdateDTO>().ReverseMap();
            CreateMap<StaffTask, TaskBatchCreateRequest>().ReverseMap();
            CreateMap<StaffTask, TaskDetailRequest>().ReverseMap();


            //Payment mapping
            CreateMap<Payment, PaymentDTORequest>().ReverseMap();
            CreateMap<Payment, PaymentDTOResponse>().ReverseMap();

            //Feeback mapping
            CreateMap<FeedbackDtoRequest, Feedback>().ReverseMap();
            CreateMap<Feedback, FeedbackDtoResponse>().ReverseMap();
            CreateMap<Feedback, FeedbackContentDtoRequest>().ReverseMap();
            CreateMap<Feedback, FeedbackResponseDtoRequest>().ReverseMap();

            //HistoricalEvent mapping
            CreateMap<CreateHistoricalEventDTORequest, HistoricalEvent>().ReverseMap();
            CreateMap<HistoricalEventDTOResponse, HistoricalEvent>().ReverseMap();

            //Schedule mapping
<<<<<<< Updated upstream
            CreateMap<CreateScheduleDTORequest, Schedule_Staff>().ReverseMap();
            CreateMap<UpdateScheduleDTORequest, Schedule_Staff>().ReverseMap();
            CreateMap<ScheduleDTOResponse, Schedule_Staff>().ReverseMap();
=======
            CreateMap<CreateScheduleDTORequest, Schedule>().ReverseMap();
>>>>>>> Stashed changes
        }
    }
}
