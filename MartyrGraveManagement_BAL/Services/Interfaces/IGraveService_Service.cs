using MartyrGraveManagement_BAL.ModelViews.ServiceCategoryDTOs;
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
        Task<(bool status, string result)> AddService(ServiceDtoRequest service);
        Task<(bool status, string result)> UpdateService(ServiceDtoRequest service, int serviceId);
        Task<List<ServiceDtoResponse>> GetAllServices();
    }
}
