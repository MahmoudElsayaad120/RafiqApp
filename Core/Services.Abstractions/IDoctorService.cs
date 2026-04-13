using Rafiq.Api.DTOs;

namespace Rafiq.Api.Services.Abstractions;

public interface IDoctorService
{
    Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync(string? specialization = null, int pageIndex = 1, int pageSize = 5);
    Task<DoctorDto?> GetDoctorByIdAsync(int id);
}
