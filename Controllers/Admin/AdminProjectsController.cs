using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioCourses.Api.Data;
using PortfolioCourses.Api.DTOs.Admin;
using PortfolioCourses.Api.Models;
using PortfolioCourses.Api.Services; // 🔥 CloudinaryService ke liye
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioCourses.Api.Controllers.Admin;

[Authorize]
[ApiController]
[Route("api/admin/projects")]
public class AdminProjectsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly CloudinaryService _cloudinaryService; // 🔥 Local Environment ki jagah Cloudinary

    public AdminProjectsController(ApplicationDbContext db, CloudinaryService cloudinaryService)
    {
        _db = db;
        _cloudinaryService = cloudinaryService;
    }

    // CREATE PROJECT 
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ProjectCreateDto dto)
    {
        var project = new Project
        {
            Title = dto.Title,
            Description = dto.Description ?? "",
            InstructorName = string.IsNullOrWhiteSpace(dto.InstructorName) ? "Harshit Vyas" : dto.InstructorName
        };

        if (dto.ImageFile != null && dto.ImageFile.Length > 0)
        {
            // 🔥 Upload to Cloudinary
            var fileUrl = await _cloudinaryService.UploadFileAsync(dto.ImageFile, "projects");
            if (!string.IsNullOrEmpty(fileUrl))
            {
                project.ImageUrl = fileUrl;
            }
        }

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        return Ok(project.Id);
    }

    // UPDATE PROJECT
    [HttpPut("{projectId}")]
    public async Task<IActionResult> Update(int projectId, [FromForm] ProjectUpdateDto dto)
    {
        var project = await _db.Projects.FindAsync(projectId);
        if (project == null) return NotFound();

        project.Title = dto.Title;
        project.Description = dto.Description ?? "";
        project.InstructorName = string.IsNullOrWhiteSpace(dto.InstructorName) ? "Harshit Vyas" : dto.InstructorName;

        if (dto.ImageFile != null && dto.ImageFile.Length > 0)
        {
            // 🔥 Purani image Cloudinary se hatao
            if (!string.IsNullOrEmpty(project.ImageUrl))
            {
                await _cloudinaryService.DeleteFileAsync(project.ImageUrl);
            }

            // 🔥 Nayi image Cloudinary par daalo
            var fileUrl = await _cloudinaryService.UploadFileAsync(dto.ImageFile, "projects");
            if (!string.IsNullOrEmpty(fileUrl))
            {
                project.ImageUrl = fileUrl;
            }
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var projects = await _db.Projects
            .Include(p => p.Videos)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Description,
                p.InstructorName,
                p.ImageUrl,
                videos = p.Videos.Select(v => new
                {
                    v.Id,
                    v.Title,
                    v.YouTubeVideoId,
                    v.TheoryContent,
                    v.Duration,
                    v.ChannelName
                })
            })
            .ToListAsync();

        return Ok(projects);
    }

    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetById(int projectId)
    {
        var project = await _db.Projects
            .Include(p => p.Videos)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) return NotFound();

        return Ok(new
        {
            project.Id,
            project.Title,
            project.Description,
            project.InstructorName,
            project.ImageUrl,
            videos = project.Videos.Select(v => new
            {
                v.Id,
                v.Title,
                v.YouTubeVideoId,
                v.TheoryContent,
                v.Duration,
                v.ChannelName
            })
        });
    }
    [HttpPatch("{id}/toggle-publish")]
    public async Task<IActionResult> TogglePublish(int id)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project == null) return NotFound("Project not found");

        // 🔥 Value flip kardo (True ko False, False ko True)
        project.IsPublished = !project.IsPublished;

        await _db.SaveChangesAsync();

        return Ok(new { isPublished = project.IsPublished });
    }
    [HttpDelete("{projectId}")]
    public async Task<IActionResult> Delete(int projectId)
    {
        // 🔥 Get project along with its videos so we can clean up their assets too
        var project = await _db.Projects
            .Include(p => p.Videos)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) return NotFound();

        // 1. Project ki main image Cloudinary se uda do
        if (!string.IsNullOrEmpty(project.ImageUrl))
        {
            await _cloudinaryService.DeleteFileAsync(project.ImageUrl);
        }

        // 2. Project ke andar jitne bhi videos the, unke "Assets" dhoondo aur Cloudinary se uda do
        var videoIds = project.Videos.Select(v => v.Id).ToList();
        if (videoIds.Any())
        {
            var attachedAssets = await _db.Assets
                .Where(a => a.ProjectVideoId != null && videoIds.Contains(a.ProjectVideoId.Value))
                .ToListAsync();

            foreach (var asset in attachedAssets)
            {
                if (!string.IsNullOrEmpty(asset.FileUrl))
                {
                    await _cloudinaryService.DeleteFileAsync(asset.FileUrl);
                }
            }
        }

        // 3. Database se project delete kardo (EF Core cascade delete se ProjectVideos aur Assets ko DB se nikal dega)
        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}