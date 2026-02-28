using Rafeeq.Api.DTOs;

namespace Rafeeq.Api.Services;

public interface IDoctorService
{
    Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync(string? specialization = null);
    Task<DoctorDto?> GetDoctorByIdAsync(int id);
}
