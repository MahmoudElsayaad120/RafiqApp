using Rafiq.Api.DTOs;

namespace Rafiq.Api.Services.Abstractions;

public interface IChatService
{
    Task<ChatResponseDto> ProcessMessageAsync(int userId, string message);
    Task SaveChatMessageAsync(int userId, string message, string sender);
}
