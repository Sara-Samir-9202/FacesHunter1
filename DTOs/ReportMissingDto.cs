//DTOs/ReportMissingDto
public class ReportMissingDto
{
    public string FullName { get; set; } = string.Empty;
    public string ReporterName { get; set; } = string.Empty;
    public string ReporterPhone { get; set; } = string.Empty;
    public IFormFile ReporterIdImage { get; set; } = null!;
}

