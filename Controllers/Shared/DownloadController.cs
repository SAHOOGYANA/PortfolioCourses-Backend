using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioCourses.Api.Controllers.Shared 
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
[HttpGet]
        [HttpGet]
        public async Task<IActionResult> ForceDownload([FromQuery] string url, [FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(url)) return BadRequest("URL missing.");

            using var client = new HttpClient();

            // 🛡️ YE HEADERS LAGANA COMPULSORY HAI 🛡️
            // Inke bina Cloudinary security block kar deti hai
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            client.DefaultRequestHeaders.Add("Referer", "https://res.cloudinary.com/");

            try
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    return Content($@"
                <div style='font-family:sans-serif; padding:20px; border:1px solid #ff6b6b; border-radius:8px;'>
                    <h2 style='color:#ff6b6b;'>Cloudinary Rejected the Request</h2>
                    <p><b>Status:</b> {response.StatusCode} ({(int)response.StatusCode})</p>
                    <p><b>Reason:</b> {errorBody}</p>
                    <hr/>
                    <p><i>Tip: Check if 'Strict Transformations' is ENABLED in Cloudinary Security settings. It should be DISABLED.</i></p>
                </div>
            ", "text/html");
                }

                var stream = await response.Content.ReadAsStreamAsync();
                return File(stream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Proxy Error: {ex.Message}");
            }
        }
    }
}