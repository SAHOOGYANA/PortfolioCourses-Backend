namespace PortfolioCourses.Api.DTOs.Public;

public class CourseListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;

    // ✅ Add these so the Public API can send them to your React cards
    public string? InstructorName { get; set; }
    public string? ImageUrl { get; set; }
}