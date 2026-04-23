using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioCourses.Api.Data;
using System;
// Niche wali line ko apne project ke folder structure ke hisab se theek kar lena jahan tera AppDbContext hai
// using PortfolioCourses.Api.Data; 

namespace PortfolioCourses.Api.Controllers.Public
{
    [ApiController]
    [Route("api/public/[controller]")]
    public class SiteAssetController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Constructor mein Database context inject kar rahe hain
        public SiteAssetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ye endpoint call hoga: GET /api/public/siteasset/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAssets()
        {
            // Database se saare assets utha lo
            var assets = await _context.SiteAssets.ToListAsync();

            // Frontend ko sirf kaam ki cheezein (Key aur Url) bhejenge
            var result = assets.Select(a => new {
                key = a.Key,
                url = a.FileUrl
            });

            return Ok(result); // Ye automatic JSON format mein response bhej dega
        }
    }
}