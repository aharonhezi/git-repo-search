using GitHubSearchApi.Middleware;
using GitHubSearchApi.Models;
using GitHubSearchApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GitHubSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly SessionStore _sessionStore;
    private readonly AuthenticationService _authenticationService;

    public AuthController(
        JwtTokenService jwtTokenService, 
        SessionStore sessionStore,
        AuthenticationService authenticationService)
    {
        _jwtTokenService = jwtTokenService;
        _sessionStore = sessionStore;
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return BadRequest(new { message = "Username is required" });
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Password is required" });
        }

        if (!_authenticationService.ValidateCredentials(request.Username, request.Password))
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var token = _jwtTokenService.GenerateToken(request.Username);

        return Ok(new LoginResponseDto
        {
            Token = token,
            Username = request.Username
        });
    }
}

