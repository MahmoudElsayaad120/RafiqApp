using System.Security.Claims;
using Domain.Models;
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

        //  هعمل  service   لما اكبسل يديني اشعار في التابل بتاعع الاشعارات

        // الرد اللي هيرجع للـ React عشان يظهر Modal "تم الإلغاء بنجاح"
        return Ok(new { message = "تم إلغاء الحجز بنجاح" });
    }

    [HttpGet("info")]
    public async Task<IActionResult> GetProfileInfo()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _appointmentService.GetUserProfileAsync(userId);
        return result != null ? Ok(result) : NotFound("المريض غير مسجل في النظام");
    }

    [HttpPost("upload-record")]
    public async Task<IActionResult> UploadRecord([FromForm] UploadRecordDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _appointmentService.UploadMedicalRecordAsync(userId, dto);
        return result ? Ok(new { message = "تم الرفع بنجاح" }) : BadRequest("فشل الرفع، تأكد من بيانات المريض");
    }

    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
    {
        // 1. بنجيب الـ Id (الـ Guid) بتاع اليوزر الحالي من الـ Token
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // 2. بنباصي الـ UserId والـ DTO للـ Service عشان تعمل السحر بتاعها
        var result = await _appointmentService.UpdateUserProfileAsync(userId, dto);

        if (result)
        {
            // دي الرسالة اللي الـ Frontend مستنيها عشان يظهر الـ Modal الأخضر الجميل اللي بعتهولي
            return Ok(new { message = "تم تحديث البيانات بنجاح" });
        }

        return BadRequest(new { message = "حدث خطأ أثناء تحديث البيانات، تأكد من أن البريد الإلكتروني غير مستخدم مسبقاً" });
    }

    [HttpPut("update-medical-profile")]
    public async Task<IActionResult> UpdateMedicalProfile([FromBody] UpdateMedicalProfileDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _appointmentService.UpdateMedicalProfileAsync(userId, dto);

        if (result)
        {
            // الرسالة دي هي اللي هتخلي الـ Frontend يعرض الـ Popup بتاع النجاح
            return Ok(new { message = "تم تحديث البيانات بنجاح" });
        }

        return BadRequest(new { message = "حدث خطأ أثناء تحديث الملف الطبي" });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        // بنجيب الـ Guid من الـ Token كالعادة
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _appointmentService.ChangeUserPasswordAsync(userId, dto);

        if (result.Succeeded)
        {
            // الرسالة دي هي اللي هتخلي الـ Frontend يفتح الـ Popup الأزرق الجميل (تم تحديث كلمة المرور بنجاح)
            return Ok(new { message = "تم تحديث كلمة المرور بنجاح" });
        }

        // لو الـ Identity رفض الباسورد (مثلاً القديم غلط، أو الجديد مش قوي كفاية)
        var firstError = result.Errors.FirstOrDefault()?.Description ?? "فشل تغيير كلمة المرور";
        return BadRequest(new { message = firstError });
    }

    [HttpGet("my-files")]
    public async Task<IActionResult> GetMyFiles([FromQuery] string? fileType)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var files = await _appointmentService.GetPatientFilesAsync(userId, fileType);
        return Ok(files);
    }

    [HttpPost("upload-file")]
    public async Task<IActionResult> UploadFile([FromForm] UploadFileDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _appointmentService.UploadMedicalFileAsync(userId, dto);

        if (result)
        {
            return Ok(new { message = "تم رفع الملف بنجاح" }); // دي اللي هتشغل الـ Popup بتاع رفع الملف
        }

        return BadRequest(new { message = "فشل في رفع الملف، يرجى المحاولة مرة أخرى" });
    }

    [HttpGet("Articles")]
    public async Task<IActionResult> GetArticles([FromQuery] string? category, [FromQuery] string? search)
    {
        var result = await _appointmentService.GetAllArticlesAsync(category, search);
        return Ok(result);
    }

    [HttpGet("Articles/{id}")]
    public async Task<IActionResult> GetArticleDetails(int id)
    {
        var result = await _appointmentService.GetArticleDetailsAsync(id);
        if (result == null) return NotFound(new { message = "المقال غير موجود" });
        return Ok(result);
    }

    [HttpGet("saved")]
    public async Task<IActionResult> GetSavedArticles()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _appointmentService.GetSavedArticlesAsync(userId);
        return Ok(result);
    }

    [HttpPost("toggle-save")]
    public async Task<IActionResult> ToggleSave([FromBody] SaveArticleActionDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var message = await _appointmentService.ToggleSaveArticleAsync(userId, dto.ArticleId);

        return Ok(new { message = message });
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDto dto)
    {
        if (dto == null) return BadRequest("بيانات الإشعار غير صالحة");

        var result = await _appointmentService.CreateNotificationAsync(dto);
        if (!result) return BadRequest(new { message = "فشل في إنشاء الإشعار" });

        return Ok(new { message = "تم إنشاء الإشعار وإرساله بنجاح" });
    }

    [HttpGet("notifications")]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _appointmentService.GetPatientNotificationsAsync(userId);
        return Ok(result);
    }

    // 2. Endpoint لتحديث حالة الإشعارات إلى مقروءة
    [HttpPut("mark-as-read")]
    public async Task<IActionResult> MarkNotificationsAsRead()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _appointmentService.MarkAllAsReadAsync(userId);
        return Ok(new { message = "تم تعيين الإشعارات كمقروءة" });
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
