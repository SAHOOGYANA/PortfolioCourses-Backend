namespace PortfolioCourses.Api.DTOs.Public
{
    public class ProjectDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public string? InstructorName { get; set; } // ✅ Sent to public UI
        public string? ImageUrl { get; set; }       // ✅ Sent to public UI
        public List<ProjectVideoDto> Videos { get; set; } = new List<ProjectVideoDto>();
    }
}
