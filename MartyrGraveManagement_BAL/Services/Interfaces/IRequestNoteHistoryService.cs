using MartyrGraveManagement_BAL.ModelViews.RequestNoteHistoryDTOs;
using MartyrGraveManagement_BAL.ModelViews.RequestTypeDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IRequestNoteHistoryService
    {
        Task<List<RequestNoteHistoryResponse>> GetAllRequestNoteHistoriesAsync();
    }
}
