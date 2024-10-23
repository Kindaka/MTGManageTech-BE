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
                // Validate Account existence
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(feedbackDto.AccountId);
                if (account == null)
                {
                    return (false, "Account does not exist.", null);
                }

                // Validate Order existence
                var order = await _unitOfWork.OrderRepository.GetByIDAsync(feedbackDto.OrderId);
                if (order == null)
                {
                    return (false, "Order does not exist.", null);
                }

                // Check if the order is owned by the account
                if (order.AccountId != feedbackDto.AccountId)
                {
                    return (false, "The account is not the owner of this order.", null);
                }

                // Check if the order status is 4 (completed)
                if (order.Status != 4)
                {
                    return (false, "Feedback can only be created for completed orders (Order status must be 4).", null);
                }

                // Create new Feedback
                var feedback = _mapper.Map<Feedback>(feedbackDto);
                feedback.CreatedAt = DateTime.Now;
                feedback.UpdatedAt = DateTime.Now;
                feedback.Status = true;  // Assuming feedback is active when created

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
            // Lấy feedback bao gồm thông tin Account
            var feedback = await _unitOfWork.FeedbackRepository
                .GetAsync(f => f.FeedbackId == id, includeProperties: "Account");

            var feedbackEntity = feedback.FirstOrDefault();
            if (feedbackEntity == null)
            {
                return (false, "Feedback not found.", null);
            }

            // Map dữ liệu từ Feedback sang DTO
            var responseDto = _mapper.Map<FeedbackDtoResponse>(feedbackEntity);

            // Bổ sung thêm FullName và CustomerCode từ Account
            if (feedbackEntity.Account != null)
            {
                responseDto.FullName = feedbackEntity.Account.FullName;
                responseDto.CustomerCode = feedbackEntity.Account.CustomerCode;
            }

            return (true, "Feedback retrieved successfully.", responseDto);
        }


        public async Task<(bool success, string message, IEnumerable<FeedbackDtoResponse> feedbacks, int totalPage)> GetAllFeedbacksAsync(int page, int pageSize)
        {
            try
            {
                // Đếm tổng số phản hồi
                var totalFeedbacks = await _unitOfWork.FeedbackRepository.CountAsync();
                var totalPage = (int)Math.Ceiling(totalFeedbacks / (double)pageSize);

                // Lấy danh sách phản hồi với paging và include thông tin Account
                var feedbacks = await _unitOfWork.FeedbackRepository.GetAllAsync(
                    includeProperties: "Account", pageIndex: page, pageSize: pageSize);

                // Map dữ liệu sang DTO và bao gồm thêm FullName và CustomerCode từ Account
                var responseDtos = feedbacks.Select(f => new FeedbackDtoResponse
                {
                    FeedbackId = f.FeedbackId,
                    AccountId = f.AccountId,
                    OrderId = f.OrderId,
                    Content = f.Content,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt,
                    Status = f.Status,
                    FullName = f.Account?.FullName,         // Lấy FullName từ bảng Account
                    CustomerCode = f.Account?.CustomerCode  // Lấy CustomerCode từ bảng Account
                }).ToList();

                return (true, "Feedbacks retrieved successfully.", responseDtos, totalPage);
            }
            catch (Exception ex)
            {
                return (false, $"Error occurred: {ex.Message}", null, 0);
            }
        }


        public async Task<(bool success, string message)> UpdateFeedbackAsync(int id, FeedbackContentDtoRequest feedbackDto)
        {
            try
            {
                var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(id);
                if (feedback == null)
                {
                    return (false, "Feedback not found.");
                }

                // Cập nhật chỉ thuộc tính Content
                feedback.Content = feedbackDto.Content;
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

        public async Task<(bool success, string message)> ChangeStatusFeedbackAsync(int id)
        {
            try
            {
                var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(id);
                if (feedback == null)
                {
                    return (false, "Feedback not found.");
                }

                // Tự động thay đổi trạng thái
                feedback.Status = !feedback.Status;  // Đổi trạng thái từ true -> false và ngược lại
                feedback.UpdatedAt = DateTime.Now;

                await _unitOfWork.FeedbackRepository.UpdateAsync(feedback);
                await _unitOfWork.SaveAsync();

                return (true, $"Feedback status changed to {(feedback.Status ? "Active" : "Inactive")} successfully.");
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

        public async Task<(bool success, string message)> CreateFeedbackResponseAsync(FeedbackResponseDtoRequest feedbackDto)
        {
            try
            {
                // Validate Account existence
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(feedbackDto.AccountId);
                if (account == null)
                {
                    return (false, "Tài khoản không tìm thấy.");
                }

                var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(feedbackDto.FeedbackId);
                if (feedback != null)
                {
                    feedback.ResponseContent = feedbackDto.ResponseContent;
                    feedback.StaffId = feedbackDto.AccountId;
                    feedback.UpdatedAt = DateTime.Now;
                    await _unitOfWork.FeedbackRepository.UpdateAsync(feedback);
                    await _unitOfWork.SaveAsync();
                    return (true, "Tạo thành công");
                }
                return (false, "Phản hồi được tạo thành công.");
            }
            catch (Exception ex)
            {
                return (false, $"Error occurred: {ex.Message}");
            }
        }
    }
}
