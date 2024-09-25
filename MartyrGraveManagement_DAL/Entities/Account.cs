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
        public int RoleId { get; set; }
        public int? AreaId { get; set; }
        public string EmailAddress { get; set; }
        public string HashedPassword { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? AvatarPath { get; set; }
        public bool Status { get; set; }

        public Role? Role { get; set; }
        //public Area? Area { get; set; }
        public IEnumerable<Order>? Orders { get; set; }
        public IEnumerable<Feedback>? Feedbacks { get; set; }
        public IEnumerable<WorkPerformance>? WorkPerformances { get; set; }
        public IEnumerable<StaffTask>? Tasks { get; set; }
        public IEnumerable<CartItem>? CartItems { get; set; }
        public IEnumerable<FeedbackResponse>? FeedbackResponses { get; set; }

    }
}
