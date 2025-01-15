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
                var serviceSales = new Dictionary<int, int>();
                var startDate = new DateTime(year, 1, 1);
                var endDate = startDate.AddYears(1);

                var totalTasks = await _unitOfWork.TaskRepository
                    .CountAsync(t => t.StartDate >= startDate && t.StartDate < endDate);

                //var totalRevenue = await _unitOfWork.Orders
                //    .Where(o => o.CreatedDate >= startDate && o.CreatedDate < endDate && o.Status == "Completed")
                //    .SumAsync(o => o.TotalPrice);

                var orders = await _unitOfWork.OrderRepository
                    .GetAsync(o => o.OrderDate >= startDate && o.OrderDate < endDate);

                foreach (var order in orders)
                {
                    if (order.Status == 1 || order.Status == 4)
                    {
                        var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == order.OrderId);
                        if (orderDetails.Any())
                        {
                            foreach (var orderDetail in orderDetails)
                            {
                                if (!serviceSales.ContainsKey(orderDetail.ServiceId))
                                {
                                    serviceSales[orderDetail.ServiceId] = 0;
                                }
                                serviceSales[orderDetail.ServiceId] += 1;
                            }
                        }
                        totalOrderRevenue += order.TotalPrice;
                    }
                }

                var assignmentTasks = await _unitOfWork.AssignmentTaskRepository
                    .GetAsync(ss => ss.CreateAt >= startDate && ss.CreateAt < endDate, includeProperties: "Service_Schedule");

                foreach (var assignmentTask in assignmentTasks)
                {
                    totalServiceScheduleRevenue += assignmentTask.Service_Schedule.Amount;
                }

                var requestTasks = await _unitOfWork.RequestTaskRepository
                    .GetAsync(t => t.CreateAt >= startDate && t.CreateAt < endDate, includeProperties: "RequestCustomer");

                foreach (var requestTask in requestTasks)
                {
                    totalRequestRevenue += requestTask.RequestCustomer.Price;
                }

                //var topSellingServices = await _unitOfWork.OrderDetailRepository
                //    .GetAllAsync(
                //        od => od.Order.OrderDate >= startDate && od.Order.OrderDate < endDate,
                //        includeProperties: "Service,Order"
                //    )
                //    .GroupBy(od => od.ServiceId)
                //    .Select(g => new ServiceDtoResponse
                //    {
                //        ServiceId = g.Key,
                //        ServiceName = g.First().Service.Name,
                //        QuantitySold = g.Sum(od => od.Quantity),
                //        Revenue = g.Sum(od => od.Quantity * od.Price)
                //    })
                //    .OrderByDescending(s => s.QuantitySold)
                //    .Take(5)
                //    .ToListAsync();
                var response = new DashboardDto
                {
                    totalTask = totalTasks,
                    totalRevenue = totalOrderRevenue + totalRequestRevenue + totalServiceScheduleRevenue,
                    totalOrder = orders.Count(),
                    totalAssignmentTask = assignmentTasks.Count(),
                    totalRequestTask = requestTasks.Count()
                };
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
                var serviceSales = new Dictionary<int, int>();
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
                        var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == order.OrderId && od.MartyrGrave.AreaId == area.AreaId, includeProperties: "MartyrGrave,Service");
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
                            }

                        }
                    }
                }

                var assignmentTasks = await _unitOfWork.AssignmentTaskRepository
                    .GetAsync(ss => ss.CreateAt >= startDate && ss.CreateAt < endDate && ss.Service_Schedule.MartyrGrave.AreaId == area.AreaId, includeProperties: "Service_Schedule.MartyrGrave");

                foreach (var assignmentTask in assignmentTasks)
                {
                    totalServiceScheduleRevenue += assignmentTask.Service_Schedule.Amount;
                }

                var requestTasks = await _unitOfWork.RequestTaskRepository
                    .GetAsync(t => t.CreateAt >= startDate && t.CreateAt < endDate && t.RequestCustomer.MartyrGrave.AreaId == area.AreaId, includeProperties: "RequestCustomer.MartyrGrave");

                foreach (var requestTask in requestTasks)
                {
                    totalRequestRevenue += requestTask.RequestCustomer.Price;
                }

                var response = new DashboardDto
                {
                    totalTask = totalTasks.Count(),
                    totalRevenue = totalOrderRevenue + totalRequestRevenue + totalServiceScheduleRevenue,
                    totalOrder = orders.Count(),
                    totalAssignmentTask = assignmentTasks.Count(),
                    totalRequestTask = requestTasks.Count()
                };
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
