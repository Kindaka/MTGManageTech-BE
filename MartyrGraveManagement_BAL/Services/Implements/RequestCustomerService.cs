using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.RequestCustomerDTOs;
using MartyrGraveManagement_BAL.ModelViews.RequestMaterialDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using static MartyrGraveManagement_BAL.ModelViews.RequestCustomerDTOs.RequestCustomerDtoResponse;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class RequestCustomerService : IRequestCustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public RequestCustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(bool status, string response)> AcceptRequestForManagerAsync(RequestCustomerDtoManagerResponse dtoManagerResponse)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Kiểm tra AccountID có tồn tại không
                    var manager = await _unitOfWork.AccountRepository.GetByIDAsync(dtoManagerResponse.ManagerId);
                    if (manager == null || manager.RoleId != 2)
                    {
                        return (false, "AccountId không tồn tại hoặc bạn không có quyền.");
                    }
                    var request = (await _unitOfWork.RequestCustomerRepository.GetAsync(r => r.RequestId == dtoManagerResponse.RequestId, includeProperties: "MartyrGrave")).FirstOrDefault();
                    if (request == null)
                    {
                        return (false, "Request không tồn tại.");
                    }
                    if (request.MartyrGrave.AreaId != manager.AreaId)
                    {
                        return (false, "Request không thuộc khu vực của bạn.");
                    }
                    var requestType = await _unitOfWork.RequestTypeRepository.GetByIDAsync(request.TypeId);
                    if (dtoManagerResponse.status == true)
                    {
                        if (requestType.TypeId == 1)
                        {
                            var staffAccounts = await _unitOfWork.AccountRepository
                                            .FindAsync(a => a.RoleId == 3 && a.AreaId == request.MartyrGrave.AreaId);

                            if (!staffAccounts.Any())
                            {
                                return (false, "Trong khu vực không tồn tại nhân viên, không thể thực hiện giao việc.");
                            }

                            // Sắp xếp nhân viên theo số lượng công việc hiện tại
                            var staffWorkloads = new Dictionary<int, int>();
                            foreach (var staff in staffAccounts)
                            {
                                var taskCount = await _unitOfWork.ReportGraveRepository
                                    .CountAsync(t => t.StaffId == staff.AccountId);
                                staffWorkloads[staff.AccountId] = taskCount;
                            }

                            var selectedStaff = staffAccounts
                                .OrderBy(staff => staffWorkloads[staff.AccountId])
                                .First();
                            var reportGrave = new ReportGrave
                            {
                                RequestId = request.RequestId,
                                StaffId = selectedStaff.AccountId,
                                Description = request.Note,
                                CreateAt = DateTime.Now,
                                UpdateAt = DateTime.Now,
                                Status = false,
                            };
                            await _unitOfWork.ReportGraveRepository.AddAsync(reportGrave);
                            request.Status = 2;
                            await _unitOfWork.RequestCustomerRepository.UpdateAsync(request);


                        }
                        else if (requestType.TypeId == 2)
                        {
                            if (dtoManagerResponse.MaterialIds.Any())
                            {
                                decimal expectedPrice = 0;
                                foreach (var materialId in dtoManagerResponse.MaterialIds)
                                {
                                    var material = await _unitOfWork.MaterialRepository.GetByIDAsync(materialId);
                                    if (material == null)
                                    {
                                        continue;
                                    }
                                    var existedRequestMaterial = (await _unitOfWork.RequestMaterialRepository.GetAsync(r => r.RequestId == request.RequestId && r.MaterialId == materialId)).FirstOrDefault();
                                    if (existedRequestMaterial != null)
                                    {
                                        continue;
                                    }
                                    expectedPrice += material.Price;
                                    var request_Material = new Request_Material
                                    {
                                        RequestId = request.RequestId,
                                        MaterialId = materialId,
                                        CreatedAt = DateTime.Now,
                                    };
                                    await _unitOfWork.RequestMaterialRepository.AddAsync(request_Material);
                                }
                                if (expectedPrice > 10000)
                                {
                                    request.Price = expectedPrice + (expectedPrice * 0.05m);
                                    request.Status = 4;
                                    await _unitOfWork.RequestCustomerRepository.UpdateAsync(request);
                                }
                                else
                                {
                                    return (false, "Giá dịch vụ phải lớn hơn 10000 mới thực hiện được.");
                                }
                            }
                            else
                            {
                                return (false, "Phải thêm ít nhất một vật liệu để báo giá dịch vụ");
                            }
                        }
                        else if (requestType.TypeId == 3)
                        {

                            if (request.ServiceId != null )
                            {
                                var duplicateRecords = await _unitOfWork.GraveServiceRepository
                                 .FindAsync(gs => gs.ServiceId == request.ServiceId && gs.MartyrId == request.MartyrId);

                                if (duplicateRecords.Any())
                                {
                                    return (false, "Dịch vụ đã tồn tại cho liệt sĩ này.");
                                }



                                var graveService = new GraveService
                                {
                                    MartyrId = request.MartyrId,
                                    ServiceId = (int)request.ServiceId,
                                    CreatedDate = DateTime.Now,
                                };
                                await _unitOfWork.GraveServiceRepository.AddAsync(graveService);
                                request.Status = 2;
                                await _unitOfWork.RequestCustomerRepository.UpdateAsync(request);
                            }
                            else
                            {
                                return (false, "Dịch vụ không tìm thấy");
                            }
                        }
                        await transaction.CommitAsync();
                        return (true, "Đã duyệt yêu cầu thành công");
                    }
                    else
                    {
                        if (dtoManagerResponse.Note == null)
                        {
                            return (false, "Từ chối yêu cầu phải ghi rõ nguyên nhân.");
                        }
                        var noteHistory = new RequestNoteHistory
                        {
                            RequestId = request.RequestId,
                            AccountId = manager.AccountId,
                            Note = dtoManagerResponse.Note,
                            CreateAt = DateTime.Now,
                            UpdateAt = DateTime.Now,
                            Status = true
                        };
                        await _unitOfWork.RequestNoteHistoryRepository.AddAsync(noteHistory);
                        request.Status = 3;
                        await _unitOfWork.RequestCustomerRepository.UpdateAsync(request);
                        await transaction.CommitAsync();
                        return (true, "Đã từ chối yêu cầu");
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<(bool status, string response)> AcceptServiceRequestForCustomerAsync(int requestId, int customerId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var customerWallet = (await _unitOfWork.CustomerWalletRepository.GetAsync(c => c.CustomerId == customerId)).FirstOrDefault();
                    if (customerWallet == null)
                    {
                        return (false, $"Khách hàng {customerId} không tìm thấy ví.");
                    }

                    var request = (await _unitOfWork.RequestCustomerRepository.GetAsync(r => r.RequestId == requestId, includeProperties: "MartyrGrave")).FirstOrDefault();
                    if (request == null)
                    {
                        return (false, "Request không tồn tại.");
                    }

                    if (request.TypeId != 2)
                    {
                        return (false, "Đây không phải là yêu cầu đặt dịch vụ.");
                    }

                    if (request.Price < 10000)
                    {
                        return (false, "Số tiền để thực hiện phải lớn hơn 10000.");
                    }

                    // Kiểm tra số dư ví của khách hàng
                    if (customerWallet.CustomerBalance < request.Price)
                    {
                        return (false, $"Khách hàng {customerWallet.CustomerId} không đủ số dư để thực hiện yêu cầu.");
                    }

                    // Tính toán số dư sau khi thanh toán
                    decimal balanceAfterTransaction = customerWallet.CustomerBalance - request.Price;

                    // Trừ số dư ví của khách hàng
                    customerWallet.CustomerBalance = balanceAfterTransaction;
                    await _unitOfWork.CustomerWalletRepository.UpdateAsync(customerWallet);

                    // Lưu lịch sử giao dịch
                    var transactionHistory = new TransactionBalanceHistory
                    {
                        CustomerId = customerWallet.CustomerId,
                        Amount = -(request.Price),
                        TransactionDate = DateTime.Now,
                        BalanceAfterTransaction = balanceAfterTransaction, // Cập nhật số dư sau giao dịch
                        Description = $"Thanh toán dịch vụ theo yêu cầu khách hàng. Mã yêu cầu: {request.RequestId}",
                        TransactionType = "Payment" // Loại giao dịch: Trừ tiền
                    };
                    await _unitOfWork.TransactionBalanceHistoryRepository.AddAsync(transactionHistory);

                    // Lấy thông tin MartyrGrave liên quan
                    var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(request.MartyrId);
                    if (martyrGrave == null)
                    {
                        return (false, "Không tìm thấy mộ");
                    }

                    // Lấy danh sách nhân viên phù hợp
                    var staffAccounts = await _unitOfWork.AccountRepository
                        .FindAsync(a => a.RoleId == 3 && a.AreaId == martyrGrave.AreaId);

                    if (!staffAccounts.Any())
                    {
                        return (false, "Không có nhân viên trong khu vực.");
                    }

                    // Sắp xếp nhân viên theo số lượng công việc hiện tại
                    var staffWorkloads = new Dictionary<int, int>();
                    foreach (var staff in staffAccounts)
                    {
                        var taskCount = await _unitOfWork.ReportGraveRepository
                            .CountAsync(t => t.StaffId == staff.AccountId);
                        staffWorkloads[staff.AccountId] = taskCount;
                    }

                    var selectedStaff = staffAccounts
                        .OrderBy(staff => staffWorkloads[staff.AccountId])
                        .First();

                    // Tạo công việc mới với ngày kết thúc là `nextServiceDate`
                    var taskEntity = new RequestTask
                    {
                        StaffId = selectedStaff.AccountId,
                        RequestId = request.RequestId,
                        StartDate = DateOnly.FromDateTime(DateTime.Now),
                        EndDate = (DateOnly)request.EndDate, // Ngày hoàn thành công việc
                        Description = request.Note,
                        CreateAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        Status = 1 // Trạng thái ban đầu
                    };

                    await _unitOfWork.RequestTaskRepository.AddAsync(taskEntity);
                    request.Status = 5; // Khách hàng đã đồng ý yêu cầu dịch vụ
                    await _unitOfWork.RequestCustomerRepository.UpdateAsync(request);
                    await transaction.CommitAsync();

                    return (true, "Bạn đã chấp nhận dịch vụ thành công, hãy kiểm tra lại.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }


        public async Task<(bool status, string response)> CreateRequestsAsync(RequestCustomerDtoRequest request)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Kiểm tra AccountID có tồn tại không
                    var account = await _unitOfWork.AccountRepository.GetByIDAsync(request.CustomerId);
                    if (account == null)
                    {
                        return (false, "AccountId không tồn tại.");
                    }
                    var requestType = await _unitOfWork.RequestTypeRepository.GetByIDAsync(request.TypeId);
                    if (requestType == null)
                    {
                        return (false, "RequestType không tồn tại");
                    }
                    if ((requestType.TypeId == 1 || requestType.TypeId == 2) && request.CompleteDate == DateTime.MinValue)
                    {
                        return (false, "Loại yêu cầu này cần thời gian dự kiến để hoàn thành. Yêu cầu bạn điền thông tin thời gian.");
                    }
                    // Kiểm tra ngày hoàn thành dự kiến
                    if ((requestType.TypeId == 1 || requestType.TypeId == 2) && request.CompleteDate != DateTime.MinValue && DateOnly.FromDateTime(request.CompleteDate) < DateOnly.FromDateTime(DateTime.Now.AddDays(3)))
                    {
                        return (false, "Ngày yêu cầu hoàn thành dự kiến phải ít nhất sau 3 ngày kể từ bây giờ.");
                    }
                    var service = new Service();
                    // Kiểm tra ServiceID có tồn tại không
                    if (request.ServiceId != null && requestType.TypeId == 3)
                    {
                        service = await _unitOfWork.ServiceRepository.GetByIDAsync(request.ServiceId);
                        if (service == null)
                        {
                            return (false, "Không tìm thấy mộ.");
                        }
                        var graveService = (await _unitOfWork.GraveServiceRepository.GetAsync(g => g.ServiceId == service.ServiceId && g.MartyrId == request.MartyrId)).FirstOrDefault();
                        if (graveService != null)
                        {
                            return (false, "Mộ thân nhân của bạn đã tồn tại dịch vụ này rồi");
                        }
                    }
                    // Tìm MartyrGrave dựa trên MartyrId
                    var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(request.MartyrId);
                    if (martyrGrave == null)
                    {
                        return (false, "MartyrId không tồn tại.");
                    }

                    if (martyrGrave.AccountId != account.AccountId)
                    {
                        return (false, "Chỉ có người thân nhân của mộ mới được đặt yêu cầu này.");
                    }
                    // Kiểm tra nếu mục đã tồn tại trong giỏ hàng
                    var currentMonth = DateTime.Now.Month;
                    var currentYear = DateTime.Now.Year;
                    // Kiểm tra nếu mục đã tồn tại quá 10 yêu cầu của khách hàng trong tháng
                    var existingRequest = await _unitOfWork.RequestCustomerRepository.FindAsync(c =>
                        c.CustomerId == request.CustomerId &&
                        c.MartyrId == request.MartyrId &&
                        c.CreateAt.Month == currentMonth &&
                        c.CreateAt.Year == currentYear);
                    if (existingRequest.Any() && existingRequest.Count() >= 10)
                    {
                        return (false, $"Bạn đã đặt quá giới hạn 10 yêu cầu trong 1 tháng rồi. Hãy kiểm tra lại.");
                    }
                    var requestCustomer = new RequestCustomer
                    {
                        CustomerId = account.AccountId,
                        MartyrId = martyrGrave.MartyrId,
                        TypeId = requestType.TypeId,
                        Price = 0,
                        CreateAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        Note = request.Note,
                        Status = 1
                    };
                    if (service.ServiceId != 0 && requestType.TypeId == 3)
                    {
                        requestCustomer.ServiceId = service.ServiceId;
                    }
                    else if (requestType.TypeId == 3 && service?.ServiceId == 0)
                    {
                        return (false, "Thêm dịch vụ cho mộ phải có dịch vụ đính kèm là gì");
                    }
                    if (requestType.TypeId == 1 || requestType.TypeId == 2)
                    {
                        if (request.CompleteDate != DateTime.MinValue)
                        {
                            requestCustomer.EndDate = DateOnly.FromDateTime(request.CompleteDate);
                        }
                    }

                    await _unitOfWork.RequestCustomerRepository.AddAsync(requestCustomer);
                    await transaction.CommitAsync();
                    return (true, "Đã tạo yêu cầu thành công, chờ xác nhận từ quản lý");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<RequestCustomerDtoResponse> GetRequestByIdAsync(int requestId)
        {
            try
            {
                var requestResponse = new RequestCustomerDtoResponse();

                // Lấy thông tin request
                var request = await _unitOfWork.RequestCustomerRepository.GetAsync(
                    r => r.RequestId == requestId,
                    includeProperties: "Account,MartyrGrave.MartyrGraveInformations,RequestType,RequestNoteHistories,RequestTask.RequestTaskImages,ReportGrave.ReportImages,RequestMaterials.Material");

                var requestEntity = request.FirstOrDefault();
                if (requestEntity == null)
                {
                    return requestResponse;
                }

                // Ánh xạ thông tin cơ bản
                requestResponse = _mapper.Map<RequestCustomerDtoResponse>(requestEntity);

                // Lấy thêm thông tin từ các bảng liên quan
                if (requestEntity.Account != null)
                {
                    requestResponse.CustomerName = requestEntity.Account.FullName;
                    requestResponse.CustomerPhone = requestEntity.Account.PhoneNumber;
                }

                if (requestEntity.MartyrGrave != null)
                {
                    requestResponse.MartyrCode = requestEntity.MartyrGrave.MartyrCode;
                    var martyrInfo = requestEntity.MartyrGrave.MartyrGraveInformations?.FirstOrDefault();
                    requestResponse.MartyrName = martyrInfo?.Name;
                }

                if (requestEntity.RequestType != null)
                {
                    requestResponse.RequestTypeName = requestEntity.RequestType.TypeName;
                }

                if (requestEntity.RequestNoteHistories != null && requestEntity.RequestNoteHistories.Any())
                {
                    requestResponse.Reasons = requestEntity.RequestNoteHistories
                        .OrderByDescending(n => n.CreateAt)
                        .Select(n => new ReasonDto
                        {
                            RejectReason = n.Note,
                            RejectReason_CreateAt = n.CreateAt
                        })
                        .ToList();
                }


                if (requestEntity.ServiceId.HasValue)
                {
                    var service = await _unitOfWork.ServiceRepository.GetAsync(s => s.ServiceId == requestEntity.ServiceId.Value);
                    var serviceEntity = service.FirstOrDefault();
                    if (serviceEntity != null)
                    {
                        requestResponse.ServiceName = serviceEntity.ServiceName; // Gán tên dịch vụ từ bảng Service
                    }
                }


                // Lấy thông tin từ RequestTask
                if (requestEntity.RequestTask != null)
                {
                    requestResponse.RequestTask = new RequestTaskDto
                    {
                        RequestTaskId = requestEntity.RequestTask.RequestTaskId,
                        Description = requestEntity.RequestTask.Description,
                        ImageWorkSpace = requestEntity.RequestTask.ImageWorkSpace,
                        Reason = requestEntity.RequestTask.Reason,
                        Status = requestEntity.RequestTask.Status,
                        CreateAt = requestEntity.RequestTask.CreateAt,
                        TaskImages = requestEntity.RequestTask.RequestTaskImages?.Select(img => new RequestTaskImageDto
                        {
                            RequestTaskImageId = img.RequestTaskImageId,
                            ImageRequestTaskCustomer = img.ImageRequestTaskCustomer,
                            CreateAt = img.CreateAt
                        }).ToList()
                    };
                }

                // Lấy thông tin từ ReportGrave
                if (requestEntity.ReportGrave != null)
                {
                    requestResponse.ReportTask = new ReportTaskDto
                    {
                        ReportId = requestEntity.ReportGrave.ReportId,
                        VideoFile = requestEntity.ReportGrave.VideoFile,
                        Description = requestEntity.ReportGrave.Description,
                        CreateAt = requestEntity.ReportGrave.CreateAt,
                        ReportImages = requestEntity.ReportGrave.ReportImages?.Select(img => new ReportImageDto
                        {
                            ImageId = img.ImageId,
                            UrlPath = img.UrlPath,
                            CreateAt = img.CreateAt
                        }).ToList()
                    };
                }

                // Lấy danh sách RequestMaterials
                if (requestEntity.RequestMaterials != null)
                {
                    requestResponse.RequestMaterials = requestEntity.RequestMaterials.Select(m => new RequestMaterialDTOResponse
                    {
                        RequestMaterialId = m.RequestMaterialId,
                        MaterialId = m.MaterialId,
                        MaterialName = m.Material?.MaterialName,
                        Description = m.Material?.Description,
                        ImagePath = m.Material?.ImagePath,
                        Price = m.Material?.Price ?? 0
                    }).ToList();
                }

                return requestResponse;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching request: {ex.Message}");
            }
        }





        public async Task<(IEnumerable<RequestCustomerDtoResponse> requestList, int totalPage)> GetRequestsByAccountIdAsync(int accountId, int pageIndex, int pageSize, DateTime Date)
        {
            try
            {
                // Kiểm tra xem Manager có tồn tại không
                var customer = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (customer == null || customer.RoleId != 4)
                {
                    throw new KeyNotFoundException("Customer not found or invalid role.");
                }

                int totalPage = 0;
                int totalTask = 0;
                IEnumerable<RequestCustomer> requests = new List<RequestCustomer>();

                if (Date == DateTime.MinValue)
                {
                    totalTask = (await _unitOfWork.RequestCustomerRepository.GetAsync(
                        s => s.CustomerId == customer.AccountId,
                        includeProperties: "MartyrGrave,Account")).Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                    requests = await _unitOfWork.RequestCustomerRepository.GetAsync(
                        s => s.CustomerId == customer.AccountId,
                        includeProperties: "MartyrGrave,Account",
                        orderBy: q => q.OrderByDescending(r => r.CreateAt),
                        pageIndex: pageIndex,
                        pageSize: pageSize
                    );
                }
                else
                {
                    totalTask = (await _unitOfWork.RequestCustomerRepository.GetAsync(
                        s => s.CustomerId == customer.AccountId &&
                        s.CreateAt.Date == Date.Date,
                        includeProperties: "MartyrGrave,Account")).Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                    requests = await _unitOfWork.RequestCustomerRepository.GetAsync(
                        t => t.CustomerId == customer.AccountId &&
                        t.CreateAt.Date == Date.Date,
                        includeProperties: "MartyrGrave,Account",
                        orderBy: q => q.OrderByDescending(r => r.CreateAt),
                        pageIndex: pageIndex,
                        pageSize: pageSize
                    );
                }

                if (!requests.Any())
                {
                    return (new List<RequestCustomerDtoResponse>(), 0);
                }
                var responses = new List<RequestCustomerDtoResponse>();
                // Lấy thông tin vị trí cho tất cả các task
                foreach (var request in requests)
                {
                    var requestReponse = _mapper.Map<RequestCustomerDtoResponse>(request);
                    responses.Add(requestReponse);
                }


                //var responses = tasks.Select(t => _mapper.Map<AssignmentTaskResponse>(t)).ToList();
                return (responses, totalPage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<(IEnumerable<RequestCustomerDtoResponse> requestList, int totalPage)> GetRequestsForManager(int managerId, int pageIndex, int pageSize, DateTime Date)
        {
            try
            {
                // Kiểm tra xem Manager có tồn tại không
                var manager = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
                if (manager == null || manager.RoleId != 2)
                {
                    throw new KeyNotFoundException("Manager not found or invalid role.");
                }

                int totalPage = 0;
                int totalTask = 0;
                IEnumerable<RequestCustomer> requests = new List<RequestCustomer>();

                if (Date == DateTime.MinValue)
                {
                    totalTask = (await _unitOfWork.RequestCustomerRepository.GetAsync(
                        s => s.MartyrGrave.AreaId == manager.AreaId,
                        includeProperties: "MartyrGrave,Account")).Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                    requests = await _unitOfWork.RequestCustomerRepository.GetAsync(
                        s => s.MartyrGrave.AreaId == manager.AreaId,
                        includeProperties: "MartyrGrave,Account",
                        orderBy: q => q.OrderByDescending(r => r.CreateAt),
                        pageIndex: pageIndex,
                        pageSize: pageSize
                    );
                }
                else
                {
                    totalTask = (await _unitOfWork.RequestCustomerRepository.GetAsync(
                        s => s.MartyrGrave.AreaId == manager.AreaId &&
                        s.CreateAt.Date == Date.Date,
                        includeProperties: "MartyrGrave,Account")).Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                    requests = await _unitOfWork.RequestCustomerRepository.GetAsync(
                        t => t.MartyrGrave.AreaId == manager.AreaId &&
                        t.CreateAt.Date == Date.Date,
                        includeProperties: "MartyrGrave,Account",
                        orderBy: q => q.OrderByDescending(r => r.CreateAt),
                        pageIndex: pageIndex,
                        pageSize: pageSize
                    );
                }

                if (!requests.Any())
                {
                    return (new List<RequestCustomerDtoResponse>(), 0);
                }
                var responses = new List<RequestCustomerDtoResponse>();
                // Lấy thông tin vị trí cho tất cả các task
                foreach (var request in requests)
                {
                    var requestReponse = _mapper.Map<RequestCustomerDtoResponse>(request);
                    responses.Add(requestReponse);
                }


                //var responses = tasks.Select(t => _mapper.Map<AssignmentTaskResponse>(t)).ToList();
                return (responses, totalPage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
