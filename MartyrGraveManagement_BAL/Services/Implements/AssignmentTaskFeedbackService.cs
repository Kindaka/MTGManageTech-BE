using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.AssignmentTaskFeedbackDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class AssignmentTaskFeedbackService : IAssignmentTaskFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AssignmentTaskFeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(bool success, string message, AssignmentTaskFeedbackDtoResponse feedback)> CreateFeedbackAsync(AssignmentTaskFeedbackDtoRequest feedbackDto)
        {
            try
            {
                // Kiểm tra tồn tại của tài khoản khách hàng
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(feedbackDto.CustomerId);
                if (account == null)
                {
                    return (false, "Account does not exist.", null);
                }

                // Kiểm tra tồn tại của AssignmentTask và lấy thông tin Service_Schedule
                var assignmentTask = await _unitOfWork.AssignmentTaskRepository.GetAsync(
                    od => od.AssignmentTaskId == feedbackDto.AssignmentTaskId,
                    includeProperties: "Service_Schedule"
                );

                var assignmentTaskEntity = assignmentTask.FirstOrDefault();
                if (assignmentTaskEntity == null || assignmentTaskEntity.Service_Schedule == null)
                {
                    return (false, "Assignment Task does not exist.", null);
                }

                // Kiểm tra xem tài khoản có phải là chủ sở hữu của đơn hàng không
                if (assignmentTaskEntity.Service_Schedule.AccountId != feedbackDto.CustomerId)
                {
                    return (false, "The account is not the owner of this service.", null);
                }

                // Kiểm tra trạng thái đơn hàng (chỉ cho phép tạo phản hồi nếu đơn hàng đã hoàn thành - status = 4)
                if (assignmentTaskEntity.Status != 4)
                {
                    return (false, "Feedback can only be created for completed tasks (Task status must be 4).", null);
                }

                // Tạo phản hồi mới
                var feedback = new AssignmentTask_Feedback
                {
                    CustomerId = feedbackDto.CustomerId,
                    AssignmentTaskId = feedbackDto.AssignmentTaskId,
                    Content = feedbackDto.Content,
                    Rating = feedbackDto.Rating,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Status = true
                };

                await _unitOfWork.AssignmentTaskFeedbackRepository.AddAsync(feedback);
                await _unitOfWork.SaveAsync();

                var responseDto = new AssignmentTaskFeedbackDtoResponse
                {
                    AssignmentTaskFeedbackId = feedback.AssignmentTaskFeedbackId,
                    CustomerId = feedback.CustomerId,
                    AssignmentTaskId = feedback.AssignmentTaskId,
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



        public async Task<(bool success, string message, AssignmentTaskFeedbackDtoResponse feedback)> GetFeedbackByIdAsync(int id)
        {
            // Truy xuất phản hồi bao gồm thông tin tài khoản khách hàng và chi tiết đơn hàng
            var feedback = await _unitOfWork.AssignmentTaskFeedbackRepository.GetAsync(
                f => f.AssignmentTaskId == id
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
                var staffAccount = await _unitOfWork.AccountRepository.GetByIDAsync(feedbackEntity.StaffId);
                fullNameStaff = staffAccount?.FullName;
            }
            var customerAccount = await _unitOfWork.AccountRepository.GetByIDAsync(feedbackEntity.CustomerId);

            // Tạo đối tượng phản hồi DTO
            var responseDto = new AssignmentTaskFeedbackDtoResponse
            {
                AssignmentTaskFeedbackId = feedbackEntity.AssignmentTaskFeedbackId,
                CustomerId = feedbackEntity.CustomerId,
                CustomerName = customerAccount.FullName,
                AvatarPath = customerAccount.AvatarPath,
                AssignmentTaskId = feedbackEntity.AssignmentTaskId,
                Content = feedbackEntity.Content,
                CreatedAt = feedbackEntity.CreatedAt,
                UpdatedAt = feedbackEntity.UpdatedAt,
                Status = feedbackEntity.Status,
                ResponseContent = feedbackEntity.ResponseContent,
                Rating = feedbackEntity.Rating,
                StaffId = feedbackEntity.StaffId ?? 0, // Gán StaffId nếu có, ngược lại gán giá trị mặc định
                FullNameStaff = fullNameStaff
            };

            return (true, "Feedback retrieved successfully.", responseDto);
        }




        public async Task<(bool success, string message, IEnumerable<AssignmentTaskFeedbackDtoResponse> feedbacks, int totalPage)> GetAllFeedbacksAsync(int page, int pageSize)
        {
            try
            {
                var totalFeedbacks = await _unitOfWork.AssignmentTaskFeedbackRepository.CountAsync();
                var totalPage = (int)Math.Ceiling(totalFeedbacks / (double)pageSize);

                // Lấy danh sách phản hồi với thông tin tài khoản khách hàng và chi tiết đơn hàng
                var feedbacks = await _unitOfWork.AssignmentTaskFeedbackRepository.GetAllAsync(
                    pageIndex: page,
                    pageSize: pageSize
                );

                var responseDtos = new List<AssignmentTaskFeedbackDtoResponse>();

                foreach (var feedback in feedbacks)
                {
                    // Kiểm tra nếu có StaffId, lấy FullName của nhân viên đó
                    string? fullNameStaff = null;
                    if (feedback.StaffId != null)
                    {
                        var staffAccount = await _unitOfWork.AccountRepository.GetByIDAsync(feedback.StaffId.Value);
                        fullNameStaff = staffAccount?.FullName;
                    }
                    var customerAccount = await _unitOfWork.AccountRepository.GetByIDAsync(feedback.CustomerId);
                    // Tạo đối tượng phản hồi DTO
                    var feedbackDto = new AssignmentTaskFeedbackDtoResponse
                    {
                        AssignmentTaskFeedbackId = feedback.AssignmentTaskFeedbackId,
                        CustomerId = feedback.CustomerId,
                        AssignmentTaskId = feedback.AssignmentTaskId,
                        Content = feedback.Content,
                        CreatedAt = feedback.CreatedAt,
                        UpdatedAt = feedback.UpdatedAt,
                        Status = feedback.Status,
                        ResponseContent = feedback.ResponseContent,
                        Rating = feedback.Rating,
                        CustomerName = customerAccount.FullName, // Tên khách hàng
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




        public async Task<(bool success, string message)> UpdateFeedbackAsync(int id, AssignmentTaskFeedbackContentDtoRequest feedbackDto)
        {
            try
            {
                var feedback = await _unitOfWork.AssignmentTaskFeedbackRepository.GetByIDAsync(id);
                if (feedback == null)
                {
                    return (false, "Feedback not found.");
                }

                // Cập nhật chỉ thuộc tính Content
                feedback.Content = feedbackDto.Content;
                feedback.UpdatedAt = DateTime.Now;

                await _unitOfWork.AssignmentTaskFeedbackRepository.UpdateAsync(feedback);
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
                var feedback = await _unitOfWork.AssignmentTaskFeedbackRepository.GetByIDAsync(id);
                if (feedback == null)
                {
                    return (false, "Feedback not found.");
                }

                // Tự động thay đổi trạng thái
                feedback.Status = !feedback.Status;  // Đổi trạng thái từ true -> false và ngược lại
                feedback.UpdatedAt = DateTime.Now;

                await _unitOfWork.AssignmentTaskFeedbackRepository.UpdateAsync(feedback);
                await _unitOfWork.SaveAsync();

                return (true, $"Feedback status changed to {(feedback.Status ? "Active" : "Inactive")} successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error occurred: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> UpdateFeedbackResponseAsync(int feedbackId, AssignmentTaskFeedbackResponseDtoRequest feedbackDto)
        {
            try
            {
                // Kiểm tra tồn tại của Feedback
                var feedback = await _unitOfWork.AssignmentTaskFeedbackRepository.GetByIDAsync(feedbackId);
                if (feedback == null)
                {
                    return (false, "Feedback not found.");
                }

                // Kiểm tra nếu nhân viên có quyền chỉnh sửa phản hồi này
                if (feedback.StaffId != feedbackDto.StaffId)
                {
                    return (false, "You do not have permission to update this feedback response.");
                }

                // Lấy thông tin AssignmentTask để kiểm tra AreaId
                var assignmentTask = await _unitOfWork.AssignmentTaskRepository.GetAsync(
                    od => od.AssignmentTaskId == feedback.AssignmentTaskId,
                    includeProperties: "Service_Schedule.MartyrGrave"
                );

                var assignmentTaskEntity = assignmentTask.FirstOrDefault();
                if (assignmentTaskEntity?.Service_Schedule.MartyrGrave == null)
                {
                    return (false, "The related MartyrGrave for this feedback could not be found.");
                }

                // Kiểm tra nếu nhân viên thuộc AreaId của MartyrGrave
                var staffAccount = await _unitOfWork.AccountRepository.GetByIDAsync(feedbackDto.StaffId);
                if (staffAccount?.AreaId != assignmentTaskEntity.Service_Schedule.MartyrGrave.AreaId)
                {
                    return (false, "You do not have permission to update responses for feedback in this area.");
                }

                // Cập nhật nội dung phản hồi
                feedback.ResponseContent = feedbackDto.ResponseContent;
                feedback.UpdatedAt = DateTime.Now;

                await _unitOfWork.AssignmentTaskFeedbackRepository.UpdateAsync(feedback);
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

        public async Task<(bool success, string message)> CreateFeedbackResponseAsync(AssignmentTaskFeedbackResponseDtoRequest feedbackDto)
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
                var feedback = await _unitOfWork.AssignmentTaskFeedbackRepository.GetByIDAsync(feedbackDto.AssignmentTaskFeedbackId);
                if (feedback == null)
                {
                    return (false, "Phản hồi không tồn tại.");
                }

                // Lấy OrderDetail để kiểm tra khu vực (AreaId)
                var assignmentTask = await _unitOfWork.AssignmentTaskRepository.GetAsync(
                    od => od.AssignmentTaskId == feedback.AssignmentTaskId,
                    includeProperties: "Service_Schedule.MartyrGrave"
                );

                var assignmentTaskEntity = assignmentTask.FirstOrDefault();
                if (assignmentTaskEntity.Service_Schedule?.MartyrGrave == null)
                {
                    return (false, "Không tìm thấy mộ liệt sĩ liên quan đến phản hồi.");
                }

                // Kiểm tra quyền khu vực
                if (staffAccount.AreaId != assignmentTaskEntity.Service_Schedule.MartyrGrave.AreaId)
                {
                    return (false, "Bạn không có quyền trả lời phản hồi cho khu vực này.");
                }

                // Cập nhật phản hồi của nhân viên
                feedback.ResponseContent = feedbackDto.ResponseContent;
                feedback.StaffId = feedbackDto.StaffId;
                feedback.UpdatedAt = DateTime.Now;

                await _unitOfWork.AssignmentTaskFeedbackRepository.UpdateAsync(feedback);
                await _unitOfWork.SaveAsync();

                return (true, "Phản hồi đã được tạo thành công.");
            }
            catch (Exception ex)
            {
                return (false, $"Có lỗi xảy ra: {ex.Message}");
            }
        }

        public async Task<(bool success, string message, AssignmentTaskFeedbackDtoResponse feedback)> GetFeedbackByAssignmentTaskId(int taskId)
        {
            // Truy xuất phản hồi bao gồm thông tin tài khoản khách hàng và chi tiết đơn hàng
            var feedback = await _unitOfWork.AssignmentTaskFeedbackRepository.GetAsync(
                f => f.AssignmentTaskId == taskId
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
            var customerAccount = await _unitOfWork.AccountRepository.GetByIDAsync(feedbackEntity.CustomerId);
            // Tạo đối tượng phản hồi DTO
            var responseDto = new AssignmentTaskFeedbackDtoResponse
            {
                AssignmentTaskFeedbackId = feedbackEntity.AssignmentTaskFeedbackId,
                CustomerId = feedbackEntity.CustomerId,
                AssignmentTaskId = feedbackEntity.AssignmentTaskId,
                Content = feedbackEntity.Content,
                CreatedAt = feedbackEntity.CreatedAt,
                UpdatedAt = feedbackEntity.UpdatedAt,
                Status = feedbackEntity.Status,
                ResponseContent = feedbackEntity.ResponseContent,
                Rating = feedbackEntity.Rating,
                CustomerName = customerAccount.FullName,
                StaffId = feedbackEntity.StaffId ?? 0, // Gán StaffId nếu có, ngược lại gán giá trị mặc định
                FullNameStaff = fullNameStaff
            };

            return (true, "Feedback retrieved successfully.", responseDto);
        }
    }
}
