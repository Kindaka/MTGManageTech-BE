﻿using System;
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
        public int AccountId { get; set; } 
        public long OrderId { get; set; }
        public int  DetailId { get; set; }
        public DateTime StartDate { get; set; } // Create At
        public DateTime EndDate { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? Description { get; set; }
        public string? ImageWorkSpace { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string? Reason { get; set; }
        public int Status { get; set; }
        public Account? Account { get; set; }
        public OrderDetail? OrderDetail { get; set; }
        //public IEnumerable<ScheduleDetail>? ScheduleTasks { get; set; }
        public IEnumerable<TaskImage>? TaskImages { get; set; }


    }

}
