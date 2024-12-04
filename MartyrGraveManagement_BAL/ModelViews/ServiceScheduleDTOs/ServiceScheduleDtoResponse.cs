using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.ServiceScheduleDTOs
{
    public class ServiceScheduleDtoResponse
    {
        public int ServiceScheduleId { get; set; }
        public int AccountId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceImage { get; set; }
        public int MartyrId { get; set; }
        public decimal Amount { get; set; }
        public DateOnly ScheduleDate { get; set; }
        public int DayOfMonth { get; set; } = 0;
        public int DayOfWeek { get; set; } = 0;
        public string? Note { get; set; }
        public int Status { get; set; }

        // Thông tin liệt sĩ
        public string MartyrName { get; set; }

        // Thông tin vị trí
        public int RowNumber { get; set; }
        public int MartyrNumber { get; set; }
        public int AreaNumber { get; set; }
    }
}
