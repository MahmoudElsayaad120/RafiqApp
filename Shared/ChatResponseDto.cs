namespace Rafeeq.Api.DTOs;

public class ChatResponseDto
{
    public string Reply { get; set; } = string.Empty;
    public string RecommendedSpecialization { get; set; } = string.Empty;
    public string UrgencyLevel { get; set; } = string.Empty; // low, medium, high
}
