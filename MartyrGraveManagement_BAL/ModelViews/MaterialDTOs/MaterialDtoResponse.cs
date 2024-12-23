namespace MartyrGraveManagement_BAL.ModelViews.MaterialDTOs
{
    public class MaterialDtoResponse
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; }
        public string? ImagePath { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool Status { get; set; }
    }
}
