using MartyrGraveManagement_BAL.ModelViews.ServiceCategoryDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                throw new Exception(ex.Message);
            }
        }

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
                throw new Exception(ex.Message);
            }
        }

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
                throw new Exception(ex.Message);
            }
        }
    }
}
