using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioCourses.Api.Data;
using PortfolioCourses.Api.DTOs.Admin;
using PortfolioCourses.Api.Models;
using PortfolioCourses.Api.Services; // 🔥 NAYA: CloudinaryService ke liye
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioCourses.Api.Controllers.Admin;

[Authorize]
[ApiController]
[Route("api/admin/projects/{projectId}/videos")]
public class AdminProjectVideosController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly CloudinaryService _cloudinaryService; // 🔥 NAYA: Service add ki

    public AdminProjectVideosController(ApplicationDbContext db, CloudinaryService cloudinaryService)
    {
        _db = db;
        _cloudinaryService = cloudinaryService;
    }

    // CREATE VIDEO
    [HttpPost]
    public async Task<IActionResult> Create(int projectId, ProjectVideoCreateDto dto)
    {
        var project = await _db.Projects.FindAsync(projectId);
        if (project == null) return NotFound();

        var video = new ProjectVideo
        {
            Title = dto.Title,
            YouTubeVideoId = dto.YouTubeVideoId,
            TheoryContent = dto.TheoryContent,
            Duration = dto.Duration ?? "0:00",               // 🔥 SAVING METADATA
            ChannelName = dto.ChannelName ?? "Unknown",      // 🔥 SAVING METADATA
            ProjectId = projectId
        };

        _db.ProjectVideos.Add(video);
        await _db.SaveChangesAsync();
        Console.WriteLine($"Created video with ID: {video.Id} for project ID: {projectId}");
        return Ok(video.Id);
    }

    // UPDATE VIDEO
    [HttpPut("/api/admin/project-videos/{videoId}")]
    public async Task<IActionResult> Update(int videoId, ProjectVideoUpdateDto dto)
    {
        var video = await _db.ProjectVideos.FindAsync(videoId);
        if (video == null) return NotFound();

        video.Title = dto.Title;
        video.YouTubeVideoId = dto.YouTubeVideoId;
        video.TheoryContent = dto.TheoryContent;

        // Allow updating metadata if provided
        if (!string.IsNullOrEmpty(dto.Duration)) video.Duration = dto.Duration;
        if (!string.IsNullOrEmpty(dto.ChannelName)) video.ChannelName = dto.ChannelName;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE VIDEO
    [HttpDelete("/api/admin/project-videos/{videoId}")]
    public async Task<IActionResult> Delete(int videoId)
    {
        var video = await _db.ProjectVideos.FindAsync(videoId);
        if (video == null) return NotFound();

        // 🔥 THE FIX: Video delete karne se pehle, is video se jude saare assets Cloudinary se uda do
        var attachedAssets = await _db.Assets.Where(a => a.ProjectVideoId == videoId).ToListAsync();

        foreach (var asset in attachedAssets)
        {
            if (!string.IsNullOrEmpty(asset.FileUrl))
            {
                await _cloudinaryService.DeleteFileAsync(asset.FileUrl);
            }
        }

        // Cloudinary se files clear hone ke baad database se video uda do
        _db.ProjectVideos.Remove(video);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}