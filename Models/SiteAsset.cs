namespace PortfolioCourses.Api.Models
{
    public class SiteAsset
    {
        public int Id { get; set; }
        public string Key { get; set; } // e.g., "HomeHeroVideo"
        public string FileUrl { get; set; } // e.g., "/uploads/assets/hero.mp4"
    }
}
