using MartyrGraveManagement_BAL.ModelViews.AssignmentTaskFeedbackDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IAssignmentTaskFeedbackService
    {
        Task<(bool success, string message, AssignmentTaskFeedbackDtoResponse feedback)> CreateFeedbackAsync(AssignmentTaskFeedbackDtoRequest feedbackDto);
        Task<(bool success, string message, AssignmentTaskFeedbackDtoResponse feedback)> GetFeedbackByIdAsync(int id);
        Task<(bool success, string message, AssignmentTaskFeedbackDtoResponse feedback)> GetFeedbackByAssignmentTaskId(int taskId);
        Task<(bool success, string message, IEnumerable<AssignmentTaskFeedbackDtoResponse> feedbacks, int totalPage)> GetAllFeedbacksAsync(int page, int pageSize);
        Task<(bool success, string message)> UpdateFeedbackAsync(int id, AssignmentTaskFeedbackContentDtoRequest feedbackDto);
        Task<(bool success, string message)> ChangeStatusFeedbackAsync(int id);
        Task<(bool success, string message)> DeleteFeedbackAsync(int id);
        Task<(bool success, string message)> CreateFeedbackResponseAsync(AssignmentTaskFeedbackResponseDtoRequest feedbackDto);
        Task<(bool success, string message)> UpdateFeedbackResponseAsync(int feedbackId, AssignmentTaskFeedbackResponseDtoRequest feedbackDto);
    }
}
