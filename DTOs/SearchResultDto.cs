
//Dtos/SearchResultDto.cs

using FacesHunter.Dtos;

namespace FacesHunter.Dtos
{
    public class SearchResultDto
    {
        public double Similarity { get; set; }
        public PersonDto Person { get; set; } = null!;
    }

    public class PersonDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public DateTime DateMissing { get; set; }
        public string LastSeenLocation { get; set; } = string.Empty;
        public DateTime ReportDate { get; set; }
        public bool IsFound { get; set; }
        public string? ReporterName { get; set; }
        public string? ReporterPhone { get; set; }
    }
}

