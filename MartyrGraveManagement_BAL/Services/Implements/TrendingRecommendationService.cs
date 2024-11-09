using MartyrGraveManagement_BAL.MLModels;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    //Note: những cái comment là Manual Scoring áp dụng ML không nhiều
    //Tính điểm dựa trên công thức cố định với các trọng số
    //Đơn giản và dễ hiểu
    //Không học từ dữ liệu lịch sử

    //Code hiện tại chuyển từ Manual Scoring sang TrainModel:
    //Sử dụng mô hình ML đã được train
    //Có khả năng học từ dữ liệu lịch sử
    //Có thể phát hiện các mẫu phức tạp hơn
    public class TrendingRecommendationService : ITrendingRecommendationService
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private readonly IUnitOfWork _unitOfWork;

        public TrendingRecommendationService(IUnitOfWork unitOfWork)
        {
            _mlContext = new MLContext();
            _unitOfWork = unitOfWork;
            Task.Run(async () => _model = await TrainModel()).Wait(); // Huấn luyện mô hình một lần khi khởi tạo
        }

        // private async Task<ITransformer> TrainModel()
        // {
        //     var serviceFrequencyData = (await _unitOfWork.OrderDetailRepository
        // .GetAllAsync())
        // .GroupBy(o => o.ServiceId)
        // .Select(g => new ServiceData
        // {
        //     ServiceId = g.Key,
        //     Frequency = g.Count() * 10f,
        //     AveragePrice = (float)g.Average(o => o.OrderPrice),
        //     Quantity = (float)g.Sum(o => o.Quantity),
        //     // Thêm kiểm tra null cho Order
        //     RecentOrdersCount = g.Count(o => o.Order != null && o.Order.OrderDate >= DateTime.Now.AddMonths(-1)) * 10f
        // })
        // .ToList();

        //     if (!serviceFrequencyData.Any())
        //     {
        //         return null;
        //     }

        //     var dataView = _mlContext.Data.LoadFromEnumerable(serviceFrequencyData);

        //     var pipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(ServiceData.Frequency))
        //// Chuyển đổi tất cả các cột về kiểu float trước khi concatenate
        //.Append(_mlContext.Transforms.Conversion.ConvertType("ServiceIdFloat", nameof(ServiceData.ServiceId), Microsoft.ML.Data.DataKind.Single))
        //.Append(_mlContext.Transforms.Concatenate("Features",
        //    "ServiceIdFloat",  // Sử dụng cột đã chuyển đổi
        //    nameof(ServiceData.AveragePrice),
        //    nameof(ServiceData.Quantity),
        //    nameof(ServiceData.RecentOrdersCount)))
        //.Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", maximumNumberOfIterations: 200));

        //     return pipeline.Fit(dataView);
        // }



        //  public async Task<List<ServiceDtoResponse>> RecommendTopTrendingServices(int topN = 5)
        //  {
        //      if (_model == null)
        //      {
        //          _model = await TrainModel();
        //      }

        //      if (_model == null)
        //      {
        //          return new List<ServiceDtoResponse>();
        //      }

        //      var serviceFrequencyData = (await _unitOfWork.OrderDetailRepository
        //          .GetAllAsync(includeProperties: "Order")) // Thêm Include Order
        //          .GroupBy(o => o.ServiceId)
        //          .Select(g => new ServiceData
        //          {
        //              ServiceId = g.Key,
        //              Frequency = g.Count() * 10f,
        //              AveragePrice = (float)g.Average(o => o.OrderPrice),
        //              Quantity = (float)g.Sum(o => o.Quantity),
        //              RecentOrdersCount = g.Count(o => o.Order != null &&
        //                  o.Order.OrderDate >= DateTime.Now.AddMonths(-1)) * 10f
        //          })
        //          .ToList();

        //      if (!serviceFrequencyData.Any())
        //      {
        //          return new List<ServiceDtoResponse>();
        //      }

        //      // Chuẩn hóa các giá trị
        //      var maxFrequency = serviceFrequencyData.Max(f => f.Frequency);
        //      var maxQuantity = serviceFrequencyData.Max(f => f.Quantity);
        //      var maxRecentOrders = serviceFrequencyData.Max(f => f.RecentOrdersCount);

        //      // Tính điểm xu hướng với trọng số
        //      var trendingServices = serviceFrequencyData
        //.Select(data => new
        //{
        //    ServiceId = data.ServiceId,
        //    TrendScore = (
        //        (maxFrequency > 0 ? (data.Frequency / maxFrequency) : 0) * 0.4f + // Tần suất: 40%
        //        (maxQuantity > 0 ? (data.Quantity / maxQuantity) : 0) * 0.3f + // Số lượng: 30%
        //        (maxRecentOrders > 0 ? (data.RecentOrdersCount / maxRecentOrders) : 0) * 0.3f // Đơn hàng gần đây: 30%
        //    )
        //})
        //.OrderByDescending(x => x.TrendScore)
        //.Take(topN)
        //.ToList();

        //      return await MapToServiceDtoResponses(trendingServices.Select(x => x.ServiceId).ToList());
        //  }


        private async Task<ITransformer> TrainModel()
        {
            var serviceFrequencyData = (await _unitOfWork.OrderDetailRepository
                .GetAllAsync())
                .GroupBy(o => o.ServiceId)
                .Select(g =>
                {
                    float frequency = g.Count() * 10f;
                    float quantity = (float)g.Sum(o => o.Quantity);
                    float recentOrdersCount = g.Count(o => o.Order != null &&
                        o.Order.OrderDate >= DateTime.Now.AddMonths(-1)) * 10f;

                    // Chuẩn hóa dữ liệu
                    float maxFrequency = frequency;
                    float maxQuantity = quantity;
                    float maxRecentOrders = recentOrdersCount;

                    // Tính TrendScore tương tự như cách manual
                    float trendScore =
                        (maxFrequency > 0 ? (frequency / maxFrequency) : 0) * 0.4f +
                        (maxQuantity > 0 ? (quantity / maxQuantity) : 0) * 0.3f +
                        (maxRecentOrders > 0 ? (recentOrdersCount / maxRecentOrders) : 0) * 0.3f;

                    return new ServiceData
                    {
                        ServiceId = g.Key,
                        Frequency = frequency,
                        AveragePrice = (float)g.Average(o => o.OrderPrice),
                        Quantity = quantity,
                        RecentOrdersCount = recentOrdersCount,
                        TrendScore = trendScore
                    };
                })
                .ToList();

            if (!serviceFrequencyData.Any())
            {
                return null;
            }

            var dataView = _mlContext.Data.LoadFromEnumerable(serviceFrequencyData);

            var pipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(ServiceData.TrendScore))
                .Append(_mlContext.Transforms.Concatenate("Features",
                    nameof(ServiceData.Frequency),
                    nameof(ServiceData.Quantity),
                    nameof(ServiceData.RecentOrdersCount)))
                .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", maximumNumberOfIterations: 200));

            return pipeline.Fit(dataView);
        }

        public async Task<List<ServiceDtoResponse>> RecommendTopTrendingServices(int topN = 5)
        {
            if (_model == null)
            {
                _model = await TrainModel();
            }

            if (_model == null)
            {
                return new List<ServiceDtoResponse>();
            }

            var orderDetails = await _unitOfWork.OrderDetailRepository
                .GetAllAsync(includeProperties: "Order");

            var serviceIds = orderDetails
                .GroupBy(o => o.ServiceId)
                .Select(g => g.Key)
                .ToList();

            var trendingServices = new List<(int ServiceId, float TrendScore)>();
            foreach (var serviceId in serviceIds)
            {
                var score = await PredictServiceTrendScore(serviceId);
                trendingServices.Add((serviceId, score));
            }

            var topServices = trendingServices
                .OrderByDescending(x => x.TrendScore)
                .Take(topN)
                .Select(x => x.ServiceId)
                .ToList();

            return await MapToServiceDtoResponses(topServices);
        }


        //private float PredictServiceTrendScore(int serviceId)
        //{
        //    var predictionEngine = _mlContext.Model.CreatePredictionEngine<ServiceData, ServicePrediction>(_model);
        //    var prediction = predictionEngine.Predict(new ServiceData { ServiceId = serviceId });
        //    return prediction.Score;
        //}

        private async Task<float> PredictServiceTrendScore(int serviceId)
        {
            var orderDetails = await _unitOfWork.OrderDetailRepository.GetAllAsync(includeProperties: "Order");

            var serviceData = orderDetails
                .Where(od => od.ServiceId == serviceId)
                .GroupBy(o => o.ServiceId)
                .Select(g => new ServiceData
                {
                    ServiceId = serviceId,
                    Frequency = g.Count() * 10f,
                    Quantity = (float)g.Sum(o => o.Quantity),
                    RecentOrdersCount = g.Count(o => o.Order != null &&
                        o.Order.OrderDate >= DateTime.Now.AddMonths(-1)) * 10f
                })
                .FirstOrDefault();

            if (serviceData == null)
                return 0;

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<ServiceData, ServicePrediction>(_model);
            var prediction = predictionEngine.Predict(serviceData);
            return prediction.Score;
        }

        private async Task<List<ServiceDtoResponse>> MapToServiceDtoResponses(List<int> serviceIds)
        {
            var result = new List<ServiceDtoResponse>();
            foreach (var serviceId in serviceIds)
            {
                var service = await _unitOfWork.ServiceRepository.GetByIDAsync(serviceId);
                if (service != null)
                {
                    var serviceDto = new ServiceDtoResponse
                    {
                        ServiceId = service.ServiceId,
                        CategoryId = service.CategoryId,
                        CategoryName = service.ServiceCategory?.CategoryName ?? "N/A",
                        ServiceName = service.ServiceName,
                        Description = service.Description,
                        Price = service.Price,
                        ImagePath = service.ImagePath,
                        Status = service.Status
                    };
                    result.Add(serviceDto);
                }
            }

            return result;
        }
    }

}
