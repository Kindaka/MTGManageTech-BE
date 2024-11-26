//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using MartyrGraveManagement_BAL.MLModels;
//using MartyrGraveManagement_BAL.Services.Interfaces;
//using MartyrGraveManagement_DAL.Entities;
//using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
//using Microsoft.Extensions.Logging;
//using Microsoft.ML;
//using System.IO;
//using System.Threading;
//using MartyrGraveManagement_BAL.ModelViews.StaffPerformanceDTOs;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Options;
//using ClosedXML.Excel;
//using Microsoft.EntityFrameworkCore;
//using AutoMapper;
//using System.Linq.Expressions;

//namespace MartyrGraveManagement_BAL.Services.Implements
//{
//    public class StaffPerformanceService : IStaffPerformanceService
//    {
//           private readonly IUnitOfWork _unitOfWork;
//    private readonly ILogger<StaffPerformanceService> _logger;
//    private readonly IMapper _mapper;

//    public StaffPerformanceService(
//        IUnitOfWork unitOfWork,
//        ILogger<StaffPerformanceService> logger,
//        IMapper mapper)
//    {
//        _unitOfWork = unitOfWork;
//        _logger = logger;
//        _mapper = mapper;
//    }

//    public async Task<WorkPerformanceDTO> EvaluatePerformance(int staffId, DateTime startDate, DateTime endDate)
//    {
//        try
//        {
//            // Lấy thông tin staff
//            var staff = await _unitOfWork.AccountRepository.GetByIDAsync(staffId);
//            if (staff == null)
//                throw new Exception($"Không tìm thấy nhân viên với ID {staffId}");

//            var metrics = await GetPerformanceMetrics(staffId, startDate, endDate);
            
//            float qualityScore = CalculateQualityScore(metrics);
//            float timeScore = CalculateTimeScore(metrics);
//            float interactionScore = CalculateInteractionScore(metrics);
//            float overallScore = (qualityScore * 0.4f) + (timeScore * 0.3f) + (interactionScore * 0.3f);

//            string description = GenerateDescription(qualityScore, timeScore, interactionScore, overallScore);

//            // Tạo entity WorkPerformance để lưu vào database
//            var workPerformance = new WorkPerformance
//            {
//                AccountId = staffId,
//                StartDate = DateOnly.FromDateTime(startDate),
//                EndDate = DateOnly.FromDateTime(endDate),
//                UploadTime = DateTime.Now,
//                QualityMaintenancePoint = Math.Round(qualityScore, 2),
//                TimeCompletePoint = Math.Round(timeScore, 2),
//                InteractionPoint = Math.Round(interactionScore, 2),
//                Description = description,
//                Status = true
//            };

//            // Lưu vào database
//            await _unitOfWork.WorkPerformanceRepository.AddAsync(workPerformance);
//            await _unitOfWork.SaveAsync();

//            // Trả về DTO
//            return new WorkPerformanceDTO
//            {
//                WorkId = workPerformance.WorkId, // Lấy ID mới được tạo
//                AccountId = staffId,
//                AccountFullName = staff.FullName,
//                PhoneNumber = staff.PhoneNumber,
//                StartDate = workPerformance.StartDate,
//                EndDate = workPerformance.EndDate,
//                UploadTime = workPerformance.UploadTime,
//                QualityMaintenancePoint = workPerformance.QualityMaintenancePoint,
//                TimeCompletePoint = workPerformance.TimeCompletePoint,
//                InteractionPoint = workPerformance.InteractionPoint,
//                OverallPoint = Math.Round(overallScore, 2),
//                Description = description,
//                PerformanceLevel = GetPerformanceLevel(overallScore),
//                Status = true,
//                Metrics = new PerformanceMetricsDTO
//                {
//                    // Copy từ metrics
//                    TotalTasks = metrics.TotalTasks,
//                    CompletedTasks = metrics.CompletedTasks,
//                    FailedTasks = metrics.FailedTasks,
//                    RejectedTasks = metrics.RejectedTasks,
//                    OnTimeTasks = metrics.OnTimeTasks,
//                    TotalFeedbacks = metrics.TotalFeedbacks,
//                    RespondedFeedbacks = metrics.RespondedFeedbacks,
//                    AverageRating = Math.Round(metrics.AverageRating, 2),
//                    AverageResponseTime = Math.Round(metrics.AverageResponseTime, 2),
//                    TotalWorkDays = metrics.TotalWorkDays,
//                    PresentDays = metrics.PresentDays,
//                    PunctualDays = metrics.PunctualDays
//                }
//            };
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error evaluating performance for staff {StaffId}", staffId);
//            throw;
//        }
//    }

