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
        public IGenericRepository<CartItem> CartItemRepository { get; }
        public IGenericRepository<Order> OrderRepository { get; }
        public IGenericRepository<OrderDetail> OrderDetailRepository { get; }
        public IGenericRepository<Payment> PaymentRepository { get; }
        public IGenericRepository<MartyrGrave> MartyrGraveRepository { get; }
        public IGenericRepository<GraveImage> GraveImageRepository { get; }
        public IGenericRepository<MartyrGraveInformation> MartyrGraveInformationRepository { get; }
        public IGenericRepository<ServiceCategory> ServiceCategoryRepository { get; }
        public IGenericRepository<Service> ServiceRepository { get; }
        public IGenericRepository<StaffTask> TaskRepository { get; }
        public IGenericRepository<StaffJob> JobRepository { get; }
        public IGenericRepository<Material> MaterialRepository { get; }


        Task SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();

    }
}
