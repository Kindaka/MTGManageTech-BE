using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.BlogCategoryDTOs
{
    public class BlogCategoryDtoRequest
    {
        public string? BlogCategoryName { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
    }
}
