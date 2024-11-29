﻿using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.Repositories.Implements;
using MartyrGraveManagement_DAL.Repositories.Interfaces;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.UnitOfWorks.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _context;
        private GenericRepository<Account> _accountRepository;
        private GenericRepository<Role> _roleRepository;
        private GenericRepository<Area> _areaRepository;
        private GenericRepository<Location> _locationRepository;
        private GenericRepository<MartyrGrave> _martyrGraveRepository;
        private GenericRepository<GraveImage> _graveImageRepository;
        private GenericRepository<MartyrGraveInformation> _martyrGraveInformationRepository;
        private GenericRepository<ServiceCategory> _serviceCategoryRepository;
        private GenericRepository<Service> _serviceRepository;
        private GenericRepository<CartItemCustomer> _cartItemRepository;

        private GenericRepository<Order> _orderRepository;
        private GenericRepository<OrderDetail> _orderDetailRepository;
        private GenericRepository<Payment> _paymentRepository;

        private GenericRepository<StaffTask> _taskRepository;
        private GenericRepository<Material> _materialRepository;
        private GenericRepository<Feedback> _feedbackRepository;
        private GenericRepository<GraveService> _graveServiceRepository;
        private GenericRepository<BlogCategory> _historicalEventRepository;
        private GenericRepository<Blog> _blogRepository;
        private GenericRepository<HistoricalImage> _historicalImageRepository;
        private GenericRepository<HistoricalRelatedMartyr> _historicalRelatedMartyrRepository;
        //private GenericRepository<Slot> _slotRepository;
        private GenericRepository<ScheduleDetail> _scheduleDetailRepository;
        //private GenericRepository<Attendance> _attendanceRepository;
        private GenericRepository<Comment> _commentRepository;
        private GenericRepository<Comment_Icon> _commentIconRepository;
        private GenericRepository<Comment_Report> _commentReportRepository;
        private GenericRepository<Holiday_Event> _holidayEventsRepository;
        private GenericRepository<Event_Image> _eventImagesRepository;
        private GenericRepository<Notification> _notificationRepository;
        private GenericRepository<NotificationAccount> _notificationAccountsRepository;
        private GenericRepository<Material_Service> _materialServiceRepository;
        private GenericRepository<BlogCategory> _blogCategoryRepository;
        private GenericRepository<WorkPerformance> _workPerformanceRepository;
        private GenericRepository<CustomerWallet> _customerWalletRepository;
        private GenericRepository<TransactionBalanceHistory> _transactionBalanceHistoryRepository;
        private GenericRepository<TaskImage> _taskImageRepository;
        private GenericRepository<Service_Schedule> _serviceScheduleRepository;
        private GenericRepository<AssignmentTask> _assignmentTaskRepository;
        private GenericRepository<AssignmentTaskImage> _assignmentTaskImageRepository;


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Account> AccountRepository => _accountRepository ??= new GenericRepository<Account>(_context);
        public IGenericRepository<Role> RoleRepository => _roleRepository ??= new GenericRepository<Role>(_context);
        public IGenericRepository<Area> AreaRepository => _areaRepository ??= new GenericRepository<Area>(_context);
        public IGenericRepository<Location> LocationRepository => _locationRepository ??= new GenericRepository<Location>(_context);
        public IGenericRepository<MartyrGrave> MartyrGraveRepository => _martyrGraveRepository ??= new GenericRepository<MartyrGrave>(_context);
        public IGenericRepository<GraveImage> GraveImageRepository => _graveImageRepository ??= new GenericRepository<GraveImage>(_context);
        public IGenericRepository<MartyrGraveInformation> MartyrGraveInformationRepository => _martyrGraveInformationRepository ??= new GenericRepository<MartyrGraveInformation>(_context);
        public IGenericRepository<ServiceCategory> ServiceCategoryRepository => _serviceCategoryRepository ??= new GenericRepository<ServiceCategory>(_context);
        public IGenericRepository<Service> ServiceRepository => _serviceRepository ??= new GenericRepository<Service>(_context);
        public IGenericRepository<Feedback> FeedbackRepository => _feedbackRepository ??= new GenericRepository<Feedback>(_context);
        public IGenericRepository<CartItemCustomer> CartItemRepository => _cartItemRepository ??= new GenericRepository<CartItemCustomer>(_context);
        public IGenericRepository<StaffTask> TaskRepository => _taskRepository ??= new GenericRepository<StaffTask>(_context);
        public IGenericRepository<Material> MaterialRepository => _materialRepository ??= new GenericRepository<Material>(_context);

        public IGenericRepository<Order> OrderRepository => _orderRepository ??= new GenericRepository<Order>(_context);

        public IGenericRepository<OrderDetail> OrderDetailRepository => _orderDetailRepository ??= new GenericRepository<OrderDetail>(_context);

        public IGenericRepository<Payment> PaymentRepository => _paymentRepository ??= new GenericRepository<Payment>(_context);
        public IGenericRepository<GraveService> GraveServiceRepository => _graveServiceRepository ??= new GenericRepository<GraveService>(_context);
        public IGenericRepository<BlogCategory> HistoricalEventRepository => _historicalEventRepository ??= new GenericRepository<BlogCategory>(_context);
        public IGenericRepository<Blog> BlogRepository => _blogRepository ??= new GenericRepository<Blog>(_context);
        public IGenericRepository<HistoricalImage> HistoricalImageRepository => _historicalImageRepository ??= new GenericRepository<HistoricalImage>(_context);
        public IGenericRepository<HistoricalRelatedMartyr> HistoricalRelatedMartyrRepository => _historicalRelatedMartyrRepository ??= new GenericRepository<HistoricalRelatedMartyr>(_context);
        //public IGenericRepository<Slot> SlotRepository => _slotRepository ??= new GenericRepository<Slot>(_context);
        public IGenericRepository<ScheduleDetail> ScheduleDetailRepository => _scheduleDetailRepository ??= new GenericRepository<ScheduleDetail>(_context);
        //public IGenericRepository<Attendance> AttendanceRepository => _attendanceRepository ??= new GenericRepository<Attendance>(_context);
        public IGenericRepository<Comment> CommentRepository => _commentRepository ??= new GenericRepository<Comment>(_context);
        public IGenericRepository<Comment_Icon> CommentIconRepository => _commentIconRepository ??= new GenericRepository<Comment_Icon>(_context);
        public IGenericRepository<Comment_Report> CommentReportRepository => _commentReportRepository ??= new GenericRepository<Comment_Report>(_context);
        public IGenericRepository<Holiday_Event> HolidayEventsRepository => _holidayEventsRepository ??= new GenericRepository<Holiday_Event>(_context);
        public IGenericRepository<Event_Image> EventImagesRepository => _eventImagesRepository ??= new GenericRepository<Event_Image>(_context);
        public IGenericRepository<Notification> NotificationRepository => _notificationRepository ??= new GenericRepository<Notification>(_context);
        public IGenericRepository<NotificationAccount> NotificationAccountsRepository => _notificationAccountsRepository ??= new GenericRepository<NotificationAccount>(_context);
        public IGenericRepository<Material_Service> MaterialServiceRepository => _materialServiceRepository ??= new GenericRepository<Material_Service>(_context);
        public IGenericRepository<BlogCategory> BlogCategoryRepository => _blogCategoryRepository ??= new GenericRepository<BlogCategory>(_context);
        public IGenericRepository<WorkPerformance> WorkPerformanceRepository => _workPerformanceRepository ??= new GenericRepository<WorkPerformance>(_context);
        public IGenericRepository<CustomerWallet> CustomerWalletRepository => _customerWalletRepository ??= new GenericRepository<CustomerWallet>(_context);
        public IGenericRepository<TransactionBalanceHistory> TransactionBalanceHistoryRepository => _transactionBalanceHistoryRepository ??= new GenericRepository<TransactionBalanceHistory>(_context);
        public IGenericRepository<TaskImage> TaskImageRepository => _taskImageRepository ??= new GenericRepository<TaskImage>(_context);
        public IGenericRepository<Service_Schedule> ServiceScheduleRepository => _serviceScheduleRepository ??= new GenericRepository<Service_Schedule>(_context);
        public IGenericRepository<AssignmentTask> AssignmentTaskRepository => _assignmentTaskRepository ??= new GenericRepository<AssignmentTask>(_context);
        public IGenericRepository<AssignmentTaskImage> AssignmentTaskImageRepository => _assignmentTaskImageRepository ??= new GenericRepository<AssignmentTaskImage>(_context);


        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
