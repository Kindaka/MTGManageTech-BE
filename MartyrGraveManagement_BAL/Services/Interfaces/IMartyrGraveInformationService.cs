using MartyrGraveManagement_BAL.ModelViews.MartyrGraveInformationDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IMartyrGraveInformationService
    {
        Task<IEnumerable<MartyrGraveInformationDtoResponse>> GetAllAsync();
        Task<MartyrGraveInformationDtoResponse> GetByIdAsync(int id);
        Task<MartyrGraveInformationDtoResponse> CreateAsync(MartyrGraveInformationDtoRequest martyrGraveInformationDto);
        Task<MartyrGraveInformationDtoResponse> UpdateAsync(int id, MartyrGraveInformationDtoRequest martyrGraveInformationDto);
        Task<bool> DeleteAsync(int id);
    }
}
