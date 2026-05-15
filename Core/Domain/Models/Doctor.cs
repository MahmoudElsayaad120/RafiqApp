using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Domain.Models
{
    public class Doctor : BaseEntity<int>
    {
        // Basic details
        public string userId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        //public string Specialization { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal Rate { get; set; } = 5;

        public string? Phone { get; set; }


        public bool IsAvailableToday { get; set; } // عشان كلمة "متوفر اليوم" اللي بالأخضر


        // Personal details
        public int Age { get; set; }
        public string Gender { get; set; }
        public string? ImagePath { get; set; }
        public int AppointmentsCount { get; set; }


        // Professional details 
        public string? MedicalLicense { get; set; }
        public string? ClinicName { get; set; }
        public string? Address { get; set; }
        public string? WorkHours { get; set; }


        // Banking details
        public string? AccountHolderName { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? IBAN { get; set; }
        public string? TransferPeriod { get; set; }

        // Additional details
        public string Bio { get; set; } = string.Empty; // نبذة عن الطبيب (النص الطويل)
        public int YearsOfExperience { get; set; } // سنوات الخبرة (+8)
        public int WaitingTime { get; set; } // مدة الانتظار (15 د)
        public int ReviewCount { get; set; }
        public int PatientsCount { get; set; }




        // Navigation properties
        public ICollection<DoctorAvailability> Availabilities { get; set; } = new List<DoctorAvailability>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        [ForeignKey("Specialization")]        
        public int? SpecializationId { get; set; }
        public Specialization? Specialization { get; set; } 
    }
}
