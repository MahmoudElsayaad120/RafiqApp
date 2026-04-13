using Rafiq.Api.DTOs;

namespace Rafiq.Api.Services;

public interface IAvailabilityService
{
    Task<AvailabilityDto> AddAvailabilityAsync(int doctorId, AvailabilityDto availabilityDto);
    Task<IEnumerable<AvailabilityDto>> GetDoctorAvailabilitiesAsync(int doctorId);
    Task<bool> DeleteAvailabilityAsync(int availabilityId, int doctorId);
}
