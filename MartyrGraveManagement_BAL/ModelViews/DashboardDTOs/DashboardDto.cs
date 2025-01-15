using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;

namespace MartyrGraveManagement_BAL.ModelViews.DashboardDTOs
{
    public class DashboardDto
    {
        public int totalTask { get; set; } = 0;
        public decimal totalRevenue { get; set; } = 0;
        public int totalOrder { get; set; } = 0;
        public int totalAssignmentTask { get; set; } = 0;
        public int totalRequestTask { get; set; } = 0;
        public List<ServiceDtoResponse>? topSellingServices { get; set; } = new List<ServiceDtoResponse>();
    }
}
