using System.Security.Claims;
using Domain.Contracts;
using Domain.Models;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<DoctorsController> _logger;
    private readonly UserManager<AppUser> _userManager;


    public DoctorsController(IDoctorService doctorService, ILogger<DoctorsController> logger, UserManager<AppUser> userManager)
    {
        _doctorService = doctorService;
        _logger = logger;
        _userManager = userManager;
       
    }

    [HttpGet] // doctor can view all doctors with pagination and filter by specialization
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

    [HttpGet("{id}")] // doctor can view his profile by id
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

    [HttpGet("GetDoctorHomePageDetails")] // doctor can view his home page details like his appointments and his patients
    public async Task<ActionResult<DoctorHomeDetailsDto>> GetDoctorHomePageDetails()
    {
        // doctor id
        var doctorId = await GetDoctorIdFromUserIdAsync(GetCurrentUserId()!);

        // get data 
        var doctorDetails = await _doctorService.GetDoctorDetailsByIdAsync(doctorId);

        if (doctorDetails == null)
            return NotFound(new { message = "Doctor details not found" });

        return Ok(doctorDetails);
    }

    [HttpPost("CreateCoupon")] // doctor can create coupon to give it to his patients
    public async Task<ActionResult> CreateCoupon(CouponDto couponDto)
    {
        // الحصول علي doctorId 
        var doctorId = await GetDoctorIdFromUserIdAsync(GetCurrentUserId()!);

        // create new coupon
        var newcoupon = new Coupon
        {
            Code = couponDto.Code,
            UsageLimit = couponDto.UsageLimit,
            DoctorId = doctorId,
            CreateAt = DateTime.UtcNow,
            IsActive = true
        };
        using var scope = HttpContext.RequestServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RafiqDbContext>();
        context.coupons.Add(newcoupon);
        await context.SaveChangesAsync();
        return Ok(new { message = "created a new coupon successfully", couponId = newcoupon.Id });
    }

    [HttpGet("GetProfile")] // doctor can view his profile
    public async Task<ActionResult<DoctorProfileDto>> GetProfile()
    {

        // create my Acount
        var doctorId = await GetDoctorIdFromUserIdAsync(GetCurrentUserId()!);
        var doctor = await _doctorService.GetDoctorProfileByIdAsync(doctorId);
        if (doctor == null) return NotFound();

        return Ok(doctor);

    }

    [HttpPut("UpdateProfile")]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateDoctorProfileDto updateDto)
    {
        // جلب الـ ID الخاص بالطبيب من الـ Token أو الـ Session
        var doctorId = await GetDoctorIdFromUserIdAsync(GetCurrentUserId()!);

        try
        {
            await _doctorService.UpdateDoctorProfileAsync(doctorId, updateDto);
            return Ok(new { message = "Profile updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("logOut")] // doctor can log out
    public IActionResult LogOut() 
    {
        return Ok(new { message = "logged out" });
    }

    [HttpPost("ChangePassword")] // doctor can change his password
    public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto) 
    {
        var user = await _userManager.FindByIdAsync(GetCurrentUserId()!);
        var result = await _userManager.ChangePasswordAsync(user!, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
        if (!result.Succeeded)
        {
            return Ok(new { message = "Password changed successfully" });
        }

        return BadRequest(result.Errors);

    }

    [HttpPut("UpdateClinicInfo")] // doctor can update his clinic info
    public async Task<IActionResult> UpdateClinicInfo(UpdateClinicDto updateClinicDto) 
    {
        var doctorId = await GetDoctorIdFromUserIdAsync(GetCurrentUserId()!);
        var doctor = await _doctorService.UpdateClinicInfoAsync(doctorId, updateClinicDto);
        if(!doctor) return BadRequest(new { message = "Failed to update clinic info" });


        return Ok(new { message = "Clinic info updated successfully" });
    }

    [HttpPut("UpdatePaymentInfo")] // doctor can update his payment info
    public async Task<IActionResult> UpdatePaymentInfo(UpdatePaymentDto updatePaymentDto) 
    {
        var userId = GetCurrentUserId();
        return Ok(new { message = "Payment info updated successfully" });   
    }
    [HttpGet("GetWalletDetails")] 
    public async Task<IActionResult> GetWalletDetails()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var result = await _doctorService.GetWalletDetailsAsync(userId);

        return Ok(result);
    }
    [HttpGet("GetNotifications")] 
    public async Task<IActionResult> GetNotifications() 
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _doctorService.GetDoctorNotificationAsync(userId);
        return Ok(result);
    }

    [HttpGet("TopDoctors")]
    public async Task<IActionResult> GetTopDoctors()
    {
        // بنطلب 5 دكاترة للعرض في الهوم سكرين
        var result = await _doctorService.GetTopDoctorsAsync(5);

        if (result == null || !result.Any())
            return NotFound("لا يوجد دكاترة متاحين حالياً");

        return Ok(result);
    }

    [HttpGet("Search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            // لو البحث فاضي ممكن نرجع قائمة فاضية أو أول 5 دكاترة كـ Suggestion
            return Ok(new List<PatientHomeDoctorDto>());
        }

        var result = await _doctorService.SearchDoctorsAsync(query);
        return Ok(result);
    }

    [HttpGet("Specializations")]
    public async Task<IActionResult> GetSpecializations()
    {
        var result = await _doctorService.GetSpecializationsWithCountAsync();

        if (result == null)
            return NotFound("لا توجد تخصصات حالياً");

        return Ok(result);
    }

    [HttpGet("allDoctors")]
    public async Task<IActionResult> GetAllDoctors([FromQuery] string? search)
    {
        var doctors = await _doctorService.GetDoctorsForPatientAsync(search);
        return Ok(doctors);
    }

    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetDoctorDetails(int id)
    {
        var result = await _doctorService.GetDoctorDetailsForPatientAsync(id);

        if (result == null)
        {
            return NotFound(new { message = "عفواً، لم يتم العثور على هذا الطبيب" });
        }

        return Ok(result);
    }

    [HttpGet("{id}/available-slots")]
    public async Task<IActionResult> GetSlots(int id, [FromQuery] DateTime date)
    {
        var slots = await _doctorService.GetAvailableSlotsAsync(id, date);
        return Ok(slots);
    }

    [HttpPost("book")]
    public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentRequestDto request)
    {
        // المريض اللي داخل دلوقتي (مؤقتاً)
        var userId = GetCurrentUserId();
        var patientId = await GetPatientIdFromUserIdAsync(userId);


        var isSuccess = await _doctorService.BookAppointmentAsync(patientId, request);

        if (isSuccess)
            return Ok(new { message = "تم طلب الحجز بنجاح" });

        return BadRequest(new { message = "حدث خطأ أثناء الحجز" });
    }







    private string? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //if (int.TryParse(userIdClaim, out var userId))
        return userIdClaim;
        //throw new UnauthorizedAccessException("Invalid user ID");
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

    private async Task<int> GetPatientIdFromUserIdAsync(string userId)
    {
        using var scope = HttpContext.RequestServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RafiqDbContext>();
        var patient = await context.Patients.FirstOrDefaultAsync(d => d.userId == userId);
        if (patient == null)
            throw new UnauthorizedAccessException("patient profile not found");
        return patient.Id;
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }
}
