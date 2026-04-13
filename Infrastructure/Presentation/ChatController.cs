using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services;
using Microsoft.Extensions.Logging;
using Rafiq.Api.Services.Abstractions;
namespace Rafiq.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Patient")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IDoctorService _doctorService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IChatService chatService, IDoctorService doctorService, ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _doctorService = doctorService;
        _logger = logger;
    }

    [HttpPost("message")]
    public async Task<ActionResult<ChatResponseDto>> SendMessage([FromBody] ChatRequestDto chatRequestDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _chatService.ProcessMessageAsync(userId, chatRequestDto.Message);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat message");
            return StatusCode(500, new { message = "An error occurred while processing your message" });
        }
    }

    [HttpGet("recommended-doctors")]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> GetRecommendedDoctors([FromQuery] string specialization)
    {
        try
        {
            if (string.IsNullOrEmpty(specialization))
                return BadRequest(new { message = "Specialization is required" });

            var doctors = await _doctorService.GetAllDoctorsAsync(specialization);
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recommended doctors");
            return StatusCode(500, new { message = "An error occurred while fetching doctors" });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out var userId))
            return userId;
        throw new UnauthorizedAccessException("Invalid user ID");
    }
}
