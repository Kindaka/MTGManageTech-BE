using MartyrGraveManagement_BAL.ModelViews.MartyrGraveInformationDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class MartyrGraveInformationService : IMartyrGraveInformationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MartyrGraveInformationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MartyrGraveInformationDtoResponse>> GetAllAsync()
        {
            var informationList = await _unitOfWork.MartyrGraveInformationRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MartyrGraveInformationDtoResponse>>(informationList);
        }

        public async Task<MartyrGraveInformationDtoResponse> GetByIdAsync(int id)
        {
            var information = await _unitOfWork.MartyrGraveInformationRepository.GetByIDAsync(id);
            return _mapper.Map<MartyrGraveInformationDtoResponse>(information);
        }

        public async Task<MartyrGraveInformationDtoResponse> CreateAsync(MartyrGraveInformationDtoRequest martyrGraveInformationDto)
        {
            // Kiểm tra sự tồn tại của MartyrId
            var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(martyrGraveInformationDto.MartyrId);
            if (martyrGrave == null)
            {
                throw new KeyNotFoundException("MartyrId does not exist.");
            }

            var martyrGraveInformation = _mapper.Map<MartyrGraveInformation>(martyrGraveInformationDto);
            await _unitOfWork.MartyrGraveInformationRepository.AddAsync(martyrGraveInformation);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<MartyrGraveInformationDtoResponse>(martyrGraveInformation);
        }

        public async Task<MartyrGraveInformationDtoResponse> UpdateAsync(int id, MartyrGraveInformationDtoRequest martyrGraveInformationDto)
        {
            // Kiểm tra sự tồn tại của MartyrId
            var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(martyrGraveInformationDto.MartyrId);
            if (martyrGrave == null)
            {
                throw new KeyNotFoundException("MartyrId does not exist.");
            }

            var martyrGraveInformation = await _unitOfWork.MartyrGraveInformationRepository.GetByIDAsync(id);
            if (martyrGraveInformation == null)
            {
                return null;
            }

            _mapper.Map(martyrGraveInformationDto, martyrGraveInformation);
            await _unitOfWork.MartyrGraveInformationRepository.UpdateAsync(martyrGraveInformation);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<MartyrGraveInformationDtoResponse>(martyrGraveInformation);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var martyrGraveInformation = await _unitOfWork.MartyrGraveInformationRepository.GetByIDAsync(id);
            if (martyrGraveInformation == null)
            {
                return false;
            }

            await _unitOfWork.MartyrGraveInformationRepository.DeleteAsync(martyrGraveInformation);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
