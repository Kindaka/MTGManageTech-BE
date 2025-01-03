using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.ServiceScheduleDTOs;
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
    public class ServiceSchedule_Service : IServiceSchedule_Service
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssignmentTaskService _assignmentTaskService;
        private readonly IMapper _mapper;
        public ServiceSchedule_Service(IUnitOfWork unitOfWork, IMapper mapper, IAssignmentTaskService assignmentTaskService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _assignmentTaskService = assignmentTaskService;
        }

        public async Task<(bool status, string response)> CreateServiceSchedule(ServiceScheduleDtoRequest request)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Kiểm tra AccountID có tồn tại không
                    var account = await _unitOfWork.AccountRepository.GetByIDAsync(request.AccountId);
                    if (account == null)
                    {
                        return (false, "AccountId không tồn tại.");
                    }
                    var customerWallet = (await _unitOfWork.CustomerWalletRepository.GetAsync(w => w.CustomerId == request.AccountId)).FirstOrDefault();
                    if (customerWallet == null)
                    {
                        return (false, "Tài khoản chưa có ví, hãy tạo trước khi sử dụng dịch vụ này.");
                    }
                    // Kiểm tra ServiceID có tồn tại không
                    var service = await _unitOfWork.ServiceRepository.GetByIDAsync(request.ServiceId);
                    if (service == null)
                    {
                        return (false, "ServiceId không tồn tại.");
                    }
                    else if (service.isScheduleService == false)
                    {
                        return (false, "Không phải là dịch vụ đặt định kì.");
                    }

                    if (customerWallet.CustomerBalance < service.Price)
                    {
                        return (false, "Số dư không đủ để đặt dịch vụ.");
                    }
                    
                    var serviceName = service.ServiceName;
                    // Tìm MartyrGrave dựa trên MartyrId
                    var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(request.MartyrId);
                    if (martyrGrave == null)
                    {
                        return (false, "MartyrId không tồn tại.");
                    }

                    if (martyrGrave.AccountId != account.AccountId)
                    {
                        return (false, "Chỉ có người thân nhân của mộ mới được đặt dịch vụ này.");
                    }
                    // Lấy thông tin Name từ MartyrGraveInformation
                    var martyrInfo = (await _unitOfWork.MartyrGraveInformationRepository.FindAsync(m => m.MartyrId == request.MartyrId)).FirstOrDefault();
                    var martyrName = martyrInfo?.Name ?? "Unknown Martyr";
                    // Kiểm tra nếu GraveService tồn tại cho MartyrId và ServiceId
                    var graveService = (await _unitOfWork.GraveServiceRepository.FindAsync(gs =>
                        gs.MartyrId == request.MartyrId &&
                        gs.ServiceId == request.ServiceId)).FirstOrDefault();
                    if (graveService == null)
                    {
                        return (false, $"Không thể đặt dịch vụ {serviceName} vì nó không khả dụng cho Liệt sĩ {martyrName}.");
                    }

                    // Kiểm tra nếu mục đã tồn tại trong giỏ hàng
                    var existingRecurringServiceSchedule = await _unitOfWork.ServiceScheduleRepository.FindAsync(c =>
                        c.AccountId == request.AccountId &&
                        c.ServiceId == request.ServiceId &&
                        c.MartyrId == request.MartyrId);
                    if (existingRecurringServiceSchedule.Any())
                    {
                        return (false, $"Bạn đã đặt dịch vụ định kì {serviceName} dành cho mộ liệt sĩ {martyrName}. Hãy kiểm tra lại ở dịch vụ định kì.");
                    }
                    var serviceSchedule = new Service_Schedule
                    {
                        AccountId = request.AccountId,
                        ServiceId = request.ServiceId,
                        MartyrId = request.MartyrId,
                        Amount = service.Price,
                        ScheduleDate = DateOnly.FromDateTime(DateTime.Now),
                        Note = request.Note,
                        Status = true
                    };
                    if (service.RecurringType == 1)
                    {
                        if (request.DayOfService >= 1 && request.DayOfService <= 7) //7 là chủ nhật, 1 là thứ 2
                        {
                            serviceSchedule.DayOfWeek = request.DayOfService;
                        }
                        else
                        {
                            return (false, "Chọn sai ngày cho dịch vụ hàng tuần");
                        }
                    }
                    else if (service.RecurringType == 2)
                    {
                        if (request.DayOfService >= 1 && request.DayOfService <= 31)
                        {
                            serviceSchedule.DayOfMonth = request.DayOfService;
                        }
                        else
                        {
                            return (false, "Chọn sai ngày cho dịch vụ hàng tháng");
                        }
                    }
                    await _unitOfWork.ServiceScheduleRepository.AddAsync(serviceSchedule);
                    
                    customerWallet.CustomerBalance = customerWallet.CustomerBalance - service.Price;
                    await _unitOfWork.CustomerWalletRepository.UpdateAsync(customerWallet);

                    var updateBalanceHistory = new TransactionBalanceHistory
                    {
                        CustomerId = customerWallet.CustomerId,
                        TransactionType = "Payment",
                        Amount = -(service.Price),
                        TransactionDate = DateTime.Now,
                        Description = "Bạn đã thanh toán dịch vụ định kì bằng số dư ví",
                        BalanceAfterTransaction = customerWallet.CustomerBalance
                    };

                    await _unitOfWork.TransactionBalanceHistoryRepository.AddAsync(updateBalanceHistory);

                    var check = await _assignmentTaskService.CreateTasksAsync(serviceSchedule.ServiceScheduleId);

                    if (check)
                    {
                        await transaction.CommitAsync();
                        return (true, "Đã đăng kí dịch vụ định kì thành công");
                    }
                    else
                    {
                        return (false, "Giao công việc cho nhân viên thất bại");
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<List<ServiceScheduleDtoResponse>> GetServiceScheduleByAccountId(int accountId)
        {
            try
            {
                var serviceScheduleReponses = new List<ServiceScheduleDtoResponse>();
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (account == null) {
                    return serviceScheduleReponses;
                }
                var serviceSchedules = await _unitOfWork.ServiceScheduleRepository.GetAsync(
                    s => s.AccountId == accountId, 
                    includeProperties: "Service,MartyrGrave,MartyrGrave.Location,MartyrGrave.MartyrGraveInformations");
                
                if (serviceSchedules != null) {
                    foreach (var serviceSchedule in serviceSchedules) { 
                        var item = _mapper.Map<ServiceScheduleDtoResponse>(serviceSchedule);
                        item.ServiceName = serviceSchedule.Service.ServiceName;
                        item.ServiceImage = serviceSchedule.Service.ImagePath;
                        
                        // Thông tin liệt sĩ
                        item.MartyrName = serviceSchedule.MartyrGrave.MartyrGraveInformations.FirstOrDefault()?.Name;
                        
                        // Thông tin vị trí
                        item.RowNumber = serviceSchedule.MartyrGrave.Location.RowNumber;
                        item.MartyrNumber = serviceSchedule.MartyrGrave.Location.MartyrNumber;
                        item.AreaNumber = serviceSchedule.MartyrGrave.Location.AreaNumber;

                        serviceScheduleReponses.Add(item);
                    }
                    return serviceScheduleReponses;
                }
                return serviceScheduleReponses;
            }
            catch (Exception ex) { 
                throw new Exception(ex.Message); 
            }
        }

        public async Task<ServiceScheduleDetailResponse> GetServiceScheduleById(int serviceScheduleId)
        {
            try
            {
                var serviceSchedule = (await _unitOfWork.ServiceScheduleRepository.GetAsync(
                    s => s.ServiceScheduleId == serviceScheduleId,
                    includeProperties: "Service,MartyrGrave,MartyrGrave.Location,Account,MartyrGrave.MartyrGraveInformations,AssignmentTasks,AssignmentTasks.Account,AssignmentTasks.AssignmentTaskImages"))
                    .FirstOrDefault();

                if (serviceSchedule != null)
                {
                    var item = _mapper.Map<ServiceScheduleDetailResponse>(serviceSchedule);
                    item.ServiceName = serviceSchedule.Service.ServiceName;
                    item.ServiceImage = serviceSchedule.Service.ImagePath;

                    // Thông tin liệt sĩ
                    item.MartyrName = serviceSchedule.MartyrGrave.MartyrGraveInformations.FirstOrDefault()?.Name;
                    item.MartyrCode = serviceSchedule.MartyrGrave.MartyrCode;

                    // Thông tin vị trí
                    item.RowNumber = serviceSchedule.MartyrGrave.Location.RowNumber;
                    item.MartyrNumber = serviceSchedule.MartyrGrave.Location.MartyrNumber;
                    item.AreaNumber = serviceSchedule.MartyrGrave.Location.AreaNumber;

                    // Thông tin người đặt lịch
                    item.AccountName = serviceSchedule.Account.FullName;
                    item.PhoneNumber = serviceSchedule.Account.PhoneNumber;

                    // Lấy AssignmentTask mới nhất
                    var latestAssignment = serviceSchedule.AssignmentTasks
                        ?.OrderByDescending(t => t.CreateAt)
                        .FirstOrDefault();

                    if (latestAssignment != null)
                    {
                        item.LatestAssignment = new AssignmentTaskInfo
                        {
                            AssignmentTaskId = latestAssignment.AssignmentTaskId,
                            StaffName = latestAssignment.Account?.FullName,
                            PhoneNumber = latestAssignment.Account?.PhoneNumber,
                            ImageWorkSpace = latestAssignment.ImageWorkSpace,
                            Status = latestAssignment.Status,
                            TaskImages = latestAssignment.AssignmentTaskImages
                                ?.Select(i => new AssignmentTaskImageDto
                                {
                                    ImagePath = i.ImagePath,
                                    CreateAt = i.CreateAt 
                                })
                                .ToList() ?? new List<AssignmentTaskImageDto>()
                        };
                    }

                    return item;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