//    private async Task<PerformanceMetrics> GetPerformanceMetrics(int staffId, DateTime startDate, DateTime endDate)
//    {
//        var metrics = new PerformanceMetrics();

//        // Lấy tất cả tasks trong khoảng thời gian
//        var tasks = await _unitOfWork.TaskRepository.GetAllAsync(
//            t => t.AccountId == staffId && 
//            t.StartDate >= startDate && 
//            t.EndDate <= endDate);

//        // Tính các metrics liên quan đến task
//        metrics.TotalTasks = tasks.Count();
//        metrics.CompletedTasks = tasks.Count(t => t.Status == 4);
//        metrics.FailedTasks = tasks.Count(t => t.Status == 5);
//        metrics.RejectedTasks = tasks.Count(t => t.Status == 2);

//        // Lấy feedbacks
//        var feedbacks = await _unitOfWork.FeedbackRepository.GetAllAsync(
//            f => f.StaffId == staffId && 
//            f.CreatedAt >= startDate && 
//            f.CreatedAt <= endDate);

//        metrics.TotalFeedbacks = feedbacks.Count();
//        metrics.RespondedFeedbacks = feedbacks.Count(f => !string.IsNullOrEmpty(f.ResponseContent));
//        metrics.AverageRating = feedbacks.Any() ? 
//            feedbacks.Average(f => f.Rating) : 0;
        
//        // Tính average response time (in hours)
//        if (feedbacks.Any())
//        {
//            var responseTimes = feedbacks
//                .Where(f => f.UpdatedAt != DateTime.MinValue)
//                .Select(f => (f.UpdatedAt - f.CreatedAt).TotalHours);
//            metrics.AverageResponseTime = responseTimes.Any() ? 
//                responseTimes.Average() : 0;
//        }

//        // Lấy attendance data
//        var attendances = await _unitOfWork.AttendanceRepository.GetAllAsync(
//            a => a.AccountId == staffId && 
//            a.Date >= DateOnly.FromDateTime(startDate) && 
//            a.Date <= DateOnly.FromDateTime(endDate));

//        metrics.TotalWorkDays = (endDate - startDate).Days + 1; // Có thể điều chỉnh theo lịch làm việc
//        metrics.PresentDays = attendances.Count(a => a.Status == 1);
        
//        // Tính số ngày đúng giờ (có thể thêm logic phức tạp hơn)
//        metrics.PunctualDays = metrics.PresentDays; // Giả sử tất cả ngày có mặt đều đúng giờ

//        // Tính số task hoàn thành đúng hạn
//        metrics.OnTimeTasks = tasks.Count(t => {
//            if (t.Status != 4) return false; // Chỉ xét các task đã hoàn thành
            
//            // Lấy Order thông qua OrderDetail
//            if (t.OrderDetail?.Order != null)
//            {
//                return t.EndDate <= t.OrderDetail.Order.ExpectedCompletionDate;
//            }
            
//            // Nếu không có Order/OrderDetail, coi như đúng hạn
//            return true;
//        });

//        return metrics;
//    }

//    private float CalculateQualityScore(PerformanceMetrics metrics)
//    {
//        float score = 0;
        
//        // 1. Đánh giá chất lượng hoàn thành công việc (60%)
//        if (metrics.TotalTasks > 0)
//        {
//            // Tỷ lệ hoàn thành = Tasks hoàn thành / Tổng số tasks
//            float completionRate = (float)metrics.CompletedTasks / metrics.TotalTasks;
            
//            // Tỷ lệ thành công = 1 - (Tasks thất bại / Tổng số tasks)
//            float successRate = 1 - ((float)metrics.FailedTasks / metrics.TotalTasks);
            
//            // Tỷ lệ chấp nhận = 1 - (Tasks bị từ chối / Tổng số tasks)
//            float acceptanceRate = 1 - ((float)metrics.RejectedTasks / metrics.TotalTasks);
            
//            float taskScore = (completionRate * 0.4f + // Tỷ lệ hoàn thành (40%)
//                             successRate * 0.4f +      // Tỷ lệ thành công (40%)
//                             acceptanceRate * 0.2f)    // Tỷ lệ chấp nhận (20%)
//                             * 60;                     // 60% của tổng điểm
//            score += taskScore;
//        }

