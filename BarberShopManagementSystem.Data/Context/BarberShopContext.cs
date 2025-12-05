using BarberShopManagementSystem.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

            modelBuilder.Entity<User>()
                .HasMany(u => u.CustomerAppointments)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.BarberAppointments)
                .WithOne(a => a.Barber)
                .HasForeignKey(a => a.BarberId)
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

                entity.Property(a => a.AppointmentStart)
                      .IsRequired();

                entity.Property(a => a.AppointmentDuration)
                      .IsRequired();

                // Computed column for end time (optional)
                entity.Ignore(a => a.AppointmentEndTime);

                // One-to-one Review
                entity.HasOne(a => a.Review)
                      .WithOne(r => r.Appointment)
                      .HasForeignKey<Review>(r => r.AppointmentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // --------------------------
            // Review
            // --------------------------
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Rating)
                      .IsRequired();

                entity.Property(r => r.Comment)
                      .HasMaxLength(500)
                      .HasDefaultValue(string.Empty);

                entity.Property(r => r.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(r => r.Barber)
                      .WithMany()
                      .HasForeignKey(r => r.BarberId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Customer)
                      .WithMany()
                      .HasForeignKey(r => r.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --------------------------
            // BarberSchedule
            // --------------------------
            modelBuilder.Entity<BarberSchedule>(entity =>
            {
                entity.HasKey(bs => bs.Id);

                entity.Property(bs => bs.StartHour)
                      .IsRequired();

                entity.Property(bs => bs.EndHour)
                      .IsRequired();

                entity.HasOne(bs => bs.Barber)
                      .WithMany()
                      .HasForeignKey(bs => bs.BarberId)
                      .OnDelete(DeleteBehavior.Cascade);
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
                    Name = "Barber",
                    NormalizedName = "BARBER",
                    Description = "Barber role with permission to manage appointments",
                    ConcurrencyStamp = "33333333-3333-3333-3333-333333333333"
                }
            );
        }

        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<BarberSchedule> BarberSchedules { get; set; }
    }
}
