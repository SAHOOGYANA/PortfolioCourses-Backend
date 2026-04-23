namespace PortfolioCourses.Api.DTOs.Admin;

public class CourseUpdateDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsPublished { get; set; }
    public string? InstructorName { get; set; }
    public IFormFile? ImageFile { get; set; } // The actual file from React
}
