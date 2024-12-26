using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.RequestMaterialDTOs;
using MartyrGraveManagement_BAL.ModelViews.RequestNoteHistoryDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class RequestMaterialService : IRequestMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RequestMaterialService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<RequestMaterialDtoResponse>> GetAllRequestMaterialsAsync()
        {
            var requestMaterials = await _unitOfWork.RequestMaterialRepository.GetAsync(
                includeProperties: "Material");

            return requestMaterials.Select(rm => new RequestMaterialDtoResponse
            {
                RequestMaterialId = rm.RequestMaterialId,
                MaterialId = rm.MaterialId,
                RequestId = rm.RequestId,
                CreatedAt = rm.CreatedAt,
                MaterialName = rm.Material?.MaterialName, 
                Description = rm.Material?.Description,
                ImagePath = rm.Material?.ImagePath,
                Price = rm.Material?.Price

            }).ToList();
        }

    }
}
