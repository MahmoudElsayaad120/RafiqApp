using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Data;


namespace Rafiqeq.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Doctor")]
public class AvailabilityController : ControllerBase
{
  
    private readonly IAvailabilityService _availabilityService;
    private readonly ILogger<AvailabilityController> _logger;

    public AvailabilityController(IAvailabilityService availabilityService, ILogger<AvailabilityController> logger)
    {
        _availabilityService = availabilityService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<AvailabilityDto>> AddAvailability([FromBody] AvailabilityDto availabilityDto)
    {
        try
        {
            var doctorId = await GetDoctorIdFromUserIdAsync();
            availabilityDto.DoctorId = doctorId;
            var result = await _availabilityService.AddAvailabilityAsync(doctorId, availabilityDto);
            return CreatedAtAction(nameof(GetAvailabilities), new { }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding availability");
            return StatusCode(500, new { message = "An error occurred while adding availability" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AvailabilityDto>>> GetAvailabilities()
    {
        try
        {
            var doctorId = await GetDoctorIdFromUserIdAsync();
            var availabilities = await _availabilityService.GetDoctorAvailabilitiesAsync(doctorId);
            return Ok(availabilities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching availabilities");
            return StatusCode(500, new { message = "An error occurred while fetching availabilities" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAvailability(int id)
    {
        try
        {
            var doctorId = await GetDoctorIdFromUserIdAsync();
            var success = await _availabilityService.DeleteAvailabilityAsync(id, doctorId);
            if (!success)
                return NotFound(new { message = "Availability not found" });

            return Ok(new { message = "Availability deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting availability");
            return StatusCode(500, new { message = "An error occurred while deleting availability" });
        }
    }

    private async Task<int> GetDoctorIdFromUserIdAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user ID");

        using var scope = HttpContext.RequestServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RafiqDbContext>();
        var doctor = await context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
        if (doctor == null)
            throw new UnauthorizedAccessException("Doctor profile not found");
        return doctor.Id;
    }

    
}
