namespace FacesHunter.DTOs
{
    public class ReportPersonDto
    {
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;

        public string LastSeenLocation { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
    }
}

