using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.AssignmentTaskFeedbackDTOs;
using MartyrGraveManagement_BAL.ModelViews.RequestFeedbackDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class RequestFeedbackService : IRequestFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RequestFeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(bool success, string message, RequestFeedbackDtoResponse feedback)> CreateFeedbackAsync(RequestFeedbackDtoRequest feedbackDto)
        {
            try
            {
                // Kiểm tra tồn tại của tài khoản khách hàng
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(feedbackDto.CustomerId);
                if (account == null)
                {
                    return (false, "Account does not exist.", null);
                }


                var requestCustomers = await _unitOfWork.RequestCustomerRepository.GetAsync(
                    od => od.RequestId == feedbackDto.RequestId,
                    includeProperties: "RequestType"
                );

                var requestCustomerEntity = requestCustomers.FirstOrDefault();
                if (requestCustomerEntity == null || requestCustomerEntity.RequestType == null)
                {
                    return (false, "Request does not exist.", null);
                }

                // Kiểm tra xem tài khoản có phải là chủ sở hữu của đơn hàng không
                if (requestCustomerEntity.CustomerId != feedbackDto.CustomerId)
                {
                    return (false, "The account is not the owner of this request.", null);
                }

                // Kiểm tra trạng thái đơn hàng (chỉ cho phép tạo phản hồi nếu đơn hàng đã hoàn thành - status = 7)
                if (requestCustomerEntity.Status != 7)
                {
                    return (false, "Feedback can only be created for completed tasks (Request status must be 7).", null);
                }

                if (requestCustomerEntity.RequestType.TypeId != 2)
                {
                    return (false, "Feedback can only be created for booking service (RequestType 2).", null);
                }
                var requestTask = (await _unitOfWork.RequestTaskRepository.GetAsync(r => r.RequestId == requestCustomerEntity.RequestId)).FirstOrDefault();
                if (requestTask == null)
                {
                    return (false, "Không tìm thấy task của request.", null);
                }
                // Tạo phản hồi mới
                var feedback = new RequestFeedback
                {
                    CustomerId = feedbackDto.CustomerId,
                    StaffId = requestTask.StaffId,
                    RequestId = feedbackDto.RequestId,
                    Content = feedbackDto.Content,
                    Rating = feedbackDto.Rating,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Status = true
                };

                await _unitOfWork.RequestFeedbackRepository.AddAsync(feedback);
                await _unitOfWork.SaveAsync();

                var responseDto = new RequestFeedbackDtoResponse
                {
                    FeedbackId = feedback.FeedbackId,
                    CustomerId = feedback.CustomerId,
                    RequestId = feedback.RequestId,
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



        public async Task<(bool success, string message, RequestFeedbackDtoResponse feedback)> GetFeedbackByIdAsync(int id)
        {
            // Truy xuất phản hồi bao gồm thông tin tài khoản khách hàng và chi tiết đơn hàng
            var feedback = await _unitOfWork.RequestFeedbackRepository.GetAsync(
                f => f.FeedbackId == id,
                includeProperties: "RequestCustomer.Account,RequestCustomer.RequestType"
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
            var responseDto = new RequestFeedbackDtoResponse
            {
                FeedbackId = feedbackEntity.FeedbackId,
                CustomerId = feedbackEntity.CustomerId,
                CustomerName = customerAccount.FullName,
                AvatarPath = customerAccount.AvatarPath,
                RequestId = feedbackEntity.RequestId,
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




        public async Task<(bool success, string message, IEnumerable<RequestFeedbackDtoResponse> feedbacks, int totalPage)> GetAllFeedbacksAsync(int page, int pageSize)
        {
            try
            {
                var totalFeedbacks = await _unitOfWork.RequestFeedbackRepository.CountAsync();
                var totalPage = (int)Math.Ceiling(totalFeedbacks / (double)pageSize);

                // Lấy danh sách phản hồi với thông tin tài khoản khách hàng và chi tiết đơn hàng
                var feedbacks = await _unitOfWork.RequestFeedbackRepository.GetAllAsync(
                    includeProperties: "RequestCustomer.RequestType,RequestCustomer.Account",
                    pageIndex: page,
                    pageSize: pageSize
                );

                var responseDtos = new List<RequestFeedbackDtoResponse>();

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
                    var feedbackDto = new RequestFeedbackDtoResponse
                    {
                        FeedbackId = feedback.FeedbackId,
                        CustomerId = feedback.CustomerId,
                        RequestId = feedback.RequestId,
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




        public async Task<(bool success, string message)> UpdateFeedbackAsync(int id, int customerId, AssignmentTaskFeedbackContentDtoRequest feedbackDto)
        {
            try
            {
                var feedback = await _unitOfWork.RequestFeedbackRepository.GetByIDAsync(id);
                if (feedback == null)
                {
                    return (false, "Feedback not found.");
                }
                if (feedback.CustomerId != customerId)
                {
                    return (false, "Feedback không phải của bạn");
                }

                // Cập nhật chỉ thuộc tính Content
                feedback.Content = feedbackDto.Content;
                feedback.UpdatedAt = DateTime.Now;

                await _unitOfWork.RequestFeedbackRepository.UpdateAsync(feedback);
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
                var feedback = await _unitOfWork.RequestFeedbackRepository.GetByIDAsync(id);
                if (feedback == null)
                {
                    return (false, "Feedback not found.");
                }

                // Tự động thay đổi trạng thái
                feedback.Status = !feedback.Status;  // Đổi trạng thái từ true -> false và ngược lại
                feedback.UpdatedAt = DateTime.Now;

                await _unitOfWork.RequestFeedbackRepository.UpdateAsync(feedback);
                await _unitOfWork.SaveAsync();

                return (true, $"Feedback status changed to {(feedback.Status ? "Active" : "Inactive")} successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error occurred: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> UpdateFeedbackResponseAsync(RequestFeedbackResponseDtoRequest feedbackDto)
        {
            try
            {
                // Kiểm tra tồn tại của Feedback
                var feedback = (await _unitOfWork.RequestFeedbackRepository.GetAsync(f => f.FeedbackId == feedbackDto.FeedbackId, includeProperties: "RequestCustomer.MartyrGrave")).FirstOrDefault();
                if (feedback == null)
                {
                    return (false, "Feedback not found.");
                }

                // Kiểm tra nếu nhân viên có quyền chỉnh sửa phản hồi này
                if (feedback.StaffId != feedbackDto.StaffId)
                {
                    return (false, "You do not have permission to update this feedback response.");
                }

                // Cập nhật nội dung phản hồi
                feedback.ResponseContent = feedbackDto.ResponseContent;
                feedback.UpdatedAt = DateTime.Now;

                await _unitOfWork.RequestFeedbackRepository.UpdateAsync(feedback);
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

        public async Task<(bool success, string message)> CreateFeedbackResponseAsync(RequestFeedbackResponseDtoRequest feedbackDto)
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
                var feedback = await _unitOfWork.RequestFeedbackRepository.GetByIDAsync(feedbackDto.FeedbackId);
                if (feedback == null)
                {
                    return (false, "Phản hồi không tồn tại.");
                }

                if (staffAccount.AccountId != feedback.StaffId)
                {
                    return (false, "Đây không phải phản hồi của bạn.");
                }

                // Cập nhật phản hồi của nhân viên
                feedback.ResponseContent = feedbackDto.ResponseContent;
                feedback.StaffId = feedbackDto.StaffId;
                feedback.UpdatedAt = DateTime.Now;

                await _unitOfWork.RequestFeedbackRepository.UpdateAsync(feedback);
                await _unitOfWork.SaveAsync();

                return (true, "Phản hồi đã được tạo thành công.");
            }
            catch (Exception ex)
            {
                return (false, $"Có lỗi xảy ra: {ex.Message}");
            }
        }

        public async Task<(bool success, string message, RequestFeedbackDtoResponse feedback)> GetFeedbackByRequestId(int requestId)
        {
            // Truy xuất phản hồi bao gồm thông tin tài khoản khách hàng và chi tiết đơn hàng
            var feedback = await _unitOfWork.RequestFeedbackRepository.GetAsync(
                f => f.RequestId == requestId,
                includeProperties: "RequestCustomer.Account,RequestCustomer.RequestType"
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
            var responseDto = new RequestFeedbackDtoResponse
            {
                FeedbackId = feedbackEntity.FeedbackId,
                CustomerId = feedbackEntity.CustomerId,
                RequestId = feedbackEntity.RequestId,
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
