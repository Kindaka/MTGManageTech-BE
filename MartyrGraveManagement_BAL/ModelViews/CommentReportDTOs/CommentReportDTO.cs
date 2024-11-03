using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.CommentReportDTOs
{
    public class CommentReportDTO
    {
        public int ReportId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public int CommentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Status { get; set; }
    }
}
