using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.BlogCategoryDTOs
{
    public class BlogDtoResponse
    {
        public int BlogId { get; set; }
        public int AccountId { get; set; }
        public int HistoryId { get; set; }
        public string? FullName { get; set; }
        public string BlogName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Status { get; set; }
    }
}
