// Controllers/SuccessStoriesController.cs

using FacesHunter.Data;
using FacesHunter.DTOs;
using FacesHunter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacesHunter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuccessStoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SuccessStoriesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ✅ رفع قصة جديدة (محمي بـ JWT)
        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddStory([FromForm] SuccessStoryCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Video == null || dto.Video.Length == 0)
                return BadRequest("Video file is required");

            var allowedExtensions = new[] { ".mp4", ".mov", ".avi" };
            var extension = Path.GetExtension(dto.Video.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Only MP4, MOV, and AVI videos are allowed");

            // 📁 إنشاء مجلد success_videos لو مش موجود
            var folderPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "success_videos");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.Video.FileName)}";
            var fullPath = Path.Combine(folderPath, uniqueFileName);

            // 💾 حفظ الفيديو
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await dto.Video.CopyToAsync(stream);
            }

            var story = new SuccessStory
            {
                Title = dto.Title ?? "Untitled",
                Description = dto.Description ?? "",
                VideoPath = $"/success_videos/{uniqueFileName}",
                DatePosted = dto.DatePosted ?? DateTime.UtcNow
            };

            _context.SuccessStories.Add(story);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Success story uploaded successfully",
                story
            });
        }

        // ✅ عرض جميع القصص
        [AllowAnonymous]
        [HttpGet("all")]
        public IActionResult GetAllStories()
        {
            var stories = _context.SuccessStories
                .OrderByDescending(s => s.DatePosted)
                .ToList();

            return Ok(stories);
        }
    }
}
