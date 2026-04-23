using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioCourses.Api.Data;

namespace PortfolioCourses.Api.Controllers.Public;

[ApiController]
[Route("api/lectures")]
public class LectureAssetsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public LectureAssetsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet("{lectureId}/assets")]
    public async Task<IActionResult> GetAssets(int lectureId)
    {
        var assets = await _db.Assets
            .Where(a => a.LectureId == lectureId)
            .Select(a => new
            {
                a.Id,
                a.FileName,
                a.FileUrl
            })
            .ToListAsync();

        return Ok(assets);
    }
}
