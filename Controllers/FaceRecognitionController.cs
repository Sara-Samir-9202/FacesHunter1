
// Controllers/FaceRecognitionController.cs
using AutoMapper;
using FacesHunter.Dtos;
using FacesHunter.Services;
using FacesHunter.Data;
using Microsoft.AspNetCore.Mvc;


namespace FacesHunter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaceRecognitionController : ControllerBase
    {
        private readonly AiService _aiService;
        private readonly FaceSearchService _faceSearchService;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public FaceRecognitionController(
            AiService aiService,
            FaceSearchService faceSearchService,
            AppDbContext dbContext,
            IMapper mapper)
        {
            _aiService = aiService;
            _faceSearchService = faceSearchService;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // ✅ ArcFace: مقارنة وجه وإرجاع النتائج
        [HttpPost("compare")]
        public async Task<IActionResult> CompareFace(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded");

            var result = await _aiService.CompareFaceAsync(image);
            if (result == null)
                return StatusCode(500, "ArcFace service failed");

            return Ok(new
            {
                message = "ArcFace comparison successful",
                similarity = result.Similarity,
                matches = result.Vectors?.Keys.ToList() ?? new List<string>()
            });
        }

        // ✅ ArcFace: البحث بصورة عن أقرب شخص (تفاصيل كاملة + باستخدام DTO)
        [HttpPost("search")]
        public async Task<IActionResult> SearchByImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded");

            var result = await _aiService.CompareFaceAsync(image);
            if (result?.Vectors == null || result.Vectors.Count == 0)
                return BadRequest("No vectors returned from ArcFace");

            var firstVector = result.Vectors.First().Value
                .EnumerateArray()
                .Select(v => v.GetDouble())
                .ToList();

            var personId = _faceSearchService.FindClosestMatch(firstVector);
            if (personId == null)
                return NotFound("No match found");

            var person = await _dbContext.Persons.FindAsync(personId);
            if (person == null)
                return NotFound("Matched person not found");

            var dto = new SearchResultDto
            {
                Similarity = result.Similarity,
                Person = _mapper.Map<PersonDto>(person)
            };

            return Ok(dto);
        }

        // ✅ SAM: توليد صورة بسن مستهدف
        [HttpPost("age-transform")]
        public async Task<IActionResult> AgeTransform(IFormFile image, [FromForm] int targetAge)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded");

            if (targetAge < 1 || targetAge > 120)
                return BadRequest("Invalid target age");

            var agedImageBytes = await _aiService.GenerateOlderFaceAsync(image, targetAge);
            if (agedImageBytes == null)
                return StatusCode(500, "SAM service failed");

            return File(agedImageBytes, "image/jpeg");
        }
    }
}
