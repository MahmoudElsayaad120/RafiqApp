using Rafeeq.Api.DTOs;

namespace Rafeeq.Api.Services;

public interface IChatService
{
    Task<ChatResponseDto> ProcessMessageAsync(int userId, string message);
    Task SaveChatMessageAsync(int userId, string message, string sender);
}
