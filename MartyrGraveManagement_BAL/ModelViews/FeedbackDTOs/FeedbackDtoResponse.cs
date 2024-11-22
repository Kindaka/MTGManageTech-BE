using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.FeedbackDTOs
{
    public class FeedbackDtoResponse
    {
        public int FeedbackId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string AvatarPath { get; set; }
        public int DetailId { get; set; }
        public int StaffId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Status { get; set; }
        public string? ResponseContent { get; set; }
        public int Rating { get; set; }
        public string FullName { get; set; }  
        public string FullNameStaff { get; set; }
    }
}
