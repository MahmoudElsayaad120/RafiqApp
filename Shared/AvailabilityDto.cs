using System.ComponentModel.DataAnnotations;

namespace Rafeeq.Api.DTOs;

public class AvailabilityDto
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    
    [Required]
    [Range(0, 6)]
    public DayOfWeek DayOfWeek { get; set; }
    
    [Required]
    public TimeSpan FromTime { get; set; }
    
    [Required]
    public TimeSpan ToTime { get; set; }
}
