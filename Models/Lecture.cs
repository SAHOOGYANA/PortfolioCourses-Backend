namespace PortfolioCourses.Api.Models;

public class Lecture
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string YouTubeVideoId { get; set; } = null!;
    public bool IsFree { get; set; } = true;

    public int SectionId { get; set; }
    public Section Section { get; set; } = null!;

    // 🔥 Add this line
    public string? TheoryContent { get; set; }
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
}
