using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Persistence.Data;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Rafiq.Api.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly RafiqDbContext _context;

        public AppointmentService(RafiqDbContext context)
        {
            _context = context;
        }

        public async Task<AppointmentDto> BookAppointmentAsync(int patientId, CreateAppointmentDto createAppointmentDto)
        {
            // Check if doctor exists
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == createAppointmentDto.DoctorId);

            if (doctor == null)
                throw new InvalidOperationException("Doctor not found");

            // Check if patient exists
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == patientId);

            if (patient == null)
                throw new InvalidOperationException("Patient not found");

            // Check for conflicts - same doctor at the same date/time
            var existingAppointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.DoctorId == createAppointmentDto.DoctorId &&
                                         a.Date.Date == createAppointmentDto.Date.Date &&
                                         a.Date.Hour == createAppointmentDto.Date.Hour &&
                                         a.Status != "Cancelled");

            if (existingAppointment != null)
                throw new InvalidOperationException("This time slot is already booked. Please choose another time.");

            // Check if the appointment date is in the past
            if (createAppointmentDto.Date < DateTime.UtcNow)
                throw new InvalidOperationException("Cannot book appointments in the past");

            // Check doctor availability
            var dayOfWeek = createAppointmentDto.Date.DayOfWeek;
            var appointmentTime = createAppointmentDto.Date.TimeOfDay;

            var isAvailable = await _context.DoctorAvailabilities
                .AnyAsync(a => a.DoctorId == createAppointmentDto.DoctorId &&
                              a.DayOfWeek == dayOfWeek &&
                              appointmentTime >= a.FromTime &&
                              appointmentTime <= a.ToTime);

            if (!isAvailable)
                throw new InvalidOperationException("Doctor is not available at this time");

            // Create appointment
            var appointment = new Appointment
            {
                DoctorId = createAppointmentDto.DoctorId,
                PatientId = patientId,
                Date = createAppointmentDto.Date,
                Status = "Pending"
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return new AppointmentDto
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorId,
                DoctorName = doctor.User.Name,
                DoctorSpecialization = doctor.Specialization,
                PatientId = appointment.PatientId,
                PatientName = patient.User.Name,
                Date = appointment.Date,
                Status = appointment.Status
            };
        }

        public async Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(int patientId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.User.Name,
                DoctorSpecialization = a.Doctor.Specialization,
                PatientId = a.PatientId,
                PatientName = a.Patient.User.Name,
                Date = a.Date,
                Status = a.Status
            });
        }

        public async Task<IEnumerable<AppointmentDto>> GetDoctorAppointmentsAsync(int doctorId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.User.Name,
                DoctorSpecialization = a.Doctor.Specialization,
                PatientId = a.PatientId,
                PatientName = a.Patient.User.Name,
                Date = a.Date,
                Status = a.Status
            });
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return false;

            var validStatuses = new[] { "Pending", "Confirmed", "Cancelled", "Completed" };
            if (!validStatuses.Contains(status))
                throw new InvalidOperationException("Invalid status");

            appointment.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
