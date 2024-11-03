using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.CommentDTOs
{
    public class CreateCommentDTO
    {
        public int BlogId { get; set; }
        public string Content { get; set; }
    }
}
