//DTOs/AnalyticsResultDto.cs
namespace FacesHunter.DTOs
{
        public class AnalyticsResultDto
        {
            public Dictionary<string, int> MostCommonLocations { get; set; } = new();
            public double AverageAge { get; set; }
            public Dictionary<string, int> GenderDistribution { get; set; } = new();
            public Dictionary<string, int> MonthlyCases { get; set; } = new();
        }

    }

