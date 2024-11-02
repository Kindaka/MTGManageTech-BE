using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }
        public string? CustomerCode { get; set; }
        public int RoleId { get; set; }
        public int? AreaId { get; set; }
        public string HashedPassword { get; set; }
        public string? EmailAddress { get; set; } // để gửi WeeklyReport qua mail không dùng đề login
        [Column(TypeName = "nvarchar(255)")]
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string? Address { get; set; }
        public string? AvatarPath { get; set; }
        public DateTime CreateAt { get; set; }
        public bool Status { get; set; }

        public Role? Role { get; set; }
        public IEnumerable<Order>? Orders { get; set; }
        public IEnumerable<Feedback>? Feedbacks { get; set; }
        public IEnumerable<WorkPerformance>? WorkPerformances { get; set; }
        public IEnumerable<StaffTask>? Tasks { get; set; }
        public IEnumerable<CartItemCustomer>? CartItems { get; set; }
        public IEnumerable<Comment>? Comments { get; set; }
        public IEnumerable<MartyrGrave>? MartyrGraves { get; set; }
        public IEnumerable<Comment_Icon>? Comment_Icons { get; set; }
        public IEnumerable<Comment_Report>? Comment_Reports { get; set; }
        public IEnumerable<ScheduleDetail>? ScheduleTasks { get; set; }
        public IEnumerable<Holiday_Event>? Holiday_Events { get; set; }
        public IEnumerable<NotificationAccount>? NotificationAccounts { get; set; }
        public IEnumerable<Blog>? Blogs { get; set; }
        public IEnumerable<Attendance>? Attendances { get; set; }


    }
}
