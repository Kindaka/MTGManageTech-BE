using MartyrGraveManagement_BAL.ModelViews.AreaDTos;
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
        Task<AreaDTOResponse> GetAreaById(int id);
        Task<bool> UpdateArea(int id, AreaDtoRequest updateArea);
        Task<bool> ToggleStatusArea(int id);
        Task<bool> DeleteArea(int id);
        Task<List<AreaDTOResponse>> GetAllAreasForStaffOrManager();
    }
}
