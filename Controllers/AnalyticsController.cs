// Controllers/AnalyticsController.cs
using FacesHunter.Services;
using Microsoft.AspNetCore.Mvc;

namespace FacesHunter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AnalyticsService _analyticsService;

        public AnalyticsController(AnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("summary")]
        public IActionResult GetAnalyticsSummary()
        {
            var result = _analyticsService.Analyze();
            return Ok(result);
        }
    }
}
