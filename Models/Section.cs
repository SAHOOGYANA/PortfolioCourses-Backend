namespace PortfolioCourses.Api.Models;

public class Section
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
}
