using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rafiq.Api.DTOs;

namespace Shared
{
    public class DoctorProfileForPatientDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public string? ImagePath { get; set; }
        public decimal Rate { get; set; }
        public int ReviewsCount { get; set; }
        public string Bio { get; set; }
        public decimal? Price { get; set; }
        public int YearsOfExperience { get; set; }
        public int PatientsCount { get; set; }
        public int WaitingTime { get; set; }
        public string? Address { get; set; }


        public List<DoctorWeeklyAvailabilityDto> WeeklySchedule { get; set; }
        // المواعيد (الجدول)
        public List<AvailabilityDto> WeeklyAvailabilities { get; set; } = new();
        // آراء المرضى
        public List<DoctorForPatientDto> Reviews { get; set; } = new();
    }
}
