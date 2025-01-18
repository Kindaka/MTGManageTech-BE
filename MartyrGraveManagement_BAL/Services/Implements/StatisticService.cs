using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.DashboardDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class StatisticService : IStatisticService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StatisticService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<DashboardDto> GetDashboard(int year)
        {
            try
            {
                decimal totalOrderRevenue = 0;
                decimal totalServiceScheduleRevenue = 0;
                decimal totalRequestRevenue = 0;
                var customerSpending = new Dictionary<int, decimal>();
                var serviceSales = new Dictionary<int, int>();
                var monthlySales = new Dictionary<int, decimal>();

                // Khởi tạo doanh thu từng tháng với giá trị mặc định là 0
                for (int i = 1; i <= 12; i++)
                {
                    monthlySales[i] = 0;
                }

                var startDate = new DateTime(year, 1, 1);
                var endDate = startDate.AddYears(1);

                var totalTasks = await _unitOfWork.TaskRepository
                    .CountAsync(t => t.StartDate >= startDate && t.StartDate < endDate);

                var orders = await _unitOfWork.OrderRepository
                    .GetAsync(o => o.OrderDate >= startDate && o.OrderDate < endDate);

                foreach (var order in orders)
                {
                    if (order.Status == 1 || order.Status == 4)
                    {
                        var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == order.OrderId, includeProperties: "Service");
                        if (orderDetails.Any())
                        {
                            foreach (var orderDetail in orderDetails)
                            {
                                if (!serviceSales.ContainsKey(orderDetail.ServiceId))
                                {
                                    serviceSales[orderDetail.ServiceId] = 0;
                                }
                                serviceSales[orderDetail.ServiceId] += 1;
                                totalOrderRevenue += orderDetail.Service.Price;

                                // Tính doanh thu theo tháng
                                int month = order.OrderDate.Month;
                                monthlySales[month] += orderDetail.Service.Price;
                                if (!customerSpending.ContainsKey(order.AccountId))
                                {
                                    customerSpending[order.AccountId] = 0;
                                }
                                customerSpending[order.AccountId] += orderDetail.Service.Price;
                            }
                        }
                        //totalOrderRevenue += order.TotalPrice;
                    }
                }

                var assignmentTasks = await _unitOfWork.AssignmentTaskRepository
                    .GetAsync(ss => ss.CreateAt >= startDate && ss.CreateAt < endDate, includeProperties: "Service_Schedule");

                foreach (var assignmentTask in assignmentTasks)
                {
                    int month = assignmentTask.CreateAt.Month;
                    monthlySales[month] += assignmentTask.Service_Schedule.Amount;
                    totalServiceScheduleRevenue += assignmentTask.Service_Schedule.Amount;
                }

                var requestTasks = await _unitOfWork.RequestTaskRepository
                    .GetAsync(t => t.CreateAt >= startDate && t.CreateAt < endDate, includeProperties: "RequestCustomer");

                foreach (var requestTask in requestTasks)
                {
                    int month = requestTask.CreateAt.Month;
                    monthlySales[month] += requestTask.RequestCustomer.Price;
                    totalRequestRevenue += requestTask.RequestCustomer.Price;
                }

                // Lấy tổng số manager và staff
                var totalManager = await _unitOfWork.AccountRepository.CountAsync(a => a.RoleId == 2);
                var totalStaff = await _unitOfWork.AccountRepository.CountAsync(a => a.RoleId == 3);

                // Chuyển doanh thu hàng tháng từ Dictionary sang List<MontlySalesDTO>
                var monthSalesList = monthlySales.Select(ms => new MontlySalesDTO
                {
                    Month = ms.Key,
                    TotalSales = ms.Value
                }).ToList();

                var response = new DashboardDto
                {
                    totalManager = totalManager,
                    totalStaff = totalStaff,
                    totalTask = totalTasks,
                    totalRevenue = totalOrderRevenue + totalRequestRevenue + totalServiceScheduleRevenue,
                    totalOrder = orders.Count(),
                    totalAssignmentTask = assignmentTasks.Count(),
                    totalRequestTask = requestTasks.Count(),
                    MonthSales = monthSalesList
                };



                // Top 3 customers
                var topCustomers = customerSpending.OrderByDescending(cs => cs.Value).Take(3);
                foreach (var customer in topCustomers)
                {
                    var customerEntity = await _unitOfWork.AccountRepository.GetByIDAsync(customer.Key);
                    var customerResponse = _mapper.Map<Top3CustomertDtoResponse>(customerEntity);
                    customerResponse.customerSpending += customer.Value;
                    response.topCustomer.Add(customerResponse);
                }

                // Top-selling services
                var topSellingServices = serviceSales.OrderByDescending(ps => ps.Value).Take(5).ToDictionary(ps => ps.Key, ps => ps.Value);
                foreach (var product in topSellingServices)
                {
                    var pd = await _unitOfWork.ServiceRepository.GetByIDAsync(product.Key);
                    var serviceResponse = _mapper.Map<ServiceDtoResponse>(pd);

                    response.topSellingServices.Add(serviceResponse);
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<DashboardDto> GetDashboardByAreaId(int year, int areaId)
        {
            try
            {
                decimal totalOrderRevenue = 0;
                decimal totalServiceScheduleRevenue = 0;
                decimal totalRequestRevenue = 0;
                var customerSpending = new Dictionary<int, decimal>();
                var serviceSales = new Dictionary<int, int>();
                var monthlySales = new Dictionary<int, decimal>();

                // Khởi tạo doanh thu hàng tháng với giá trị mặc định là 0
                for (int i = 1; i <= 12; i++)
                {
                    monthlySales[i] = 0;
                }

                var startDate = new DateTime(year, 1, 1);
                var endDate = startDate.AddYears(1);

                var area = await _unitOfWork.AreaRepository.GetByIDAsync(areaId);
                if (area == null)
                {
                    return new DashboardDto();
                }

                var totalTasks = await _unitOfWork.TaskRepository
                    .GetAsync(t => t.StartDate >= startDate && t.StartDate < endDate && t.OrderDetail.MartyrGrave.AreaId == area.AreaId, includeProperties: "OrderDetail.MartyrGrave");

                var orders = await _unitOfWork.OrderRepository
                    .GetAsync(o => o.OrderDate >= startDate && o.OrderDate < endDate);

                foreach (var order in orders)
                {
                    if (order.Status == 1 || order.Status == 4)
                    {
                        var orderDetails = await _unitOfWork.OrderDetailRepository
                            .GetAsync(od => od.OrderId == order.OrderId && od.MartyrGrave.AreaId == area.AreaId, includeProperties: "MartyrGrave,Service");

                        if (orderDetails.Any())
                        {
                            foreach (var orderDetail in orderDetails)
                            {
                                if (!serviceSales.ContainsKey(orderDetail.ServiceId))
                                {
                                    serviceSales[orderDetail.ServiceId] = 0;
                                }
                                serviceSales[orderDetail.ServiceId] += 1;
                                totalOrderRevenue += orderDetail.Service.Price;

                                // Tính doanh thu theo tháng
                                int month = order.OrderDate.Month;
                                monthlySales[month] += orderDetail.Service.Price;

                                if (!customerSpending.ContainsKey(order.AccountId))
                                {
                                    customerSpending[order.AccountId] = 0;
                                }
                                customerSpending[order.AccountId] += orderDetail.Service.Price;
                            }
                        }
                    }
                }

                var assignmentTasks = await _unitOfWork.AssignmentTaskRepository
                    .GetAsync(ss => ss.CreateAt >= startDate && ss.CreateAt < endDate && ss.Service_Schedule.MartyrGrave.AreaId == area.AreaId, includeProperties: "Service_Schedule.MartyrGrave");

                foreach (var assignmentTask in assignmentTasks)
                {
                    int month = assignmentTask.CreateAt.Month;
                    monthlySales[month] += assignmentTask.Service_Schedule.Amount;
                    totalServiceScheduleRevenue += assignmentTask.Service_Schedule.Amount;
                }

                var requestTasks = await _unitOfWork.RequestTaskRepository
                    .GetAsync(t => t.CreateAt >= startDate && t.CreateAt < endDate && t.RequestCustomer.MartyrGrave.AreaId == area.AreaId, includeProperties: "RequestCustomer.MartyrGrave");

                foreach (var requestTask in requestTasks)
                {
                    int month = requestTask.CreateAt.Month;
                    monthlySales[month] += requestTask.RequestCustomer.Price;
                    totalRequestRevenue += requestTask.RequestCustomer.Price;
                }

                // Lấy tổng số manager và staff
                var totalManager = await _unitOfWork.AccountRepository.CountAsync(a => a.RoleId == 2 && a.AreaId == area.AreaId);
                var totalStaff = await _unitOfWork.AccountRepository.CountAsync(a => a.RoleId == 3 && a.AreaId == area.AreaId);

                // Chuyển doanh thu hàng tháng từ Dictionary sang List<MontlySalesDTO>
                var monthSalesList = monthlySales.Select(ms => new MontlySalesDTO
                {
                    Month = ms.Key,
                    TotalSales = ms.Value
                }).ToList();

                var response = new DashboardDto
                {
                    totalManager = totalManager,
                    totalStaff = totalStaff,
                    totalTask = totalTasks.Count(),
                    totalRevenue = totalOrderRevenue + totalRequestRevenue + totalServiceScheduleRevenue,
                    totalAssignmentTask = assignmentTasks.Count(),
                    totalRequestTask = requestTasks.Count(),
                    MonthSales = monthSalesList
                };

                // Top 3 customers
                var topCustomers = customerSpending.OrderByDescending(cs => cs.Value).Take(3);
                foreach (var customer in topCustomers)
                {
                    var customerEntity = await _unitOfWork.AccountRepository.GetByIDAsync(customer.Key);
                    var customerResponse = _mapper.Map<Top3CustomertDtoResponse>(customerEntity);
                    customerResponse.customerSpending += customer.Value;
                    response.topCustomer.Add(customerResponse);
                }

                // Top-selling services
                var topSellingServices = serviceSales.OrderByDescending(ps => ps.Value).Take(5).ToDictionary(ps => ps.Key, ps => ps.Value);
                foreach (var product in topSellingServices)
                {
                    var pd = await _unitOfWork.ServiceRepository.GetByIDAsync(product.Key);
                    var serviceResponse = _mapper.Map<ServiceDtoResponse>(pd);

                    response.topSellingServices.Add(serviceResponse);
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<WorkPerformanceStaff> GetWorkPerformanceStaff(int staffId, int managerId, int month, int year)
        {
            try
            {
                int totalFinishTask = 0;
                int totalFinishAssignmentTask = 0;
                int totalFinishRequestTask = 0;
                int totalFailTask = 0;
                int totalFailAssignmentTask = 0;
                int totalFailRequestTask = 0;
                decimal totalFeedbackRate = 0;
                var staff = await _unitOfWork.AccountRepository.GetByIDAsync(staffId);
                var manager = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
                if (staff == null || manager == null || staff.AreaId != manager.AreaId)
                {
                    return new WorkPerformanceStaff();
                }

                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1);

                // Total tasks assigned to the staff
                var tasks = await _unitOfWork.TaskRepository
                    .GetAsync(t => t.AccountId == staffId && t.StartDate >= startDate && t.StartDate < endDate);

                var assignmentTask = await _unitOfWork.AssignmentTaskRepository
                    .GetAsync(t => t.StaffId == staffId && t.CreateAt >= startDate && t.CreateAt < endDate);

                var requestTask = await _unitOfWork.RequestTaskRepository
                    .GetAsync(t => t.StaffId == staffId && t.CreateAt >= startDate && t.CreateAt < endDate);

                if (tasks.Any())
                {
                    foreach (var task in tasks)
                    {
                        if (task.Status == 4)
                        {
                            totalFinishTask++;
                        }
                        else if (task.Status == 5)
                        {
                            totalFailTask++;
                        }
                    }
                }

                if (assignmentTask.Any())
                {
                    foreach (var task in assignmentTask)
                    {
                        if (task.Status == 4)
                        {
                            totalFinishAssignmentTask++;
                        }
                        else if (task.Status == 5)
                        {
                            totalFailAssignmentTask++;
                        }
                    }
                }

                if (requestTask.Any())
                {
                    foreach (var task in requestTask)
                    {
                        if (task.Status == 4)
                        {
                            totalFinishRequestTask++;
                        }
                        else if (task.Status == 5)
                        {
                            totalFailRequestTask++;
                        }
                    }
                }

                int totalFeedbackCount = 0; // Tổng số feedback

                // Task Feedback
                var taskFeedback = await _unitOfWork.FeedbackRepository.GetAsync(f => f.StaffId == staffId && f.CreatedAt >= startDate && f.CreatedAt < endDate);
                if (taskFeedback.Any())
                {
                    totalFeedbackCount += taskFeedback.Count(); // Cộng số lượng feedback
                    foreach (var feedback in taskFeedback)
                    {
                        totalFeedbackRate += feedback.Rating;
                    }
                }

                // Assignment Task Feedback
                var assignTaskFeedback = await _unitOfWork.AssignmentTaskFeedbackRepository.GetAsync(f => f.StaffId == staffId && f.CreatedAt >= startDate && f.CreatedAt < endDate);
                if (assignTaskFeedback.Any())
                {
                    totalFeedbackCount += assignTaskFeedback.Count(); // Cộng số lượng feedback
                    foreach (var feedback in assignTaskFeedback)
                    {
                        totalFeedbackRate += feedback.Rating;
                    }
                }

                // Request Task Feedback
                var requestTaskFeedback = await _unitOfWork.RequestFeedbackRepository.GetAsync(f => f.StaffId == staffId && f.CreatedAt >= startDate && f.CreatedAt < endDate);
                if (requestTaskFeedback.Any())
                {
                    totalFeedbackCount += requestTaskFeedback.Count(); // Cộng số lượng feedback
                    foreach (var feedback in requestTaskFeedback)
                    {
                        totalFeedbackRate += feedback.Rating;
                    }
                }

                // Tính toán kết quả
                var response = new WorkPerformanceStaff
                {
                    totalTask = tasks.Count(),
                    totalAssignmentTask = assignmentTask.Count(),
                    totalRequestTask = requestTask.Count(),

                    totalFinishTask = totalFinishTask,
                    totalFinishAssignmentTask = totalFinishAssignmentTask,
                    totalFinishRequestTask = totalFinishRequestTask,

                    totalFailTask = totalFailTask,
                    totalFailAssignmentTask = totalFailAssignmentTask,
                    totalFailRequestTask = totalFailRequestTask
                };

                int totalAllTask = response.totalTask + response.totalAssignmentTask + response.totalRequestTask;
                int totalAllFinishTask = totalFinishTask + totalFinishAssignmentTask + totalFinishRequestTask;

                // Hiệu suất công việc hoàn thành
                response.workPerformance = $"Hiệu suất công việc hoàn thành: {(totalAllTask > 0 ? (double)totalAllFinishTask / totalAllTask : 0):P}";

                // Tính trung bình feedback
                if (totalFeedbackCount > 0)
                {
                    response.averageAllFeedbackRate = totalFeedbackRate / totalFeedbackCount; // Chia cho tổng số feedback
                    if (response.averageAllFeedbackRate >= 4)
                    {
                        response.workQuality = "Rất tốt";
                    }
                    else if (response.averageAllFeedbackRate >= 3)
                    {
                        response.workQuality = "Khá";
                    }
                    else if (response.averageAllFeedbackRate >= 2)
                    {
                        response.workQuality = "Trung bình";
                    }
                    else
                    {
                        response.workQuality = "Kém";
                    }
                }

                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
