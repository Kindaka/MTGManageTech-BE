using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MartyrGraveManagement_DAL.Entities
{
    public class RequestTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestTaskId { get; set; }
        public int StaffId { get; set; }
        public int RequestId { get; set; }
        public DateTime StartDate { get; set; } // Create At
        public DateTime EndDate { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string? Description { get; set; }
        public string? ImageWorkSpace { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string? Reason { get; set; }
        public int Status { get; set; }
        public Account? Account { get; set; }
        public RequestCustomer? RequestCustomer { get; set; }
        //public IEnumerable<ScheduleDetail>? ScheduleTasks { get; set; }
        public IEnumerable<RequestTaskImage>? RequestTaskImages { get; set; }
    }
}
