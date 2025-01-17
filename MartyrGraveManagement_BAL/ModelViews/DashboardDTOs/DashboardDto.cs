using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;

namespace MartyrGraveManagement_BAL.ModelViews.DashboardDTOs
{
    public class DashboardDto
    {
        public int totalManager { get; set; } = 0;
        public int totalStaff { get; set; } = 0;
        public int totalTask { get; set; } = 0;
        public decimal totalRevenue { get; set; } = 0;
        public int totalOrder { get; set; } = 0;
        public int totalAssignmentTask { get; set; } = 0;
        public int totalRequestTask { get; set; } = 0;
        public List<MontlySalesDTO> MonthSales { get; set; } = new List<MontlySalesDTO>();
        public List<Top3CustomertDtoResponse>? topCustomer { get; set; } = new List<Top3CustomertDtoResponse>();
        public List<ServiceDtoResponse>? topSellingServices { get; set; } = new List<ServiceDtoResponse>();
    }
}
