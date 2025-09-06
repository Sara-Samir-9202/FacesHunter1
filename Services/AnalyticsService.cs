// Services/AnalyticsService.cs
using FacesHunter.Data;
using FacesHunter.DTOs;

namespace FacesHunter.Services
{
    public class AnalyticsService
    {
        private readonly AppDbContext _context;

        public AnalyticsService(AppDbContext context)
        {
            _context = context;
        }

        // ✅ تنفيذ التحليل: حساب المواقع الشائعة، متوسط الأعمار، توزيع النوع، الحالات الشهرية
        public AnalyticsResultDto Analyze()
        {
            var persons = _context.Persons.ToList();

            // ✅ أكثر 5 أماكن اختفاء تكرارًا
            var mostCommonLocations = persons
                .Where(p => !string.IsNullOrWhiteSpace(p.LocationLost))
                .GroupBy(p => p.LocationLost!)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .ToDictionary(g => g.Key, g => g.Count());

            // ✅ متوسط الأعمار
            double averageAge = persons.Average(p => (double?)p.Age) ?? 0;


            // ✅ توزيع النوع (لو النوع Enum)
            var genderDistribution = persons
                .GroupBy(p => p.Gender.ToString()) // نحوله لنص عشان نعرضه في JSON
                .ToDictionary(g => g.Key, g => g.Count());

            // ✅ عدد الحالات كل شهر
            var monthlyCases = persons
                .GroupBy(p => p.DateMissing.ToString("yyyy-MM"))
                .OrderByDescending(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Count());

            return new AnalyticsResultDto
            {
                MostCommonLocations = mostCommonLocations,
                AverageAge = averageAge,
                GenderDistribution = genderDistribution,
                MonthlyCases = monthlyCases
            };
        }
    }
}
