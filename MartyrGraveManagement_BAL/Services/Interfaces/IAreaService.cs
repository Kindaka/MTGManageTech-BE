﻿using MartyrGraveManagement_BAL.ModelViews.AreaDTos;
using MartyrGraveManagement_BAL.ModelViews.AreaDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IAreaService
    {
        Task<bool> CreateNewArea(AreaDtoRequest newArea);
        Task<List<AreaDTOResponse>> GetAreas();
    }
}
