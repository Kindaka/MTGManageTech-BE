using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.AreaDTos;
using MartyrGraveManagement_BAL.ModelViews.AreaDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
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
                var areas = await _unitOfWork.AreaRepository.GetAllAsync();
                return _mapper.Map<List<AreaDTOResponse>>(areas);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AreaDTOResponse> GetAreaById(int id)
        {
            try
            {
                var area = await _unitOfWork.AreaRepository.GetByIDAsync(id);
                return area == null ? null : _mapper.Map<AreaDTOResponse>(area);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateArea(int id, AreaDtoRequest updateArea)
        {
            try
            {
                var area = await _unitOfWork.AreaRepository.GetByIDAsync(id);
                if (area == null)
                {
                    return false;
                }

                area.AreaName = updateArea.AreaName;
                area.Description = updateArea.Description;
                await _unitOfWork.AreaRepository.UpdateAsync(area);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteArea(int id)
        {
            try
            {
                var area = await _unitOfWork.AreaRepository.GetByIDAsync(id);
                if (area == null)
                {
                    return false;
                }

                await _unitOfWork.AreaRepository.DeleteAsync(area);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