//        // 2. Đánh giá từ feedback của khách hàng (40%)
//        if (metrics.TotalFeedbacks > 0)
//        {
//            // Điểm đánh giá trung bình (thang điểm 5)
//            float ratingScore = (float)(metrics.AverageRating / 5.0) * 25; // 25% của tổng iểm
            
//            // Tỷ lệ phản hồi feedback
//            float responseRate = (float)metrics.RespondedFeedbacks / metrics.TotalFeedbacks;
//            float responseScore = responseRate * 15; // 15% của tổng điểm
            
//            score += (ratingScore + responseScore);
//        }

//        return Math.Min(100, score); // Đảm bảo điểm không vượt quá 100
//    }

//    private float CalculateTimeScore(PerformanceMetrics metrics)
//    {
//        float score = 0;

//        // 1. Đánh giá tiến độ công việc (70%)
//        if (metrics.CompletedTasks > 0)
//        {
//            // Tỷ lệ hoàn thành đúng hạn
//            float onTimeRate = (float)metrics.OnTimeTasks / metrics.CompletedTasks;
//            float timelinessScore = onTimeRate * 70; // 70% của tổng điểm
//            score += timelinessScore;
//        }

//        // 2. Đánh giá chuyên cần (30%)
//        if (metrics.TotalWorkDays > 0)
//        {
//            // Tỷ lệ có mặt
//            float attendanceRate = (float)metrics.PresentDays / metrics.TotalWorkDays;
            
//            // Tỷ lệ đúng giờ trong những ngày có mặt
//            float punctualityRate = metrics.PresentDays > 0 ? 
//                (float)metrics.PunctualDays / metrics.PresentDays : 0;

//            float attendanceScore = (attendanceRate * 0.6f +    // Tỷ lệ có mặt (60%)
//                                   punctualityRate * 0.4f) *    // Tỷ lệ đúng giờ (40%)
//                                   30;                          // 30% của tổng điểm
            
//            score += attendanceScore;
//        }

//        return Math.Min(100, score);
//    }

//    private float CalculateInteractionScore(PerformanceMetrics metrics)
//    {
//        float score = 0;

//        // 1. Đánh giá tương tác với khách hàng (60%)
//        if (metrics.TotalFeedbacks > 0)
//        {
//            // Thời gian phản hồi trung bình (đánh giá nghịch đảo - càng nhanh càng tốt)
//            // Giả sử thời gian phản hồi lý tưởng là trong vòng 24h
//            float responseTimeScore = Math.Max(0, 1 - (float)(metrics.AverageResponseTime / 24)) * 30; // 30%
            
//            // Tỷ lệ phản hồi feedback
//            float responseRate = (float)metrics.RespondedFeedbacks / metrics.TotalFeedbacks;
//            float responseRateScore = responseRate * 30; // 30%
            
//            score += (responseTimeScore + responseRateScore);
//        }

//        // 2. Đánh giá khả năng nhận việc (40%)
//        if (metrics.TotalTasks > 0)
//        {
//            // Tỷ lệ chấp nhận công việc = 1 - tỷ lệ từ chối
//            float taskAcceptanceRate = 1 - ((float)metrics.RejectedTasks / metrics.TotalTasks);
//            float acceptanceScore = taskAcceptanceRate * 40; // 40% của tổng điểm
            
//            score += acceptanceScore;
//        }

//        return Math.Min(100, score);
//    }

//    private string GenerateDescription(float qualityScore, float timeScore, float interactionScore, float overallScore)
//    {
//        var description = new StringBuilder();

//        // Đánh giá tổng quan
//        description.AppendLine($"Đánh giá tổng thể: {GetPerformanceLevel(overallScore)}");
//        description.AppendLine();

//        // Đánh giá chất lượng
//        description.AppendLine("1. Chất lượng công việc:");
//        description.AppendLine(GenerateQualityDescription(qualityScore));
//        description.AppendLine();

//        // Đánh giá thời gian
//        description.AppendLine("2. Quản lý thời gian:");
//        description.AppendLine(GenerateTimeDescription(timeScore));
//        description.AppendLine();

//        // Đánh giá tương tác
//        description.AppendLine("3. Kỹ năng tương tác:");
//        description.AppendLine(GenerateInteractionDescription(interactionScore));

