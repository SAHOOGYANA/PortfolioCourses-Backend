using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioCourses.Api.Data;
using PortfolioCourses.Api.DTOs.Admin;
using PortfolioCourses.Api.Models;
using PortfolioCourses.Api.Services; // 🔥 NAYA: CloudinaryService ke liye
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioCourses.Api.Controllers.Admin;

[Authorize]
[ApiController]
public class AdminSectionsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly CloudinaryService _cloudinaryService; // 🔥 NAYA: Service add ki

    public AdminSectionsController(ApplicationDbContext db, CloudinaryService cloudinaryService)
    {
        _db = db;
        _cloudinaryService = cloudinaryService;
    }

    // CREATE (needs courseId)
    [HttpPost("api/admin/courses/{courseId}/sections")]
    public async Task<IActionResult> Create(int courseId, SectionCreateDto dto)
    {
        var course = await _db.Courses.FindAsync(courseId);
        if (course == null) return NotFound();

        var section = new Section
        {
            Title = dto.Title,
            CourseId = courseId
        };

        _db.Sections.Add(section);
        await _db.SaveChangesAsync();

        return Ok(section.Id);
    }

    // UPDATE (sectionId only)
    [HttpPut("api/admin/sections/{sectionId}")]
    public async Task<IActionResult> Update(int sectionId, SectionUpdateDto dto)
    {
        var section = await _db.Sections.FindAsync(sectionId);
        if (section == null) return NotFound();

        section.Title = dto.Title;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // DELETE (sectionId only)
    [HttpDelete("api/admin/sections/{sectionId}")]
    public async Task<IActionResult> Delete(int sectionId)
    {
        // 🔥 THE FIX: Section ke saath uske Lectures bhi fetch karo
        var section = await _db.Sections
            .Include(s => s.Lectures)
            .FirstOrDefaultAsync(s => s.Id == sectionId);

        if (section == null) return NotFound();

        // 1. Is section ke andar aane wale saare Lectures ki ID nikaalo
        var lectureIds = section.Lectures.Select(l => l.Id).ToList();

        if (lectureIds.Any())
        {
            // 2. Un sabhi Lectures ke attached Assets dhoondo
            var attachedAssets = await _db.Assets
                .Where(a => a.LectureId != null && lectureIds.Contains(a.LectureId.Value))
                .ToListAsync();

            // 3. Ek-ek karke Cloudinary se uda do
            foreach (var asset in attachedAssets)
            {
                if (!string.IsNullOrEmpty(asset.FileUrl))
                {
                    await _cloudinaryService.DeleteFileAsync(asset.FileUrl);
                }
            }
        }

        // 4. Cloudinary saaf hone ke baad database se Section uda do 
        // (EF Core apne aap section ke lectures aur unke assets DB se uda dega)
        _db.Sections.Remove(section);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}