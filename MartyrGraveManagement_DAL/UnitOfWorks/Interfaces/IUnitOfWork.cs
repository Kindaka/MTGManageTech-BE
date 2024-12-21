using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace MartyrGraveManagement_DAL.UnitOfWorks.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IGenericRepository<Account> AccountRepository { get; }
        public IGenericRepository<Role> RoleRepository { get; }
        public IGenericRepository<Area> AreaRepository { get; }
        public IGenericRepository<Location> LocationRepository { get; }
        public IGenericRepository<CartItemCustomer> CartItemRepository { get; }
        public IGenericRepository<Order> OrderRepository { get; }
        public IGenericRepository<OrderDetail> OrderDetailRepository { get; }
        public IGenericRepository<Payment> PaymentRepository { get; }
        public IGenericRepository<MartyrGrave> MartyrGraveRepository { get; }
        public IGenericRepository<GraveImage> GraveImageRepository { get; }
        public IGenericRepository<MartyrGraveInformation> MartyrGraveInformationRepository { get; }
        public IGenericRepository<ServiceCategory> ServiceCategoryRepository { get; }
        public IGenericRepository<Service> ServiceRepository { get; }
        public IGenericRepository<StaffTask> TaskRepository { get; }
        public IGenericRepository<Material> MaterialRepository { get; }
        public IGenericRepository<Feedback> FeedbackRepository { get; }
        public IGenericRepository<GraveService> GraveServiceRepository { get; }
        public IGenericRepository<BlogCategory> HistoricalEventRepository { get; }
        public IGenericRepository<Blog> BlogRepository { get; }
        public IGenericRepository<HistoricalImage> HistoricalImageRepository { get; }
        public IGenericRepository<HistoricalRelatedMartyr> HistoricalRelatedMartyrRepository { get; }
        //public IGenericRepository<Slot> SlotRepository { get; }
        public IGenericRepository<ScheduleDetail> ScheduleDetailRepository { get; }
        //public IGenericRepository<Attendance> AttendanceRepository { get; }
        public IGenericRepository<Comment> CommentRepository { get; }
        public IGenericRepository<Comment_Icon> CommentIconRepository { get; }
        public IGenericRepository<Comment_Report> CommentReportRepository { get; }
        public IGenericRepository<Holiday_Event> HolidayEventsRepository { get; }
        public IGenericRepository<Event_Image> EventImagesRepository { get; }
        public IGenericRepository<NotificationAccount> NotificationAccountsRepository { get; }
        public IGenericRepository<Notification> NotificationRepository { get; }
        public IGenericRepository<Material_Service> MaterialServiceRepository { get; }
        public IGenericRepository<BlogCategory> BlogCategoryRepository { get; }
        public IGenericRepository<WorkPerformance> WorkPerformanceRepository { get; }
        public IGenericRepository<CustomerWallet> CustomerWalletRepository { get; }
        public IGenericRepository<TransactionBalanceHistory> TransactionBalanceHistoryRepository { get; }
        public IGenericRepository<TaskImage> TaskImageRepository { get; }
        public IGenericRepository<Service_Schedule> ServiceScheduleRepository { get; }
        public IGenericRepository<AssignmentTask> AssignmentTaskRepository { get; }
        public IGenericRepository<AssignmentTaskImage> AssignmentTaskImageRepository { get; }
        public IGenericRepository<AssignmentTask_Feedback> AssignmentTaskFeedbackRepository { get; }
        public IGenericRepository<RequestType> RequestTypeRepository { get; }
        public IGenericRepository<RequestCustomer> RequestCustomerRepository { get; }
        public IGenericRepository<ReportGrave> ReportGraveRepository { get; }
        public IGenericRepository<ReportImage> ReportImageRepository { get; }
        public IGenericRepository<Request_Material> RequestMaterialRepository { get; }



        Task SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();

    }
}
