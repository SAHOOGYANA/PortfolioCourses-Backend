namespace PortfolioCourses.Api.DTOs.Public;

public class LectureDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string YouTubeVideoId { get; set; } = null!;

    // 🔥 ADD THIS LINE
    public string? TheoryContent { get; set; }

}
