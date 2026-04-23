using System.ComponentModel.DataAnnotations;

public class LectureUpdateDto
{
    [Required(ErrorMessage = "Lecture title is required.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "YouTube Video ID is required.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "YouTube Video ID must be exactly 11 characters.")]
    public string YouTubeVideoId { get; set; } = string.Empty;

    public string? TheoryContent { get; set; }
}