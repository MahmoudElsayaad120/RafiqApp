using Rafiq.Api.DTOs;
using Shared;

namespace Rafiq.Api.Services.Abstractions;

public interface IAvailabilityService
{
    Task AddAvailabilityAsync(int doctorId, AvailabilityDto availabilityDto);
    Task<IEnumerable<AvailabilityToReturnDto>> GetDoctorAvailabilitiesAsync(int doctorId, DateOnly date);
    Task<bool> DeleteFullDayAvailabilityAsync(int doctorId, DateOnly date);
    Task<bool> DeleteFulltimeAvailabilityAsync(int id);
    Task UpdateAvailabilityAsync(int id, UpdateAvailibilityDto availabilityDto);


}
