using System.Security.Claims;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services;
using Rafiq.Api.Services.Abstractions;
using Shared;


namespace Rafiq.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly UserManager<AppUser> _userManager;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger,
        UserManager<AppUser> userManager)
    {
        _authService = authService;
        _logger = logger;
        _userManager = userManager;
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


    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpDto sendOtpDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.SendOtpAsync(sendOtpDto);
        if (!result)
            return BadRequest(new { message = "??? ????? ?????? ???? ?????? ?? ?????? ??????????" });

        return Ok(new { message = "?? ????? ??? ?????? ?????" });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var isValid = await _authService.VerifyOtpAsync(verifyOtpDto);
        if (!isValid)
            return BadRequest(new { message = "The code you entered is incorrect or expired." }); // ??????? ?? ??? UI ??????

        return Ok(new { message = "The code was successfully verified." });
    }

    private ActionResult<AuthResponseDto> Unauthorized(object result)
    {
        throw new NotImplementedException();
    }
}
