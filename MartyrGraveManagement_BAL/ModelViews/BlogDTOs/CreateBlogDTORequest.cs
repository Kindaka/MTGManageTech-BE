using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.BlogDTOs
{
    public class CreateBlogDTORequest
    {
        public int AccountId { get; set; }
        public int HistoryId { get; set; }
        public string? BlogName { get; set; }
        public string? BlogDescription { get; set; }
        public string? BlogContent { get; set; }
        public List<string>? HistoricalImageUrls { get; set; } 
        public List<int>? RelatedMartyrIds { get; set; } 
    }

}
