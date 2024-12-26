using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestMaterialController : ControllerBase
    {
        private readonly IRequestMaterialService _requestMaterialService;

        public RequestMaterialController(IRequestMaterialService requestMaterialService)
        {
            _requestMaterialService = requestMaterialService;
        }

        /// <summary>
        /// Get all RequestMaterial
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _requestMaterialService.GetAllRequestMaterialsAsync();
            return Ok(result);
        }
    }
}
