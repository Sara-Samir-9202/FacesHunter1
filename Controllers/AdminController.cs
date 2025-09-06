//Controllers/AdminController.cs
using FacesHunter.Data;
using FacesHunter.Models;
using FacesHunter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FacesHunter.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly INotificationService _notificationService;

        public AdminController(AppDbContext db, INotificationService notificationService)
        {
            _db = db;
            _notificationService = notificationService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalUsers = await _db.Users.CountAsync();
            var totalReports = await _db.Persons.CountAsync();
            var approvedReports = await _db.Persons.CountAsync(r => r.IsApproved == true);
            var pendingReports = await _db.Persons.CountAsync(r => r.IsApproved == false);

            return Ok(new
            {
                users = totalUsers,
                totalReports,
                approvedReports,
                pendingReports
            });
        }

        [HttpGet("persons")]
        public async Task<IActionResult> GetAllPersons()
        {
            var persons = await _db.Persons.ToListAsync();
            return Ok(persons);
        }

        [HttpGet("persons/filter")]
        public async Task<IActionResult> GetPersonsFiltered([FromQuery] string? status)
        {
            var query = _db.Persons.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                switch (status.ToLower())
                {
                    case "approved": query = query.Where(p => p.IsApproved); break;
                    case "pending": query = query.Where(p => !p.IsApproved); break;
                    case "found": query = query.Where(p => p.IsFound); break;
                    case "notfound": query = query.Where(p => !p.IsFound); break;
                    default: return BadRequest("Invalid status value. Use: approved, pending, found, notfound.");
                }
            }

            var persons = await query.ToListAsync();
            return Ok(persons);
        }

        [HttpPut("person/{id}")]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] Person updated)
        {
            var person = await _db.Persons.FindAsync(id);
            if (person == null) return NotFound();

            person.FullName = updated.FullName;
            person.Age = updated.Age;
            person.Status = updated.Status;
            person.Gender = updated.Gender;
            person.ImagePath = updated.ImagePath;
            person.LastSeenLocation = updated.LastSeenLocation;
            person.IsFound = updated.IsFound;

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _db.AuditLogs.Add(new AuditLog
            {
                Action = "Update",
                EntityName = "Person",
                EntityId = id,
                UserId = userId,
                Changes = $"تم تعديل بيانات الشخص: {person.FullName}",
                Timestamp = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            return Ok(person);
        }

        [HttpDelete("person/{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _db.Persons.FindAsync(id);
            if (person == null) return NotFound();

            _db.Persons.Remove(person);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _db.AuditLogs.Add(new AuditLog
            {
                Action = "Delete",
                EntityName = "Person",
                EntityId = id,
                UserId = userId,
                Changes = $"تم حذف بلاغ الشخص: {person.FullName}",
                Timestamp = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            return Ok(new { message = "تم حذف الشخص بنجاح" });
        }

        [HttpPatch("user/toggle/{id}")]
        public async Task<IActionResult> ToggleUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsVerified = !user.IsVerified;
            await _db.SaveChangesAsync();

            return Ok(new { message = $"تم تغيير حالة التحقق للمستخدم إلى: {user.IsVerified}" });
        }

        [HttpGet("unapproved")]
        public async Task<IActionResult> GetUnapprovedReports()
        {
            var reports = await _db.Persons
                .Where(r => !r.IsApproved)
                .ToListAsync();

            return Ok(reports);
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveCase(int id)
        {
            var report = await _db.Persons.FindAsync(id);
            if (report == null || report.IsDeleted) return NotFound("البلاغ غير موجود");

            if (report.IsApproved) return BadRequest("تمت الموافقة مسبقًا على هذا البلاغ");

            report.IsApproved = true;
            await _db.SaveChangesAsync();

            if (report.CreatedByUserId.HasValue)
            {
                var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                await _notificationService.SendAsync(
                    receiverUserId: report.CreatedByUserId.Value,
                    message: $"✅ تمت الموافقة على البلاغ الخاص بـ {report.FullName}",
                    personId: report.Id,
                    senderUserId: adminId
                );
            }

            return Ok("تمت الموافقة على البلاغ بنجاح");
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var totalUsers = await _db.Users.CountAsync();
            var totalReports = await _db.Persons.CountAsync();
            var approvedReports = await _db.Persons.CountAsync(p => p.IsApproved);
            var pendingReports = await _db.Persons.CountAsync(p => !p.IsApproved);
            var foundPersons = await _db.Persons.CountAsync(p => p.IsFound);

            return Ok(new
            {
                totalUsers,
                totalReports,
                approvedReports,
                pendingReports,
                foundPersons
            });
        }

        [HttpGet("audit-logs")]
        public async Task<IActionResult> GetAuditLogs()
        {
            var logs = await _db.AuditLogs
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();

            return Ok(logs);
        }
    }
}
