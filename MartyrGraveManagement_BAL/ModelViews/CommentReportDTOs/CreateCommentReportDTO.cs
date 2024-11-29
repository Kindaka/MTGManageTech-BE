using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.CommentReportDTOs
{
    public class CreateCommentReportDTO
    {
        public int CommentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
