using System.ComponentModel.DataAnnotations;

namespace Rafiq.Api.DTOs;

public class UpdateAppointmentStatusDto
{
    [Required]
    [RegularExpression("^(Pending|Confirmed|Cancelled|Completed)$", ErrorMessage = "Status must be one of: Pending, Confirmed, Cancelled, Completed")]
    public string Status { get; set; } = string.Empty;
}
