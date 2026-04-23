using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioCourses.Api.Data;
using PortfolioCourses.Api.DTOs.Admin;
using PortfolioCourses.Api.Models;
using PortfolioCourses.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioCourses.Api.Controllers.Admin;

[Authorize]
[ApiController]
[Route("api/admin/courses")]
public class AdminCoursesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly CloudinaryService _cloudinaryService;

    public AdminCoursesController(ApplicationDbContext db, IWebHostEnvironment env, CloudinaryService cloudinaryService)
    {
        _db = db;
        _env = env;
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CourseCreateDto dto)
    {
        var course = new Course
        {
            Title = dto.Title,
            // ✅ Fallback to empty string if null
            Description = dto.Description ?? "",
            // ✅ Fallback to default if null
            InstructorName = string.IsNullOrWhiteSpace(dto.InstructorName) ? "Harshit Vyas" : dto.InstructorName,
            IsPublished = false
        };

        if (dto.ImageFile != null && dto.ImageFile.Length > 0)
        {
            // 🔥 Cloudinary Upload for Course Image
            var fileUrl = await _cloudinaryService.UploadFileAsync(dto.ImageFile, "courses");

            if (!string.IsNullOrEmpty(fileUrl))
            {
                course.ImageUrl = fileUrl;
            }
        }

        _db.Courses.Add(course);
        await _db.SaveChangesAsync();
        return Ok(course.Id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] CourseUpdateDto dto)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course == null) return NotFound();

        course.Title = dto.Title;
        // ✅ Ensure no NULL values hit the DB
        course.Description = dto.Description ?? "";
        course.InstructorName = string.IsNullOrWhiteSpace(dto.InstructorName) ? "Harshit Vyas" : dto.InstructorName;
        course.IsPublished = dto.IsPublished;

        if (dto.ImageFile != null && dto.ImageFile.Length > 0)
        {
            // 🔥 Purani image Cloudinary se delete karo
            if (!string.IsNullOrEmpty(course.ImageUrl))
            {
                await _cloudinaryService.DeleteFileAsync(course.ImageUrl);
            }

            // 🔥 Nayi image Cloudinary par upload karo
            var fileUrl = await _cloudinaryService.UploadFileAsync(dto.ImageFile, "courses");

            if (!string.IsNullOrEmpty(fileUrl))
            {
                course.ImageUrl = fileUrl;
            }
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course == null) return NotFound();

        // 🔥 Course delete hone se pehle uski image Cloudinary se hatao
        if (!string.IsNullOrEmpty(course.ImageUrl))
        {
            await _cloudinaryService.DeleteFileAsync(course.ImageUrl);
        }

        _db.Courses.Remove(course);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.Courses.ToListAsync());

    [HttpGet("{courseId}/structure")]
    public async Task<IActionResult> GetStructure(int courseId)
    {
        var course = await _db.Courses
            .Include(c => c.Sections).ThenInclude(s => s.Lectures)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null) return NotFound();

        return Ok(new
        {
            course.Id,
            course.Title,
            course.Description,
            course.InstructorName,
            course.ImageUrl,
            course.IsPublished,
            sections = course.Sections.Select(s => new
            {
                s.Id,
                s.Title,
                lectures = s.Lectures.Select(l => new { l.Id, l.Title, l.YouTubeVideoId, l.TheoryContent })
            })
        });
    }

    [HttpPatch("{id}/toggle-publish")]
    public async Task<IActionResult> TogglePublish(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course == null) return NotFound();
        course.IsPublished = !course.IsPublished;
        await _db.SaveChangesAsync();
        return Ok(new { course.Id, course.IsPublished });
    }
}