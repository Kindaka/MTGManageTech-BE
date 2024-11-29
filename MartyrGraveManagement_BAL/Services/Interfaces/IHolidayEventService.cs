using MartyrGraveManagement_BAL.ModelViews.HolidayEventDTOs;
using MartyrGraveManagement_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IHolidayEventService
    {
        Task<bool> DeleteHolidayEventAsync(int eventId);
        Task<HolidayEventResponseDto> GetHolidayEventByIdAsync(int eventId);
        Task<List<HolidayEventResponseDto>> GetAllHolidayEventsAsync();
        Task<bool> UpdateHolidayEventAsync(int eventId, int accountId, HolidayEventRequestDto holidayEventDto);
        Task<bool> CreateHolidayEventAsync(int accountId, HolidayEventRequestDto holidayEventDto);
        Task<bool> UpdateHolidayEventStatusAsync(int eventId, bool status);

    }
}
