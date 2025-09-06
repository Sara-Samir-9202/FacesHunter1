using FacesHunter.Data;
using FacesHunter.DTOs;
using FacesHunter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FacesHunter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ReportsController(AppDbContext db)
        {
            _db = db;
        }

        // 🔁 دالة لتحويل string إلى Gender Enum بأمان
        private Gender ParseGender(string input)
        {
            return Enum.TryParse<Gender>(input, true, out var gender) ? gender : Gender.Other;
        }

        [HttpPost("report-missing")]
        [Authorize]
        public async Task<IActionResult> ReportMissing([FromBody] ReportPersonDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var person = new Person
            {
                FullName = dto.FullName,
                Age = dto.Age,
                Gender = ParseGender(dto.Gender),
                Status = "Missing",
                LastSeenLocation = dto.LastSeenLocation,
                ImagePath = dto.ImagePath,
                IsApproved = false,
                IsFound = false,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Persons.Add(person);
            await _db.SaveChangesAsync();

            return Ok(new { message = "تم إرسال البلاغ بنجاح، في انتظار موافقة الأدمن" });
        }

        [HttpPost("report-found")]
        [Authorize]
        public async Task<IActionResult> ReportFound([FromBody] ReportPersonDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var person = new Person
            {
                FullName = dto.FullName,
                Age = dto.Age,
                Gender = ParseGender(dto.Gender),
                Status = "Found",
                LastSeenLocation = dto.LastSeenLocation,
                ImagePath = dto.ImagePath,
                IsApproved = false,
                IsFound = true,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Persons.Add(person);
            await _db.SaveChangesAsync();

            return Ok(new { message = "تم إرسال بلاغ العثور على شخص، بانتظار المراجعة" });
        }

        [HttpGet("my-reports")]
        [Authorize]
        public async Task<IActionResult> GetMyReports()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var reports = await _db.Persons
                .Where(p => p.CreatedByUserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return Ok(reports);
        }
    }
}
