using Rafeeq.Api.DTOs;

namespace Rafeeq.Api.Services;

public interface IAppointmentService
{
    Task<AppointmentDto> BookAppointmentAsync(int patientId, CreateAppointmentDto createAppointmentDto);
    Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(int patientId);
    Task<IEnumerable<AppointmentDto>> GetDoctorAppointmentsAsync(int doctorId);
    Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status);
}