//        return description.ToString();
//    }

//    private string GetPerformanceLevel(float score)
//    {
//        if (score >= 90) return "Xuất sắc";
//        if (score >= 80) return "Tốt";
//        if (score >= 70) return "Khá";
//        if (score >= 60) return "Trung bình";
//        return "Cần cải thiện";
//    }

//    private string GenerateQualityDescription(float score)
//    {
//        var description = new StringBuilder();
        
//        if (score >= 90)
//        {
//            description.AppendLine("- Thể hiện chất lượng công việc xuất sắc");
//            description.AppendLine("- Hoàn thành công việc với độ chính xác cao");
//            description.AppendLine("- Nhận được phản hồi tích cực từ khách hàng");
//        }
//        else if (score >= 80)
//        {
//            description.AppendLine("- Chất lượng công việc tốt và ổn định");
//            description.AppendLine("- Có ít lỗi trong quá trình thực hiện");
//            description.AppendLine("- Khách hàng hài lòng với kết quả");
//        }
//        else if (score >= 70)
//        {
//            description.AppendLine("- Chất lượng công việc đạt yêu cầu");
//            description.AppendLine("- Cần cải thiện độ chính xác");
//            description.AppendLine("- Phản hồi từ khách hàng ở mức khá");
//        }
//        else if (score >= 60)
//        {
//            description.AppendLine("- Chất lượng công việc ở mức trung bình");
//            description.AppendLine("- Cần giảm thiểu lỗi trong công việc");
//            description.AppendLine("- Cần cải thiện sự hài lòng của khách hàng");
//        }
//        else
//        {
//            description.AppendLine("- Chất lượng công việc chưa đạt yêu cầu");
//            description.AppendLine("- Cần đào tạo thêm về kỹ năng chuyên môn");
//            description.AppendLine("- Cần có kế hoạch cải thiện cụ thể");
//        }

//        return description.ToString();
//    }

//    private string GenerateTimeDescription(float score)
//    {
//        var description = new StringBuilder();

//        if (score >= 90)
//        {
//            description.AppendLine("- Quản lý thời gian xuất sắc");
//            description.AppendLine("- Luôn hoàn thành công việc đúng hoặc trước hạn");
//            description.AppendLine("- Chuyên cần và đúng giờ");
//        }
//        else if (score >= 80)
//        {
//            description.AppendLine("- Quản lý thời gian tốt");
//            description.AppendLine("- Thường xuyên hoàn thành công việc đúng hạn");
//            description.AppendLine("- Có ý thức tốt về chuyên cần");
//        }
//        else if (score >= 70)
//        {
//            description.AppendLine("- Quản lý thời gian khá");
//            description.AppendLine("- Đôi khi trễ deadline nhưng không đáng kể");
//            description.AppendLine("- Cần cải thiện về tính đúng giờ");
//        }
//        else if (score >= 60)
//        {
//            description.AppendLine("- Quản lý thời gian trung bình");
//            description.AppendLine("- Hay bị trễ deadline");
//            description.AppendLine("- Cần cải thiện về chuyên cần");
//        }
//        else
//        {
//            description.AppendLine("- Quản lý thời gian kém");
//            description.AppendLine("- Thường xuyên trễ hạn và vắng mặt");
//            description.AppendLine("- Cần có biện pháp cải thiện ngay");
//        }

//        return description.ToString();
//    }

//    private string GenerateInteractionDescription(float score)
//    {
//        var description = new StringBuilder();

//        if (score >= 90)
//        {
//            description.AppendLine("- Kỹ năng tương tác xuất sắc");
//            description.AppendLine("- Phản hồi nhanh chóng và chuyên nghiệp");
//            description.AppendLine("- Tích cực trong việc nhận và xử lý công việc");
//        }
//        else if (score >= 80)
//        {
//            description.AppendLine("- Kỹ năng tương tác tốt");
//            description.AppendLine("- Phản hồi kịp thời với khách hàng");
//            description.AppendLine("- Sẵn sàng nhận nhiệm vụ được giao");
//        }
//        else if (score >= 70)
//        {
//            description.AppendLine("- Kỹ năng tương tác khá");
//            description.AppendLine("- Thời gian phản hồi có thể cải thiện");
//            description.AppendLine("- Cần chủ động hơn trong việc nhận task");
//        }
//        else if (score >= 60)
//        {
//            description.AppendLine("- Kỹ năng tương tác trung bình");
//            description.AppendLine("- Phản hồi chậm với khách hàng");
//            description.AppendLine("- Thường từ chối nhận công việc");
//        }
//        else
//        {
//            description.AppendLine("- Kỹ năng tương tác kém");
//            description.AppendLine("- Hiếm khi phản hồi khách hàng");
//            description.AppendLine("- Thường xuyên từ chối công việc");
//        }

