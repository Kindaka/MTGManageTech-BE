using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.RequestNoteHistoryDTOs;
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
    public class RequestNoteHistoryService : IRequestNoteHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RequestNoteHistoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<RequestNoteHistoryResponse>> GetAllRequestNoteHistoriesAsync()
        {
            var requestTypes = await _unitOfWork.RequestNoteHistoryRepository.GetAllAsync();
            return requestTypes.Select(b => new RequestNoteHistoryResponse
            {
                NoteId = b.NoteId,
                RequestId = b.RequestId,
                AccountId = b.AccountId,
                Status = b.Status,
                CreateAt = b.CreateAt,
                UpdateAt = b.UpdateAt,
                Note = b.Note
            }).ToList();
        }
    }
}
