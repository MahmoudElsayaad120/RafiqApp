using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rafeeq.Api.DTOs;
using Rafeeq.Api.Services;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Microsoft.Extensions.DependencyInjection;



namespace Rafeeq.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(IAppointmentService appointmentService, ILogger<AppointmentsController> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Patient")]
    public async Task<ActionResult<AppointmentDto>> BookAppointment([FromBody] CreateAppointmentDto createAppointmentDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var patientId = await GetPatientIdFromUserIdAsync(userId);
            var appointment = await _appointmentService.BookAppointmentAsync(patientId, createAppointmentDto);
            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment");
            return StatusCode(500, new { message = "An error occurred while booking appointment" });
        }
    }

    [HttpGet("my-appointments")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetMyAppointments()
    {
        try
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            IEnumerable<AppointmentDto> appointments;

            if (role == "Patient")
            {
                var patientId = await GetPatientIdFromUserIdAsync(userId);
                appointments = await _appointmentService.GetPatientAppointmentsAsync(patientId);
            }
            else if (role == "Doctor")
            {
                var doctorId = await GetDoctorIdFromUserIdAsync(userId);
                appointments = await _appointmentService.GetDoctorAppointmentsAsync(doctorId);
            }
            else
            {
                return Forbid();
            }

            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching appointments");
            return StatusCode(500, new { message = "An error occurred while fetching appointments" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            IEnumerable<AppointmentDto> appointments;

            if (role == "Patient")
            {
                var patientId = await GetPatientIdFromUserIdAsync(userId);
                appointments = await _appointmentService.GetPatientAppointmentsAsync(patientId);
            }
            else if (role == "Doctor")
            {
                var doctorId = await GetDoctorIdFromUserIdAsync(userId);
                appointments = await _appointmentService.GetDoctorAppointmentsAsync(doctorId);
            }
            else
            {
                return Forbid();
            }

            var appointment = appointments.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
                return NotFound(new { message = "Appointment not found" });

            return Ok(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching appointment");
            return StatusCode(500, new { message = "An error occurred while fetching appointment" });
        }
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Doctor")]
    public async Task<ActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateAppointmentStatusDto dto)
    {
        try
        {
            var success = await _appointmentService.UpdateAppointmentStatusAsync(id, dto.Status);
            if (!success)
                return NotFound(new { message = "Appointment not found" });

            return Ok(new { message = "Appointment status updated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating appointment status");
            return StatusCode(500, new { message = "An error occurred while updating appointment status" });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out var userId))
            return userId;
        throw new UnauthorizedAccessException("Invalid user ID");
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }

    private async Task<int> GetPatientIdFromUserIdAsync(int userId)
    {
        using var scope = HttpContext.RequestServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RafiqDbContext>();
        var patient = await context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        if (patient == null)
            throw new UnauthorizedAccessException("Patient profile not found");
        return patient.Id;
    }

    private async Task<int> GetDoctorIdFromUserIdAsync(int userId)
    {
        using var scope = HttpContext.RequestServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RafiqDbContext>();
        var doctor = await context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
        if (doctor == null)
            throw new UnauthorizedAccessException("Doctor profile not found");
        return doctor.Id;
    }
}
