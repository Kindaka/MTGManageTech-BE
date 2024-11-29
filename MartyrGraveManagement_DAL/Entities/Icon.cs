﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Icon
    {
        public int IconId { get; set; }
        public string? IconImage { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string? IconName { get; set; }
        public IEnumerable<Comment_Icon>? Comment_Icons { get; set; }
    }
}
