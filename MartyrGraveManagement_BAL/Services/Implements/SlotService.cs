using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.SlotDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using Org.BouncyCastle.Math.Field;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class SlotService : ISlotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SlotService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<SlotDtoResponse>> GetAllSlots()
        {
            try
            {
                var slots = await _unitOfWork.SlotRepository.GetAllAsync();
                if(slots != null)
                {
                    var slotListResponse = new List<SlotDtoResponse>();
                    foreach (var slot in slots) { 
                        var slotResponse = _mapper.Map<SlotDtoResponse>(slot);
                        slotListResponse.Add(slotResponse);
                    }
                    return slotListResponse;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SlotDtoResponse> GetDetailSlot(int slotId)
        {
            try
            {
                var slot = await _unitOfWork.SlotRepository.GetByIDAsync(slotId);
                if (slot != null)
                {
                    var slotResponse = _mapper.Map<SlotDtoResponse>(slot);
                    return slotResponse;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