//        return description.ToString();
//    }

//    public async Task<(IEnumerable<WorkPerformanceDTO> performances, int totalPage)> GetStaffPerformanceHistory(
//        int staffId, 
//        DateTime? startDate = null, 
//        DateTime? endDate = null,
//        int page = 1,
//        int pageSize = 10)
//    {
//        try
//        {
//            var staff = await _unitOfWork.AccountRepository.GetByIDAsync(staffId);
//            if (staff == null)
//                throw new Exception($"Không tìm thấy nhân viên với ID {staffId}");

//            // Tạo filter dựa trên điều kiện
//            Expression<Func<WorkPerformance, bool>> filter;
//            if (startDate.HasValue && endDate.HasValue)
//            {
//                var startDateOnly = DateOnly.FromDateTime(startDate.Value);
//                var endDateOnly = DateOnly.FromDateTime(endDate.Value);
//                filter = w => w.AccountId == staffId && 
//                             w.StartDate >= startDateOnly && 
//                             w.EndDate <= endDateOnly;
//            }
//            else
//            {
//                filter = w => w.AccountId == staffId;
//            }

//            // Đếm tổng số bản ghi thỏa mãn điều kiện
//            var totalRecords = await _unitOfWork.WorkPerformanceRepository.CountAsync(filter);
//            var totalPage = (int)Math.Ceiling(totalRecords / (double)pageSize);

//            // Lấy danh sách theo trang
//            var performances = await _unitOfWork.WorkPerformanceRepository.GetAllAsync(
//                filter: filter,
//                orderBy: q => q.OrderByDescending(p => p.UploadTime),
//                pageIndex: page,
//                pageSize: pageSize
//            );

//            // Map sang DTO
//            var performanceDTOs = performances.Select(p => new WorkPerformanceDTO
//            {
//                WorkId = p.WorkId,
//                AccountId = p.AccountId,
//                AccountFullName = staff.FullName,
//                PhoneNumber = staff.PhoneNumber,
//                StartDate = p.StartDate,
//                EndDate = p.EndDate,
//                UploadTime = p.UploadTime,
//                QualityMaintenancePoint = p.QualityMaintenancePoint,
//                TimeCompletePoint = p.TimeCompletePoint,
//                InteractionPoint = p.InteractionPoint,
//                OverallPoint = Math.Round(
//                    (p.QualityMaintenancePoint * 0.4) + 
//                    (p.TimeCompletePoint * 0.3) + 
//                    (p.InteractionPoint * 0.3), 
//                    2),
//                Description = p.Description,
//                PerformanceLevel = GetPerformanceLevel(
//                    (float)((p.QualityMaintenancePoint * 0.4) + 
//                    (p.TimeCompletePoint * 0.3) + 
//                    (p.InteractionPoint * 0.3))),
//                Status = p.Status
//            });

//            return (performanceDTOs, totalPage);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error getting performance history for staff {StaffId}", staffId);
//            throw;
//        }
//    }

   

    

    

//    public async Task<Stream> GenerateStaffPerformanceReport(int workId)
//    {
//        try
//        {
//            // Lấy thông tin đánh giá từ database
//            var performance = await _unitOfWork.WorkPerformanceRepository.GetByIDAsync(workId);
//            if (performance == null)
//                throw new Exception($"Không tìm thấy đánh giá với ID {workId}");

//            // Lấy thông tin staff
//            var staff = await _unitOfWork.AccountRepository.GetByIDAsync(performance.AccountId);
//            if (staff == null)
//                throw new Exception($"Không tìm thấy nhân viên với ID {performance.AccountId}");

//            // Lấy metrics từ khoảng thời gian của đánh giá
//            var metrics = await GetPerformanceMetrics(
//                performance.AccountId,
//                performance.StartDate.ToDateTime(TimeOnly.MinValue),
//                performance.EndDate.ToDateTime(TimeOnly.MinValue)
//            );
//            var metricsDTO = _mapper.Map<PerformanceMetricsDTO>(metrics);

