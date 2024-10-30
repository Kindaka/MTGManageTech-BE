using MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs;
using MartyrGraveManagement_BAL.ModelViews.GraveServiceDTOs;
using MartyrGraveManagement_BAL.Services.Implements;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraveServiceController : ControllerBase
    {
        private readonly IGraveService_Service _serviceOfGrave;
        private readonly IAuthorizeService _authorizeService;
        public GraveServiceController(IGraveService_Service serviceOfGrave, IAuthorizeService authorizeService)
        {
            _serviceOfGrave = serviceOfGrave;
            _authorizeService = authorizeService;
        }

        [Authorize(Policy = "RequireManagerRole")]
        [HttpPost]
        public async Task<IActionResult> CreateServiceGrave(GraveServiceDtoRequest graveServiceDTO)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                if (graveServiceDTO == null)
                {
                    return BadRequest("Cannot add empty service to grave");
                }
                var check = await _serviceOfGrave.CreateServiceForGrave(graveServiceDTO);
                if(check.check)
                {
                    return Ok(check.response);
                }
                else
                {
                    return NotFound(check.response);
                }
                
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireManagerRole")]
        [HttpPut]
        public async Task<IActionResult> UpdateServiceGrave(int martyrId, UpdateServiceForGraveDtoRequest graveServiceDTO)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                if (graveServiceDTO == null)
                {
                    return BadRequest("Cannot add empty service to grave");
                }
                var check = await _serviceOfGrave.UpdateServiceForGrave(martyrId, graveServiceDTO);
                if (check.check)
                {
                    return Ok(check.response);
                }
                else
                {
                    return NotFound(check.response);
                }

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("grave-services")]
        public async Task<IActionResult> GetServicesInGrave(int martyrId)
        {
            try
            {
                var services = await _serviceOfGrave.GetAllServicesForGrave(martyrId);
                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
