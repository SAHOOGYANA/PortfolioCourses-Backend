using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioCourses.Api.Data;
using PortfolioCourses.Api.DTOs.Public;
using System.Linq; // Ye ensure karne ke liye ki .Select() theek se chale
using System.Threading.Tasks;

namespace PortfolioCourses.Api.Controllers.Public;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ProjectsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var projects = await _db.Projects
            .Where(p => p.IsPublished)
            .Select(p => new ProjectListDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                InstructorName = p.InstructorName,
                ImageUrl = p.ImageUrl
            })
            .ToListAsync();

        return Ok(projects);
    }

    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetProject(int projectId)
    {
        var project = await _db.Projects
            .Where(p => p.IsPublished)
            .Where(p => p.Id == projectId)
            .Select(p => new ProjectDetailDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                InstructorName = p.InstructorName,
                ImageUrl = p.ImageUrl,
                Videos = p.Videos.Select(v => new ProjectVideoDto
                {
                    Id = v.Id,
                    Title = v.Title,
                    YouTubeVideoId = v.YouTubeVideoId,
                    TheoryContent = v.TheoryContent,
                    Duration = v.Duration,
                    ChannelName = v.ChannelName,

                    // 🔥 THE FIX: Ye block add kiya taaki frontend ko Assets mil jayein
                    Assets = v.Assets.Select(a => new AssetDto
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        FileUrl = a.FileUrl,
                    }).ToList()
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (project == null) return NotFound();

        return Ok(project);
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("Projects API is alive");
    }
}