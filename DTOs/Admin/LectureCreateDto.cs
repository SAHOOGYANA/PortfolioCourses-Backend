using System.ComponentModel.DataAnnotations;

namespace PortfolioCourses.Api.DTOs.Admin;

public class LectureCreateDto
{
    [Required(ErrorMessage = "Title is mandatory.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be 3-100 characters.")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "YouTube Video ID is mandatory.")]
    public string YouTubeVideoId { get; set; } = null!;

    // 🔥 Added to support creating with theory content right away
    public string? TheoryContent { get; set; }
}