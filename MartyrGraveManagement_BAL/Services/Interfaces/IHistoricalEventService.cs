using MartyrGraveManagement_BAL.ModelViews.HistoricalEventDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IHistoricalEventService
    {
        Task<List<HistoricalEventDTOResponse>> GetAllHistoricalEvents();
        Task<List<HistoricalEventDTOResponse>> GetHistoricalEventByAccount(int accountId);
        Task<HistoricalEventDTOResponse> CreateHistoricalEvent(CreateHistoricalEventDTORequest newEventRequest);
        Task<HistoricalEventDTOResponse> GetHistoricalEventById(int historicalEventId);
        Task<HistoricalEventDTOResponse> UpdateHistoricalEvent(int historyId, CreateHistoricalEventDTORequest updateRequest);
        Task<HistoricalEventDTOResponse> UpdateHistoricalEventStatus(int historyId, bool newStatus);
    }
}
