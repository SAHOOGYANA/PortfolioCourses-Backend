using System.ComponentModel.DataAnnotations;

namespace PortfolioCourses.Api.DTOs.Admin;

public class SectionCreateDto
{
    [Required(ErrorMessage = "Title is mandatory.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be 3-100 characters.")]
    public string Title { get; set; } = null!;
}
