using MartyrGraveManagement_BAL.ModelViews.RequestMaterialDTOs;
using System.ComponentModel.DataAnnotations.Schema;

namespace MartyrGraveManagement_BAL.ModelViews.RequestCustomerDTOs
{
    public class RequestCustomerDtoResponse
    {
        public int RequestId { get; set; }
        public int CustomerId { get; set; }
        public int MartyrId { get; set; }
        public int TypeId { get; set; }
        public int? ServiceId { get; set; }
        public string ServiceName { get; set; }

        //public int? StaffId { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string? Note { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; } = 0;
        public DateOnly? EndDate { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public int Status { get; set; } // 1 là đang chờ xác nhận từ manager, 2 là đã duyệt (dành cho báo cáo mộ và yêu cầu thêm dịch vụ vào mộ), 3 là từ chối, 4 là báo giá (dành cho đặt dịch vụ riêng), 5 là khách hàng đồng ý đặt dịch vụ riêng, 6 là hoàn thành

        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }

        public string ManagerPhoneNumber { get; set; }
        public string ManagerName { get; set; }

        public string? MartyrCode { get; set; }
        public string? MartyrName { get; set; }
        public string? RequestTypeName { get; set; }

        public List<ReasonDto>? Reasons { get; set; }
        public RequestTaskDto? RequestTask { get; set; }
        public ReportTaskDto? ReportTask { get; set; }
        public List<RequestMaterialDTOResponse>? RequestMaterials { get; set; }


        public class ReasonDto
        {
            public string? RejectReason { get; set; }
            public DateTime RejectReason_CreateAt { get; set; }
        }

        public class RequestTaskDto
        {
            public int RequestTaskId { get; set; }
            public int? StaffId { get; set; }
            public string? PhoneNumber { get; set; }
            public string? StaffName { get; set; }
            public string? Description { get; set; }
            public string? ImageWorkSpace { get; set; }
            public string? Reason { get; set; }
            public int Status { get; set; }
            public DateTime CreateAt { get; set; }
            public DateTime UpdateAt { get; set; }
            public List<RequestTaskImageDto>? TaskImages { get; set; }
        }

        public class RequestTaskImageDto
        {
            public int RequestTaskImageId { get; set; }
            public string? ImageRequestTaskCustomer { get; set; }
            public DateTime CreateAt { get; set; }
        }

        public class ReportTaskDto
        {
            public int ReportId { get; set; }
            public int? StaffId { get; set; } 
            public string? PhoneNumber { get; set; }
            public string? StaffName { get; set; }
            public string? VideoFile { get; set; }
            public string? Description { get; set; }
            public DateTime CreateAt { get; set; }
            public List<ReportImageDto>? ReportImages { get; set; }
        }

        public class ReportImageDto
        {
            public int ImageId { get; set; }
            public string? UrlPath { get; set; }
            public DateTime CreateAt { get; set; }
        }

        public class RequestMaterialDTOResponse
        {
            public int RequestMaterialId { get; set; }
            public int MaterialId { get; set; }
            public string? MaterialName { get; set; }
            public string? Description { get; set; }
            public string? ImagePath { get; set; }
            [Column(TypeName = "decimal(18, 2)")]
            public decimal Price { get; set; }
        }
    }
}
