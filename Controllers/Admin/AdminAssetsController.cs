using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioCourses.Api.Data;
using PortfolioCourses.Api.Models;
using PortfolioCourses.Api.Services; 
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioCourses.Api.Controllers.Admin;

[Authorize]
[ApiController]
public class AdminAssetsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly CloudinaryService _cloudinaryService; // 🔥 Local Environment ki jagah Cloudinary

    public AdminAssetsController(ApplicationDbContext db, CloudinaryService cloudinaryService)
    {
        _db = db;
        _cloudinaryService = cloudinaryService;
    }

    // ================= LECTURE ASSETS =================

    [HttpGet("api/admin/lectures/{lectureId}/assets")]
    public async Task<IActionResult> GetAssets(int lectureId)
    {
        var assets = await _db.Assets
            .Where(a => a.LectureId == lectureId)
            .ToListAsync();

        return Ok(assets);
    }

    [HttpPost("api/admin/lectures/{lectureId}/assets")]
    public async Task<IActionResult> Upload(int lectureId, IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

        var lecture = await _db.Lectures.FindAsync(lectureId);
        if (lecture == null) return NotFound();

        // 🔥 Seedha Cloudinary par upload (Folder name dynamically banaya)
        var fileUrl = await _cloudinaryService.UploadFileAsync(file, $"lectures/lecture-{lectureId}");

        if (string.IsNullOrEmpty(fileUrl)) return StatusCode(500, "Failed to upload to Cloudinary.");

        var asset = new Asset
        {
            FileName = file.FileName,
            FileUrl = fileUrl, // Ab yahan /files/lectures... nahi, Cloudinary ka link aayega
            LectureId = lectureId
        };

        _db.Assets.Add(asset);
        await _db.SaveChangesAsync();

        return Ok(asset.Id);
    }

    // ================= PROJECT VIDEO ASSETS =================

    [HttpGet("api/admin/project-videos/{videoId}/assets")]
    public async Task<IActionResult> GetProjectVideoAssets(int videoId)
    {
        var assets = await _db.Assets
            .Where(a => a.ProjectVideoId == videoId)
            .ToListAsync();

        return Ok(assets);
    }

    [HttpPost("api/admin/project-videos/{videoId}/assets")]
    public async Task<IActionResult> UploadProjectVideoAsset(int videoId, IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

        var video = await _db.ProjectVideos.FindAsync(videoId);
        if (video == null) return NotFound();

        // 🔥 Seedha Cloudinary par upload
        var fileUrl = await _cloudinaryService.UploadFileAsync(file, $"projects/video-{videoId}");

        if (string.IsNullOrEmpty(fileUrl)) return StatusCode(500, "Failed to upload to Cloudinary.");

        var asset = new Asset
        {
            FileName = file.FileName,
            FileUrl = fileUrl,
            ProjectVideoId = videoId
        };

        _db.Assets.Add(asset);
        await _db.SaveChangesAsync();

        return Ok(asset.Id);
    }

    // ================= DELETE =================

    [HttpDelete("api/admin/assets/{assetId}")]
    public async Task<IActionResult> Delete(int assetId)
    {
        var asset = await _db.Assets.FindAsync(assetId);
        if (asset == null) return NotFound();

        // 🔥 Asset database se udane se pehle Cloudinary se uda do
        if (!string.IsNullOrEmpty(asset.FileUrl))
        {
            await _cloudinaryService.DeleteFileAsync(asset.FileUrl);
        }

        _db.Assets.Remove(asset);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}