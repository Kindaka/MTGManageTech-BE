namespace MartyrGraveManagement_BAL.ModelViews.RequestCustomerDTOs
{
    public class RequestCustomerDtoManagerResponse
    {
        public int RequestId { get; set; }
        public int ManagerId { get; set; }
        public string? Note { get; set; }
        public bool status { get; set; }
        public List<int>? MaterialIds { get; set; } = new List<int>();
    }
}
