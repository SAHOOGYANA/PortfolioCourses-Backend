namespace PortfolioCourses.Api.DTOs.Public
{
    public class AssetDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public long? FileSize { get; set; }
    }
}