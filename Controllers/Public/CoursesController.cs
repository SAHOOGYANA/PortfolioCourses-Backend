using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioCourses.Api.Data;
using PortfolioCourses.Api.DTOs.Public;

namespace PortfolioCourses.Api.Controllers.Public;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public CoursesController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetCourses()
    {
        var courses = await _db.Courses
            .Where(c => c.IsPublished)
            .Select(c => new CourseListDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                InstructorName = c.InstructorName, // 🔥 Added for public cards
                ImageUrl = c.ImageUrl              // 🔥 Added for public cards
            })
            .ToListAsync();

        return Ok(courses);
    }

    [HttpGet("{courseId}")]
    public async Task<IActionResult> GetCourse(int courseId)
    {
        var course = await _db.Courses
            .Where(c => c.Id == courseId && c.IsPublished)
            .Select(c => new CourseDetailDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                InstructorName = c.InstructorName, // 🔥 Added for course landing page
                ImageUrl = c.ImageUrl,             // 🔥 Added for course landing page
                Sections = c.Sections.Select(s => new SectionDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Lectures = s.Lectures.Select(l => new LectureDto
                    {
                        Id = l.Id,
                        Title = l.Title,
                        YouTubeVideoId = l.YouTubeVideoId,
                        TheoryContent = l.TheoryContent
                    }).ToList()
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (course == null) return NotFound();

        return Ok(course);
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("API is alive");
    }
}