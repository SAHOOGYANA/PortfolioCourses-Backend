namespace PortfolioCourses.Api.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        // 🔥 Naya Feature
        public bool IsPublished { get; set; } = false;
        public string? ImageUrl { get; set; }
        public string InstructorName { get; set; } = "Harshit Vyas"; // ✅ Default Value
        public ICollection<ProjectVideo> Videos { get; set; } = new List<ProjectVideo>();
    }
}