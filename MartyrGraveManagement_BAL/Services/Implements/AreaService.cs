using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.AreaDTos;
using MartyrGraveManagement_BAL.ModelViews.AreaDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class AreaService : IAreaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AreaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<bool> CreateNewArea(AreaDtoRequest newArea)
        {
            try
            {
                var area = new Area()
                {
                    AreaName = newArea.AreaName,
                    Description = newArea.Description,
                    Status = true
                };
                await _unitOfWork.AreaRepository.AddAsync(area);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AreaDTOResponse>> GetAreas()
        {
            try
            {
                List<AreaDTOResponse> areaList = new List<AreaDTOResponse> ();
                var areas = await _unitOfWork.AreaRepository.GetAllAsync();
                if (areas != null)
                {
                    foreach (var area in areas)
                    {
                        var areaResponse = _mapper.Map<AreaDTOResponse>(area);
                        areaList.Add(areaResponse);
                    }
                }
                return areaList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
