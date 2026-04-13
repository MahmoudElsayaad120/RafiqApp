using Microsoft.AspNetCore.Mvc;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Shared.OrderModels;
using Rafiq.Api.Services.Abstractions;


namespace Rafiq.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }


    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginAsync(loginDto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var result = await _authService.RegisterAsync(registerDto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    [HttpGet("{email}")]
    public async Task<IActionResult> CheckEmailExists(string email) 
    {
      var result = await _authService.CheckEmailExistsAsync(email);
        return Ok(result);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var result = await _authService.GetCurrentUserAsync(email);
        return Ok(result);
    }

    [HttpGet("Address")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUserAddress()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var result = await _authService.GetCurrentUserAddressAsync(email);
        return Ok(result);
    }

    [HttpPut("Address")]
    [Authorize]
    public async Task<IActionResult> UpdateCurrentUserAddress(addressDto addressDto)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var result = await _authService.UpdateCurrentUserAddressAsync(addressDto, email);
        return Ok(result);
    }


    private ActionResult<AuthResponseDto> Unauthorized(object result)
    {
        throw new NotImplementedException();
    }
}
