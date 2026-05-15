using Rafiq.Api.DTOs;
using Shared;

namespace Rafiq.Api.Services.Abstractions
{
    public interface IDoctorService
    {
        // 1. ???????? ???????? ???? ???????
        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync(string? specialization = null, int pageIndex = 1, int pageSize = 5);
        Task<DoctorDto?> GetDoctorByIdAsync(int id);
        // 2. ?????? ???? ???? ?????? (Home & Profile)
        Task<DoctorHomeDetailsDto?> GetDoctorDetailsByIdAsync(int id);
        Task<DoctorProfileDto?> GetDoctorProfileByIdAsync(int id);
        Task<bool> UpdateClinicInfoAsync(int doctorId, UpdateClinicDto updateClinicDto);
        Task UpdateDoctorProfileAsync(int doctorId, UpdateDoctorProfileDto updateDto);

        // 3. ??????? ??????????
        Task<WalletDetailsDto> GetWalletDetailsAsync(string doctorId);
        Task<IEnumerable<NotificationDto>> GetDoctorNotificationAsync(string doctorId);

        // 4. ?????? ???? ?????? (Home & Search)
        Task<IEnumerable<PatientHomeDoctorDto>> GetTopDoctorsAsync(int count);
        Task<IEnumerable<PatientHomeDoctorDto>> SearchDoctorsAsync(string searchWord);
        Task<IEnumerable<PatientSpecializationDto>> GetSpecializationsWithCountAsync();
        Task<IEnumerable<DoctorForPatientDto>> GetDoctorsForPatientAsync(string? search);

        Task<DoctorProfileForPatientDto?> GetDoctorDetailsForPatientAsync(int id);
        Task<IEnumerable<PatientForAvailableSlotDto>> GetAvailableSlotsAsync(int doctorId, DateTime date);
        Task<bool> BookAppointmentAsync(int patientId, BookAppointmentRequestDto request);
        

    }
}