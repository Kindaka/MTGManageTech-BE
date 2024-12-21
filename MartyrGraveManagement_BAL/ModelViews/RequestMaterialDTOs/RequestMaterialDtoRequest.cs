using System.ComponentModel.DataAnnotations;

namespace MartyrGraveManagement_BAL.ModelViews.RequestMaterialDTOs
{
    public class RequestMaterialDtoRequest
    {
        [Required]
        public List<int>? MaterialIds { get; set; } = new List<int>();
    }
}
