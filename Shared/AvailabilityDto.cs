using System.ComponentModel.DataAnnotations;

namespace Rafiq.Api.DTOs;

public class AvailabilityDto
{

    public DateOnly SpecificDate { get; set; }

    [Required]
    public TimeSpan FromTime { get; set; }

    [Required]
    public TimeSpan ToTime { get; set; }
}
