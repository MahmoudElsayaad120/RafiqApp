using Microsoft.AspNetCore.Identity;
using Rafiq.Api.DTOs;
using Shared;

namespace Rafiq.Api.Services.Abstractions;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(int patientId); // حجوزاتي اللي في ال Patient
    Task<bool> CancelAppointmentAsync(int appointmentId); // الغاء الحجز في ال  Patient
    Task<IEnumerable<AppointmentDto>> GetDoctorAppointmentsAsync(int doctorId);
    Task<IEnumerable<AppointmentForPatientDto>> GetPatientAppointmentsAsync(int patientId, string? status = null);  // حجوزاتي القادمه والمتكمله 
    Task<AppointmentForPatientDetailsDto?> GetAppointmentDetailsAsync(int appointmentId); // تفاصيل الحجز المكتمل في ال Patient
    Task<bool> UpdateBookAppointmentAsync(int patientId, UpdateBookAppointmentRequestDto request); // تعديل الحجز في ال patient
    Task<string> ProcessPaymentAsync(PaymentRequestDto paymentRequestDto); //  الدفع في ال patient
    Task<bool> CancelAppointmentByIdAsync(int appointmentId); // الغاء الحجز في ال Patient  الل بعد الدفع
    Task<UserProfileDto> GetUserProfileAsync(string identityUserId); // الملف الشخصي في ال Patient
    Task<bool> UploadMedicalRecordAsync(string identityUserId, UploadRecordDto dto); // رفع ملف طبي في ال Patient
    Task<bool> UpdateUserProfileAsync(string identityUserId, UpdateProfileDto dto); // تعديل الملف الشخصي في ال Patient
    Task<bool> UpdateMedicalProfileAsync(string identityUserId, UpdateMedicalProfileDto dto); // تعديل الملف الطبي في ال Patient
    Task<IdentityResult> ChangeUserPasswordAsync(string identityUserId, ChangePasswordDto dto); // تغيير كلمة المرور في ال Patient
    Task<List<MedicalFileResponseDto>> GetPatientFilesAsync(string identityUserId, string? fileType);//رفع الملف الطبي في ال Patient
    Task<bool> UploadMedicalFileAsync(string identityUserId, UploadFileDto dto); // رفع ملف طبي في ال Patient
    Task<List<ArticleListDto>> GetAllArticlesAsync(string? category, string? searchKey); // المقالات في ال Patient
    Task<ArticleDetailsDto> GetArticleDetailsAsync(int id); // تفاصيل المقال في ال Patient
    Task<List<ArticleListDto>> GetSavedArticlesAsync(string identityUserId); // المقالات المحفوظة في ال Patient
    Task<string> ToggleSaveArticleAsync(string identityUserId, int articleId);
    Task<bool> CreateNotificationAsync(CreateNotificationDto dto);// ميثود ال  ppost  بتاعت الاشعارات في ال Patient 
    Task<List<NotificationResponseDto>> GetPatientNotificationsAsync(string identityUserId); // الاشعارات في ال Patient
    Task<bool> MarkAllAsReadAsync(string identityUserId); // تعليم كل الاشعارات كمقروءة في ال Patient
   


}   
