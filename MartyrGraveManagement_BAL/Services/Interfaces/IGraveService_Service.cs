using MartyrGraveManagement_BAL.ModelViews.GraveServiceDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IGraveService_Service
    {
        Task<(bool check, string response)> CreateServiceForGrave(GraveServiceDtoRequest request);
        Task<(bool check, string response)> DeleteServiceOfGrave(int graveServiceId);
        Task<(bool check, string response)> UpdateServiceForGrave(int graveServiceId, UpdateServiceForGraveDtoRequest request);
        Task<List<GraveServiceDtoResponse>> GetAllServicesForGrave(int martyrId, int? categoryId);
    }
}
