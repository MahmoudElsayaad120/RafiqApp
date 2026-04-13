using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services;
using Microsoft.Extensions.Logging;


namespace Rafiq.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<DoctorsController> _logger;

    public DoctorsController(IDoctorService doctorService, ILogger<DoctorsController> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAllDoctors([FromQuery] string? specialization,int pageIndex =1,int pageSize =5)
    {
        try
        {
            var doctors = await _doctorService.GetAllDoctorsAsync(specialization,pageIndex,pageSize);
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching doctors");
            return StatusCode(500, new { message = "An error occurred while fetching doctors" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorDto>> GetDoctor(int id)
    {
        try
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found" });

            return Ok(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching doctor");
            return StatusCode(500, new { message = "An error occurred while fetching doctor" });
        }
    }
}
