//Controllers/NotificationsController.cs
using FacesHunter.Data;
using FacesHunter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FacesHunter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ 1. عرض إشعارات المستخدم الحالي فقط
        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var notifications = await _context.Notifications
                .Where(n => n.ReceiverUserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(notifications);
        }

        // ✅ 2. عرض كل الإشعارات (Admin فقط)
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllNotifications()
        {
            var notifications = await _context.Notifications
                .Include(n => n.ReceiverUser)
                .Include(n => n.SenderUser)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(notifications);
        }

        // ✅ 3. إرسال إشعار لمستخدم (Admin فقط)
        [Authorize(Roles = "Admin")]
        [HttpPost("send-to-user")]
        public async Task<IActionResult> SendToUser(int userId, [FromBody] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return BadRequest("Message cannot be empty");

            var senderId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var notification = new Notification
            {
                ReceiverUserId = userId,
                Message = message,
                SenderUserId = senderId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok("Notification sent");
        }

        // ✅ 4. تعليم إشعار كمقروء (PATCH)
        [Authorize]
        [HttpPatch("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.ReceiverUserId == userId);

            if (notification == null)
                return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok("Notification marked as read");
        }

        // ✅ 5. تعليم إشعار كمقروء (PUT - بديل إضافي)
        [Authorize]
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsReadAlternative(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.ReceiverUserId == userId);

            if (notification == null)
                return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok("Notification marked as read (PUT version)");
        }
    }
}
