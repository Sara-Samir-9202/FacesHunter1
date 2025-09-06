// Controllers/SearchController.cs
using FacesHunter.Data;
using FacesHunter.DTOs;
using FacesHunter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FacesHunter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppDbContext _context;

        public SearchController(IHttpClientFactory httpClientFactory, AppDbContext context)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
        }

        // ✅ POST: /api/search/search
        [HttpPost("search")]
        public async Task<IActionResult> SearchWithAI([FromForm] ImageUploadDto dto)
        {
            var image = dto.Image;

            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded.");

            var client = _httpClientFactory.CreateClient();
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(image.OpenReadStream());

            streamContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
            content.Add(streamContent, "image", image.FileName);

            var aiApiUrl = "http://localhost:5000/api/face/match"; // 🎯 غيّريه لو FastAPI مختلف

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(aiApiUrl, content);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error connecting to AI service: {ex.Message}");
            }

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "AI Service Error");

            var jsonResponse = await response.Content.ReadAsStringAsync();

            // ✅ محاولة قراءة استجابة التطابق
            try
            {
                var matchResult = JsonSerializer.Deserialize<FaceMatchResult>(jsonResponse);

                // ✅ لو فيه تطابق، أضف إشعار
                if (matchResult != null && matchResult.match_found)
                {
                    var notification = new Notification
                    {
                        Message = $"Match found for person: {matchResult.matched_person_name}",
                        PersonId = matchResult.matched_person_id,
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    };

                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();
                }

                // ✅ رجّع الرد كما هو
                return Content(jsonResponse, "application/json");
            }
            catch
            {
                return StatusCode(500, "Failed to parse AI response.");
            }
        }
    }

    // ✅ كلاس لمطابقة الـ JSON من FastAPI
    public class FaceMatchResult
    {
        public bool match_found { get; set; }
        public int matched_person_id { get; set; }
        public string matched_person_name { get; set; } = string.Empty;
    }
}
