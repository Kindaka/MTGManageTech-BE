using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }
        public int AccountId { get; set; }
        public int BlogId { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Status { get; set; }

        // Navigation properties
        public Account? Account { get; set; }
        public Blog? Blog { get; set; }
        public IEnumerable<Comment_Icon>? Comment_Icons { get; set; }
        public IEnumerable<Comment_Report>? Comment_Reports { get; set; }
    }
}