//            // Tính điểm tổng
//            float overallScore = (float)((performance.QualityMaintenancePoint * 0.4) +
//                                       (performance.TimeCompletePoint * 0.3) +
//                                       (performance.InteractionPoint * 0.3));

//            using (var workbook = new XLWorkbook())
//            {
//                var sheet = workbook.Worksheets.Add("Đánh giá hiệu suất");

//                // Tiêu đề
//                sheet.Cell("A1").Value = "BÁO CÁO ĐÁNH GIÁ HIỆU SUẤT NHÂN VIÊN";
//                sheet.Range("A1:F1").Merge().Style.Font.Bold = true;

//                // Thông tin nhân viên
//                sheet.Cell("A3").Value = "Họ và tên:";
//                sheet.Cell("B3").Value = staff.FullName;
//                sheet.Cell("A4").Value = "Số điện thoại:";
//                sheet.Cell("B4").Value = staff.PhoneNumber;
//                sheet.Cell("A5").Value = "Thời gian đánh giá:";
//                sheet.Cell("B5").Value = $"Từ {performance.StartDate:dd/MM/yyyy} đến {performance.EndDate:dd/MM/yyyy}";

//                // Điểm số chi tiết
//                sheet.Cell("A7").Value = "ĐIỂM SỐ CHI TIẾT";
//                sheet.Range("A7:F7").Merge().Style.Font.Bold = true;

//                sheet.Cell("A8").Value = "Điểm chất lượng công việc:";
//                sheet.Cell("B8").Value = performance.QualityMaintenancePoint;
//                sheet.Cell("C8").Value = "(Trọng số: 40%)";

//                sheet.Cell("A9").Value = "Điểm quản lý thời gian:";
//                sheet.Cell("B9").Value = performance.TimeCompletePoint;
//                sheet.Cell("C9").Value = "(Trọng số: 30%)";

//                sheet.Cell("A10").Value = "Điểm kỹ năng tương tác:";
//                sheet.Cell("B10").Value = performance.InteractionPoint;
//                sheet.Cell("C10").Value = "(Trọng số: 30%)";

//                sheet.Cell("A11").Value = "ĐIỂM TỔNG KẾT:";
//                sheet.Cell("B11").Value = Math.Round(overallScore, 2);
//                sheet.Cell("A12").Value = "XẾP LOẠI:";
//                sheet.Cell("B12").Value = GetPerformanceLevel(overallScore);
//                sheet.Range("A11:B12").Style.Font.Bold = true;

//                // // Chi tiết đánh giá
//                // sheet.Cell("A14").Value = "CHI TIẾT ĐÁNH GIÁ";
//                // sheet.Range("A14:F14").Merge().Style.Font.Bold = true;

//                // sheet.Cell("A15").Value = "Tổng số nhiệm vụ:";
//                // sheet.Cell("B15").Value = metricsDTO.TotalTasks;
//                // sheet.Cell("D15").Value = "Nhiệm vụ hoàn thành:";
//                // sheet.Cell("E15").Value = metricsDTO.CompletedTasks;

//                // sheet.Cell("A16").Value = "Nhiệm vụ đúng hạn:";
//                // sheet.Cell("B16").Value = metricsDTO.OnTimeTasks;
//                // sheet.Cell("D16").Value = "Nhiệm vụ trễ hạn:";
//                // sheet.Cell("E16").Value = metricsDTO.TotalTasks - metricsDTO.OnTimeTasks;

//                // sheet.Cell("A17").Value = "Tổng số phản hồi:";
//                // sheet.Cell("B17").Value = metricsDTO.TotalFeedbacks;
//                // sheet.Cell("D17").Value = "Đã phản hồi:";
//                // sheet.Cell("E17").Value = metricsDTO.RespondedFeedbacks;

//                // sheet.Cell("A18").Value = "Điểm đánh giá TB:";
//                // sheet.Cell("B18").Value = Math.Round(metricsDTO.AverageRating, 2);
//                // sheet.Cell("D18").Value = "Thời gian phản hồi TB:";
//                // sheet.Cell("E18").Value = $"{Math.Round(metricsDTO.AverageResponseTime, 1)} giờ";

