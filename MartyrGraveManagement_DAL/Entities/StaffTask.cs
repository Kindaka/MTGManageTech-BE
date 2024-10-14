using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class StaffTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }
        public int AccountId { get; set; } // check role 3, staff khu nào làm khu đó
        public int OrderId { get; set; } // Check exisst, check status 1
        public string NameOfWork { get; set; }
        public int TypeOfWork { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string? ImagePath { get; set; }
        public int Status { get; set; }
        public Account? Account { get; set; }
        public Order? Order { get; set; }
        //----------------------------------
        public string? UrlImage { get; set; }
        public string? Reason { get; set; }
    }

}
