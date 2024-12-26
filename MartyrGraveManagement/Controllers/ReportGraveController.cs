﻿using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportGraveController : ControllerBase
    {
        private readonly IReportGraveService _reportGraveService;
        private readonly IAuthorizeService _authorizeService;
        public ReportGraveController(IReportGraveService reportGraveService, IAuthorizeService authorizeService)
        {
            _reportGraveService = reportGraveService;
            _authorizeService = authorizeService;
        }
        [Authorize(Policy = "RequireStaffRole")]
        [HttpPut("staff/update-video")]
        public async Task<IActionResult> UpdateVideoReportGrave(IFormFile file, [FromForm] int staffId, [FromForm] int reportId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkMatchedId = await _authorizeService.CheckAuthorizeStaffByAccountId(staffId, int.Parse(accountId));
                if (!checkMatchedId.isMatchedAccountStaff)
                {
                    return Forbid();
                }
                if (file == null)
                {
                    return BadRequest("No file provided.");
                }

                await _reportGraveService.UploadVideoAsync(file, staffId, reportId);

                return Ok("Video uploaded successfully!");
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("report/{reportId}")]
        public async Task<IActionResult> GetReportDetails(int reportId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var response = await _reportGraveService.GetReportGraveById(reportId);
                return Ok(new
                {
                    ReportDetails = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
