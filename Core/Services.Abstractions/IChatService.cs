using Rafiq.Api.DTOs;
using Shared;

namespace Rafiq.Api.Services.Abstractions;

public interface IChatService
{
    Task<string> StartNewChatAsync(string identityUserId);
    Task<List<ChatMessageDto>> GetChatHistoryAsync(string identityUserId);
    Task<bool> SaveMessageAsync(string identityUserId, string sender, string messageText);
    Task<bool> EndChatAsync(string identityUserId);
}
