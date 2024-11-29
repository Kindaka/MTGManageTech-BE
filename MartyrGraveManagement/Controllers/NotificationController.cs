using MartyrGraveManagement_BAL.ModelViews.NotificationDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// GetAllNotifications (Admin)
        /// </summary>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("all")]
        public async Task<ActionResult<List<Notification>>> GetAllNotifications()
        {
            var notifications = await _notificationService.GetAllNotifications();
            return Ok(notifications);
        }

        /// <summary>
        /// GetAllNotifications by accountId (Customer)
        /// </summary>
        [Authorize(Policy = "RequireCustomerRole")]
        [HttpGet("my-notifications")]
        public async Task<ActionResult<List<NotificationDto>>> GetNotificationsByAccountId()
        {
            // Lấy AccountId từ token đã xác thực
            if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int accountId))
            {
                var notifications = await _notificationService.GetNotificationsByAccountId(accountId);
                if (notifications != null && notifications.Any())
                {
                    return Ok(notifications);
                }
                return NotFound("No notifications found for this account.");
            }
            return Unauthorized("Invalid account information.");
        }



        /// <summary>
        /// Link to All Customer Accounts (Create NotificationAccount - False) (Admin)
        /// </summary>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("link-to-customers/{notificationId}")]
        public async Task<ActionResult> LinkNotificationToAllCustomerAccounts(int notificationId)
        {
            var result = await _notificationService.LinkNotificationToAllCustomerAccounts(notificationId);
            if (result)
            {
                return Ok("Notification linked to all customer accounts successfully.");
            }
            return BadRequest("Failed to link notification to customer accounts.");
        }


        /// <summary>
        /// GetNotificationById (Admin)
        /// </summary>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("detail-for-admin/{notificationId}")]
        public async Task<ActionResult<NotificationResponseDto>> GetNotificationById(int notificationId)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(notificationId);
            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

            return Ok(notification);
        }

        /// <summary>
        /// GetNotificationByIdForCustomer (Customer)
        /// </summary>
        [Authorize(Policy = "RequireCustomerRole")]
        [HttpGet("detail-for-customer/{notificationId}")]
        public async Task<ActionResult<NotificationDto>> GetNotificationByIdForCustomer(int notificationId)
        {
            // Lấy AccountId từ token đã xác thực
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int accountId))
            {
                return Unauthorized("Invalid account information.");
            }

            // Lấy thông tin chi tiết của thông báo theo NotificationId và AccountId
            var notification = await _notificationService.GetNotificationByIdForAccount(notificationId, accountId);
            if (notification != null)
            {
                return Ok(notification);
            }

            return NotFound("Notification not found or you don't have access to view this notification.");
        }

        /// <summary>
        /// UpdateNotificationAccountStatus (Admin, Update Status For All Account By notificationId )
        /// </summary>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateNotificationAccountStatus(int notificationId, bool newStatus)
        {
            var result = await _notificationService.UpdateNotificationAccountStatus(notificationId, newStatus);

            if (result)
            {
                return Ok("Notification account statuses updated successfully.");
            }

            return NotFound("Notification account(s) not found or could not be updated.");
        }

    }
}
