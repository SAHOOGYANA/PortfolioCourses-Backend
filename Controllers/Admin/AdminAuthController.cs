using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioCourses.Api.Data;
using PortfolioCourses.Api.DTOs.Auth;
using BCrypt.Net;
using PortfolioCourses.Api.Services;

namespace PortfolioCourses.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/auth")]
public class AdminAuthController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly JwtTokenService _jwtService;

    public AdminAuthController(ApplicationDbContext db, JwtTokenService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(AdminLoginRequest request)
    {
        var admin = await _db.AdminUsers
            .FirstOrDefaultAsync(a => a.Email == request.Email);

        if (admin == null ||
            !BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
        {
            return Unauthorized("Invalid credentials");
        }

        var token = _jwtService.GenerateToken(admin);

        return Ok(new AdminLoginResponse { Token = token });
    }
}
