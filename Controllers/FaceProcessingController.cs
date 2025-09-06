using Microsoft.AspNetCore.Mvc;
using FacesHunter.Services;

namespace FacesHunter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaceProcessingController : ControllerBase
    {
        private readonly AiService _aiService;

        public FaceProcessingController(AiService aiService)
        {
            _aiService = aiService;
        }

        // ✅ POST: /api/faceprocessing/age-transform?targetAge=60
        // 🧠 إرسال صورة + عمر مستهدف لموديل ArchFace لإرجاع صورة معدّلة بسن أكبر
        [HttpPost("age-transform")]
        public async Task<IActionResult> AgeTransform(IFormFile image, [FromQuery] int targetAge)
        {
            // 🔍 التحقق من وجود صورة
            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded");

            if (targetAge <= 0)
                return BadRequest("Target age must be a positive number");

            // 🚀 استدعاء خدمة ArchFace
            var result = await _aiService.GenerateOlderFaceAsync(image, targetAge);

            if (result == null)
                return StatusCode(500, "Failed to generate aged face");

            // ✅ نرجّع الصورة المعدلة بصيغة image/jpeg
            return File(result, "image/jpeg");
        }
    }
}
