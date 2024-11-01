using MartyrGraveManagement_DAL.Entities;
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
        private GenericRepository<HistoricalEvent> _historicalEventRepository;
        private GenericRepository<Blog> _blogRepository;
        private GenericRepository<HistoricalImage> _historicalImageRepository;
        private GenericRepository<HistoricalRelatedMartyr> _historicalRelatedMartyrRepository;
        private GenericRepository<Schedule_Staff> _scheduleRepository;
        private GenericRepository<Slot> _slotRepository;


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
        public IGenericRepository<HistoricalEvent> HistoricalEventRepository => _historicalEventRepository ??= new GenericRepository<HistoricalEvent>(_context);
        public IGenericRepository<Blog> BlogRepository => _blogRepository ??= new GenericRepository<Blog>(_context);
        public IGenericRepository<HistoricalImage> HistoricalImageRepository => _historicalImageRepository ??= new GenericRepository<HistoricalImage>(_context);
        public IGenericRepository<HistoricalRelatedMartyr> HistoricalRelatedMartyrRepository => _historicalRelatedMartyrRepository ??= new GenericRepository<HistoricalRelatedMartyr>(_context);
        public IGenericRepository<Schedule_Staff> ScheduleRepository => _scheduleRepository ??= new GenericRepository<Schedule_Staff>(_context);
        public IGenericRepository<Slot> SlotRepository => _slotRepository ??= new GenericRepository<Slot>(_context);


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
