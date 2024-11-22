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
                // Kiểm tra tồn tại của tài khoản khách hàng
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(feedbackDto.AccountId);
                if (account == null)
                {
                    return (false, "Account does not exist.", null);
                }

                // Kiểm tra tồn tại của OrderDetail và lấy thông tin Order
                var orderDetail = await _unitOfWork.OrderDetailRepository.GetAsync(
                    od => od.DetailId == feedbackDto.DetailId,
                    includeProperties: "Order"
                );

                var orderDetailEntity = orderDetail.FirstOrDefault();
                if (orderDetailEntity == null || orderDetailEntity.Order == null)
                {
                    return (false, "Order detail does not exist.", null);
                }

                // Kiểm tra xem tài khoản có phải là chủ sở hữu của đơn hàng không
                if (orderDetailEntity.Order.AccountId != feedbackDto.AccountId)
                {
                    return (false, "The account is not the owner of this order.", null);
                }

                // Kiểm tra trạng thái đơn hàng (chỉ cho phép tạo phản hồi nếu đơn hàng đã hoàn thành - status = 4)
                if (orderDetailEntity.Order.Status != 4)
                {
                    return (false, "Feedback can only be created for completed orders (Order status must be 4).", null);
                }

                // Tạo phản hồi mới
                var feedback = new Feedback
                {
                    AccountId = feedbackDto.AccountId,
                    DetailId = feedbackDto.DetailId,
                    Content = feedbackDto.Content,
                    Rating = feedbackDto.Rating,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Status = true
                };

                await _unitOfWork.FeedbackRepository.AddAsync(feedback);
                await _unitOfWork.SaveAsync();

                var responseDto = new FeedbackDtoResponse
                {
                    FeedbackId = feedback.FeedbackId,
                    AccountId = feedback.AccountId,
                    DetailId = feedback.DetailId,
                    Content = feedback.Content,
                    Rating = feedback.Rating,
                    CreatedAt = feedback.CreatedAt,
                    UpdatedAt = feedback.UpdatedAt,
                    Status = feedback.Status
                };

                return (true, "Feedback created successfully.", responseDto);
            }
            catch (Exception ex)
            {
                return (false, $"Error occurred: {ex.Message}", null);
            }
        }



        public async Task<(bool success, string message, FeedbackDtoResponse feedback)> GetFeedbackByIdAsync(int id)
        {
            // Truy xuất phản hồi bao gồm thông tin tài khoản khách hàng và chi tiết đơn hàng
            var feedback = await _unitOfWork.FeedbackRepository.GetAsync(
                f => f.FeedbackId == id,
                includeProperties: "Account,OrderDetail"
            );
            var feedbackEntity = feedback.FirstOrDefault();

            if (feedbackEntity == null)
            {
                return (false, "Feedback not found.", null);
            }

            // Truy xuất thông tin nhân viên nếu StaffId có giá trị
            string? fullNameStaff = null;
            if (feedbackEntity.StaffId != null)
            {
                var staffAccount = await _unitOfWork.AccountRepository.GetByIDAsync(feedbackEntity.StaffId.Value);
                fullNameStaff = staffAccount?.FullName;
            }

            // Tạo đối tượng phản hồi DTO
            var responseDto = new FeedbackDtoResponse
            {
                FeedbackId = feedbackEntity.FeedbackId,
                AccountId = feedbackEntity.AccountId,
                DetailId = feedbackEntity.DetailId,
                Content = feedbackEntity.Content,
                CreatedAt = feedbackEntity.CreatedAt,
                UpdatedAt = feedbackEntity.UpdatedAt,
                Status = feedbackEntity.Status,
                ResponseContent = feedbackEntity.ResponseContent,
                Rating = feedbackEntity.Rating,
                FullName = feedbackEntity.Account?.FullName,
                StaffId = feedbackEntity.StaffId ?? 0, // Gán StaffId nếu có, ngược lại gán giá trị mặc định
                FullNameStaff = fullNameStaff
            };

            return (true, "Feedback retrieved successfully.", responseDto);
        }




        public async Task<(bool success, string message, IEnumerable<FeedbackDtoResponse> feedbacks, int totalPage)> GetAllFeedbacksAsync(int page, int pageSize)
        {
            try
            {
                var totalFeedbacks = await _unitOfWork.FeedbackRepository.CountAsync();
                var totalPage = (int)Math.Ceiling(totalFeedbacks / (double)pageSize);

                // Lấy danh sách phản hồi với thông tin tài khoản khách hàng và chi tiết đơn hàng
                var feedbacks = await _unitOfWork.FeedbackRepository.GetAllAsync(
                    includeProperties: "Account,OrderDetail",
                    pageIndex: page,
                    pageSize: pageSize
                );

                var responseDtos = new List<FeedbackDtoResponse>();

                foreach (var feedback in feedbacks)
                {
                    // Kiểm tra nếu có StaffId, lấy FullName của nhân viên đó
                    string? fullNameStaff = null;
                    if (feedback.StaffId != null)
                    {
                        var staffAccount = await _unitOfWork.AccountRepository.GetByIDAsync(feedback.StaffId.Value);
                        fullNameStaff = staffAccount?.FullName;
                    }

                    // Tạo đối tượng phản hồi DTO
                    var feedbackDto = new FeedbackDtoResponse
                    {
                        FeedbackId = feedback.FeedbackId,
                        AccountId = feedback.AccountId,
                        DetailId = feedback.DetailId,
                        Content = feedback.Content,
                        CreatedAt = feedback.CreatedAt,
                        UpdatedAt = feedback.UpdatedAt,
                        Status = feedback.Status,
                        ResponseContent = feedback.ResponseContent,
                        Rating = feedback.Rating,
                        FullName = feedback.Account?.FullName, // Tên khách hàng
                        StaffId = feedback.StaffId ?? 0, // Gán StaffId nếu có
                        FullNameStaff = fullNameStaff // Tên nhân viên
                    };

                    responseDtos.Add(feedbackDto);
                }

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

        public async Task<(bool success, string message)> UpdateFeedbackResponseAsync(int feedbackId, FeedbackResponseDtoRequest feedbackDto)
        {
            try
            {
                // Kiểm tra tồn tại của Feedback
                var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(feedbackId);
                if (feedback == null)
                {
                    return (false, "Feedback not found.");
                }

                // Kiểm tra nếu nhân viên có quyền chỉnh sửa phản hồi này
                if (feedback.StaffId != feedbackDto.StaffId)
                {
                    return (false, "You do not have permission to update this feedback response.");
                }

                // Lấy thông tin OrderDetail để kiểm tra AreaId
                var orderDetail = await _unitOfWork.OrderDetailRepository.GetAsync(
                    od => od.DetailId == feedback.DetailId,
                    includeProperties: "MartyrGrave"
                );

                var orderDetailEntity = orderDetail.FirstOrDefault();
                if (orderDetailEntity?.MartyrGrave == null)
                {
                    return (false, "The related MartyrGrave for this feedback could not be found.");
                }

                // Kiểm tra nếu nhân viên thuộc AreaId của MartyrGrave
                var staffAccount = await _unitOfWork.AccountRepository.GetByIDAsync(feedbackDto.StaffId);
                if (staffAccount?.AreaId != orderDetailEntity.MartyrGrave.AreaId)
                {
                    return (false, "You do not have permission to update responses for feedback in this area.");
                }

                // Cập nhật nội dung phản hồi
                feedback.ResponseContent = feedbackDto.ResponseContent;
                feedback.UpdatedAt = DateTime.Now;

                await _unitOfWork.FeedbackRepository.UpdateAsync(feedback);
                await _unitOfWork.SaveAsync();

                return (true, "Feedback response updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
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
                // Kiểm tra nhân viên tồn tại
                var staffAccount = await _unitOfWork.AccountRepository.GetByIDAsync(feedbackDto.StaffId);
                if (staffAccount == null)
                {
                    return (false, "Nhân viên không tồn tại.");
                }

                // Lấy thông tin Feedback và kiểm tra nếu tồn tại
                var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(feedbackDto.FeedbackId);
                if (feedback == null)
                {
                    return (false, "Phản hồi không tồn tại.");
                }

                // Lấy OrderDetail để kiểm tra khu vực (AreaId)
                var orderDetail = await _unitOfWork.OrderDetailRepository.GetAsync(
                    od => od.DetailId == feedback.DetailId,
                    includeProperties: "MartyrGrave"
                );

                var orderDetailEntity = orderDetail.FirstOrDefault();
                if (orderDetailEntity?.MartyrGrave == null)
                {
                    return (false, "Không tìm thấy mộ liệt sĩ liên quan đến phản hồi.");
                }

                // Kiểm tra quyền khu vực
                if (staffAccount.AreaId != orderDetailEntity.MartyrGrave.AreaId)
                {
                    return (false, "Bạn không có quyền trả lời phản hồi cho khu vực này.");
                }

                // Cập nhật phản hồi của nhân viên
                feedback.ResponseContent = feedbackDto.ResponseContent;
                feedback.StaffId = feedbackDto.StaffId;
                feedback.UpdatedAt = DateTime.Now;

                await _unitOfWork.FeedbackRepository.UpdateAsync(feedback);
                await _unitOfWork.SaveAsync();

                return (true, "Phản hồi đã được tạo thành công.");
            }
            catch (Exception ex)
            {
                return (false, $"Có lỗi xảy ra: {ex.Message}");
            }
        }

        public async Task<(bool success, string message, FeedbackDtoResponse feedback)> GetFeedbackByDetailId(int detailId)
        {
            // Truy xuất phản hồi bao gồm thông tin tài khoản khách hàng và chi tiết đơn hàng
            var feedback = await _unitOfWork.FeedbackRepository.GetAsync(
                f => f.DetailId == detailId,
                includeProperties: "Account,OrderDetail"
            );
            var feedbackEntity = feedback.FirstOrDefault();

            if (feedbackEntity == null)
            {
                return (false, "Feedback not found.", null);
            }

            // Truy xuất thông tin nhân viên nếu StaffId có giá trị
            string? fullNameStaff = null;
            if (feedbackEntity.StaffId != null)
            {
                var staffAccount = await _unitOfWork.AccountRepository.GetByIDAsync(feedbackEntity.StaffId.Value);
                fullNameStaff = staffAccount?.FullName;
            }

            // Tạo đối tượng phản hồi DTO
            var responseDto = new FeedbackDtoResponse
            {
                FeedbackId = feedbackEntity.FeedbackId,
                AccountId = feedbackEntity.AccountId,
                DetailId = feedbackEntity.DetailId,
                Content = feedbackEntity.Content,
                CreatedAt = feedbackEntity.CreatedAt,
                UpdatedAt = feedbackEntity.UpdatedAt,
                Status = feedbackEntity.Status,
                ResponseContent = feedbackEntity.ResponseContent,
                Rating = feedbackEntity.Rating,
                FullName = feedbackEntity.Account?.FullName,
                StaffId = feedbackEntity.StaffId ?? 0, // Gán StaffId nếu có, ngược lại gán giá trị mặc định
                FullNameStaff = fullNameStaff
            };

            return (true, "Feedback retrieved successfully.", responseDto);
        }
    }
}
