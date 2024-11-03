using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.CommentIconDTOs
{
    public class CreateCommentIconDTO
    {
        public int Id { get; set; }
        public int IconId { get; set; }
        public int CommentId { get; set; }
        public int AccountId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Status { get; set; }
        public string? IconImage { get; set; }
        public string? AccountName { get; set; }
    }

}
