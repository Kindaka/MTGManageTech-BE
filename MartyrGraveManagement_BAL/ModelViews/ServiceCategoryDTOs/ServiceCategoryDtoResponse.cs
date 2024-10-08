using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.ServiceCategoryDTOs
{
    public class ServiceCategoryDtoResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public string? UrlImageCategory { get; set; }
        public bool Status { get; set; }
    }
}
