using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.FeedbackDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(bool success, string message, FeedbackDtoResponse feedback)> CreateFeedbackAsync(FeedbackDtoRequest feedbackDto)
        {
            try
            {
                // Validate Account and Order existence
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(feedbackDto.AccountId);
                if (account == null)
                {
                    return (false, "Account does not exist.", null);
                }

                var order = await _unitOfWork.OrderRepository.GetByIDAsync(feedbackDto.OrderId);
                if (order == null)
                {
                    return (false, "Order does not exist.", null);
                }

                // Create new Feedback
                var feedback = _mapper.Map<Feedback>(feedbackDto);
                feedback.CreatedAt = DateTime.Now;
                feedback.UpdatedAt = DateTime.Now;

                await _unitOfWork.FeedbackRepository.AddAsync(feedback);
                await _unitOfWork.SaveAsync();

                var responseDto = _mapper.Map<FeedbackDtoResponse>(feedback);
                return (true, "Feedback created successfully.", responseDto);
            }
            catch (Exception ex)
            {
                return (false, $"Error occurred: {ex.Message}", null);
            }
        }

        public async Task<(bool success, string message, FeedbackDtoResponse feedback)> GetFeedbackByIdAsync(int id)
        {
            var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(id);
            if (feedback == null)
            {
                return (false, "Feedback not found.", null);
            }

            var responseDto = _mapper.Map<FeedbackDtoResponse>(feedback);
            return (true, "Feedback retrieved successfully.", responseDto);
        }

        public async Task<(bool success, string message, IEnumerable<FeedbackDtoResponse> feedbacks)> GetAllFeedbacksAsync()
        {
            var feedbacks = await _unitOfWork.FeedbackRepository.GetAllAsync();
            var responseDtos = _mapper.Map<IEnumerable<FeedbackDtoResponse>>(feedbacks);
            return (true, "Feedbacks retrieved successfully.", responseDtos);
        }

        public async Task<(bool success, string message)> UpdateFeedbackAsync(int id, FeedbackDtoRequest feedbackDto)
        {
            try
            {
                var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(id);
                if (feedback == null)
                {
                    return (false, "Feedback not found.");
                }

                _mapper.Map(feedbackDto, feedback);
                feedback.UpdatedAt = DateTime.Now;

                await _unitOfWork.FeedbackRepository.UpdateAsync(feedback);
                await _unitOfWork.SaveAsync();

                return (true, "Feedback updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error occurred: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> DeleteFeedbackAsync(int id)
        {
            try
            {
                var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(id);
                if (feedback == null)
                {
                    return (false, "Feedback not found.");
                }

                await _unitOfWork.FeedbackRepository.DeleteAsync(feedback);
                await _unitOfWork.SaveAsync();

                return (true, "Feedback deleted successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error occurred: {ex.Message}");
            }
        }
    }
}
