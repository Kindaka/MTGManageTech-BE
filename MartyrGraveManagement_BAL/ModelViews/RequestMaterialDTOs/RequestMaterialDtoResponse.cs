using System.ComponentModel.DataAnnotations.Schema;

namespace MartyrGraveManagement_BAL.ModelViews.RequestMaterialDTOs
{
    public class RequestMaterialDtoResponse
    {
        public int RequestMaterialId { get; set; }
        public int MaterialId { get; set; }
        public int RequestId { get; set; }
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? MaterialName { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Price { get; set; }

    }
}
