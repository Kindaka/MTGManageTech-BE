using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.HolidayEventDTOs;
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
    public class HolidayEventService : IHolidayEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HolidayEventService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateHolidayEventAsync(int accountId, HolidayEventRequestDto holidayEventDto)
        {
            try
            {
                // Tạo mới sự kiện với AccountId từ tham số
                var holidayEvent = new Holiday_Event
                {
                    AccountId = accountId,
                    EventName = holidayEventDto.EventName,
                    Description = holidayEventDto.Description,
                    EventDate = new DateOnly(DateTime.Now.Year, holidayEventDto.EventDate.Month, holidayEventDto.EventDate.Day),
                    Status = true
                };

                // Thêm sự kiện vào cơ sở dữ liệu
                await _unitOfWork.HolidayEventsRepository.AddAsync(holidayEvent);
                await _unitOfWork.SaveAsync();

                // Thêm danh sách ảnh nếu có
                if (holidayEventDto.ImagePaths != null && holidayEventDto.ImagePaths.Any())
                {
                    var eventImages = holidayEventDto.ImagePaths.Select(path => new Event_Image
                    {
                        EventId = holidayEvent.EventId,
                        ImagePath = path
                    }).ToList();

                    await _unitOfWork.EventImagesRepository.AddRangeAsync(eventImages);
                    await _unitOfWork.SaveAsync();
                }

                // Tạo thông báo mới cho sự kiện đã tạo
                var notification = new Notification
                {
                    Title = $"Sự kiện mới: {holidayEvent.EventName}",
                    Description = $"Sự kiện '{holidayEvent.EventName}' đã được tạo và sẽ diễn ra vào ngày {holidayEvent.EventDate}.",
                    CreatedDate = DateTime.Now,
                    Status = true
                };

                // Thêm thông báo vào cơ sở dữ liệu
                await _unitOfWork.NotificationRepository.AddAsync(notification);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }



        public async Task<bool> UpdateHolidayEventAsync(int eventId, int accountId, HolidayEventRequestDto holidayEventDto)
        {
            try
            {
                // Tìm sự kiện cần cập nhật
                var existingEvent = await _unitOfWork.HolidayEventsRepository.GetByIDAsync(eventId);
                if (existingEvent == null)
                {
                    return false; // Không tìm thấy sự kiện
                }

                // Cập nhật thông tin sự kiện
                existingEvent.EventName = holidayEventDto.EventName;
                existingEvent.Description = holidayEventDto.Description;
                existingEvent.EventDate = new DateOnly(DateTime.Now.Year, holidayEventDto.EventDate.Month, holidayEventDto.EventDate.Day);
                existingEvent.AccountId = accountId;

                // Cập nhật sự kiện vào repository
                await _unitOfWork.HolidayEventsRepository.UpdateAsync(existingEvent);

                // Xóa các ảnh hiện tại liên kết với sự kiện (nếu có)
                var existingImages = await _unitOfWork.EventImagesRepository.GetAsync(img => img.EventId == eventId);
                if (existingImages != null && existingImages.Any())
                {
                    await _unitOfWork.EventImagesRepository.DeleteRangeAsync(existingImages);
                }

                // Thêm danh sách ảnh mới từ ImagePaths
                if (holidayEventDto.ImagePaths != null && holidayEventDto.ImagePaths.Any())
                {
                    var eventImages = holidayEventDto.ImagePaths.Select(path => new Event_Image
                    {
                        EventId = eventId,
                        ImagePath = path
                    }).ToList();

                    await _unitOfWork.EventImagesRepository.AddRangeAsync(eventImages);
                }

                // Tạo thông báo cho việc cập nhật sự kiện
                var notification = new Notification
                {
                    Title = $"Cập nhật sự kiện: {existingEvent.EventName}",
                    Description = $"Sự kiện '{existingEvent.EventName}' đã được cập nhật và sẽ diễn ra vào ngày {existingEvent.EventDate}.",
                    CreatedDate = DateTime.Now,
                    Status = true
                };

                // Thêm thông báo vào cơ sở dữ liệu
                await _unitOfWork.NotificationRepository.AddAsync(notification);

                // Lưu tất cả thay đổi vào cơ sở dữ liệu
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch
            {
                // Xử lý lỗi, trả về false
                return false;
            }
        }



        public async Task<bool> UpdateHolidayEventStatusAsync(int eventId, bool status)
        {
            try
            {
                // Tìm sự kiện cần cập nhật
                var existingEvent = await _unitOfWork.HolidayEventsRepository.GetByIDAsync(eventId);
                if (existingEvent == null)
                {
                    return false; // Không tìm thấy sự kiện
                }

                // Cập nhật trạng thái của sự kiện
                existingEvent.Status = status;

                // Lưu thay đổi vào cơ sở dữ liệu
                await _unitOfWork.HolidayEventsRepository.UpdateAsync(existingEvent);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch
            {
                // Xử lý lỗi, trả về false
                return false;
            }
        }



        public async Task<List<HolidayEventResponseDto>> GetAllHolidayEventsAsync()
        {
            var holidayEvents = await _unitOfWork.HolidayEventsRepository.GetAllAsync(includeProperties: "EventImages,Account");

            // Chuyển đổi danh sách thực thể thành danh sách DTO
            var responseDtos = holidayEvents.Select(e => new HolidayEventResponseDto
            {
                EventId = e.EventId,
                AccountId = e.AccountId,
                EventName = e.EventName,
                Description = e.Description,
                EventDate = e.EventDate,
                Status = e.Status,
                AccountFullName = e.Account?.FullName,
                EventImages = e.EventImages?.Select(img => img.ImagePath).ToList() ?? new List<string>()
            }).ToList();

            return responseDtos;
        }

        public async Task<HolidayEventResponseDto> GetHolidayEventByIdAsync(int eventId)
        {
            var holidayEventTask = await _unitOfWork.HolidayEventsRepository.GetAsync(
                he => he.EventId == eventId,
                includeProperties: "Account,EventImages"
            );

            var holidayEvent = holidayEventTask.FirstOrDefault();

            if (holidayEvent == null)
                return null;

            return new HolidayEventResponseDto
            {
                EventId = holidayEvent.EventId,
                AccountId = holidayEvent.AccountId,
                EventName = holidayEvent.EventName,
                Description = holidayEvent.Description,
                EventDate = holidayEvent.EventDate,
                Status = holidayEvent.Status,
                AccountFullName = holidayEvent.Account?.FullName,
                EventImages = holidayEvent.EventImages?.Select(img => img.ImagePath).ToList() ?? new List<string>()
            };
        }


        public async Task<bool> DeleteHolidayEventAsync(int eventId)
        {
            try
            {
                var existingEvent = await _unitOfWork.HolidayEventsRepository.GetByIDAsync(eventId);
                if (existingEvent == null)
                {
                    return false; // Không tìm thấy sự kiện
                }

                // Xóa sự kiện và lưu thay đổi
               await _unitOfWork.HolidayEventsRepository.DeleteAsync(existingEvent);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                // Xử lý lỗi, trả về false
                return false;
            }
        }

    }
}
    