using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        // Demo User
        if (request.UserName == "admin" &&
            request.Password == "password")
        {
            var token =
                _tokenService.GenerateToken(
                    request.UserName);

            return Ok(new
            {
                Token = token
            });
        }

        return Unauthorized(
            new
            {
                Message = "Invalid credentials"
            });
    }
}