using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortfolioCourses.Api.Services; // 🔥 Zaroori: CloudinaryService ke liye
using System.Threading.Tasks;

namespace PortfolioCourses.Api.Controllers.Admin;

[Authorize]
[ApiController]
[Route("api/admin/upload")]
public class AdminUploadController : ControllerBase
{
    private readonly CloudinaryService _cloudinaryService;

    public AdminUploadController(CloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost("image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        // 🔥 Seedha Cloudinary par upload (Folder name: theory_images)
        var fileUrl = await _cloudinaryService.UploadFileAsync(file, "theory_images");

        if (string.IsNullOrEmpty(fileUrl))
            return StatusCode(500, "Failed to upload image to Cloudinary.");

        // 🔥 Tiptap Editor ko exact yahi JSON format chahiye hota hai { url: "..." }
        return Ok(new { url = fileUrl });
    }
}