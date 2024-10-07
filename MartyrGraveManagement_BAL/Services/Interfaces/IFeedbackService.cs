using MartyrGraveManagement_BAL.ModelViews.FeedbackDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<(bool success, string message, FeedbackDtoResponse feedback)> CreateFeedbackAsync(FeedbackDtoRequest feedbackDto);
        Task<(bool success, string message, FeedbackDtoResponse feedback)> GetFeedbackByIdAsync(int id);
        Task<(bool success, string message, IEnumerable<FeedbackDtoResponse> feedbacks)> GetAllFeedbacksAsync();
        Task<(bool success, string message)> UpdateFeedbackAsync(int id, FeedbackDtoRequest feedbackDto);
        Task<(bool success, string message)> DeleteFeedbackAsync(int id);
    }
}
