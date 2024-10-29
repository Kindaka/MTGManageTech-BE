using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Comment_Report
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReportId { get; set; }
        public int AccountId { get; set; }
        public int CommentId { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string Title { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Status { get; set; }

        public Comment? Comment { get; set; }
        public Account? Account { get;set; } 
    }
}
