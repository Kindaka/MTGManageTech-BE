﻿using AutoMapper;
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

        public async Task<bool> ChangeStatusUser(int accountId, int userAccountId)
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                var userAccount = await _unitOfWork.AccountRepository.GetByIDAsync(userAccountId);
                if (account == null || userAccount == null) {
                    return false;
                }
                if ((account.RoleId == 4 && userAccount.RoleId == 3) || (account.RoleId == 3 && userAccount.RoleId == 2) || (account.RoleId==2 && userAccountId ==1))
                {
                    if (account.Status == true)
                    {
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
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AccountDtoResponse> GetAccountProfile(int accountId)
        {
            try
            {
                var account = (await _unitOfWork.AccountRepository.GetAsync(a => a.AccountId == accountId, includeProperties: "Role")).FirstOrDefault();
                if (account != null) {
                    var accountResponse = new AccountDtoResponse()
                    {
                        AccountId = account.AccountId,
                        RoleName = account.Role.RoleName,
                        AreaId = account.AreaId,
                        FullName = account.FullName,
                        DateOfBirth = account.DateOfBirth,
                        phoneNumber = account.PhoneNumber,
                        EmailAddress = account.EmailAddress,
                        Address = account.Address,
                        AvatarPath = account.AvatarPath,
                        CreateAt = account.CreateAt,
                        RoleId = account.RoleId,
                        Status = account.Status
                    };
                    return accountResponse;
                }
                return null;
            }
            catch (Exception ex) {
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

        public async Task<(List<AccountDtoResponse> staffList, int totalPage)> GetStaffList(int page, int pageSize, int? areaId = null)
        {
            try
            {
                // Lấy danh sách nhân viên
                var staffs = await _unitOfWork.AccountRepository.GetAllAsync(
                    filter: a => a.RoleId == 3 && (!areaId.HasValue || a.AreaId == areaId.Value),
                    pageIndex: page,
                    pageSize: pageSize
                );

                // Lấy tổng số nhân viên thỏa mãn điều kiện
                var totalStaff = await _unitOfWork.AccountRepository.CountAsync(a => a.RoleId == 3 && (!areaId.HasValue || a.AreaId == areaId.Value));
                var totalPage = (int)Math.Ceiling(totalStaff / (double)pageSize);

                // Chuyển đổi sang DTO
                var staffList = staffs.Select(staff => _mapper.Map<AccountDtoResponse>(staff)).ToList();

                return (staffList, totalPage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Dictionary<int, int>> GetTotalAccountsByRolesAsync(IEnumerable<int> roleIds)
        {
            try
            {
                // Initialize a dictionary to store counts for each RoleId
                var roleCounts = new Dictionary<int, int>();

                foreach (var roleId in roleIds)
                {
                    // Use FindAsync to get accounts for each role and count them
                    var accounts = await _unitOfWork.AccountRepository.FindAsync(account => account.RoleId == roleId);
                    roleCounts[roleId] = accounts.Count();
                }

                return roleCounts;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching account counts for roles: {string.Join(", ", roleIds)}. {ex.Message}");
            }
        }


        public async Task<bool> UpdateProfileForStaffOrManager(int accountId, UpdateProfileStaffOrManagerDtoRequest updateProfileDto)
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (account == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy tài khoản.");
                }

                if (account.RoleId != 2 && account.RoleId != 3)
                {
                    throw new UnauthorizedAccessException("Chỉ có tài khoản nhân viên hoặc quản lý mới được phép cập nhật thông tin.");
                }

                // Cập nhật các thông tin của nhân viên hoặc quản lý
                if (!string.IsNullOrEmpty(updateProfileDto.FullName))
                {
                    account.FullName = updateProfileDto.FullName;
                }

                if (updateProfileDto.DateOfBirth != null)
                {
                    account.DateOfBirth = updateProfileDto.DateOfBirth;
                }

                if (!string.IsNullOrEmpty(updateProfileDto.Address))
                {
                    account.Address = updateProfileDto.Address;
                }

                if (!string.IsNullOrEmpty(updateProfileDto.AvatarPath))
                {
                    account.AvatarPath = updateProfileDto.AvatarPath;
                }

                if (!string.IsNullOrEmpty(updateProfileDto.EmailAddress))
                {
                    account.EmailAddress = updateProfileDto.EmailAddress;
                }

                // Kiểm tra và cập nhật AreaId nếu có
                if (account.RoleId == 2)
                {
                    if (updateProfileDto.AreaId.HasValue)
                    {
                        var area = await _unitOfWork.AreaRepository.GetByIDAsync(updateProfileDto.AreaId.Value);
                        if (area == null)
                        {
                            throw new KeyNotFoundException("Khu vực không tồn tại.");
                        }

                        if (!area.Status)
                        {
                            throw new InvalidOperationException("Khu vực không còn hiệu lực.");
                        }

                        // Chỉ cập nhật AreaId nếu khu vực tồn tại
                        account.AreaId = updateProfileDto.AreaId;
                    }
                }

                // Lưu thông tin đã cập nhật vào cơ sở dữ liệu
                await _unitOfWork.AccountRepository.UpdateAsync(account);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Cập nhật thông tin thất bại: {ex.Message}");
            }
        }


    }
}
