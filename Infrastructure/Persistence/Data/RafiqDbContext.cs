using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Data
{
    public class RafiqDbContext : DbContext
    {
        public RafiqDbContext(DbContextOptions<RafiqDbContext> options)
        : base(options)
        {

        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Coupon> coupons { get; set; }
        public DbSet<Notification>  notifications { get; set; }
        public DbSet<Transaction> transactions { get; set; }
        public DbSet<Article> articles { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<ProfessionalFile> ProfessionalFiles { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<SavedArticle> SavedArticles { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(assemblyReference).Assembly);
            base.OnModelCreating(modelBuilder);

            // DoctorAvailability configuration
            modelBuilder.Entity<DoctorAvailability>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Doctor)
                      .WithMany(e => e.Availabilities)
                      .HasForeignKey(e => e.DoctorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Appointment configuration
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Doctor)
                      .WithMany(e => e.Appointments)
                      .HasForeignKey(e => e.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Patient)
                      .WithMany(e => e.Appointments)
                      .HasForeignKey(e => e.PatientId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.HasIndex(e => new { e.DoctorId, e.Date }).IsUnique();
            });
          
          

        }
    }
}