//                // sheet.Cell("A19").Value = "Số ngày làm việc:";
//                // sheet.Cell("B19").Value = metricsDTO.TotalWorkDays;
//                // sheet.Cell("D19").Value = "Số ngày có mặt:";
//                // sheet.Cell("E19").Value = metricsDTO.PresentDays;

//                // // Thêm các tỷ lệ phần trăm
//                // sheet.Cell("A20").Value = "Tỷ lệ hoàn thành:";
//                // sheet.Cell("B20").Value = $"{Math.Round(metricsDTO.TaskCompletionRate, 1)}%";
//                // sheet.Cell("D20").Value = "Tỷ lệ chuyên cần:";
//                // sheet.Cell("E20").Value = $"{Math.Round(metricsDTO.AttendanceRate, 1)}%";

//                // Nhận xét chi tiết
//                sheet.Cell("A22").Value = "NHẬN XÉT CHI TIẾT";
//                sheet.Range("A22:F22").Merge().Style.Font.Bold = true;
//                sheet.Cell("A23").Value = performance.Description;
//                sheet.Range("A23:F26").Merge().Style.Alignment.WrapText = true;

//                // Định dạng
//                sheet.Columns().AdjustToContents();
//                sheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
//                sheet.Range("A1:F26").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
//                sheet.Range($"A1:F{sheet.LastRowUsed().RowNumber()}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

//                var stream = new MemoryStream();
//                workbook.SaveAs(stream);
//                stream.Position = 0;
//                return stream;
//            }
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error generating performance report for work {WorkId}", workId);
//            throw;
//        }
//    }

//    public async Task<WorkPerformanceDTO> UpdatePerformance(int workId, UpdatePerformanceDTO updateDTO)
//    {
//        try
//        {
//            // Lấy performance cần update
//            var performance = await _unitOfWork.WorkPerformanceRepository.GetByIDAsync(workId);
//            if (performance == null)
//                throw new Exception($"Không tìm thấy đánh giá với ID {workId}");

//            // Lấy thông tin staff
//            var staff = await _unitOfWork.AccountRepository.GetByIDAsync(performance.AccountId);
//            if (staff == null)
//                throw new Exception($"Không tìm thấy nhân viên với ID {performance.AccountId}");

//            // Validate điểm số
//            if (updateDTO.QualityMaintenancePoint < 0 || updateDTO.QualityMaintenancePoint > 100 ||
//                updateDTO.TimeCompletePoint < 0 || updateDTO.TimeCompletePoint > 100 ||
//                updateDTO.InteractionPoint < 0 || updateDTO.InteractionPoint > 100)
//            {
//                throw new Exception("Điểm số phải nằm trong khoảng 0-100");
//            }

//            // Cập nhật các điểm số mới
//            performance.QualityMaintenancePoint = Math.Round(updateDTO.QualityMaintenancePoint, 2);
//            performance.TimeCompletePoint = Math.Round(updateDTO.TimeCompletePoint, 2);
//            performance.InteractionPoint = Math.Round(updateDTO.InteractionPoint, 2);
            
//            // Tính toán lại điểm tổng
//            float overallScore = (float)((performance.QualityMaintenancePoint * 0.4) +
//                                       (performance.TimeCompletePoint * 0.3) +
//                                       (performance.InteractionPoint * 0.3));

//            // Tự động tạo mô tả mới dựa trên điểm số mới
//            performance.Description = GenerateDescription(
//                (float)performance.QualityMaintenancePoint,
//                (float)performance.TimeCompletePoint,
//                (float)performance.InteractionPoint,
//                overallScore
//            );

//            // Cập nhật thời gian sửa đổi
//            performance.UploadTime = DateTime.Now;

//            // Lưu thay đổi vào database
//            await _unitOfWork.WorkPerformanceRepository.UpdateAsync(performance);
//            await _unitOfWork.SaveAsync();

//            // Log thông tin cập nhật
//            _logger.LogInformation(
//                "Updated performance {WorkId} for staff {StaffId}. New scores: Quality={Quality}, Time={Time}, Interaction={Interaction}, Overall={Overall}",
//                workId, staff.AccountId, performance.QualityMaintenancePoint, performance.TimeCompletePoint, 
//                performance.InteractionPoint, Math.Round(overallScore, 2));

