using MartyrGraveManagement_BAL.ModelViews.MaterialDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IMaterialService
    {
        Task<IEnumerable<MaterialDtoResponse>> GetAllMaterials();
        Task<IEnumerable<MaterialDtoResponse>> GetMaterialsForAdmin();
        Task<MaterialDtoResponse> GetMaterialById(int id);
        Task<(bool success, string message)> CreateMaterial(MaterialDtoRequest materialDto);
        Task<(bool success, string message)> UpdateMaterial(int id, MaterialDtoRequest materialDto);
        Task<(bool success, string message)> UpdateStatus(int id);
        Task<List<MaterialDtoResponse>> GetMaterialsByServiceIdAsync(int serviceId);
    }
}
