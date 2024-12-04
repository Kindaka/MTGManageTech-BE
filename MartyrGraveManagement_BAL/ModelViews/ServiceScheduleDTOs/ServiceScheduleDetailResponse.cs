using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.ServiceScheduleDTOs
{
    public class ServiceScheduleDetailResponse : ServiceScheduleDtoResponse
    {
        // Thông tin liệt sĩ
        public string MartyrName { get; set; }
        public string MartyrCode { get; set; }

        // Thông tin vị trí
        public int RowNumber { get; set; }
        public int MartyrNumber { get; set; }
        public int AreaNumber { get; set; }

        // Thông tin người đặt lịch
        public string AccountName { get; set; }
        public string PhoneNumber { get; set; }

        // Thông tin công việc được giao
        public AssignmentTaskInfo LatestAssignment { get; set; }
    }

    public class AssignmentTaskInfo
    {
        public string StaffName { get; set; }
        public string ImageWorkSpace { get; set; }
        public int Status { get; set; }
        public List<string> TaskImages { get; set; } = new List<string>();
    }
}
