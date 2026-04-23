using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioCourses.Api.Data;
using PortfolioCourses.Api.Models;
using PortfolioCourses.Api.Services;
using System;
using System.Threading.Tasks;

namespace PortfolioCourses.Api.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {

        public readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly CloudinaryService _cloudinaryService;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env, CloudinaryService cloudinaryService)
        {
            _context = context;
            _env = env;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet("dashboard-stats")]
        public async Task<ActionResult> GetDashboardStats()
        {
            var totalCourses = await _context.Courses.CountAsync();
            var totalProjects = await _context.Projects.CountAsync();
            var totalProjectVideos = await _context.ProjectVideos.CountAsync();
            var totalAssets = await _context.Assets.CountAsync();

            return Ok(new
            {
                coursesCount = totalCourses,
                projectsCount = totalProjects,
                videosCount = totalProjectVideos,
                assetsCount = totalAssets
            });
        }

        [HttpPost("siteasset/upload/{key}")]
        [DisableRequestSizeLimit]
        // Limit ko 100MB se badha kar ~500MB kar diya hai, videos heavy hoti hain!
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = 524288000)]
        public async Task<IActionResult> UploadAsset([FromRoute] string key, IFormFile file) // <-- [FromForm] hata diya aur [FromRoute] add kiya
        {
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            var fileUrl = await _cloudinaryService.UploadFileAsync(file, "site_assets");

            if (string.IsNullOrEmpty(fileUrl))
            {
                return StatusCode(500, "Failed to upload file to Cloudinary.");
            }

            var existingAsset = await _context.SiteAssets.FirstOrDefaultAsync(a => a.Key == key);
            if (existingAsset != null)
            {
                if (!string.IsNullOrEmpty(existingAsset.FileUrl))
                {
                    await _cloudinaryService.DeleteFileAsync(existingAsset.FileUrl);
                }
                existingAsset.FileUrl = fileUrl;
            }
            else
            {
                _context.SiteAssets.Add(new SiteAsset { Key = key, FileUrl = fileUrl });
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Uploaded successfully to Cloud", url = fileUrl });
        }
    }
}