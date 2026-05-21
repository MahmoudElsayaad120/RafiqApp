using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services;
using Rafiq.Api.Services.Abstractions;
using Shared;
namespace Rafiq.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Patient")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
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

    [HttpPost("message")]
    public async Task<IActionResult> SaveMessage([FromBody] ChatMessageDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // ??????? ????? ?????? ????? ??? ???? CreatedAt ????? ??? ?? ?????? ?? ??????? ??? ????? ?????
        var result = await _chatService.SaveMessageAsync(userId, dto.Sender, dto.MessageText);

        if (!result) return BadRequest(new { message = "??? ??? ???????" });
        return Ok(new { message = "?? ??? ??????? ?????" });
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
