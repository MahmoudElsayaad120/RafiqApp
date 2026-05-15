using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services;
using Rafiq.Api.Services.Abstractions;
using Shared;



namespace Rafiq.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<AppointmentsController> _logger;

    public  AppointmentsController(IAppointmentService appointmentService, ILogger<AppointmentsController> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    //[HttpPost]
    //[Authorize(Roles = "Patient")]
    //public async Task<ActionResult<AppointmentDto>> BookAppointment([FromBody] CreateAppointmentDto createAppointmentDto)
    //{
    //    try
    //    {
    //        var userId = GetCurrentUserId();
    //        var patientId = await GetPatientIdFromUserIdAsync(userId);
    //        var appointment = await _appointmentService.BookAppointmentAsync(patientId, createAppointmentDto);
    //        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
    //    }
    //    catch (InvalidOperationException ex)
    //    {
    //        return BadRequest(new { message = ex.Message });
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error booking appointment");
    //        return StatusCode(500, new { message = "An error occurred while booking appointment" });
    //    }
    //}

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
    } // done 

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        // نادى الميثود بالـ ID بتاع الحجز بس
        var isCancelled = await _appointmentService.CancelAppointmentAsync(id);

        if (!isCancelled)
            return BadRequest(new { message = "فشل الإلغاء، تأكد من ID الحجز" });

        return Ok(new { message = "تم الإلغاء بنجاح" });
    }

    //[HttpGet("{id}")]
    //public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
    //{
    //    try
    //    {
    //        var userId = GetCurrentUserId();
    //        var role = GetCurrentUserRole();

    //        IEnumerable<AppointmentDto> appointments;

    //        if (role == "Patient")
    //        {
    //            var patientId = await GetPatientIdFromUserIdAsync(userId);
    //            appointments = await _appointmentService.GetPatientAppointmentsAsync(patientId);
    //        }
    //        else if (role == "Doctor")
    //        {
    //            var doctorId = await GetDoctorIdFromUserIdAsync(userId);
    //            appointments = await _appointmentService.GetDoctorAppointmentsAsync(doctorId);
    //        }
    //        else
    //        {
    //            return Forbid();
    //        }

    //        var appointment = appointments.FirstOrDefault(a => a.Id == id);
    //        if (appointment == null)
    //            return NotFound(new { message = "Appointment not found" });

    //        return Ok(appointment);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error fetching appointment");
    //        return StatusCode(500, new { message = "An error occurred while fetching appointment" });
    //    }
    //}

    //[HttpPut("{id}/status")]
    //[Authorize(Roles = "Doctor")]
    //public async Task<ActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateAppointmentStatusDto dto)
    //{
    //    try
    //    {
    //        var success = await _appointmentService.UpdateAppointmentStatusAsync(id, dto.Status);
    //        if (!success)
    //            return NotFound(new { message = "Appointment not found" });

    //        return Ok(new { message = "Appointment status updated successfully" });
    //    }
    //    catch (InvalidOperationException ex)
    //    {
    //        return BadRequest(new { message = ex.Message });
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error updating appointment status");
    //        return StatusCode(500, new { message = "An error occurred while updating appointment status" });
    //    }
    //}

    [HttpGet("patient-bookings")] 
    public async Task<ActionResult<IEnumerable<AppointmentForPatientDto>>> GetPatientBookings([FromQuery] string? status)
    {
        var patientId = await GetPatientIdFromUserIdAsync(GetCurrentUserId());

        try
        {
           

            // 2. مناداة الـ Service
            var appointments = await _appointmentService.GetPatientAppointmentsAsync(patientId, status);

            return Ok(appointments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "حدث خطأ غير متوقع" });
        }
    }


    [HttpGet("details/{appointmentId}")]
    public async Task<ActionResult<AppointmentForPatientDetailsDto>> GetAppointmentDetails(int appointmentId)
    {
        try
        {
            // بنمرر الـ appointmentId للـ Service
            var result = await _appointmentService.GetAppointmentDetailsAsync(appointmentId);

            if (result == null)
            {
                return NotFound(new { message = "عذراً، لم يتم العثور على تفاصيل هذا الحجز." });
            }

            return Ok(result);
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new
            {
                message = "حدث خطأ أثناء جلب بيانات الحجز.",
                details = ex.Message
            });
        }
    }


    //[HttpGet("{id}/available-UpdateSlots")]
    //public async Task<IActionResult> UpdateSlots(int id, [FromQuery] DateTime date)
    //{
    //    var slots = await _appointmentService.UpdateAvailableSlotsAsync(id, date);
    //    return Ok(slots);
    //}

    //[HttpPost("Update-book")]
    //public async Task<IActionResult> UpdateBookAppointment([FromBody] BookAppointmentRequestDto request)
    //{
    //    // المريض اللي داخل دلوقتي (مؤقتاً)
    //    var userId = GetCurrentUserId();
    //    var patientId = await GetPatientIdFromUserIdAsync(userId);


    //    var isSuccess = await _appointmentService.UpdateBookAppointmentAsync(patientId, request);

    //    if (isSuccess)
    //        return Ok(new { message = "تم طلب الحجز بنجاح" });

    //    return BadRequest(new { message = "حدث خطأ أثناء الحجز" });
    //}


    [HttpPut("Update-book")] // استخدمنا Put لأننا بنحدث بيانات موجودة
    public async Task<IActionResult> UpdateBookAppointment([FromBody] UpdateBookAppointmentRequestDto request)
    {
        // بنجيب الـ ID بتاع المريض من الـ Token بتاع الـ Auth
        var patientId = await GetPatientIdFromUserIdAsync(GetCurrentUserId());

        // بنطلب من الـ Service تنفذ عملية التعديل
        var isSuccess = await _appointmentService.UpdateBookAppointmentAsync(patientId, request);

        if (isSuccess)
            return Ok(new { message = "تم تحديث موعدك بنجاح" });

        return BadRequest(new { message = "فشل تحديث الموعد، تأكد من البيانات" });
    }


    [HttpPost("process-payment")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDto request)
    {
        try
        {
            // نداء الميثود اللي كتبناها في الـ Service
            var result = await _appointmentService.ProcessPaymentAsync(request);

            // نرجع استجابة ناجحة للـ React
            return Ok(new
            {
                message = request,
                status = "Success",
                appointmentId = request.AppointmentId
            });
        }
        catch (Exception ex)
        {
            // لو الـ Service رمت Error (زي إن الحجز مش موجود)
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("cancel/{AppointmentById}")] // استخدمنا HttpPut لأننا بنعدل حالة
    public async Task<IActionResult> CancelAppointmentById(int id)
    {
        var result = await _appointmentService.CancelAppointmentByIdAsync(id);

        if (!result)
        {
            return BadRequest(new { message = "حدث خطأ أثناء إلغاء الحجز أو الحجز غير موجود" });
        }

        // الرد اللي هيرجع للـ React عشان يظهر Modal "تم الإلغاء بنجاح"
        return Ok(new { message = "تم إلغاء الحجز بنجاح" });
    }


  


    private string? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //if (int.TryParse(userIdClaim, out var userId))
        return userIdClaim;
        //throw new UnauthorizedAccessException("Invalid user ID");
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }

    private async Task<int> GetPatientIdFromUserIdAsync(string userId)
    {
        using var scope = HttpContext.RequestServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RafiqDbContext>();
        var patient = await context.Patients.FirstOrDefaultAsync(p => p.userId == userId);
        if (patient == null)
            throw new UnauthorizedAccessException("Patient profile not found");
        return patient.Id;
    }

    private async Task<int> GetDoctorIdFromUserIdAsync(string userId)
    {
        using var scope = HttpContext.RequestServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RafiqDbContext>();
        var doctor = await context.Doctors.FirstOrDefaultAsync(d => d.userId == userId);
        if (doctor == null)
            throw new UnauthorizedAccessException("Doctor profile not found");
        return doctor.Id;
    }
}
