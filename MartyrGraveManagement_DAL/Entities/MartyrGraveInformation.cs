﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class MartyrGraveInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InformationId { get; set; }
        public int MartyrId { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string? NickName { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string? Position { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? Medal { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string? HomeTown { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime DateOfSacrifice { get; set; }

        public MartyrGrave MartyrGrave { get; set; }
    }

}
