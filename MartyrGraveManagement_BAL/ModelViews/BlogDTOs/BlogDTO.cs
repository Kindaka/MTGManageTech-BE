using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.BlogDTOs
{
    public class BlogDTO
    {
        public int BlogId { get; set; }
        public string BlogName { get; set; }
        public string BlogDescription { get; set; }
        public string BlogContent { get; set; }
        public int AccountId { get; set; }
        public string FullName { get; set; }
        public int HistoryId { get; set; }
        public string BlogCategoryName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Status { get; set; }
        public List<string> HistoricalImages { get; set; }

        public List<CommentDetailDTO> Comments { get; set; }
        public List<MartyrDetailDTO> RelatedMartyrDetails { get; set; }
    }

    public class MartyrDetailDTO
    {
        public string Name { get; set; }
        public List<string> Images { get; set; }
    }

    public class CommentDetailDTO
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string AccountName { get; set; } // Tên của người bình luận

        // Danh sách các biểu tượng liên kết với bình luận này
        public List<CommentIconDetailDTO> CommentIcons { get; set; }

    }

    public class CommentIconDetailDTO
    {
        public int Id { get; set; }            // Id của Comment_Icon
        public int IconId { get; set; }        // Id của biểu tượng
        public string IconImage { get; set; } // Đường dẫn biểu tượng
        public List<string> AccountNames { get; set; }
        public int Count { get; set; }        // Số lần xuất hiện của biểu tượng này
    }
}


