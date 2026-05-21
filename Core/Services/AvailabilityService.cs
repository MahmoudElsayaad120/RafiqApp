using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services.Abstractions;
using Shared;

namespace Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly RafiqDbContext _context;
        private readonly IUnitOfWork unitOfWork;

        public AvailabilityService(RafiqDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            unitOfWork = unitOfWork;
        }

        public async Task AddAvailabilityAsync(int doctorId, AvailabilityDto availabilityDto)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null)
                throw new InvalidOperationException("Doctor not found");

            if (availabilityDto.FromTime >= availabilityDto.ToTime)
                throw new InvalidOperationException("FromTime must be before ToTime");

            var result = new List<AvailabilityDto>();

            var start = availabilityDto.FromTime;
            var end = availabilityDto.ToTime;

            while (start < end)
            {
                var slotEnd = start.Add(TimeSpan.FromMinutes(30));

                // تأكد إننا مش بنعدي ToTime
                if (slotEnd > end)
                    slotEnd = end;

                // Check overlapping لكل Slot
                var overlapping = await _context.DoctorAvailabilities.AnyAsync(a =>
                    a.DoctorId == doctorId &&
                    a.SpecificDate == availabilityDto.SpecificDate &&
                    ((start >= a.FromTime && start < a.ToTime) ||
                     (slotEnd > a.FromTime && slotEnd <= a.ToTime)));

                if (!overlapping)
                {
                    var availability = new DoctorAvailability
                    {
                        DoctorId = doctorId,
                        SpecificDate = availabilityDto.SpecificDate,
                        FromTime = start,
                        ToTime = slotEnd
                    };

                    _context.DoctorAvailabilities.Add(availability);

                    result.Add(new AvailabilityDto
                    {
                        SpecificDate = availability.SpecificDate,
                        FromTime = availability.FromTime,
                        ToTime = availability.ToTime
                    });
                }

                start = slotEnd;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AvailabilityToReturnDto>> GetDoctorAvailabilitiesAsync(int doctorId, DateOnly date)
        {
            var availabilities = await _context.DoctorAvailabilities
                .Where(a => a.DoctorId == doctorId && a.SpecificDate == date)
                .OrderBy(a => a.SpecificDate)
                .ThenBy(a => a.FromTime)
                .ToListAsync();

            return availabilities.Select(a => new AvailabilityToReturnDto
            {
                Id = a.Id,
                SpecificDate = a.SpecificDate,
                FromTime = a.FromTime,
                ToTime = a.ToTime
            });
        }

        public async Task<bool> DeleteFullDayAvailabilityAsync(int doctorId, DateOnly date)
        {
            // الحصول على يوم الأسبوع من التاريخ المرسل
            var dayOfWeek = date.DayOfWeek;

            var availabilities = await _context.DoctorAvailabilities
                .Where(a => a.DoctorId == doctorId && a.SpecificDate == date)
                .ToListAsync();

            if (!availabilities.Any()) return false;

            _context.DoctorAvailabilities.RemoveRange(availabilities);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFulltimeAvailabilityAsync(int id)
        {
            //  delete doctorAvailibility By Id
            var availability = await _context.DoctorAvailabilities.FindAsync(id);
            if (availability == null) return false;

            _context.DoctorAvailabilities.Remove(availability);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task UpdateAvailabilityAsync(int id, UpdateAvailibilityDto availabilityDto)
        {
            var availability = await _context.DoctorAvailabilities.FirstOrDefaultAsync(a => a.Id == id);
            if (availability == null)
                throw new InvalidOperationException("Availability not found");

            // Check overlapping
            var overlapping = await _context.DoctorAvailabilities.AnyAsync(a =>
                a.Id != id &&
                a.DoctorId == availability.DoctorId &&
                a.SpecificDate == availability.SpecificDate &&
                ((availabilityDto.FromTime >= a.FromTime && availabilityDto.FromTime < a.ToTime) ||
                 (availabilityDto.FromTime.Add(TimeSpan.FromMinutes(30)) > a.FromTime && availabilityDto.FromTime.Add(TimeSpan.FromMinutes(30)) <= a.ToTime)));

            if (overlapping)
                throw new InvalidOperationException("The updated time slot overlaps with an existing availability");

            availability.FromTime = availabilityDto.FromTime;
            availability.ToTime = availabilityDto.FromTime.Add(TimeSpan.FromMinutes(30));

            //unitOfWork.GetRepository<DoctorAvailability, int>().Update(availability);
            _context.DoctorAvailabilities.Update(availability);
            await _context.SaveChangesAsync();    
        }
    }
}
