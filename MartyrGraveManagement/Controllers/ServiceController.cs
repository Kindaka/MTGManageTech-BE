using MartyrGraveManagement_BAL.ModelViews.ServiceCategoryDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IService_Service _service;
        private readonly ITrendingRecommendationService _trendingService;

        public ServiceController(IService_Service service, ITrendingRecommendationService trendingService)
        {
            _service = service;
            _trendingService = trendingService;
        }

        /// <summary>
        /// Get all services.
        /// </summary>
        /// <returns>Returns a list of all services.</returns>
        /// <response code="200">Returns the list of services</response>
        /// <response code="500">If there is any server error</response>
        [AllowAnonymous]
        [HttpGet("services")]
        public async Task<IActionResult> GetAllServices(int? categoryId)
        {
            try
            {
                var services = await _service.GetAllServices(categoryId);
                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("admin/services")]
        public async Task<IActionResult> GetServicesAdmin(int? categoryId, int page = 1, int pageSize = 5)
        {
            try
            {
                var services = await _service.GetServicesForAdmin(categoryId, page, pageSize);
                return Ok(new { services = services.serviceList, totalPage = services.totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Add a new service.
        /// </summary>
        /// <param name="service">Details of the new service.</param>
        /// <returns>Returns success or failure message.</returns>
        /// <response code="200">If the service is created successfully</response>
        /// <response code="400">If the service creation fails</response>
        /// <response code="500">If there is any server error</response>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("services")]
        public async Task<IActionResult> AddService(ServiceDtoRequest service)
        {
            try
            {
                var check = await _service.AddService(service);
                if (check.status)
                {
                    return Ok(check.result);
                }
                else
                {
                    return BadRequest(check.result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update an existing service.
        /// </summary>
        /// <param name="service">Updated details of the service.</param>
        /// <param name="serviceId">ID of the service to update.</param>
        /// <returns>Returns the result of the update operation.</returns>
        /// <response code="200">If the service is updated successfully</response>
        /// <response code="400">If the update fails</response>
        /// <response code="500">If there is any server error</response>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("services")]
        public async Task<IActionResult> UpdateService(ServiceDtoRequest service, int serviceId)
        {
            try
            {
                var check = await _service.UpdateService(service, serviceId);
                if (check.status)
                {
                    return Ok(check.result);
                }
                else
                {
                    return BadRequest(check.result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("status-service")]
        public async Task<IActionResult> UpdateStatusService(int serviceId)
        {
            try
            {
                var check = await _service.ChangeStatus(serviceId);
                if (check.status)
                {
                    return Ok(check.result);
                }
                else
                {
                    return BadRequest(check.result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [AllowAnonymous]
        [HttpGet("service-detail")]
        public async Task<IActionResult> GetService(int serviceId)
        {
            try
            {
                var service = await _service.GetServiceById(serviceId);
                if (service != null)
                {
                    return Ok(service);
                }
                else
                {
                    return NotFound("Không tìm thấy dịch vụ");
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpGet("trending-services")]
        public async Task<IActionResult> GetTrendingServices(int topN = 5)
        {
            try
            {
                // Gọi phương thức RecommendTopTrendingServices từ TrendingRecommendationService
                var trendingServices = await _trendingService.RecommendTopTrendingServices(topN);

                // Kiểm tra nếu không có dịch vụ nào được trả về
                if (trendingServices == null || trendingServices.Count == 0)
                {
                    return NotFound(new { message = "Không tìm thấy dịch vụ phổ biến nào." });
                }

                return Ok(trendingServices);
            }
            catch (InvalidOperationException ex) // Bắt InvalidOperationException từ Service
            {
                // Log lỗi
                Console.WriteLine($"Lỗi: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                Console.WriteLine($"Lỗi không mong muốn: {ex}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




    }
}
