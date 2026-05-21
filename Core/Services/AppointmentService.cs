using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Models;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence;
using Persistence.Data;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services;
using Rafiq.Api.Services.Abstractions;
using Services.Specifications;
using Shared;
using Shared.Enums;

namespace Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly RafiqDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public AppointmentService(RafiqDbContext context, IConfiguration configuration, IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _context = context;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

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
                Method = paymentRequestDto.PaymentMethod.ToString(),
                Status = paymentRequestDto.PaymentMethod == PaymentMethod.Cash ? "Pending" : "Completed", // الكاش بيبقى معلق والفيزا مكتملة وهمياً
                TransactionId = paymentRequestDto.PaymentMethod == PaymentMethod.Card ? Guid.NewGuid().ToString() : null, // رقم عشوائي لو فيزا
                CreatedAt = DateTime.Now
            };

            appointment.Status = "Confirmed";

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


        public async Task<UserProfileDto> GetUserProfileAsync(string identityUserId)
        {
            // بنجيب المريض اللي الـ UserId (الـ Guid) بتاعه بيطابق اللي جاي من التوكن
            var patient = (await _unitOfWork.GetRepository<Patient, int>().GetAllAsync())
                           .FirstOrDefault(p => p.userId == identityUserId);

            if (patient == null) return null;

            var appointments = await _unitOfWork.GetRepository<Appointment, int>().GetAllAsync();
            var count = appointments.Count(a => a.PatientId == patient.Id);

            return new UserProfileDto
            {
                FullName = patient.Name, // من الجدول اللي بعتهولي (Ali, hema, etc.)
                Age = 23,
                AppointmentsCount = count
            };
        } // الملف الشخصي في ال Patient

        public async Task<bool> UploadMedicalRecordAsync(string identityUserId, UploadRecordDto dto)
        {
            var patient = (await _unitOfWork.GetRepository<Patient, int>().GetAllAsync())
                           .FirstOrDefault(p => p.userId == identityUserId);

            if (patient == null) return false;

            var path = await SaveFileToLocal(dto.File);

            var record = new MedicalRecord
            {
                Title = dto.Title,
                FilePath = path,
                PatientId = patient.Id // بنربط بالـ int (1, 2, 1002...)
            };

            await _unitOfWork.GetRepository<MedicalRecord, int>().AddAsync(record);
            return await _unitOfWork.CompleteAsync() > 0;
        } // رفع ملف طبي في ال Patient في صفحه الملف الشخصي


        public async Task<bool> UpdateUserProfileAsync(string identityUserId, UpdateProfileDto dto)
        {
            var patient = (await _unitOfWork.GetRepository<Patient, int>().GetAllAsync())
                           .FirstOrDefault(p => p.userId == identityUserId);

            if (patient == null) return false;

            // نقل البيانات بأمان متوافق مع الـ Nullable
            patient.Name = dto.Name;
            patient.Age = dto.Age;       // هيقبل الـ int? عادي
            patient.Gender = dto.Gender;   // هيقبل الـ string? عادي

            // التليفون لو حابب تسيفه في الـ Patient برضه (لو ضفت العمود بعدين) 
            // أو تسيبه يسمع في جدول الـ Identity بس زي ما عملنا بالـ UserManager

            if (dto.Image != null)
            {
                patient.PictureUrl = await SaveFileToLocal(dto.Image);
            }

            _unitOfWork.GetRepository<Patient, int>().Update(patient);

            // تحديث بيانات الـ Identity (الإيميل والتليفون)
            var user = await _userManager.FindByIdAsync(identityUserId);
            if (user != null)
            {
                user.Email = dto.Email;
                user.UserName = dto.Email;
                user.NormalizedEmail = dto.Email.ToUpper();
                user.NormalizedUserName = dto.Email.ToUpper();
                user.PhoneNumber = dto.PhoneNumber; // تسييف التليفون في جدول الـ Users الأساسي

                var identityResult = await _userManager.UpdateAsync(user);
                if (!identityResult.Succeeded) return false;
            }

            return await _unitOfWork.CompleteAsync() > 0;
        } // تعديل الملف الشخصي في ال Patient


        public async Task<bool> UpdateMedicalProfileAsync(string identityUserId, UpdateMedicalProfileDto dto)
        {
            // 1. بندور على المريض بالـ UserId الـ Guid كالعادة
            var patient = (await _unitOfWork.GetRepository<Patient, int>().GetAllAsync())
                           .FirstOrDefault(p => p.userId == identityUserId);

            if (patient == null) return false;

            // 2. بنحول لستة الأمراض لنص واحد مفصول بفاصلة عشان يتسيف في عمود واحد
            if (dto.ChronicDiseases != null && dto.ChronicDiseases.Any())
            {
                patient.ChronicDiseases = string.Join(",", dto.ChronicDiseases);
            }
            else
            {
                patient.ChronicDiseases = "لا يوجد";
            }

            // 3. تحديث باقي البيانات الطبية
            patient.Allergy = dto.Allergy;
            patient.BloodType = dto.BloodType;
            patient.Height = dto.Height;
            patient.Weight = dto.Weight;

            // 4. حفظ في الداتابيز
            _unitOfWork.GetRepository<Patient, int>().Update(patient);
            return await _unitOfWork.CompleteAsync() > 0;
        } // تعديل الملف الطبي في ال Patient

        public async Task<IdentityResult> ChangeUserPasswordAsync(string identityUserId, ChangePasswordDto dto)
        {
            // 1. هنجيب اليوزر من جدول الـ Identity الأساسي باستخدام الـ Guid
            var user = await _userManager.FindByIdAsync(identityUserId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "المستخدم غير موجود" });
            }

            // 2. بننادي ميثود الـ Identity الجاهزة وهي بتتكفل بالتشفير والمقارنة
            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            return result;
        } // تغيير كلمة المرور في ال Patient

        public async Task<List<MedicalFileResponseDto>> GetPatientFilesAsync(string identityUserId, string? fileType)
        {
            // الحصول على الـ PatientId أولاً
            var patient = (await _unitOfWork.GetRepository<Patient, int>().GetAllAsync())
                           .FirstOrDefault(p => p.userId == identityUserId);

            if (patient == null) return new List<MedicalFileResponseDto>();

            var filesRepo = _unitOfWork.GetRepository<MedicalRecord, int>();
            var allFiles = await filesRepo.GetAllAsync();

            // فلترة الملفات الخاصة بالمريض، وفلترتها حسب النوع لو الـ Frontend طالب نوع معين (الكل، تحاليل، إلخ)
            var query = allFiles.Where(f => f.PatientId == patient.Id);

            if (!string.IsNullOrEmpty(fileType) && fileType != "الكل")
            {
                query = query.Where(f => f.FileType == fileType);
            }

            return query.Select(f => new MedicalFileResponseDto
            {
                Id = f.Id,
                Title = f.Title,
                FilePath = f.FilePath,
                FileType = f.FileType,
                // تنسيق التاريخ للغة العربية (مثال: ديسمبر 2025)
                FormattedDate = f.CreatedAt.ToString("MMMM yyyy", new System.Globalization.CultureInfo("ar-EG")),
                Notes = f.Notes
            }).ToList();
        } // رفع الملف الطبي في ال Patient

        public async Task<bool> UploadMedicalFileAsync(string identityUserId, UploadFileDto dto)
        {
            var patient = (await _unitOfWork.GetRepository<Patient, int>().GetAllAsync())
                           .FirstOrDefault(p => p.userId == identityUserId);

            if (patient == null) return false;

            // حفظ الملف الفعلي على السيرفر وجلب المسار لـ تسييفه
            string folderPath = "Files/MedicalDocuments";
            string savedFilePath = await SaveFileToLocal(dto.File); // الميثود الجاهزة عندك لحفظ الملفات

            // تحديد اسم افتراضي للملف بناءً على نوعه واسم المريض أو اسم الملف الأصلي
            //string documentTitle = dto.FileType + " - " + Path.GetFileNameWithoutExtension(dto.File.FileName);

            var medicalFile = new MedicalRecord
            {
                Title = dto.FileName,
                FilePath = savedFilePath,
                FileType = dto.FileType,
                Notes = dto.Notes,
                PatientId = patient.Id
            };

            await _unitOfWork.GetRepository<MedicalRecord, int>().AddAsync(medicalFile);
            return await _unitOfWork.CompleteAsync() > 0;
        } // رفع الملف الطبي في ال Patient


        
        public async Task<List<ArticleListDto>> GetAllArticlesAsync(string? category, string? searchKey)
        {
            var baseUrl = _configuration["BaseUrl"];
            var articlesRepo = _unitOfWork.GetRepository<Article, int>();
            var articles = await articlesRepo.GetAllAsync(); // أو استخدم Include لو عندك

            var query = articles.AsQueryable();

            // فلترة بالتصنيف (الكل، تغذية، نصائح عامة)
            if (!string.IsNullOrEmpty(category) && category != "جميع المقالات")
            {
                query = query.Where(a => a.Category == category);
            }

            // بحث بالكلمة في العنوان أو الوصف
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(a => a.Title.Contains(searchKey) || a.ShortDescription.Contains(searchKey));
            }

            return query.Select(a => new ArticleListDto
            {
                Id = a.Id,
                Title = a.Title,
                ShortDescription = a.ShortDescription,
                ImagePath = baseUrl + a.ImagePath,
                Category = a.Category,
                TimeAgo = GetTimeAgo(a.CreateAt) 
            }).ToList();
        }  // المقالات في ال Patient

       
        public async Task<ArticleDetailsDto> GetArticleDetailsAsync(int id)
        {
            var baseUrl = _configuration["BaseUrl"];
            var article = (await _unitOfWork.GetRepository<Article, int>().GetAllAsync())
                           .FirstOrDefault(a => a.Id == id);

            if (article == null) return null;

            var doctor = (await _unitOfWork.GetRepository<Doctor, int>().GetAllAsync())
                          .FirstOrDefault(d => d.Id == article.DoctorId);

            // جلب مقالات ذات صلة من نفس الفئة باستثناء المقال الحالي (بحد أقصى 2 كالتصميم)
            var allArticles = await _unitOfWork.GetRepository<Article, int>().GetAllAsync();
            var related = allArticles
                .Where(a => a.Category == article.Category && a.Id != id)
                .Take(2)
                .Select(a => new ArticleListDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    ShortDescription = a.ShortDescription,
                    ImagePath = a.ImagePath,
                    Category = a.Category,
                    TimeAgo = GetTimeAgo(a.CreateAt)
                }).ToList();

            return new ArticleDetailsDto
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                ImagePath = baseUrl + article.ImagePath,
                Category = article.Category,
                TimeAgo = GetTimeAgo(article.CreateAt),
                DoctorName = doctor?.FullName ?? "طبيب رفيق",
                DoctorImagePath = doctor != null ? baseUrl + doctor.ImagePath : null,
                RelatedArticles = related
            };
        }  // تفاصيل المقال في ال Patient

        public async Task<List<ArticleListDto>> GetSavedArticlesAsync(string identityUserId)
        {
            var baseUrl = _configuration["BaseUrl"];

            var patient = (await _unitOfWork.GetRepository<Patient, int>().GetAllAsync())
                           .FirstOrDefault(p => p.userId == identityUserId);

            if (patient == null) return new List<ArticleListDto>();

            // هنجيب داتا جدول الربط ونعمل فلترة بـ Id المريض
            var savedRepo = _unitOfWork.GetRepository<SavedArticle, int>();
            var articlesRepo = _unitOfWork.GetRepository<Article, int>();

            var savedRelations = (await savedRepo.GetAllAsync()).Where(s => s.PatientId == patient.Id).ToList();
            var allArticles = await articlesRepo.GetAllAsync();

            // نربط الجداول ببعض عشان نرجع تفاصيل المقال
            var savedArticles = allArticles
                .Where(a => savedRelations.Any(s => s.ArticleId == a.Id))
                .Select(a => new ArticleListDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    ShortDescription = a.ShortDescription,
                    ImagePath = baseUrl + a.ImagePath,
                    Category = a.Category,
                    TimeAgo = "منذ يومين" // الميثود المساعدة المرة اللي فاتت GetTimeAgo(a.CreatedAt)
                }).ToList();

            return savedArticles;
        } // المقالات المحفوظة في ال Patient

        // 2. حفظ أو إلغاء حفظ المقال (Toggle Save)
        public async Task<string> ToggleSaveArticleAsync(string identityUserId, int articleId)
        {
            var patient = (await _unitOfWork.GetRepository<Patient, int>().GetAllAsync())
                           .FirstOrDefault(p => p.userId == identityUserId);

            if (patient == null) return "المريض غير مسجل";

            var savedRepo = _unitOfWork.GetRepository<SavedArticle, int>();

            // نشوف هل المقال ده اتحفظ قبل كده للمريض ده ولا لأ
            var existingSave = (await savedRepo.GetAllAsync())
                .FirstOrDefault(s => s.PatientId == patient.Id && s.ArticleId == articleId);

            if (existingSave != null)
            {
                // لو موجود يبقى اليوزر عايز يلغي الحفظ (Unsave)
                savedRepo.Delete(existingSave.Id);
                await _unitOfWork.CompleteAsync();
                return "تم إزالة المقال من المحفوظات";
            }
            else
            {
                // لو مش موجود يبقى اليوزر عايز يحفظه
                var newSave = new SavedArticle
                {
                    PatientId = patient.Id,
                    ArticleId = articleId
                };
                await savedRepo.AddAsync(newSave);
                await _unitOfWork.CompleteAsync();
                return "تم حفظ المقال بنجاح";
            }
        } // حفظ المقال في ال Patient


        public async Task<bool> CreateNotificationAsync(CreateNotificationDto dto)
        {
            var notificationsRepo = _unitOfWork.GetRepository<Notification, int>();

            var newNotification = new Notification
            {
                PatientId = dto.PatientId,
                Title = dto.Title,
                Message = dto.Message,
                Type = dto.Type,
                IsRead = false,
                CreateAt = DateTime.UtcNow
            };

            await notificationsRepo.AddAsync(newNotification);
            return await _unitOfWork.CompleteAsync() > 0;
        }
        // 1. جلب كل إشعارات المريض الحالي مرتبة من الأحدث للأقدم
        public async Task<List<NotificationResponseDto>> GetPatientNotificationsAsync(string identityUserId)
        {
            var patient = (await _unitOfWork.GetRepository<Patient, int>().GetAllAsync())
                           .FirstOrDefault(p => p.userId == identityUserId);

            if (patient == null) return new List<NotificationResponseDto>();

            var notificationsRepo = _unitOfWork.GetRepository<Notification, int>();
            var allNotifications = await notificationsRepo.GetAllAsync();

            // فلترة الإشعارات الخاصة بالمريض وترتيبها تنازلياً حسب التاريخ
            var patientNotifications = allNotifications
                .Where(n => n.PatientId == patient.Id)
                .OrderByDescending(n => n.CreateAt)
                .Select(n => new NotificationResponseDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    TimeAgo = GetNotificationTimeAgo(n.CreateAt) // ميثود حساب الوقت الرايقة
                }).ToList();

            return patientNotifications;
        }

        // 2. تحديث حالة الإشعارات لتصبح مقروءة بمجرد فتح الشاشة
        public async Task<bool> MarkAllAsReadAsync(string identityUserId)
        {
            var patient = (await _unitOfWork.GetRepository<Patient, int>().GetAllAsync())
                           .FirstOrDefault(p => p.userId == identityUserId);

            if (patient == null) return false;

            var notificationsRepo = _unitOfWork.GetRepository<Notification, int>();
            var unreadNotifications = (await notificationsRepo.GetAllAsync())
                .Where(n => n.PatientId == patient.Id && !n.IsRead).ToList();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notificationsRepo.Update(notification);
            }

            return await _unitOfWork.CompleteAsync() > 0;
        }

     

















        // ميثود مساعدة لضبط الوقت بالظبط زي التصميم (قبل 15 دقيقة، قبل يوم...)
        private string GetNotificationTimeAgo(DateTime createdAt)
        {
            var span = DateTime.UtcNow - createdAt;
            if (span.TotalMinutes < 60) return $"قبل {Math.Max(1, (int)span.TotalMinutes)} دقيقة";
            if (span.TotalHours < 24) return span.TotalHours >= 2 ? $"قبل {(int)span.TotalHours} ساعات" : "قبل ساعة";
            if (span.TotalDays < 2) return "قبل يوم";
            if (span.TotalDays < 3) return "قبل يومين";
            return createdAt.ToString("yyyy/MM/dd");
        }
        // ميثود مساعدة بسيطة لتهيئة الوقت زي الـ UI
        private string GetTimeAgo(DateTime dateTime)
        {
            var span = DateTime.UtcNow - dateTime;
            if (span.Days >= 2) return "منذ يومين";
            if (span.Days >= 1) return "منذ يوم";
            return "مؤخراً";
        }

        private async Task<string> SaveFileToLocal(IFormFile file)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Records");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/Files/Records/{fileName}";
        } // ميثود لرفع الملف
    }
}
