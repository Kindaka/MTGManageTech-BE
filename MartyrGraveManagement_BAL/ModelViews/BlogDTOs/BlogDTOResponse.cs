using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.BlogDTOs
{
    public class BlogDTOResponse
    {
        public int BlogId { get; set; }
        public int AccountId { get; set; }
        public string? FullName { get; set; }
        public int HistoryId { get; set; }
        public string? BlogName { get; set; }
        public string? BlogContent { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Status { get; set; }

        public List<BlogImageDTOResponse> Images { get; set; } = new List<BlogImageDTOResponse>();
    }
}