//            // Trả về DTO với thông tin đã cập nhật
//            return new WorkPerformanceDTO
//            {
//                WorkId = performance.WorkId,
//                AccountId = performance.AccountId,
//                AccountFullName = staff.FullName,
//                PhoneNumber = staff.PhoneNumber,
//                StartDate = performance.StartDate,
//                EndDate = performance.EndDate,
//                UploadTime = performance.UploadTime,
//                QualityMaintenancePoint = performance.QualityMaintenancePoint,
//                TimeCompletePoint = performance.TimeCompletePoint,
//                InteractionPoint = performance.InteractionPoint,
//                OverallPoint = Math.Round(overallScore, 2),
//                Description = performance.Description,
//                PerformanceLevel = GetPerformanceLevel(overallScore),
//                Status = performance.Status,
//                Metrics = await GetPerformanceMetricsDTO(performance.AccountId, 
//                    performance.StartDate.ToDateTime(TimeOnly.MinValue), 
//                    performance.EndDate.ToDateTime(TimeOnly.MinValue))
//            };
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error updating performance {WorkId}", workId);
//            throw;
//        }
//    }


//        private async Task<PerformanceMetricsDTO> GetPerformanceMetricsDTO(int staffId, DateTime startDate, DateTime endDate)
//        {
//            var metrics = await GetPerformanceMetrics(staffId, startDate, endDate);
//            return _mapper.Map<PerformanceMetricsDTO>(metrics);
//        }

//    public async Task<bool> DeletePerformance(int workId)
//    {
//        try
//        {
//            // Lấy performance cần xóa
//            var performance = await _unitOfWork.WorkPerformanceRepository.GetByIDAsync(workId);
//            if (performance == null)
//                throw new Exception($"Không tìm thấy đánh giá với ID {workId}");

//            // Lấy thông tin staff để log
//            var staff = await _unitOfWork.AccountRepository.GetByIDAsync(performance.AccountId);
//            if (staff == null)
//                throw new Exception($"Không tìm thấy nhân viên với ID {performance.AccountId}");

//            // Xóa khỏi database
//            await _unitOfWork.WorkPerformanceRepository.DeleteAsync(performance);
//            await _unitOfWork.SaveAsync();

//            // Log thông tin xóa
//            _logger.LogInformation(
//                "Deleted performance {WorkId} for staff {StaffId} ({StaffName})", 
//                workId, 
//                staff.AccountId,
//                staff.FullName);

//            return true;
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error deleting performance {WorkId}", workId);
//            throw;
//        }
//    }

//    public async Task<WorkPerformanceDTO> GetPerformanceById(int workId)
//    {
//        try
//        {
//            // Lấy performance
//            var performance = await _unitOfWork.WorkPerformanceRepository.GetByIDAsync(workId);
//            if (performance == null)
//                throw new Exception($"Không tìm thấy đánh giá với ID {workId}");

//            // Lấy thông tin staff
//            var staff = await _unitOfWork.AccountRepository.GetByIDAsync(performance.AccountId);
//            if (staff == null)
//                throw new Exception($"Không tìm thấy nhân viên với ID {performance.AccountId}");

//            // Tính overall score
//            var overallScore = (float)((performance.QualityMaintenancePoint * 0.4) +
//                                     (performance.TimeCompletePoint * 0.3) +
//                                     (performance.InteractionPoint * 0.3));

//            // Lấy metrics
//            var metrics = await GetPerformanceMetricsDTO(
//                performance.AccountId,
//                performance.StartDate.ToDateTime(TimeOnly.MinValue),
//                performance.EndDate.ToDateTime(TimeOnly.MinValue));

//            // Map sang DTO
//            return new WorkPerformanceDTO
//            {
//                WorkId = performance.WorkId,
//                AccountId = performance.AccountId,
//                AccountFullName = staff.FullName,
//                PhoneNumber = staff.PhoneNumber,
//                StartDate = performance.StartDate,
//                EndDate = performance.EndDate,
//                UploadTime = performance.UploadTime,
//                QualityMaintenancePoint = performance.QualityMaintenancePoint,
//                TimeCompletePoint = performance.TimeCompletePoint,
//                InteractionPoint = performance.InteractionPoint,
//                OverallPoint = Math.Round(overallScore, 2),
//                Description = performance.Description,
//                PerformanceLevel = GetPerformanceLevel(overallScore),
//                Status = performance.Status,
//                Metrics = metrics
//            };
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error getting performance details for WorkId: {WorkId}", workId);
//            throw;
//        }
//    }

//    }
//}
