using MartyrGraveManagement_BAL.ModelViews.ServiceCategoryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IServiceCategory_Service
    {
        Task<bool> AddServiceCategory(ServiceCategoryDto category);
        Task<(bool status, string result)> UpdateServiceCategory(ServiceCategoryDto category, int categoryId);
        Task<List<ServiceCategoryDtoResponse>> GetAllServiceCategories();
    }
}
