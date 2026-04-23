using System.ComponentModel.DataAnnotations;

namespace PortfolioCourses.Api.DTOs.Admin
{
    public class ProjectVideoUpdateDto
    {
        [Required(ErrorMessage = "Video title is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Video title must be between 3 and 200 characters.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "YouTube Video ID is required.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "YouTube Video ID must be exactly 11 characters.")]
        [RegularExpression("^[a-zA-Z0-9_-]{11}$", ErrorMessage = "Invalid YouTube Video ID. It must not contain spaces or special symbols.")]
        public string YouTubeVideoId { get; set; } = null!;

        public string? TheoryContent { get; set; }

        [StringLength(20, ErrorMessage = "Duration cannot exceed 20 characters.")]
        public string? Duration { get; set; }

        [StringLength(150, ErrorMessage = "Channel name cannot exceed 150 characters.")]
        public string? ChannelName { get; set; }
    }
}