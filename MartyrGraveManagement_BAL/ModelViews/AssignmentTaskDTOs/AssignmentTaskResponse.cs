using MartyrGraveManagement_BAL.ModelViews.StaffDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.AssignmentTaskDTOs
{
    public class AssignmentTaskResponse
    {
        public int AssignmentTaskId { get; set; }
        public int ServiceScheduleId { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string StaffPhone { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string ImageWorkSpace { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }

        // Thông tin từ Service_Schedule
        public int AccountId { get; set; }  // ID của khách hàng đặt lịch
        public string CustomerName { get; set; }  // Tên khách hàng
        public string CustomerPhone { get; set; }  // SĐT khách hàng
        public decimal Amount { get; set; }
        public DateOnly ScheduleDate { get; set; }
        public int DayOfMonth { get; set; }
        public int DayOfWeek { get; set; }
        public string Note { get; set; }

        // Thông tin từ Service
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public string ServiceImage { get; set; }
        public string CategoryName { get; set; }
        public int RecurringType { get; set; }

        // Thông tin vị trí mộ
        public string GraveLocation { get; set; }

        // Danh sách hình ảnh task
        public List<string> TaskImages { get; set; } = new List<string>();

        public List<StaffDtoResponse>? Staffs { get; set; } = new List<StaffDtoResponse>();
    }
}
