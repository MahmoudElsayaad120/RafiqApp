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
   
}
