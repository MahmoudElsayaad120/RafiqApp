using System.ComponentModel.DataAnnotations;

namespace Rafiq.Api.DTOs;

public class CreateAppointmentDto
{
    [Required]
    public int DoctorId { get; set; }

    [Required]
    public DateTime Date { get; set; }
}
