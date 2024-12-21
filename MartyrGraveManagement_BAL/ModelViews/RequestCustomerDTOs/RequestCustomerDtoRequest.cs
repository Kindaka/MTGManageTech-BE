namespace MartyrGraveManagement_BAL.ModelViews.RequestCustomerDTOs
{
    public class RequestCustomerDtoRequest
    {
        public int CustomerId { get; set; }
        public int MartyrId { get; set; }
        public int TypeId { get; set; }
        public string Note { get; set; }
        public int? ServiceId { get; set; }
        public DateTime CompleteDate { get; set; }
    }
}
