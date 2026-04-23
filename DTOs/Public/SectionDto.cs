namespace PortfolioCourses.Api.DTOs.Public;

public class SectionDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public List<LectureDto> Lectures { get; set; } = new();
}
