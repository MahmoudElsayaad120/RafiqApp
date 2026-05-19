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
    //private readonly IDoctorService _doctorService;
    //private readonly ILogger<ChatController> _logger;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
        //_doctorService = doctorService;
        //_logger = logger;
    }


    [HttpPost("start")]
    public async Task<IActionResult> StartChat()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var sessionId = await _chatService.StartNewChatAsync(userId);
        if (sessionId == null) return BadRequest(new { message = "??? ??? ???????? ?? ????" });
        return Ok(new { session_id = sessionId });
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var history = await _chatService.GetChatHistoryAsync(userId);
        return Ok(history);
    }

    [HttpDelete("end")]
    public async Task<IActionResult> EndChat()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _chatService.EndChatAsync(userId);
        if (!result) return BadRequest(new { message = "??? ?? ????? ????????" });
        return Ok(new { message = "?? ????? ???????? ???? ???? ???????? ?????" });
    }


}
