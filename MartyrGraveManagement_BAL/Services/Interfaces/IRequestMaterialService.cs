using MartyrGraveManagement_BAL.ModelViews.RequestMaterialDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IRequestMaterialService
    {
        Task<List<RequestMaterialDtoResponse>> GetAllRequestMaterialsAsync();
    }
}
