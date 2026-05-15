using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services;
using Rafiq.Api.Services.Abstractions;
using Services.Specifications;
using Shared;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class DoctorService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : IDoctorService

    {
        private readonly IConfiguration _configuration = configuration;

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync(string? specialization = null, int pageIndex = 1, int pageSize = 5)
        {
            var specification = new BaseSpecifications<Doctor, int>(null);

            var doctors = await unitOfWork.GetRepository<Doctor, int>().GetAllAsync(specification);
            var result = mapper.Map<IEnumerable<DoctorDto>>(doctors);
            return result;
        }

        public async Task<DoctorDto?> GetDoctorByIdAsync(int id)
        {
            var specification = new DoctorSpecifications(id);
            var doctor = await unitOfWork.GetRepository<Doctor, int>().GetAsync(specification);
            if (doctor is null) return null;

            var result = mapper.Map<DoctorDto>(doctor);
            return result;
        }

        public async Task<DoctorHomeDetailsDto?> GetDoctorDetailsByIdAsync(int id)
        {
            var specification = new DoctorHomeDetailsSpecifications(id);
            var doctor = await unitOfWork.GetRepository<Doctor, int>().GetAsync(specification);
            if (doctor is null) return null;

            // get patient names for each appointment of the doctor
            Dictionary<int, string> patientNames = new Dictionary<int, string>();

            foreach (var item in doctor.Appointments)
            {
                // Patient details 
                var specification2 = new AppointmentByDoctorIdSpecifications(item.Id);
                var AppointmentsWithpatients = await unitOfWork.GetRepository<Appointment, int>().GetAsync(specification2);

                patientNames.Add(item.Id, AppointmentsWithpatients.Patient.Name);
            }

            // Helper Variables
            var startOfWeek = DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);
            var monthAppoitments = doctor.Appointments.Where(a => a.Date.Month == DateTime.Now.Month).Count();

            // fill AppointmentDetailsDtos data
            var appointmentDetailsDtos = new List<AppointmentDetailsDto>();
            foreach (var appointment in doctor.Appointments.Where(a => a.Date.Day == DateTime.Now.Day).ToList())
            {
                appointmentDetailsDtos.Add(new AppointmentDetailsDto
                {
                    PatientName = patientNames[appointment.Id],
                    AppointmentDate = appointment.Date,
                    AppointmentStatus = appointment.Status
                });
            }


            var DoctorDetails = new DoctorHomeDetailsDto
            {
                NumberOfDayAppointments = doctor.Appointments.Where(a => a.Date.Day == DateTime.Now.Day).Count(),
                NumberOfWeekAppointments = doctor.Appointments
                        .Where(a => a.Date >= startOfWeek && a.Date < endOfWeek)
                        .Count(),
                Rate = doctor.Rate,
                MonthProfit = (decimal)(doctor.Price * monthAppoitments),

                ListofDayAppointments = appointmentDetailsDtos
            };

            return DoctorDetails;
        }

        public async Task<DoctorProfileDto?> GetDoctorProfileByIdAsync(int id)
        {
            var doctor = await unitOfWork.GetRepository<Doctor, int>().FindAsync(id);
            if (doctor is null) return null;

            return mapper.Map<DoctorProfileDto>(doctor);
        }

        public async Task<bool> UpdateClinicInfoAsync(int doctorId, UpdateClinicDto updateClinicDto) 
        {
            var doctor = await unitOfWork.GetRepository<Doctor, int>().FindAsync(doctorId);
            if (doctor is null) return false;
            doctor.MedicalLicense = updateClinicDto.MedicalLicense ?? doctor.MedicalLicense;
            doctor.ClinicName = updateClinicDto.ClinicName ?? doctor.ClinicName;
            doctor.Address = updateClinicDto.Address ?? doctor.Address;
            doctor.WorkHours = updateClinicDto.WorkHours ?? doctor.WorkHours;
            doctor.Price = updateClinicDto.Price ?? doctor.Price;
            unitOfWork.GetRepository<Doctor, int>().Update(doctor);
            await unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<WalletDetailsDto> GetWalletDetailsAsync(string doctorId) 
        {
            var transactions = await unitOfWork.GetRepository<Transaction, int>().GetAllAsync();
            var doctortransactions = transactions.Where(t => t.DoctorId == doctorId).ToList();
            var monthlyEarnings = doctortransactions
                .Where(t => t.Type == "ارباح دوري" && t.Date.Month == DateTime.Now.Month && t.Date.Year == DateTime.Now.Year)
                .Sum(t => t.Amount);
            var totalEarnings = doctortransactions
                .Where(t => t.Type == "ارباح دوري ")
                .Sum(t =>t.Amount);
            var lastWithdrawal = doctortransactions
                .Where(t => t.Type == "تحويل بنكي")
                .OrderByDescending(t => t.Date)
                .Select(t => t.Amount)
                .FirstOrDefault();

             return new WalletDetailsDto
            {
                MonthlyEarnings = monthlyEarnings,
                TotalEarnings = totalEarnings,
                LastWithdrawal = lastWithdrawal,
                TransactionsCount = doctortransactions.Count,
                RecentTransactions = doctortransactions.OrderByDescending(t => t.Date).Take(5).Select(t => new TransactionDto
                {
                    Amount = t.Amount,
                    Date = t.Date.ToString("dd MMMM yyyy"),
                    Status = t.Status,
                    Type = t.Type
                }).ToList()
            };
            
        }

        public async Task<IEnumerable<NotificationDto>> GetDoctorNotificationAsync(string doctorId)
        {
            var notificationslist = await unitOfWork.GetRepository<Notification, int>().GetAllAsync();
            return notificationslist.Where(n => n.DoctorId == doctorId).OrderByDescending(n => n.CreateAt).Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                NotificationType = n.NotificationType,
                IsRead = n.IsRead,
                TimeAge = GetTimeAge(n.CreateAt)
            }).ToList();
        }

        public async Task UpdateDoctorProfileAsync(int doctorId, UpdateDoctorProfileDto updateDto)
        {
            // 1. جلب بيانات الطبيب الحالية من قاعدة البيانات
            var doctor = await unitOfWork.GetRepository<Doctor, int>().GetAsync(doctorId);
            if (doctor == null) throw new Exception("Doctor not found");

            // 2. معالجة رفع الصورة (نفس منطق المقالات)
            if (updateDto.Image != null && updateDto.Image.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(updateDto.Image.FileName)}";
                var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/profiles");

                if (!Directory.Exists(uploadDirectory)) Directory.CreateDirectory(uploadDirectory);

                var fullPath = Path.Combine(uploadDirectory, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await updateDto.Image.CopyToAsync(stream);
                }

                // مسح الصورة القديمة (اختياري لكن يفضل لتوفير المساحة)
                if (!string.IsNullOrEmpty(doctor.ImagePath))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", doctor.ImagePath.TrimStart('/'));
                    if (File.Exists(oldPath)) File.Delete(oldPath);
                }

                // تحديث المسار الجديد
                doctor.ImagePath = $"/uploads/profiles/{fileName}";
            }

            // 3. تحديث باقي البيانات
            doctor.FullName = updateDto.FullName;
            doctor.Phone = updateDto.Phone;
            doctor.Bio = updateDto.Bio;


            // 4. حفظ التغييرات
            unitOfWork.GetRepository<Doctor, int>().Update(doctor);
            await unitOfWork.SaveChangesAsync();
        }



        // من اول هنا دخلنا علي حزء ال patient

        public async Task<IEnumerable<PatientHomeDoctorDto>> GetTopDoctorsAsync(int count)
        {
            var spec = new TopDoctorsSpecs(1, count);

            // سحب الدكاترة من الداتا بيز
            var doctors = await unitOfWork.GetRepository<Doctor, int>().GetAllAsync(spec);
            var baseUrl = configuration["BaseUrl"];

            // تحويل البيانات (Mapping)
            return doctors.Select(d => new PatientHomeDoctorDto
            {
                Id = d.Id,
                Name = d.FullName,
                Specialization = d.Specialization?.Name,
                Address = d.Address,
                ImagePath = $"{baseUrl}{d.ImagePath}",
                Rate = d.Rate
            }).ToList();
        }  //  الريسيه في ال  patient

        public async Task<IEnumerable<PatientHomeDoctorDto>> SearchDoctorsAsync(string searchWord)
        {

            var searchSpecification = new SearchDoctorsSpecs(searchWord);
            // بنجيب الدكاترة اللي اسمهم أو تخصصهم بيحتوي على كلمة البحث
            var doctors = await unitOfWork.GetRepository<Doctor, int>().GetAllAsync(searchSpecification);

            var baseUrl = _configuration["BaseUrl"];


            return doctors.Select(d => new PatientHomeDoctorDto
            {
                Id = d.Id,
                Name = d.FullName,
                Specialization = d.Specialization?.Name,
                Address = d.Address,
                ImagePath = $"{baseUrl}{d.ImagePath}",
                Rate = d.Rate
            }).ToList();
        }  // البحث في ال patient

        public async Task<IEnumerable<PatientSpecializationDto>> GetSpecializationsWithCountAsync()
        {
            // 1.  كل التخصصات من الداتا بيز
            var specializations = await unitOfWork.GetRepository<Specialization, int>().GetAllAsync();

            // 2.  كل الدكاترة عشان نعدهم بناءً على تخصصهم
            var doctors = await unitOfWork.GetRepository<Doctor, int>().GetAllAsync();

            // 3. تحويل البيانات للـ DTO وحساب العدد لكل تخصص
            return specializations.Select(s => new PatientSpecializationDto
            {
                Id = s.Id,
                Name = s.Name,
                IconUrl = s.IconUrl,
                // بنعد الدكاترة اللي تخصصهم يطابق اسم التخصص الحالي
                DoctorsCount = doctors.Count(d => d.Specialization?.Name == s.Name)
            }).ToList();
        } // عامل موديل اسمها Specialization 

        public async Task<IEnumerable<DoctorForPatientDto>> GetDoctorsForPatientAsync(string? search)
        {
            var searchSpecification = new SearchDoctorsWithincludeSpecs(search);
            // بننادي الميثود الجديدة وبنبعت أسماء الجداول اللي عايزينها
            var doctors = await unitOfWork.GetRepository<Doctor, int>().GetAllAsync(searchSpecification);


            var baseUrl = _configuration["BaseUrl"];


            return doctors.Select(d => new DoctorForPatientDto
            {
                Id = d.Id,
                FullName = d.FullName,
                Specialization = d.Specialization?.Name,
                ImagePath = $"{baseUrl}{d.ImagePath}",
                Address = d.Address,
                Price = d.Price,
                Rate = (d.Reviews != null && d.Reviews.Any()) ? d.Reviews.Average(r => r.Rate) : 0m, // حساب التقييم بناءً على المراجعات
                IsAvailableToday = d.Availabilities != null && d.Availabilities.Any(a => a.DayOfWeek == DateTime.Today.DayOfWeek) // التحقق من توفر الطبيب اليوم
                // ... باقي الحقول
            }).ToList();
        } // عامل موديل اسمها Review------------- الاطباء في ال patient

        public async Task<DoctorProfileForPatientDto?> GetDoctorDetailsForPatientAsync(int id)
        {
            var specification = new DoctorDetailsForPatientSpecs(id);
            // سحب البيانات مع الجداول المرتبطة (Eager Loading)
            var doctor = await unitOfWork.GetRepository<Doctor, int>().GetAllAsync(specification);
              
            

            var d = doctor.FirstOrDefault();
            if (d == null) return null;

            var baseUrl = _configuration["BaseUrl"];

            return new DoctorProfileForPatientDto
            {
                Id = d.Id,
                FullName = d.FullName,
                Specialization = d.Specialization?.Name,
                ImagePath = $"{baseUrl}{d.ImagePath}",
                Address = d.Address,
                Price = d.Price,
                Bio = d.Bio ?? "لا توجد نبذة تعريفية.",
                YearsOfExperience = d.YearsOfExperience,
                PatientsCount = d.PatientsCount,
                WaitingTime = d.WaitingTime,
                Rate = (d.Reviews != null && d.Reviews.Any()) ? d.Reviews.Average(r => r.Rate) : 0m, // حساب التقييم بناءً على المراجعات
                ReviewsCount = d.Reviews.Count(),

                // تحويل المواعيد الأسبوعية
                WeeklySchedule = d.Availabilities.Select(a => new DoctorWeeklyAvailabilityDto
                {
                    DayName = TranslateDay(a.DayOfWeek),
                    // تحويل TimeSpan لشكل مقروء AM/PM
                    TimeRange = a.IsClosed ? "مغلق" :
                        $"{DateTime.Today.Add(a.FromTime):hh:mm tt} - {DateTime.Today.Add(a.ToTime):hh:mm tt}",
                    IsClosed = a.IsClosed
                }).ToList()
            };
        } // ملف الطبيب في ال patient

        public async Task<IEnumerable<PatientForAvailableSlotDto>> GetAvailableSlotsAsync(int doctorId, DateTime date)
        {
            // 1. نجيب مواعيد عمل الدكتور في اليوم ده (مثلاً السبت من 10 لـ 4)
            var specs = new DoctorAvailabilitySpec(doctorId,date);
            var availabilities = await unitOfWork.GetRepository<DoctorAvailability, int>().GetAllAsync(specs);

            var slots = availabilities.Select(a => new PatientForAvailableSlotDto()
            {
                Id = a.Id,
                Time = a.FromTime,
                IsReserved = a.IsClosed
            });

            //// 2. نجيب الحجوزات اللي محجوزة فعلياً في اليوم ده عشان منطلعهاش متاحة
            //var reservedTimes = (await unitOfWork.GetRepository<Appointment, int>()
            //    .GetAllAsync())
            //    .Where(a => a.DoctorId == doctorId && a.Date.Date == date.Date)
            //    .Select(a => a.Date.TimeOfDay).ToList();

            //var slots = new List<PatientForAvailableSlotDto>();
            //var current = availability.FromTime;

            //// 3. نقسم الوقت لـ Slots (كل 30 دقيقة كشف مثلاً)
            //while (current + TimeSpan.FromMinutes(30) <= availability.ToTime)
            //{
            //    slots.Add(new PatientForAvailableSlotDto
            //    {
            //        Time = DateTime.Today.Add(current).ToString("hh:mm tt"),
            //        RawTime = current,
            //        IsReserved = reservedTimes.Contains(current) // لو الوقت ده موجود في الحجوزات يبقى محجوز
            //    });
            //    current = current.Add(TimeSpan.FromMinutes(30));
            //}

            return slots;
        }  // حجز موعد في ال patient

        public async Task<bool> BookAppointmentAsync(int patientId, BookAppointmentRequestDto bookAppointment)
        {
            var AvailabilitySpec = new DoctorAvailabilityForPatientSpec(bookAppointment.DoctorId, bookAppointment.SlotId);
            var availability = await unitOfWork.GetRepository<DoctorAvailability, int>().GetAsync(AvailabilitySpec);

            // update is closed ==> true
            availability.IsClosed = true;
            unitOfWork.GetRepository<DoctorAvailability, int>().Update(availability);

            // 1. بنحول الـ DTO لـ Model (Appointment) عشان الداتابيز متفهمش DTO
            var appointment = new Appointment
            {
                DoctorId = bookAppointment.DoctorId,
                PatientId = patientId,
                // بنجمع التاريخ مع الساعة اللي اختارها
                
                Date = availability.SpecificDate.ToDateTime(TimeOnly.FromTimeSpan(availability.FromTime)), // هنا ممكن نضيف من الـ DTO لو فيه تاريخ معين
                Status = "Pending",
                CreateAt = DateTime.Now
            };

            // 2. بنستخدم الـ UnitOfWork عشان نضيف الحجز
            await unitOfWork.GetRepository<Appointment, int>().AddAsync(appointment);

            // 3. بنعمل Save Changes
            var result = await unitOfWork.CompleteAsync();

            return result > 0;
        } // حجز موعد في ال patient

       




        // ميثود مساعدة لترجمة الأيام للعربية
        private string TranslateDay(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Saturday => "السبت",
                DayOfWeek.Sunday => "الأحد",
                DayOfWeek.Monday => "الاثنين",
                DayOfWeek.Tuesday => "الثلاثاء",
                DayOfWeek.Wednesday => "الأربعاء",
                DayOfWeek.Thursday => "الخميس",
                DayOfWeek.Friday => "الجمعة",
                _ => day.ToString()
            };
        }

        private string GetTimeAge(DateTime dateTime)
        {
            var span = DateTime.Now - dateTime;
            if (span.TotalMinutes < 60) return $" {Math.Floor(span.TotalMinutes)} منذ دقيقه";
            if (span.TotalHours < 24) return $"{Math.Floor(span.TotalHours)} منذ ساعه";
            if (span.TotalHours < 30) return $"{Math.Floor(span.TotalDays)} منذ يوم ";

            return dateTime.ToString("dd/MM/yyyy");
        }
    }
}
