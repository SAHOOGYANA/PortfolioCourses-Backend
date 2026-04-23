using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioCourses.Api.Data;
using PortfolioCourses.Api.DTOs.Admin;
using PortfolioCourses.Api.Models;
using PortfolioCourses.Api.Services; // 🔥 CloudinaryService namespace
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioCourses.Api.Controllers.Admin;

[Authorize]
[ApiController]
[Route("api/admin/lectures")]
public class AdminLecturesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly CloudinaryService _cloudinaryService; // 🔥 Service add ki

    public AdminLecturesController(ApplicationDbContext db, CloudinaryService cloudinaryService)
    {
        _db = db;
        _cloudinaryService = cloudinaryService;
    }

    // CREATE (needs sectionId)
    [HttpPost("/api/admin/sections/{sectionId}/lectures")]
    public async Task<IActionResult> Create(int sectionId, LectureCreateDto dto)
    {
        var section = await _db.Sections.FindAsync(sectionId);
        if (section == null) return NotFound();

        var lecture = new Lecture
        {
            Title = dto.Title,
            YouTubeVideoId = dto.YouTubeVideoId,
            SectionId = sectionId,
            TheoryContent = dto.TheoryContent
        };

        _db.Lectures.Add(lecture);
        await _db.SaveChangesAsync();

        return Ok(lecture.Id);
    }

    // UPDATE
    [HttpPut("{lectureId}")]
    public async Task<IActionResult> Update(int lectureId, LectureUpdateDto dto)
    {
        var lecture = await _db.Lectures.FindAsync(lectureId);
        if (lecture == null) return NotFound();

        lecture.Title = dto.Title;
        lecture.YouTubeVideoId = dto.YouTubeVideoId;
        lecture.TheoryContent = dto.TheoryContent;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE
    [HttpDelete("{lectureId}")]
    public async Task<IActionResult> Delete(int lectureId)
    {
        var lecture = await _db.Lectures.FindAsync(lectureId);
        if (lecture == null) return NotFound();

        // 🔥 THE FIX: Lecture delete karne se pehle, uske saare assets dhoondo aur Cloudinary se uda do
        var attachedAssets = await _db.Assets.Where(a => a.LectureId == lectureId).ToListAsync();

        foreach (var asset in attachedAssets)
        {
            if (!string.IsNullOrEmpty(asset.FileUrl))
            {
                await _cloudinaryService.DeleteFileAsync(asset.FileUrl);
            }
        }

        // Ab safe hai. Entity Framework DB se udadega.
        _db.Lectures.Remove(lecture);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}