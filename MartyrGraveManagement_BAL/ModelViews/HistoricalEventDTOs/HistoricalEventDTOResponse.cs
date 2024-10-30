using MartyrGraveManagement_BAL.ModelViews.BlogDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.HistoricalEventDTOs
{
    public class HistoricalEventDTOResponse
    {
        public int HistoryId { get; set; }
        public string? HistoryEventName { get; set; }
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Status { get; set; }

        public List<BlogDTOResponse> Blogs { get; set; } = new List<BlogDTOResponse>();
    }
}
