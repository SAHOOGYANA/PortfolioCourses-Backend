using System.ComponentModel.DataAnnotations;

namespace PortfolioCourses.Api.DTOs.Admin;

public class CourseCreateDto
{
    [Required(ErrorMessage = "Title is mandatory.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be 3-100 characters.")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Please provide a short description.")]
    public string Description { get; set; } = null!;

    public string? InstructorName { get; set; }
    public IFormFile? ImageFile { get; set; } // The actual file from React
}
