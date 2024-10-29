﻿using MartyrGraveManagement_BAL.ModelViews.CustomerDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveInformationDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs
{
    public class MartyrGraveDtoRequest
    {     

        [Required(ErrorMessage = "AreaId is required.")]
        public int AreaId { get; set; }

        public int LocationId { get; set; }





        public CustomerDtoRequest Customer { get; set; } = new CustomerDtoRequest();

        public List<MartyrGraveInformationDtoRequest> Informations { get; set; } = new List<MartyrGraveInformationDtoRequest>();

        public List<GraveImageDtoRequest> Image { get; set; } = new List<GraveImageDtoRequest>();


    }
}
