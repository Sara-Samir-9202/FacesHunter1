
// Controllers/PersonsController.cs
using FacesHunter.Data;
using FacesHunter.DTOs;
using FacesHunter.Models;
using FacesHunter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FacesHunter.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly AiService _aiService;

        public PersonsController(AppDbContext context, IWebHostEnvironment env, AiService aiService)
        {
            _context = context;
            _env = env;
            _aiService = aiService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPerson([FromForm] PersonCreateDto dto)
        {
            if (dto.ReporterIdImage == null || dto.ReporterIdImage.Length == 0)
                return BadRequest("صورة بطاقة المبلغ مطلوبة");

            if (dto.PersonImage == null || dto.PersonImage.Length == 0)
                return BadRequest("صورة الشخص المفقود مطلوبة");

            var idFolder = Path.Combine(_env.WebRootPath, "id_images");
            if (!Directory.Exists(idFolder)) Directory.CreateDirectory(idFolder);

            var idFileName = $"{Guid.NewGuid()}_{dto.ReporterIdImage.FileName}";
            var idPath = Path.Combine(idFolder, idFileName);

            using (var stream = new FileStream(idPath, FileMode.Create))
            {
                await dto.ReporterIdImage.CopyToAsync(stream);
            }

            var idImagePath = $"/id_images/{idFileName}";

            var imgFolder = Path.Combine(_env.WebRootPath, "person_images");
            if (!Directory.Exists(imgFolder)) Directory.CreateDirectory(imgFolder);

            var imgFileName = $"{Guid.NewGuid()}_{dto.PersonImage.FileName}";
            var imgPath = Path.Combine(imgFolder, imgFileName);

            using (var stream = new FileStream(imgPath, FileMode.Create))
            {
                await dto.PersonImage.CopyToAsync(stream);
            }

            var personImagePath = $"/person_images/{imgFileName}";

            var ocrResult = await _aiService.ExtractNameFromIdAsync(dto.ReporterIdImage);

            bool isVerified = !string.IsNullOrWhiteSpace(ocrResult) &&
                              !string.IsNullOrWhiteSpace(dto.ReporterName) &&
                              ocrResult.ToLower().Contains(dto.ReporterName.ToLower());

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var username = User.FindFirstValue(ClaimTypes.Name)!;

            var person = new Person
            {
                FullName = dto.FullName,
                Age = dto.Age,
                Gender = dto.Gender,
                Status = dto.Status,
                LastSeenLocation = dto.LastSeenLocation,
                AdditionalInfo = dto.AdditionalInfo,
                ImagePath = personImagePath,
                ReporterName = dto.ReporterName,
                ReporterPhone = dto.ReporterPhone,
                ReporterIdImagePath = idImagePath,
                ReporterVerified = isVerified,
                IsApproved = false,
                IsFound = false,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUserId,
                DateMissing = dto.DateMissing
            };

            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();

            var admins = await _context.Users.Where(u => u.Role == "Admin").ToListAsync();

            foreach (var admin in admins)
            {
                var notification = new Notification
                {
                    Message = $"🚨 تم تقديم بلاغ جديد من المستخدم: {username}",
                    ReceiverUserId = admin.Id,
                    SenderUserId = currentUserId,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "تم إرسال البلاغ بنجاح وهو قيد المراجعة من قبل الإدارة",
                personId = person.Id
            });
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyPersons()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var reports = await _context.Persons
                .Where(p => p.CreatedByUserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return Ok(reports);
        }
    }
}
