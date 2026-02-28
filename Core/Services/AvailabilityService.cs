using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Rafeeq.Api.DTOs;
using Rafeeq.Api.Services;

namespace Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly RafiqDbContext _context;

        public AvailabilityService(RafiqDbContext context)
        {
            _context = context;
        }

        public async Task<AvailabilityDto> AddAvailabilityAsync(int doctorId, AvailabilityDto availabilityDto)
        {
            // Verify doctor exists
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null)
                throw new InvalidOperationException("Doctor not found");

            // Validate time range
            if (availabilityDto.FromTime >= availabilityDto.ToTime)
                throw new InvalidOperationException("FromTime must be before ToTime");

            // Check for overlapping availability
            var overlapping = await _context.DoctorAvailabilities
                .AnyAsync(a => a.DoctorId == doctorId &&
                              a.DayOfWeek == availabilityDto.DayOfWeek &&
                              ((availabilityDto.FromTime >= a.FromTime && availabilityDto.FromTime < a.ToTime) ||
                               (availabilityDto.ToTime > a.FromTime && availabilityDto.ToTime <= a.ToTime) ||
                               (availabilityDto.FromTime <= a.FromTime && availabilityDto.ToTime >= a.ToTime)));
            

            if (overlapping)
                throw new InvalidOperationException("This time slot overlaps with existing availability");

            var availability = new DoctorAvailability
            {
                DoctorId = doctorId,
                DayOfWeek = availabilityDto.DayOfWeek,
                FromTime = availabilityDto.FromTime,
                ToTime = availabilityDto.ToTime
            };

            _context.DoctorAvailabilities.Add(availability);
            await _context.SaveChangesAsync();

            return new AvailabilityDto
            {
                Id = availability.Id,
                DoctorId = availability.DoctorId,
                DayOfWeek = availability.DayOfWeek,
                FromTime = availability.FromTime,
                ToTime = availability.ToTime
            };
        }

        public async Task<IEnumerable<AvailabilityDto>> GetDoctorAvailabilitiesAsync(int doctorId)
        {
            var availabilities = await _context.DoctorAvailabilities
                .Where(a => a.DoctorId == doctorId)
                .OrderBy(a => a.DayOfWeek)
                .ThenBy(a => a.FromTime)
                .ToListAsync();

            return availabilities.Select(a => new AvailabilityDto
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                DayOfWeek = a.DayOfWeek,
                FromTime = a.FromTime,
                ToTime = a.ToTime
            });
        }

        public async Task<bool> DeleteAvailabilityAsync(int availabilityId, int doctorId)
        {
            var availability = await _context.DoctorAvailabilities
                .FirstOrDefaultAsync(a => a.Id == availabilityId && a.DoctorId == doctorId);

            if (availability == null)
                return false;

            _context.DoctorAvailabilities.Remove(availability);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
