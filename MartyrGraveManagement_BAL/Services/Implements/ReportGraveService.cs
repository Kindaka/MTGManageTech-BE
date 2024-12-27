using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.ReportGraveDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class ReportGraveService : IReportGraveService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IGoogleDriveService _googleDriveService;
        public ReportGraveService(IUnitOfWork unitOfWork, IMapper mapper, IGoogleDriveService googleDriveService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _googleDriveService = googleDriveService;
        }

        public async Task<ReportGraveDtoResponse> GetReportGraveById(int id)
        {
            try
            {
                ReportGraveDtoResponse? response = null;
                // Lấy thông tin báo cáo từ cơ sở dữ liệu
                var report = await _unitOfWork.ReportGraveRepository.GetByIDAsync(id);
                if (report == null)
                {
                    return response;
                }

                // Lấy video từ Google Drive
                //var videoContent = await _googleDriveService.GetVideoFromCacheOrDriveAsync(report.VideoFile, "1UK_xzKBkLdcRooUTtlAbD0Nc1JONmmWG");

                string videoDownloadUrl = null;
                if (!string.IsNullOrEmpty(report.VideoFile))
                {
                    var videoFileId = await _googleDriveService.GetFileIdByNameAsync(report.VideoFile, "1UK_xzKBkLdcRooUTtlAbD0Nc1JONmmWG");
                    if (!string.IsNullOrEmpty(videoFileId))
                    {
                        videoDownloadUrl = $"https://drive.google.com/uc?id={videoFileId}";
                    }
                }

                response = new ReportGraveDtoResponse
                {
                    ReportId = report.ReportId,
                    RequestId = report.RequestId,
                    StaffId = report.StaffId,
                    VideoFile = videoDownloadUrl, // Hoặc link tải xuống từ Google Drive
                    Description = report.Description,
                    CreateAt = report.CreateAt,
                    UpdateAt = report.UpdateAt,
                    Status = report.Status,
                    //VideoContent = videoContent
                };
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ReportGraveDtoResponse> GetReportGraveByRequestId(int requestId)
        {
            try
            {
                ReportGraveDtoResponse? response = null;
                // Lấy thông tin báo cáo từ cơ sở dữ liệu
                var report = (await _unitOfWork.ReportGraveRepository.GetAsync(r => r.RequestCustomer.RequestId == requestId)).FirstOrDefault();
                if (report == null)
                {
                    return response;
                }

                // Lấy video từ Google Drive
                //var videoContent = await _googleDriveService.GetVideoFromCacheOrDriveAsync(report.VideoFile, "1UK_xzKBkLdcRooUTtlAbD0Nc1JONmmWG");

                string videoDownloadUrl = null;
                if (!string.IsNullOrEmpty(report.VideoFile))
                {
                    var videoFileId = await _googleDriveService.GetFileIdByNameAsync(report.VideoFile, "1UK_xzKBkLdcRooUTtlAbD0Nc1JONmmWG");
                    if (!string.IsNullOrEmpty(videoFileId))
                    {
                        videoDownloadUrl = $"https://drive.google.com/uc?id={videoFileId}";
                    }
                }

                response = new ReportGraveDtoResponse
                {
                    ReportId = report.ReportId,
                    RequestId = report.RequestId,
                    StaffId = report.StaffId,
                    VideoFile = videoDownloadUrl, // Hoặc link tải xuống từ Google Drive
                    Description = report.Description,
                    CreateAt = report.CreateAt,
                    UpdateAt = report.UpdateAt,
                    Status = report.Status,
                    //VideoContent = videoContent
                };
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<(IEnumerable<ReportGraveDtoResponse> reportList, int totalPage)> GetReportsForStaff(int staffId, int pageIndex, int pageSize, DateTime Date)
        {
            try
            {
                // Kiểm tra xem Manager có tồn tại không
                var staff = await _unitOfWork.AccountRepository.GetByIDAsync(staffId);
                if (staff == null || staff.RoleId != 3)
                {
                    throw new KeyNotFoundException("Staff not found or invalid role.");
                }

                int totalPage = 0;
                int totalReport = 0;
                IEnumerable<ReportGrave> reports = new List<ReportGrave>();

                if (Date == DateTime.MinValue)
                {
                    totalReport = (await _unitOfWork.ReportGraveRepository.GetAsync(
                        s => s.StaffId == staff.AccountId,
                        includeProperties: "RequestCustomer")).Count();
                    totalPage = (int)Math.Ceiling(totalReport / (double)pageSize);

                    reports = await _unitOfWork.ReportGraveRepository.GetAsync(
                        s => s.StaffId == staff.AccountId,
                        includeProperties: "RequestCustomer",
                        orderBy: q => q.OrderByDescending(r => r.CreateAt),
                        pageIndex: pageIndex,
                        pageSize: pageSize
                    );
                }
                else
                {
                    totalReport = (await _unitOfWork.ReportGraveRepository.GetAsync(
                        s => s.StaffId == staff.AccountId &&
                        s.CreateAt.Date == Date.Date,
                        includeProperties: "RequestCustomer")).Count();
                    totalPage = (int)Math.Ceiling(totalReport / (double)pageSize);

                    reports = await _unitOfWork.ReportGraveRepository.GetAsync(
                        t => t.StaffId == staff.AccountId &&
                        t.CreateAt.Date == Date.Date,
                        includeProperties: "RequestCustomer",
                        orderBy: q => q.OrderByDescending(r => r.CreateAt),
                        pageIndex: pageIndex,
                        pageSize: pageSize
                    );
                }

                if (!reports.Any())
                {
                    return (new List<ReportGraveDtoResponse>(), 0);
                }
                var responses = new List<ReportGraveDtoResponse>();
                // Lấy thông tin vị trí cho tất cả các task
                foreach (var report in reports)
                {
                    var reportReponse = _mapper.Map<ReportGraveDtoResponse>(report);
                    string videoDownloadUrl = null;
                    if (!string.IsNullOrEmpty(report.VideoFile))
                    {
                        var videoFileId = await _googleDriveService.GetFileIdByNameAsync(report.VideoFile, "1UK_xzKBkLdcRooUTtlAbD0Nc1JONmmWG");
                        if (!string.IsNullOrEmpty(videoFileId))
                        {
                            videoDownloadUrl = $"https://drive.google.com/uc?id={videoFileId}";
                        }
                    }
                    reportReponse.VideoFile = videoDownloadUrl;
                    responses.Add(reportReponse);
                }


                //var responses = tasks.Select(t => _mapper.Map<AssignmentTaskResponse>(t)).ToList();
                return (responses, totalPage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UploadVideoAsync(IFormFile file, int staffId, int reportId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                string parentFolderId = "1UK_xzKBkLdcRooUTtlAbD0Nc1JONmmWG"; //Đường dẫn folderPath

                var maxFileSize = 50 * 1024 * 1024; // 50 MB

                if (!file.ContentType.StartsWith("video/"))
                {
                    throw new InvalidDataException("Only video files are allowed.");
                }

                // Kiểm tra dung lượng
                if (file.Length > maxFileSize)
                {
                    throw new InvalidDataException($"File size must not exceed {maxFileSize / (1024 * 1024)} MB. Current size: {file.Length / (1024 * 1024)} MB.");
                }

                var staff = await _unitOfWork.AccountRepository.GetByIDAsync(staffId);
                if (staff == null)
                {
                    throw new Exception($"Nhân viên {staffId} không tồn tại");
                }

                var report = await _unitOfWork.ReportGraveRepository.GetByIDAsync(reportId);
                if (report == null || report.Status != 1)
                {
                    throw new Exception($"Báo cáo mộ {reportId} không tồn tại hoặc video chưa được xác nhận làm.");
                }

                try
                {

                    // If an avatar exists, delete the old one
                    if (report.VideoFile != null)
                    {
                        await _googleDriveService.DeleteFileAsync(report.VideoFile, parentFolderId);
                    }

                    string guid = Guid.NewGuid().ToString();
                    string fileName = $"Report_Grave_{reportId}_{guid}";

                    var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = fileName,
                        Parents = new List<string>() { parentFolderId },
                        MimeType = file.ContentType
                    };

                    // Upload Directly to Google Drive
                    using (var stream = file.OpenReadStream())
                    {
                        await _googleDriveService.UploadFileAsync(stream, fileMetadata);
                    }

                    report.VideoFile = fileName;
                    report.Status = 4;
                    report.UpdateAt = DateTime.Now;

                    await _unitOfWork.ReportGraveRepository.UpdateAsync(report);
                    var request = (await _unitOfWork.RequestCustomerRepository.GetAsync(r => r.RequestId == report.RequestId)).FirstOrDefault();
                    if (request != null)
                    {
                        request.Status = 7;
                        await _unitOfWork.RequestCustomerRepository.UpdateAsync(request);
                    }
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Không thể tải video");
                }
            }
        }
    }
}
