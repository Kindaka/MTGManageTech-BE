using MartyrGraveManagement_BAL.ModelViews.AssignmentTaskFeedbackDTOs;
using MartyrGraveManagement_BAL.ModelViews.RequestFeedbackDTOs;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IRequestFeedbackService
    {
        Task<(bool success, string message, RequestFeedbackDtoResponse feedback)> CreateFeedbackAsync(RequestFeedbackDtoRequest feedbackDto);
        Task<(bool success, string message, RequestFeedbackDtoResponse feedback)> GetFeedbackByIdAsync(int id);
        Task<(bool success, string message, RequestFeedbackDtoResponse feedback)> GetFeedbackByRequestId(int requestId);
        Task<(bool success, string message, IEnumerable<RequestFeedbackDtoResponse> feedbacks, int totalPage)> GetAllFeedbacksAsync(int page, int pageSize);
        Task<(bool success, string message)> UpdateFeedbackAsync(int id, int customerId, AssignmentTaskFeedbackContentDtoRequest feedbackDto);
        Task<(bool success, string message)> ChangeStatusFeedbackAsync(int id);
        Task<(bool success, string message)> DeleteFeedbackAsync(int id);
        Task<(bool success, string message)> CreateFeedbackResponseAsync(RequestFeedbackResponseDtoRequest feedbackDto);
        Task<(bool success, string message)> UpdateFeedbackResponseAsync(RequestFeedbackResponseDtoRequest feedbackDto);
    }
}
