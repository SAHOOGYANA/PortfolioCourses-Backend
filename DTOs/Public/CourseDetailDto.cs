namespace PortfolioCourses.Api.DTOs.Public;

public class CourseDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? InstructorName { get; set; }
    public string? ImageUrl { get; set; }
    public List<SectionDto> Sections { get; set; } = new();
}
