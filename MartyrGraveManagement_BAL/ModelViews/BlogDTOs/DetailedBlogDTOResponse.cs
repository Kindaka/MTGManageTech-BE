using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.BlogDTOs
{
    public class DetailedBlogResponseDTO
    {
        public int BlogId { get; set; }
        public int AccountId { get; set; }
        public int HistoryId { get; set; }
        public string BlogName { get; set; }
        public string BlogDescription { get; set; }
        public string BlogContent { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Status { get; set; }
        public string FullName { get; set; } // From Account
        public string HistoryEventName { get; set; } // From HistoricalEvent
        public IEnumerable<string>? HistoricalImages { get; set; } // Assuming HistoricalImage has a URL or Path
        public IEnumerable<string>? Comments { get; set; } // Assuming Comment has a Text property
        public IEnumerable<int>? HistoricalRelatedMartyrs { get; set; } // Assuming a property to identify Martyrs
    }

}
