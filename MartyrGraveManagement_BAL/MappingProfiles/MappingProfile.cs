using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.AccountDTOs;
using MartyrGraveManagement_BAL.ModelViews.AreaDTOs;
using MartyrGraveManagement_BAL.ModelViews.BlogDTOs;
using MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs;
using MartyrGraveManagement_BAL.ModelViews.CommentDTOs;
using MartyrGraveManagement_BAL.ModelViews.CommentIconDTOs;
using MartyrGraveManagement_BAL.ModelViews.CommentReportDTOs;
using MartyrGraveManagement_BAL.ModelViews.CustomerDTOs;
using MartyrGraveManagement_BAL.ModelViews.FeedbackDTOs;
using MartyrGraveManagement_BAL.ModelViews.HistoricalEventDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveInformationDTOs;
using MartyrGraveManagement_BAL.ModelViews.OrdersDTOs;
using MartyrGraveManagement_BAL.ModelViews.PaymentDTOs;
using MartyrGraveManagement_BAL.ModelViews.ScheduleDetailDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceCategoryDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using MartyrGraveManagement_BAL.ModelViews.SlotDTOs;
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

            //Blog mapping

            CreateMap<CreateBlogDTORequest, Blog>().ReverseMap();

            //ScheduleDetail mapping
            CreateMap<ScheduleDetail, ScheduleDetailListDtoResponse>().ReverseMap();
           
            //Slot mapping
            CreateMap<Slot, SlotDtoResponse>().ReverseMap();

            //Comment mapping
                 CreateMap<CommentDTO, Comment>().ReverseMap();
                 CreateMap<CommentIconDTO, Comment_Icon>().ReverseMap();
                 CreateMap<CreateCommentDTO, Comment>().ReverseMap();
                 CreateMap<UpdateCommentDTO, Comment>().ReverseMap();
                 CreateMap<UpdateCommentStatusDTO, Comment>().ReverseMap();

            //CommentIcon mapping
            CreateMap<CreateCommentIconDTO, Comment_Icon>().ReverseMap();
            CreateMap<UpdateCommentIconDTO, Comment_Icon>().ReverseMap();
            //CommentReport mapping
            CreateMap<CommentReportDTO, Comment_Report>().ReverseMap();
            CreateMap<CreateCommentReportDTO, Comment_Report>().ReverseMap();
        }
    }
}