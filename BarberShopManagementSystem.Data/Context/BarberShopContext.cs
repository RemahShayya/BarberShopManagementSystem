using BarberShopManagementSystem.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace BarberShopManagementSystem.Data.Context
{
    public class BarberShopContext : IdentityDbContext<User, Role, string>
    {
        public BarberShopContext(DbContextOptions<BarberShopContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var utcConverter = new ValueConverter<DateTime, DateTime>(
        v => v,                       // to DB
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // from DB
    );

            modelBuilder.Entity<Appointment>()
                .Property(a => a.StartTime)
                .HasConversion(utcConverter);

            modelBuilder.Entity<Appointment>()
                .Property(a => a.EndTime)
                .HasConversion(utcConverter);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CustomerAppointments)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.EmployeeAppointments)
                .WithOne(a => a.Employee)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // --------------------------
            // Service
            // --------------------------
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(s => s.Description)
                      .HasMaxLength(500);

                entity.Property(s => s.Price)
                      .HasColumnType("decimal(18,2)");

                entity.HasMany(s => s.Appointments)
                      .WithOne(a => a.Service)
                      .HasForeignKey(a => a.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --------------------------
            // Appointment
            // --------------------------
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.StartTime)
                      .IsRequired();

                // Persist EndTime as a regular column. It is computed in application code.
                entity.Property(a => a.EndTime)
                      .IsRequired();
            });

            // --------------------------
            // Review
            // --------------------------

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasIndex(r => r.AppointmentId).IsUnique();
                entity.HasKey(r => r.Id);


                entity.HasOne(r => r.Employee)
                      .WithMany()
                      .HasForeignKey(r => r.EmployeeId)
                      .OnDelete(DeleteBehavior.NoAction);  // keep as-is

                entity.HasOne(r => r.Customer)
                      .WithMany()
                      .HasForeignKey(r => r.CustomerId)
                      .OnDelete(DeleteBehavior.NoAction);  // keep as-is

                entity.Property(r => r.Rating).IsRequired();

                entity.Property(r => r.Comment)
                      .HasMaxLength(500)
                      .HasDefaultValue(string.Empty);

                entity.Property(r => r.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

            });
            // --------------------------
            // BarberSchedule
            // --------------------------
            modelBuilder.Entity<EmployeeSchedule>(entity =>
            {
                entity.HasKey(bs => bs.Id);

                entity.Property(bs =>
                bs.StartHour)
                      .IsRequired(false);

                entity.Property(bs => bs.EndHour)
                      .IsRequired(false);

                entity.HasOne(bs => bs.Employee)
                      .WithMany()
                      .HasForeignKey(bs => bs.EmployeeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // --------------------------
            // ArchivedAppointment
            // --------------------------
            modelBuilder.Entity<ArchivedAppointment>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.HasOne(a => a.Customer)
                      .WithMany()
                      .HasForeignKey(a => a.CustomerId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(a => a.Employee)
                      .WithMany()
                      .HasForeignKey(a => a.EmployeeId)
                      .OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<Review>(entity =>
{
    entity.HasIndex(r => r.AppointmentId).IsUnique();
    entity.HasKey(r => r.Id);

    entity.HasOne(r => r.Employee)
             .WithMany()
          .HasForeignKey(r => r.EmployeeId)
          .OnDelete(DeleteBehavior.NoAction);  // keep as-is

    entity.HasOne(r => r.Customer)
          .WithMany()
          .HasForeignKey(r => r.CustomerId)
          .OnDelete(DeleteBehavior.NoAction);  // keep as-is

    // ... rest of config
});

            // Deterministic role seeding: include fixed ConcurrencyStamp values
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = "b6c3f3d1-6a0d-4b7c-b5cb-1a0f83c0f184",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Description = "Administrator role with full permissions",
                    ConcurrencyStamp = "11111111-1111-1111-1111-111111111111"
                },
                new Role
                {
                    Id = "d9f02e77-4f3c-4a91-b4a7-0f8f9d9cc55d",
                    Name = "Customer",
                    NormalizedName = "CUSTOMER",
                    Description = "Customer role with limited permissions",
                    ConcurrencyStamp = "22222222-2222-2222-2222-222222222222"
                },
                new Role
                {
                    Id = "e7c9d2a5-1b3f-4f4b-9b3a-2a1e3f5b6d77",
                    Name = "Employee",
                    NormalizedName = "EMPLOYEE",
                    Description = "Employee role with limited permissions",
                    ConcurrencyStamp = "33333333-3333-3333-3333-333333333333"
                }
            );
        }

        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<EmployeeSchedule> EmployeeSchedules { get; set; }
        public DbSet<ArchivedAppointment> ArchivedAppointments { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<EmployeeProfession> EmployeeProfessions { get; set; }
    }
}
