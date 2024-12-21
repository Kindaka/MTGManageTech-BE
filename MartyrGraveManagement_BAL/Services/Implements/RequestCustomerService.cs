using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.RequestCustomerDTOs;
using MartyrGraveManagement_BAL.ModelViews.RequestMaterialDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;

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

        public async Task<(bool status, string response)> AcceptRequestForManagerAsync(int requestId, int managerId, RequestMaterialDtoRequest? requestMaterial)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Kiểm tra AccountID có tồn tại không
                    var manager = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
                    if (manager == null || manager.RoleId != 2)
                    {
                        return (false, "AccountId không tồn tại hoặc bạn không có quyền.");
                    }
                    var request = (await _unitOfWork.RequestCustomerRepository.GetAsync(r => r.RequestId == requestId, includeProperties: "MartyrGrave")).FirstOrDefault();
                    if (request == null)
                    {
                        return (false, "Request không tồn tại.");
                    }
                    if (request.MartyrGrave.AreaId != manager.AreaId)
                    {
                        return (false, "Request không thuộc khu vực của bạn.");
                    }
                    var requestType = await _unitOfWork.RequestTypeRepository.GetByIDAsync(request.TypeId);
                    if (requestType.TypeId == 1)
                    {
                        var reportGrave = new ReportGrave
                        {
                            RequestId = request.RequestId,
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
                        if (requestMaterial.MaterialIds.Any())
                        {
                            decimal expectedPrice = 0;
                            foreach (var materialId in requestMaterial.MaterialIds)
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
                            request.Price = expectedPrice + (expectedPrice * 0.05m);
                            request.Status = 4;
                            await _unitOfWork.RequestCustomerRepository.UpdateAsync(request);
                        }
                        else
                        {
                            return (false, "Phải thêm ít nhất một vật liệu để báo giá dịch vụ");
                        }
                    }
                    else if (requestType.TypeId == 3)
                    {
                        if (request.ServiceId != null)
                        {
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
                var request = (await _unitOfWork.RequestCustomerRepository.GetAsync(r => r.RequestId == requestId)).FirstOrDefault();
                if (request == null)
                {
                    return requestResponse;
                }
                requestResponse = _mapper.Map<RequestCustomerDtoResponse>(request);
                return requestResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                        pageIndex: pageIndex,
                        pageSize: pageSize
                    );
                }
                else
                {
                    totalTask = (await _unitOfWork.RequestCustomerRepository.GetAsync(
                        s => s.CustomerId == customer.AccountId &&
                        s.EndDate == DateOnly.FromDateTime(Date.Date),
                        includeProperties: "MartyrGrave,Account")).Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                    requests = await _unitOfWork.RequestCustomerRepository.GetAsync(
                        t => t.CustomerId == customer.AccountId &&
                        t.EndDate == DateOnly.FromDateTime(Date.Date),
                        includeProperties: "MartyrGrave,Account",
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
                        pageIndex: pageIndex,
                        pageSize: pageSize
                    );
                }
                else
                {
                    totalTask = (await _unitOfWork.RequestCustomerRepository.GetAsync(
                        s => s.MartyrGrave.AreaId == manager.AreaId &&
                        s.EndDate == DateOnly.FromDateTime(Date.Date),
                        includeProperties: "MartyrGrave,Account")).Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                    requests = await _unitOfWork.RequestCustomerRepository.GetAsync(
                        t => t.MartyrGrave.AreaId == manager.AreaId &&
                        t.EndDate == DateOnly.FromDateTime(Date.Date),
                        includeProperties: "MartyrGrave,Account",
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
