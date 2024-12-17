using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    [Table("ScheduleDetail")]
    public class ScheduleDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int TaskId { get; set; } // giá trị lưu cho cả Task, AssignmentTask, Request phân biệt dựa vào ScheduleDetailType
        public DateOnly Date { get; set; }
        public string? Description { get; set; }
        public int ScheduleDetailType {  get; set; } //1 là dịch vụ bình thường, 2 là dịch vụ định kì, 3 là yêu cầu thân nhân (request)
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public int Status { get; set; }

        public Account? Account { get; set; }
        //public StaffTask? StaffTask { get; set; }
        //public Slot? Slot { get; set; }
    }
}
