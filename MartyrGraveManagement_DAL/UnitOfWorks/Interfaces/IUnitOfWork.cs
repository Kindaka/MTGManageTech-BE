using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public IGenericRepository<Slot> SlotRepository { get; }
        public IGenericRepository<ScheduleDetail> ScheduleDetailRepository { get; }
        public IGenericRepository<Attendance> AttendanceRepository { get; }
        public IGenericRepository<Comment> CommentRepository { get; }
        public IGenericRepository<Comment_Icon> CommentIconRepository { get; }
        public IGenericRepository<Comment_Report> CommentReportRepository { get; }
        public IGenericRepository<Holiday_Event> HolidayEventsRepository { get; }
        public IGenericRepository<Event_Image> EventImagesRepository { get; }
        public IGenericRepository<NotificationAccount> NotificationAccountsRepository { get; }
        public IGenericRepository<Notification> NotificationRepository { get; }


        Task SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();

    }
}
