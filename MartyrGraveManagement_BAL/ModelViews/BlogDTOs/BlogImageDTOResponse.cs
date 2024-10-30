using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.BlogDTOs
{
    public class BlogImageDTOResponse
    {
        public int ImageId { get; set; }
        public int BlogId { get; set; }
        public string? ImagePath { get; set; }
    }
}
