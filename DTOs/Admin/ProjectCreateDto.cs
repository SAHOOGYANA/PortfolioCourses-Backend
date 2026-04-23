using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PortfolioCourses.Api.DTOs.Admin
{
    public class ProjectCreateDto
    {
        [Required(ErrorMessage = "Project title is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Project title must be between 3 and 150 characters.")]
        public string Title { get; set; } = null!;

        [StringLength(3000, ErrorMessage = "Description cannot exceed 3000 characters.")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Instructor name cannot exceed 100 characters.")]
        public string? InstructorName { get; set; }

        // Note: Standard data annotations don't support file size/type natively.
        // File validation (e.g., max 2MB, JPG/PNG only) should be handled in your Controller or Service logic.
        public IFormFile? ImageFile { get; set; }
    }
}