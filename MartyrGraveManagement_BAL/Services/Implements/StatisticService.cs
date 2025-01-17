using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.DashboardDTOs;
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
                        // Tính doanh thu theo tháng
                        int month = order.OrderDate.Month;
                        monthlySales[month] += order.TotalPrice;

                        var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == order.OrderId, includeProperties: "Service");
                        if (orderDetails.Any())
                        {
                            foreach (var orderDetail in orderDetails)
                            {
                                if (!customerSpending.ContainsKey(order.AccountId))
                                {
                                    customerSpending[order.AccountId] = 0;
                                }
                                customerSpending[order.AccountId] += orderDetail.Service.Price;
                            }
                        }
                        totalOrderRevenue += order.TotalPrice;
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

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<WorkPerformanceStaff> GetWorkPerformanceStaff(int staffId, int month, int year)
        {
            try
            {
                int totalFinishTask = 0;
                int totalFinishAssignmentTask = 0;
                int totalFinishRequestTask = 0;
                int totalFailTask = 0;
                int totalFailAssignmentTask = 0;
                int totalFailRequestTask = 0;
                var staff = _unitOfWork.AccountRepository.GetByIDAsync(staffId);
                if (staff == null)
                {
                    return new WorkPerformanceStaff();
                }
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1);

                // Total tasks assigned to the staff
                var totalTasks = await _unitOfWork.TaskRepository
                    .GetAsync(t => t.AccountId == staffId && t.StartDate >= startDate && t.StartDate < endDate);

                var totalAssignmentTask = await _unitOfWork.AssignmentTaskRepository
                    .GetAsync(t => t.StaffId == staffId && t.CreateAt >= startDate && t.CreateAt < endDate);

                var totalRequestTask = await _unitOfWork.RequestTaskRepository
                    .GetAsync(t => t.StaffId == staffId && t.CreateAt >= startDate && t.CreateAt < endDate);

                if (totalTasks.Any())
                {
                    foreach (var task in totalTasks)
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

                if (totalAssignmentTask.Any())
                {
                    foreach (var task in totalAssignmentTask)
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

                if (totalRequestTask.Any())
                {
                    foreach (var task in totalRequestTask)
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

                //// Total tasks completed
                //var totalFinishTask = await _context.Tasks
                //    .CountAsync(t => t.StaffId == staffId && t.AssignedDate >= startDate && t.AssignedDate < endDate && t.Status == "Completed");

                //var totalFinishAssignmentTask = await _context.Tasks
                //    .CountAsync(t => t.StaffId == staffId && t.AssignedDate >= startDate && t.AssignedDate < endDate && t.Status == "Completed" && t.Type == "Assignment");

                //var totalFinishRequestTask = await _context.Tasks
                //    .CountAsync(t => t.StaffId == staffId && t.AssignedDate >= startDate && t.AssignedDate < endDate && t.Status == "Completed" && t.Type == "Request");

                //// Total failed tasks
                //var totalFailTask = await _context.Tasks
                //    .CountAsync(t => t.StaffId == staffId && t.AssignedDate >= startDate && t.AssignedDate < endDate && t.Status == "Failed");

                //var totalFailAssignmentTask = await _context.Tasks
                //    .CountAsync(t => t.StaffId == staffId && t.AssignedDate >= startDate && t.AssignedDate < endDate && t.Status == "Failed" && t.Type == "Assignment");

                //var totalFailRequestTask = await _context.Tasks
                //    .CountAsync(t => t.StaffId == staffId && t.AssignedDate >= startDate && t.AssignedDate < endDate && t.Status == "Failed" && t.Type == "Request");

                return new WorkPerformanceStaff
                {
                    totalTask = totalTasks.Count(),
                    totalAssignmentTask = totalAssignmentTask.Count(),
                    totalRequestTask = totalRequestTask.Count(),

                    totalFinishTask = totalFinishTask,
                    totalFinishAssignmentTask = totalFinishAssignmentTask,
                    totalFinishRequestTask = totalFinishRequestTask,

                    totalFailTask = totalFailTask,
                    totalFailAssignmentTask = totalFailAssignmentTask,
                    totalFailRequestTask = totalFailRequestTask
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
