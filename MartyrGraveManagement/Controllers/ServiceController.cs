using MartyrGraveManagement_BAL.ModelViews.ServiceCategoryDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IGraveService_Service _service;
        public ServiceController(IGraveService_Service service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all services.
        /// </summary>
        /// <returns>Returns a list of all services.</returns>
        /// <response code="200">Returns the list of services</response>
        /// <response code="500">If there is any server error</response>
        [HttpGet("services")]
        public async Task<IActionResult> GetAllCategory()
        {
            try
            {
                var services = await _service.GetAllServices();
                return Ok(services);
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
    }
}
