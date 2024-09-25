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
        Task SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();

    }
}
