using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        //public int? StaffId { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string? Note { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; } = 0;
        public DateOnly? EndDate { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public int Status { get; set; } // 1 là đang chờ xác nhận từ manager, 2 là đã duyệt (dành cho báo cáo mộ và yêu cầu thêm dịch vụ vào mộ), 3 là từ chối, 4 là báo giá (dành cho đặt dịch vụ riêng), 5 là khách hàng đồng ý đặt dịch vụ riêng, 6 là hoàn thành

        public Account? Account { get; set; }
        public MartyrGrave? MartyrGrave { get; set; }
        //public IEnumerable<RequestImage>? RequestImages { get; set; }
        public IEnumerable<Request_Material>? RequestMaterials { get; set; }
        public IEnumerable<RequestNoteHistory>? RequestNoteHistories { get; set; }
        public RequestType? RequestType { get; set; }
        public ReportGrave? ReportGrave { get; set; }
        public RequestTask? RequestTask { get; set; }
        public RequestFeedback? RequestFeedback { get; set; }
    }
}
