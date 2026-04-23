namespace PortfolioCourses.Api.Models
{
    public class ProjectVideo
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string YouTubeVideoId { get; set; }

        public string TheoryContent { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public string Duration { get; set; } = "0:00";
        public string ChannelName { get; set; } = "Unknown Channel";
        public ICollection<Asset> Assets { get; set; }
    }
}
