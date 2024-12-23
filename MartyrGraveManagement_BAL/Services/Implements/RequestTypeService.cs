using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.BlogCategoryDTOs;
using MartyrGraveManagement_BAL.ModelViews.RequestTypeDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class RequestTypeService : IRequestTypeService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RequestTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<RequestTypeResponse>> GetAllRequestTypesAsync()
        {
            var requestTypes = await _unitOfWork.RequestTypeRepository.GetAllAsync();
            return requestTypes.Select(b => new RequestTypeResponse
            {
                TypeId = b.TypeId,
                TypeName = b.TypeName,
                TypeDescription = b.TypeDescription,
                Status = b.Status
            }).ToList();
        }
    }
}
