namespace PortfolioCourses.Api.DTOs.Public
{
    public class ProjectVideoDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string YouTubeVideoId { get; set; }
        public string? TheoryContent { get; set; }

        public string Duration { get; set; }
        public string ChannelName { get; set; }

        // 🔥 BAS YE LINE ADD KAR DE
        public List<AssetDto> Assets { get; set; } = new List<AssetDto>();
    }
}
