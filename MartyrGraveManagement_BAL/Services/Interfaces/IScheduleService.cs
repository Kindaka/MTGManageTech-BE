using MartyrGraveManagement_BAL.ModelViews.ScheduleDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<string> CreateSchedule(CreateScheduleDTORequest request);
    }
}
