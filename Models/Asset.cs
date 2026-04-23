namespace PortfolioCourses.Api.Models;

public class Asset
{
    public int Id { get; set; }
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;

    public int? LectureId { get; set; }
    public Lecture Lecture { get; set; } = null!;

    public int? ProjectVideoId { get; set; }
    public ProjectVideo ProjectVideo { get; set; } = null!;
}
