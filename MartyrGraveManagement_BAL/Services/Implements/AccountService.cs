using AutoMapper;
using Azure;
using MartyrGraveManagement_BAL.ModelViews.AccountDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> ChangeStatusUser(int accountId)
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (account == null) {
                    return false;
                }
                if (account.Status == true) { 
                    account.Status = false;
                    await _unitOfWork.AccountRepository.UpdateAsync(account);
                    await _unitOfWork.SaveAsync();
                    return true;
                }
                else
                {
                    account.Status = true;
                    await _unitOfWork.AccountRepository.UpdateAsync(account);
                    await _unitOfWork.SaveAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(List<AccountDtoResponse> managerList, int totalPage)> GetManagerList(int page, int pageSize)
        {
            try
            {
                var totalManager = (await _unitOfWork.AccountRepository.GetAsync(s => s.RoleId == 2)).Count();
                var totalPage = (int)Math.Ceiling(totalManager / (double)pageSize);
                List<AccountDtoResponse> managerList = new List<AccountDtoResponse>();
                var managers = await _unitOfWork.AccountRepository.GetAsync(s => s.RoleId == 2, pageIndex: page, pageSize: pageSize);

                if (managers.Any())
                {
                    foreach (var manager in managers)
                    {
                        var mapper = _mapper.Map<AccountDtoResponse>(manager);
                        managerList.Add(mapper);
                    }

                }
                return (managerList, totalPage);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(List<AccountDtoResponse> staffList, int totalPage)> GetStaffList(int page, int pageSize)
        {
            try
            {
                var totalStaff = (await _unitOfWork.AccountRepository.GetAsync(s => s.RoleId == 3)).Count();
                var totalPage = (int)Math.Ceiling(totalStaff / (double)pageSize);
                List<AccountDtoResponse> staffList = new List<AccountDtoResponse>();
                var staffs = await _unitOfWork.AccountRepository.GetAsync(s => s.RoleId == 3, pageIndex: page, pageSize: pageSize);

                if (staffs.Any())
                {
                    foreach (var staff in staffs)
                    {
                        var mapper = _mapper.Map<AccountDtoResponse>(staff);
                        staffList.Add(mapper);
                    }

                }
                return (staffList, totalPage);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
