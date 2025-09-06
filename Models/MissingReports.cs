//Models/MissingReport.cs

namespace FacesHunter.Models
{
    public class MissingReport
    {
        public int Id { get; set; }

        public int PersonId { get; set; }
        public Person? Person { get; set; }

        public string? ReporterName { get; set; }
        public string? ReporterContact { get; set; }
        public string? ReporterIdImagePath { get; set; }

        public bool IsApproved { get; set; } = false;
        public DateTime DateReported { get; set; } = DateTime.UtcNow;
    }
}
