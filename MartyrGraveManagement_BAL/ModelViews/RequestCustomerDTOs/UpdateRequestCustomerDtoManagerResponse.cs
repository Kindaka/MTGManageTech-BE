namespace MartyrGraveManagement_BAL.ModelViews.RequestCustomerDTOs
{
    public class UpdateRequestCustomerDtoManagerResponse
    {
        public int RequestId { get; set; }
        public int ManagerId { get; set; }
        public string? Note { get; set; }
        public List<int>? MaterialIds { get; set; } = new List<int>();
    }
}
