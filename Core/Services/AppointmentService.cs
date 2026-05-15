using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence;
using Persistence.Data;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services;
using Rafiq.Api.Services.Abstractions;
using Services.Specifications;
using Shared;

namespace Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly RafiqDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(RafiqDbContext context, IConfiguration configuration, IUnitOfWork unitOfWork )
        {
            _context = context;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        //public async Task<AppointmentDto> BookAppointmentAsync(int patientId, CreateAppointmentDto createAppointmentDto)
        //{
        //    // Check if doctor exists
        //    var doctor = await _context.Doctors
        //        .FirstOrDefaultAsync(d => d.Id == createAppointmentDto.DoctorId);

        //    if (doctor == null)
        //        throw new InvalidOperationException("Doctor not found");

        //    // Check if patient exists
        //    var patient = await _context.Patients
        //        .FirstOrDefaultAsync(p => p.Id == patientId);

        //    if (patient == null)
        //        throw new InvalidOperationException("Patient not found");

        //    // Check for conflicts - same doctor at the same date/time
        //    var existingAppointment = await _context.Appointments
        //        .FirstOrDefaultAsync(a => a.DoctorId == createAppointmentDto.DoctorId &&
        //                                 a.Date.Date == createAppointmentDto.Date.Date &&
        //                                 a.Date.Hour == createAppointmentDto.Date.Hour &&
        //                                 a.Status != "Cancelled");

        //    if (existingAppointment != null)
        //        throw new InvalidOperationException("This time slot is already booked. Please choose another time.");

        //    // Check if the appointment date is in the past
        //    if (createAppointmentDto.Date < DateTime.UtcNow)
        //        throw new InvalidOperationException("Cannot book appointments in the past");

        //    // Check doctor availability
        //    var dayOfWeek = createAppointmentDto.Date.DayOfWeek;
        //    var appointmentTime = createAppointmentDto.Date.TimeOfDay;

        //    var isAvailable = await _context.DoctorAvailabilities
        //        .AnyAsync(a => a.DoctorId == createAppointmentDto.DoctorId &&
        //                      appointmentTime >= a.FromTime &&
        //                      appointmentTime <= a.ToTime);

        //    if (!isAvailable)
        //        throw new InvalidOperationException("Doctor is not available at this time");

        //    // Create appointment
        //    var appointment = new Appointment
        //    {
        //        DoctorId = createAppointmentDto.DoctorId,
        //        PatientId = patientId,
        //        Date = createAppointmentDto.Date,
        //        Status = "Pending"
        //    };

        //    _context.Appointments.Add(appointment);
        //    await _context.SaveChangesAsync();

        //    return new AppointmentDto
        //    {
        //        Id = appointment.Id,
        //        DoctorId = appointment.DoctorId,
        //        DoctorName = doctor.FullName,
        //        //DoctorSpecialization = doctor.Specialization,
        //        DoctorSpecialization = doctor.Specialization.Name,
        //        PatientId = appointment.PatientId,
        //        Date = appointment.Date,
        //        Status = appointment.Status
        //    };
        //}
        public async Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(int patientId)
        {
            var baseUrl = _configuration["BaseUrl"];

            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Specialization)
                .Include(a => a.Patient)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.FullName,
                DoctorSpecialization = a.Doctor.Specialization?.Name,
                PatientId = a.PatientId,
                //PatientName = a.Patient.Name,
                ImagePath = a.Doctor.ImagePath != null ? $"{baseUrl}{a.Doctor.ImagePath}" : null,
                Date = a.Date,
                Status = a.Status
            });
        }  //  حجوزاتي في ال  patient
        public async Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            // 1. هات الحجز بالـ ID بس دلوقتي عشان التجربة
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null) return false;

            // 2. تحديث الحالة (تأكد إن "Cancelled" مسموح بها في الداتابيز)
            appointment.Status = "Cancelled";

            // 3. لو عندك حقل تحديث التاريخ (زي اللي عمل الإيرور فات)
            // appointment.UpdatedAt = DateTime.Now; 

            var result = await _context.SaveChangesAsync();
            return result > 0;
        } //  الغاء الحجز في ال  patient 
        public async Task<IEnumerable<AppointmentDto>> GetDoctorAppointmentsAsync(int doctorId)
        {
            var baseUrl = _configuration["BaseUrl"];

            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.FullName,
                DoctorSpecialization = a.Doctor.Specialization?.Name,
                PatientId = a.PatientId,
                //ImagePath = a.Patien != null ? $"{a.Patient.ImagePath}" : null,
                //PatientName = a.Patient.Name,
                Date = a.Date,
                Status = a.Status
            });
        }
        //public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status)
        //{
        //    var appointment = await _context.Appointments.FindAsync(appointmentId);
        //    if (appointment == null)
        //        return false;

        //    var validStatuses = new[] { "Pending", "Confirmed", "Cancelled", "Completed" };
        //    if (!validStatuses.Contains(status))
        //        throw new InvalidOperationException("Invalid status");

        //    appointment.Status = status;
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        //public async Task<IEnumerable<AppointmentForPatientDto>> GetPatientAppointmentsAsync(string patientId, string? status = null)
        //{
        //    var baseUrl = _configuration["BaseUrl"];


        //    // 1. جلب البيانات الأساسية (بدون تعقيد Repository)
        //    var query = _context.Appointments
        //        .Include(a => a.Doctor)
        //        .ThenInclude(d => d.Specialization)
        //        .Where(a => a.PatientId == int.Parse(patientId)); 

        //    // 2. فلترة بسيطة لو الـ status موجود
        //    if (!string.IsNullOrEmpty(status))
        //    {
        //        query = query.Where(a => a.Status == status);
        //    }

        //    var appointments = await query.ToListAsync();

        //    // 3. تحويل الداتا للـ DTO اللي إنت بعتهولي
        //    return appointments.Select(a => new AppointmentForPatientDto
        //    {
        //        Id = a.Id,
        //        DoctorName = a.Doctor.FullName,
        //        Specialization = a.Doctor.Specialization.Name,
        //        ImagePath = $"{baseUrl}{a.Doctor.ImagePath}",
        //        AppointmentDate = a.Date.ToString("dddd، dd MMMM yyyy", new System.Globalization.CultureInfo("ar-EG")),
        //        AppointmentTime = a.Date.ToString("hh:mm tt", new System.Globalization.CultureInfo("ar-EG")),
        //        Location = "طنطا - أول شارع البحر",
        //        Price = 150,
        //        Status = a.Status
        //    });
        //} // حجوزاتي القادمه والمتكمله
        public async Task<IEnumerable<AppointmentForPatientDto>> GetPatientAppointmentsAsync(int patientId, string? status = null)
        {
            var baseUrl = _configuration["BaseUrl"] ?? "";
            var culture = new System.Globalization.CultureInfo("ar-EG");

           
            // 2. الاستعلام باستخدام الـ pid (الرقم المحول)
            var query = _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialization)
                .Where(a => a.PatientId == patientId); // المقارنة هنا بقت بين int و int

            // 3. فلترة الـ Status
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
            }

            var appointments = await query.ToListAsync();

            // 4. التحويل للـ DTO مع حماية الـ Null
            return appointments.Select(a => new AppointmentForPatientDto
            {
                Id = a.Id,
                // استخدمنا الـ ? عشان لو فيه دكتور أو تخصص ناقص الكود ميفصلش
                DoctorName = a.Doctor?.FullName ?? "طبيب غير معروف",
                Specialization = a.Doctor?.Specialization?.Name ?? "تخصص عام",
                ImagePath = a.Doctor.ImagePath == null ? null : $"{baseUrl}{a.Doctor.ImagePath}",
                AppointmentDate = a.Date.ToString("dddd، dd MMMM yyyy", culture),
                AppointmentTime = a.Date.ToString("hh:mm tt", culture),
                Location = "طنطا - أول شارع البحر",
                Price = 150,
                Status = a.Status ?? "Pending"
            });
        } // حجوزاتي القادمه والمتكمله
        public async Task<AppointmentForPatientDetailsDto?> GetAppointmentDetailsAsync(int appointmentId)
        {
            // 1. هات البيانات من الداتابيز (الـ Entity)
            
            // بنستخدم الـ Repository عشان نجيب الحجز بالـ ID بتاعه
            
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialization)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null) return null;

            var baseUrl = _configuration["BaseUrl"];
            var culture = new System.Globalization.CultureInfo("ar-EG");

            return new AppointmentForPatientDetailsDto
            {
                Id = appointment.Id,
                DoctorName = appointment.Doctor?.FullName ?? "غير معروف",
                Specialization = appointment.Doctor?.Specialization?.Name ?? "عام",
                ImagePath = appointment.Doctor?.ImagePath == null ? null : $"{baseUrl}{appointment.Doctor.ImagePath}",

                Rate = 4.9,
                ReviewsCount = 320,

                // تنسيق التاريخ والوقت (اليوم - التاريخ - الساعة)
                DayName = appointment.Date.ToString("dddd", culture),
                Date = appointment.Date.ToString("dd MMMM yyyy", culture),
                Time = appointment.Date.ToString("hh:mm tt", culture),
                Location = "طنطا - أول شارع البحر",

                // الحسابات المالية
                ConsultationPrice = 150,
                Discount = 40,
                TotalAmount = 110,
                Status = appointment.Status ?? "Pending"
            };
        } // تفاصيل الحجز المكتمل في ال  patient 
        //public async Task<IEnumerable<PatientForAvailableSlotDto>> UpdateAvailableSlotsAsync(int doctorId, DateTime date)
        //{
        //    // 1. نجيب مواعيد عمل الدكتور في اليوم ده (مثلاً السبت من 10 لـ 4)
        //    var specs = new DoctorAvailabilitySpec(doctorId, date);
        //    var availabilities = await _unitOfWork.GetRepository<DoctorAvailability, int>().GetAllAsync(specs);

        //    var slots = availabilities.Select(a => new PatientForAvailableSlotDto()
        //    {
        //        Time = a.FromTime,
        //        IsReserved = a.IsClosed
        //    });




        //    return slots;
        //}  // تعديل الحجز في ال patient

        //public async Task<bool> UpdateBookAppointmentAsync(int patientId, BookAppointmentRequestDto bookAppointment)
        //{
        //    // 1. بنحول الـ DTO لـ Model (Appointment) عشان الداتابيز متفهمش DTO
        //    var appointment = new Appointment
        //    {
        //        DoctorId = bookAppointment.DoctorId,
        //        PatientId = patientId,
        //        // بنجمع التاريخ مع الساعة اللي اختارها
        //        Date = bookAppointment.SelectedDate,
        //        Status = "Pending",
        //        CreateAt = DateTime.Now
        //    };

        //    // 2. بنستخدم الـ UnitOfWork عشان نضيف الحجز
        //    await _unitOfWork.GetRepository<Appointment, int>().AddAsync(appointment);

        //    // 3. بنعمل Save Changes
        //    var result = await _unitOfWork.CompleteAsync();

        //    return result > 0;
        //} // تعديل الحجز في ال patient

        //public async Task<IEnumerable<PatientForAvailableSlotDto>> UpdateAvailableSlotsAsync(int doctorId, DateTime date)
        //{
        //    // 1. جلب مواعيد عمل الدكتور من جدول التوافر (Availability)
        //    var specs = new DoctorAvailabilitySpec(doctorId, date);
        //    var availabilities = await _unitOfWork.GetRepository<DoctorAvailability, int>().GetAllAsync(specs);

        //    // 2. جلب الحجوزات اللي تمت فعلاً عند الدكتور ده في اليوم ده
        //    // لازم نفلتر عشان مبيحصلش "تضارب" في المواعيد
        //    var bookedAppointments = await _unitOfWork.GetRepository<Appointment, int>()
        //        .GetAllAsync(new AppointmentsByDoctorAndDateSpec(doctorId, date));

        //    // 3. تحويل الداتا لـ DTO وفحص المتاح منها
        //    var slots = availabilities.Select(a => new PatientForAvailableSlotDto()
        //    {
        //        Time = a.FromTime,
        //        // الموعد يعتبر "محجوز" لو الدكتور قفل الخانة دي يدوياً 
        //        // أو لو فيه حجز في الـ Database بنفس الوقت ده
        //        IsReserved = a.IsClosed || bookedAppointments.Any(b => b.Date.TimeOfDay == a.FromTime)
        //    });

        //    return slots;
        //}
        public async Task<bool> UpdateBookAppointmentAsync(int patientId, UpdateBookAppointmentRequestDto request)
        {
            // 1. الوصول لجدول الحجوزات
            var repository = _unitOfWork.GetRepository<Appointment, int>();

            // 2. البحث عن الحجز القديم اللي المريض عايز يعدله
            var existingAppointment = await repository.GetByIdAsync(request.AppointmentId);

            // 3. التحقق من الأمان (Security Check)
            // لازم نتأكد إن الحجز موجود ومخص بالمرض اللي مسجل دخول فعلاً
            if (existingAppointment == null || existingAppointment.PatientId != patientId)
            {
                return false;
            }


            var AvailabilitySpec = new DoctorAvailabilityForPatientSpec(existingAppointment.DoctorId, request.SlotId);
            var availability = await _unitOfWork.GetRepository<DoctorAvailability, int>().GetAsync(AvailabilitySpec);

            // update is closed ==> true
            availability.IsClosed = true;
            _unitOfWork.GetRepository<DoctorAvailability, int>().Update(availability);

            // 4. تحديث القيم (التاريخ الجديد والدكتور الجديد)
            existingAppointment.Date = availability.SpecificDate.ToDateTime(TimeOnly.FromTimeSpan(availability.FromTime));
            existingAppointment.Status = "Pending"; // بنرجعه "قيد الانتظار" عشان الدكتور يوافق تاني

            // 5. إبلاغ الـ EF إننا عملنا تعديل
            repository.Update(existingAppointment);

            // 6. حفظ التغييرات في الداتابيز
            var result = await _unitOfWork.CompleteAsync();

            return result > 0; // لو أكبر من صفر يبقى التعديل نجح
        }
        public async Task<string> ProcessPaymentAsync(PaymentRequestDto paymentRequestDto)
        {
            // 1. جلب الحجز باستخدام GetByIdAsync
            var appointment = await _unitOfWork.GetRepository<Appointment, int>().GetByIdAsync(paymentRequestDto.AppointmentId);
            if (appointment == null) throw new Exception("الحجز غير موجود");

            // 2. جلب الدكتور عشان السعر (الـ 110 جنيه) لأننا مش مستخدمين Spec
            var doctor = await _unitOfWork.GetRepository<Doctor, int>().GetByIdAsync(appointment.DoctorId);
            decimal amount = doctor?.ConsultationPrice ?? 110;

            // 3. إنشاء كائن الـ Payment من الموديل بتاعك
            var payment = new Payment
            {
                AppointmentId = paymentRequestDto.AppointmentId,
                Amount = amount,
                Method = paymentRequestDto.PaymentMethod,
                Status = paymentRequestDto.PaymentMethod == "Cash" ? "Pending" : "Completed", // الكاش بيبقى معلق والفيزا مكتملة وهمياً
                TransactionId = paymentRequestDto.PaymentMethod == "Card" ? Guid.NewGuid().ToString() : null, // رقم عشوائي لو فيزا
                CreatedAt = DateTime.Now
            };

            // 4. تحديث حالة الحجز بناءً على الصور
            if (paymentRequestDto.PaymentMethod == "Cash")
            {
                appointment.Status = "Confirmed"; //
            }
            else
            {
                appointment.Status = "Paid"; //
            }

            // 5. حفظ في جدول الـ Payments والـ Appointments
            await _unitOfWork.GetRepository<Payment, int>().AddAsync(payment);
            await _unitOfWork.CompleteAsync();

            return "Process Completed Successfully";
        } //  الدفع في ال patient

        public async Task<bool> CancelAppointmentByIdAsync(int appointmentId)
        {
            // 1. نجيب الحجز من الداتابيز
            var appointment = await _unitOfWork.GetRepository<Appointment, int>().GetByIdAsync(appointmentId);

            if (appointment == null) return false;

            // 2. نغير الحالة لـ Cancelled (بناءً على شاشة التأكيد في الصورة)
            appointment.Status = "Cancelled";

            // 3. لو فيه عملية دفع مرتبطة بالحجز ده (وهمي طبعاً)، ممكن نغير حالة الدفع لـ Refunded
            // بس حالياً خلينا في الحجز نفسه

            _unitOfWork.GetRepository<Appointment, int>().Update(appointment);
            var result = await _unitOfWork.CompleteAsync();

            return result > 0;
        } // الغاء الحجز في ال Patient  الل بعد الدفع

      
    }
}
