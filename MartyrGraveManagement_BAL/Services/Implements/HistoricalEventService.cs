using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.BlogDTOs;
using MartyrGraveManagement_BAL.ModelViews.HistoricalEventDTOs;
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
    public class HistoricalEventService : IHistoricalEventService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HistoricalEventService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper; 
        }


        public async Task<List<HistoricalEventDTOResponse>> GetAllHistoricalEvents()
        {
            var historicalEvents = await _unitOfWork.HistoricalEventRepository.GetAllAsync(includeProperties: "Blogs");

            List<HistoricalEventDTOResponse> historicalEventResponses = new List<HistoricalEventDTOResponse>();

            foreach (var historicalEvent in historicalEvents)
            {
                // Khởi tạo đối tượng HistoricalEventDTOResponse
                var historicalEventResponse = new HistoricalEventDTOResponse
                {
                    HistoryId = historicalEvent.HistoryId,
                    HistoryEventName = historicalEvent.HistoryEventName,
                    Description = historicalEvent.Description,
                    StartTime = historicalEvent.StartTime,
                    EndTime = historicalEvent.EndTime,
                    Status = historicalEvent.Status,
                    Blogs = new List<BlogDTOResponse>()
                };

                // Duyệt qua từng Blog trong HistoricalEvent và thêm vào DTO
                foreach (var blog in historicalEvent.Blogs)
                {
                    // Lấy Account của Blog để lấy FullName
                    var account = await _unitOfWork.AccountRepository.GetByIDAsync(blog.AccountId);

                    // Lấy hình ảnh của Blog từ HistoricalImageRepository
                    var images = await _unitOfWork.HistoricalImageRepository.FindAsync(img => img.BlogId == blog.BlogId);

                    // Tạo danh sách BlogImageDTOResponse từ images
                    List<BlogImageDTOResponse> imageResponses = new List<BlogImageDTOResponse>();
                    foreach (var img in images)
                    {
                        var imageResponse = new BlogImageDTOResponse
                        {
                            BlogId = img.BlogId,
                            ImagePath = img.ImagePath,
                            ImageId = img.ImageId // Gán đúng ImageId từ cơ sở dữ liệu
                        };
                        imageResponses.Add(imageResponse);
                    }

                    // Tạo đối tượng BlogDTOResponse và thêm vào danh sách Blogs của historicalEventResponse
                    var blogResponse = new BlogDTOResponse
                    {
                        BlogId = blog.BlogId,
                        AccountId = blog.AccountId,
                        FullName = account?.FullName, // Gán FullName từ account, nếu có
                        HistoryId = blog.HistoryId,
                        BlogName = blog.BlogName,
                        BlogContent = blog.BlogContent,
                        CreateDate = blog.CreateDate,
                        UpdateDate = blog.UpdateDate,
                        Status = blog.Status,
                        Images = imageResponses
                    };

                    historicalEventResponse.Blogs.Add(blogResponse);
                }

                // Thêm HistoricalEventDTOResponse vào danh sách kết quả
                historicalEventResponses.Add(historicalEventResponse);
            }

            return historicalEventResponses;
        }


        public async Task<List<HistoricalEventDTOResponse>> GetHistoricalEventByAccount(int accountId)
        {
            // Kiểm tra Role của tài khoản để xác định quyền truy cập
            var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
            if (account == null)
                throw new Exception("Account not found");

            List<HistoricalEvent> historicalEvents = new List<HistoricalEvent>();

            // Nếu là Manager
            if (account.RoleId == 2)
            {
                // Manager có thể xem tất cả các sự kiện lịch sử
                historicalEvents = (await _unitOfWork.HistoricalEventRepository.GetAllAsync(includeProperties: "Blogs")).ToList();
            }
            else if (account.RoleId == 3) // Nếu là Staff
            {
                // Lấy tất cả các Blogs mà Staff có quyền truy cập thông qua `AreaId`
                var accessibleBlogs = await _unitOfWork.BlogRepository.FindAsync(
                    b => b.HistoricalRelatedMartyrs.Any(hrm => hrm.MartyrGraveInformation.MartyrGrave.AreaId == account.AreaId)
                );

                // Lấy danh sách `HistoryId` từ các Blogs có quyền truy cập
                var historicalEventIds = accessibleBlogs.Select(b => b.HistoryId).Distinct();

                // Truy vấn các sự kiện lịch sử dựa trên `HistoryId` liên kết với khu vực của Staff
                historicalEvents = (await _unitOfWork.HistoricalEventRepository.GetAllAsync(
                    filter: e => historicalEventIds.Contains(e.HistoryId),
                    includeProperties: "Blogs"
                )).ToList();
            }
            else
            {
                throw new Exception("Unauthorized access");
            }

            // Chuyển đổi danh sách sự kiện lịch sử thành DTO
            List<HistoricalEventDTOResponse> historicalEventResponses = new List<HistoricalEventDTOResponse>();
            foreach (var historicalEvent in historicalEvents)
            {
                var historicalEventResponse = new HistoricalEventDTOResponse
                {
                    HistoryId = historicalEvent.HistoryId,
                    HistoryEventName = historicalEvent.HistoryEventName,
                    Description = historicalEvent.Description,
                    StartTime = historicalEvent.StartTime,
                    EndTime = historicalEvent.EndTime,
                    Status = historicalEvent.Status,
                    Blogs = new List<BlogDTOResponse>()
                };

                foreach (var blog in historicalEvent.Blogs)
                {
                    var accountData = await _unitOfWork.AccountRepository.GetByIDAsync(blog.AccountId);
                    var images = await _unitOfWork.HistoricalImageRepository.FindAsync(img => img.BlogId == blog.BlogId);

                    var imageResponses = images.Select(img => new BlogImageDTOResponse
                    {
                        BlogId = img.BlogId,
                        ImagePath = img.ImagePath
                    }).ToList();

                    var blogResponse = new BlogDTOResponse
                    {
                        BlogId = blog.BlogId,
                        AccountId = blog.AccountId,
                        FullName = accountData?.FullName ?? "Unknown",
                        HistoryId = blog.HistoryId,
                        BlogName = blog.BlogName,
                        BlogContent = blog.BlogContent,
                        CreateDate = blog.CreateDate,
                        UpdateDate = blog.UpdateDate,
                        Status = blog.Status,
                        Images = imageResponses
                    };

                    historicalEventResponse.Blogs.Add(blogResponse);
                }

                historicalEventResponses.Add(historicalEventResponse);
            }

            return historicalEventResponses;
        }


        public async Task<HistoricalEventDTOResponse> GetHistoricalEventById(int historicalEventId)
        {
            // Lấy sự kiện lịch sử mà không có các thuộc tính liên quan
            var historicalEvent = await _unitOfWork.HistoricalEventRepository.SingleOrDefaultAsync(he => he.HistoryId == historicalEventId);

            if (historicalEvent == null)
            {
                return null;
            }

            // Tạo đối tượng HistoricalEventDTOResponse
            var historicalEventResponse = new HistoricalEventDTOResponse
            {
                HistoryId = historicalEvent.HistoryId,
                HistoryEventName = historicalEvent.HistoryEventName,
                Description = historicalEvent.Description,
                StartTime = historicalEvent.StartTime,
                EndTime = historicalEvent.EndTime,
                Status = historicalEvent.Status,
                Blogs = new List<BlogDTOResponse>()
            };

            // Lấy danh sách Blogs liên quan đến HistoricalEvent này
            var blogs = await _unitOfWork.BlogRepository.FindAsync(b => b.HistoryId == historicalEventId);

            foreach (var blog in blogs)
            {
                // Lấy Account để lấy FullName cho blog
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(blog.AccountId);

                // Lấy danh sách hình ảnh cho từng blog
                var images = await _unitOfWork.HistoricalImageRepository.FindAsync(img => img.BlogId == blog.BlogId);

                // Tạo danh sách BlogImageDTOResponse từ images
                List<BlogImageDTOResponse> imageResponses = new List<BlogImageDTOResponse>();
                foreach (var img in images)
                {
                    imageResponses.Add(new BlogImageDTOResponse
                    {
                        BlogId = img.BlogId,
                        ImagePath = img.ImagePath
                    });
                }

                // Tạo đối tượng BlogDTOResponse và thêm vào danh sách Blogs của historicalEventResponse
                var blogResponse = new BlogDTOResponse
                {
                    BlogId = blog.BlogId,
                    AccountId = blog.AccountId,
                    FullName = account?.FullName, // FullName từ Account
                    HistoryId = blog.HistoryId,
                    BlogName = blog.BlogName,
                    BlogContent = blog.BlogContent,
                    CreateDate = blog.CreateDate,
                    UpdateDate = blog.UpdateDate,
                    Status = blog.Status,
                    Images = imageResponses
                };

                historicalEventResponse.Blogs.Add(blogResponse);
            }

            return historicalEventResponse;
        }

        public async Task<HistoricalEventDTOResponse> CreateHistoricalEvent(CreateHistoricalEventDTORequest newEventRequest)
        {
            var historicalEvent = _mapper.Map<HistoricalEvent>(newEventRequest);

            historicalEvent.Status = false;

            await _unitOfWork.HistoricalEventRepository.AddAsync(historicalEvent);
            await _unitOfWork.SaveAsync();

            var historicalEventResponse = _mapper.Map<HistoricalEventDTOResponse>(historicalEvent);
            return historicalEventResponse;
        }


        public async Task<HistoricalEventDTOResponse> UpdateHistoricalEvent(int historyId, CreateHistoricalEventDTORequest updateRequest)
        {
            // Tìm sự kiện lịch sử theo ID
            var existingEvent = await _unitOfWork.HistoricalEventRepository.GetByIDAsync(historyId);
            if (existingEvent == null)
            {
                throw new KeyNotFoundException("Historical event not found.");
            }

            // Cập nhật thông tin sự kiện bằng dữ liệu từ DTO
            _mapper.Map(updateRequest, existingEvent);

            // Lưu thay đổi
            await _unitOfWork.HistoricalEventRepository.UpdateAsync(existingEvent);

            // Trả về DTO response sau khi cập nhật
            var updatedEventResponse = _mapper.Map<HistoricalEventDTOResponse>(existingEvent);
            return updatedEventResponse;
        }


        public async Task<HistoricalEventDTOResponse> UpdateHistoricalEventStatus(int historyId, bool newStatus)
        {
            // Tìm sự kiện lịch sử theo ID
            var existingEvent = await _unitOfWork.HistoricalEventRepository.GetByIDAsync(historyId);
            if (existingEvent == null)
            {
                throw new KeyNotFoundException("Historical event not found.");
            }

            // Cập nhật trạng thái mới
            existingEvent.Status = newStatus;

            // Lưu thay đổi
            await _unitOfWork.HistoricalEventRepository.UpdateAsync(existingEvent);

            // Trả về DTO response sau khi cập nhật
            var updatedEventResponse = _mapper.Map<HistoricalEventDTOResponse>(existingEvent);
            return updatedEventResponse;
        }

    }
}
