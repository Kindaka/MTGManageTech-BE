using AutoMapper;
using MartyrGraveManagement_BAL.MLModels;
using MartyrGraveManagement_BAL.ModelViews.AccountDTOs;
using MartyrGraveManagement_BAL.ModelViews.AreaDTOs;
using MartyrGraveManagement_BAL.ModelViews.AssignmentTaskDTOs;
using MartyrGraveManagement_BAL.ModelViews.AssignmentTaskFeedbackDTOs;
using MartyrGraveManagement_BAL.ModelViews.BlogCategoryDTOs;
using MartyrGraveManagement_BAL.ModelViews.BlogDTOs;
using MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs;
using MartyrGraveManagement_BAL.ModelViews.CommentDTOs;
using MartyrGraveManagement_BAL.ModelViews.CommentIconDTOs;
using MartyrGraveManagement_BAL.ModelViews.CommentReportDTOs;
using MartyrGraveManagement_BAL.ModelViews.CustomerDTOs;
using MartyrGraveManagement_BAL.ModelViews.CustomerWalletDTOs;
using MartyrGraveManagement_BAL.ModelViews.FeedbackDTOs;
using MartyrGraveManagement_BAL.ModelViews.HistoricalEventDTOs;
using MartyrGraveManagement_BAL.ModelViews.HolidayEventDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveInformationDTOs;
using MartyrGraveManagement_BAL.ModelViews.OrdersDTOs;
using MartyrGraveManagement_BAL.ModelViews.PaymentDTOs;
using MartyrGraveManagement_BAL.ModelViews.ScheduleDetailDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceCategoryDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceScheduleDTOs;
using MartyrGraveManagement_BAL.ModelViews.SlotDTOs;
using MartyrGraveManagement_BAL.ModelViews.StaffPerformanceDTOs;
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

            //AssignmentTaskFeeback mapping
            CreateMap<AssignmentTaskFeedbackDtoRequest, AssignmentTask_Feedback>().ReverseMap();
            CreateMap<AssignmentTask_Feedback, AssignmentTaskFeedbackDtoResponse>().ReverseMap();
            CreateMap<AssignmentTask_Feedback, AssignmentTaskFeedbackContentDtoRequest>().ReverseMap();
            CreateMap<AssignmentTask_Feedback, AssignmentTaskFeedbackResponseDtoRequest>().ReverseMap();

            //HistoricalEvent mapping
            CreateMap<CreateHistoricalEventDTORequest, BlogCategory>().ReverseMap();
            CreateMap<HistoricalEventDTOResponse, BlogCategory>().ReverseMap();

            //Blog mapping

            CreateMap<CreateBlogDTORequest, Blog>().ReverseMap();

            //ScheduleDetail mapping
            CreateMap<ScheduleDetail, ScheduleDetailListDtoResponse>().ReverseMap();
           
            //Slot mapping
            //CreateMap<Slot, SlotDtoResponse>().ReverseMap();

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
            // Holiday Event mapping
            CreateMap<HolidayEventRequestDto, Holiday_Event>().ReverseMap();
            
            // BlogCategory mapping
            CreateMap<BlogCategoryDtoResponse, BlogCategory>().ReverseMap();
            CreateMap<BlogCategoryDtoRequest, BlogCategory>().ReverseMap();
            CreateMap<BlogDtoResponse, Blog>().ReverseMap();

            //StaffPerformanace mapping
            CreateMap<StaffPerformanceRequest, WorkPerformance>();

            CreateMap<WorkPerformance, WorkPerformanceDTO>()
                .ForMember(dest => dest.OverallPoint, opt => opt.MapFrom(src =>
                    Math.Round(
                        (src.QualityMaintenancePoint * 0.4) +
                        (src.TimeCompletePoint * 0.3) +
                        (src.InteractionPoint * 0.3),
                        2)))
                .ForMember(dest => dest.PerformanceLevel, opt => opt.MapFrom(src =>
                    GetPerformanceLevel(
                        (float)((src.QualityMaintenancePoint * 0.4) +
                        (src.TimeCompletePoint * 0.3) +
                        (src.InteractionPoint * 0.3)))))
                .ForMember(dest => dest.AccountFullName, opt => opt.MapFrom(src =>
                    src.Account != null ? src.Account.FullName : string.Empty))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src =>
                    src.Account != null ? src.Account.PhoneNumber : string.Empty));

            CreateMap<WorkPerformanceDTO, WorkPerformance>()
                .ForMember(dest => dest.Account, opt => opt.Ignore());

            // Thêm mapping cho PerformanceMetrics
            CreateMap<PerformanceMetrics, PerformanceMetricsDTO>();
            CreateMap<PerformanceMetricsDTO, PerformanceMetrics>();


            // Wallet mappings
            CreateMap<CustomerWallet, CustomerWalletDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src =>
                    src.Account != null ? src.Account.FullName : string.Empty));

            // Transaction History mappings
            CreateMap<TransactionBalanceHistory, TransactionBalanceHistoryDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src =>
                    src.Account != null ? src.Account.FullName : string.Empty));

            // ServiceSchedule
            CreateMap<Service_Schedule, ServiceScheduleDtoResponse>().ReverseMap();
            

            // AssignmentTask mappings
            CreateMap<TaskDtoResponse, AssignmentTask>().ReverseMap();

            CreateMap<AssignmentTask, AssignmentTaskResponse>()
                // Staff info
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => 
                    src.Account != null ? src.Account.FullName : string.Empty))
                .ForMember(dest => dest.StaffPhone, opt => opt.MapFrom(src => 
                    src.Account != null ? src.Account.PhoneNumber : string.Empty))
                
                // Service Schedule info
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => 
                    src.Service_Schedule != null ? src.Service_Schedule.AccountId : 0))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => 
                    src.Service_Schedule != null && src.Service_Schedule.Account != null 
                        ? src.Service_Schedule.Account.FullName : string.Empty))
                .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => 
                    src.Service_Schedule != null && src.Service_Schedule.Account != null 
                        ? src.Service_Schedule.Account.PhoneNumber : string.Empty))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => 
                    src.Service_Schedule != null ? src.Service_Schedule.Amount : 0))
                .ForMember(dest => dest.ScheduleDate, opt => opt.MapFrom(src => 
                    src.Service_Schedule != null ? src.Service_Schedule.ScheduleDate : default(DateOnly)))
                .ForMember(dest => dest.DayOfMonth, opt => opt.MapFrom(src => 
                    src.Service_Schedule != null ? src.Service_Schedule.DayOfMonth : 0))
                .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => 
                    src.Service_Schedule != null ? src.Service_Schedule.DayOfWeek : 0))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => 
                    src.Service_Schedule != null ? src.Service_Schedule.Note : string.Empty))
                
                // Service info
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => 
                    src.Service_Schedule != null && src.Service_Schedule.Service != null 
                        ? src.Service_Schedule.Service.ServiceName : string.Empty))
                .ForMember(dest => dest.ServiceDescription, opt => opt.MapFrom(src => 
                    src.Service_Schedule != null && src.Service_Schedule.Service != null 
                        ? src.Service_Schedule.Service.Description : string.Empty))
                .ForMember(dest => dest.ServiceImage, opt => opt.MapFrom(src => 
                    src.Service_Schedule != null && src.Service_Schedule.Service != null 
                        ? src.Service_Schedule.Service.ImagePath : string.Empty))
                .ForMember(dest => dest.RecurringType, opt => opt.MapFrom(src => 
                    src.Service_Schedule != null && src.Service_Schedule.Service != null 
                        ? src.Service_Schedule.Service.RecurringType : 0))
                
                // Task images
                .ForMember(dest => dest.TaskImages, opt => opt.MapFrom(src => 
                    src.AssignmentTaskImages != null 
                        ? src.AssignmentTaskImages.Select(i => i.ImagePath).ToList() 
                        : new List<string>()));

            CreateMap<AssignmentTaskStatusUpdateDTO, AssignmentTask>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason));

            CreateMap<AssignmentTaskImageUpdateDTO, AssignmentTask>()
                .ForMember(dest => dest.ImageWorkSpace, opt => opt.MapFrom(src => src.ImageWorkSpace));

            // ServiceScheduleDetailResponse mapping
            CreateMap<Service_Schedule, ServiceScheduleDetailResponse>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.ServiceName))
                .ForMember(dest => dest.ServiceImage, opt => opt.MapFrom(src => src.Service.ImagePath))
                .ForMember(dest => dest.MartyrName, opt => opt.MapFrom(src => src.MartyrGrave.MartyrGraveInformations.FirstOrDefault().Name))
                .ForMember(dest => dest.MartyrCode, opt => opt.MapFrom(src => src.MartyrGrave.MartyrCode))
                .ForMember(dest => dest.RowNumber, opt => opt.MapFrom(src => src.MartyrGrave.Location.RowNumber))
                .ForMember(dest => dest.MartyrNumber, opt => opt.MapFrom(src => src.MartyrGrave.Location.MartyrNumber))
                .ForMember(dest => dest.AreaNumber, opt => opt.MapFrom(src => src.MartyrGrave.Location.AreaNumber))
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber))
                .ForMember(dest => dest.LatestAssignment, opt => opt.MapFrom(src => src.AssignmentTasks.OrderByDescending(t => t.CreateAt).FirstOrDefault()));

            CreateMap<AssignmentTask, AssignmentTaskInfo>()
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Account.FullName))
                .ForMember(dest => dest.ImageWorkSpace, opt => opt.MapFrom(src => src.ImageWorkSpace))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.TaskImages, opt => opt.MapFrom(src => src.AssignmentTaskImages.Select(i => i.ImagePath).ToList()));
        }
        // Thêm helper method vào class MappingProfile
        private static string GetPerformanceLevel(float score)
        {
            if (score >= 90) return "Xuất sắc";
            if (score >= 80) return "Tốt";
            if (score >= 70) return "Khá";
            if (score >= 60) return "Trung bình";
            return "Cần cải thiện";
        }
    }
}