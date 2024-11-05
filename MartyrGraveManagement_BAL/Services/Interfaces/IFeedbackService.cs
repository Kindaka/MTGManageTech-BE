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
        Task<(bool success, string message, IEnumerable<FeedbackDtoResponse> feedbacks, int totalPage)> GetAllFeedbacksAsync(int page, int pageSize);
        Task<(bool success, string message)> UpdateFeedbackAsync(int id, FeedbackContentDtoRequest feedbackDto);
        Task<(bool success, string message)> ChangeStatusFeedbackAsync(int id);
        Task<(bool success, string message)> DeleteFeedbackAsync(int id);
        Task<(bool success, string message)> CreateFeedbackResponseAsync(FeedbackResponseDtoRequest feedbackDto);
        Task<(bool success, string message)> UpdateFeedbackResponseAsync(int feedbackId, FeedbackResponseDtoRequest feedbackDto);
    }
}
