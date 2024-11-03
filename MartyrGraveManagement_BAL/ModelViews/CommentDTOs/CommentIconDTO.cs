using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.CommentDTOs
{
    public class CommentIconDTO
    {
        public int Id { get; set; }
        public int IconId { get; set; }
        public string IconImage { get; set; }
        public int Count { get; set; }
    }
}
