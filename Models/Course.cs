using static System.Collections.Specialized.BitVector32;

namespace PortfolioCourses.Api.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsPublished { get; set; } = false;

    public string? ImageUrl { get; set; }
    public string InstructorName { get; set; } = "Gian";
    public ICollection<Section> Sections { get; set; } = new List<Section>();
}
