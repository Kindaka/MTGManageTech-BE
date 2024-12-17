using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class RequestCustomer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }
        public int CustomerId { get; set; }
        public int MartyrId { get; set; }
        public int TypeId { get; set; }
        public int? ServiceId { get; set; }
        public int? StaffId { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string? Note {  get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; } = 0;
        public DateOnly EndDate { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public int Status { get; set; } // 1 là đang chờ xác nhận từ manager, 2 là đã duyệt, 3 là từ chối

        public Account? Account { get; set; }
        public MartyrGrave? MartyrGrave { get; set; }
        public IEnumerable<RequestImage>? RequestImages { get; set; }
        public RequestType? RequestType { get; set; }
        public ReportGrave? ReportGrave { get; set; }
    }
}
