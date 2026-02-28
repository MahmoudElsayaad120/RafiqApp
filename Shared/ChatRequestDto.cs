using System.ComponentModel.DataAnnotations;

namespace Rafeeq.Api.DTOs;

public class ChatRequestDto
{
    [Required]
    [StringLength(2000)]
    public string Message { get; set; } = string.Empty;
}
