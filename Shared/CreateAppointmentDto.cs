using System.ComponentModel.DataAnnotations;

namespace Rafeeq.Api.DTOs;

public class CreateAppointmentDto
{
    [Required]
    public int DoctorId { get; set; }

    [Required]
    public DateTime Date { get; set; }
}
