using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface ITrendingRecommendationService 
    {
        Task<List<ServiceDtoResponse>> RecommendTopTrendingServices(int topN = 5);
    }
}
