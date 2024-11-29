﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.FeedbackDTOs
{
    public class FeedbackResponseDtoRequest
    {
        public int FeedbackId { get; set; }   
        public int StaffId { get; set; }
        [Required]
        public string? ResponseContent { get; set; }
    }
}
