using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace BarberShopManagementSystem.API.Services
{
    public class BackgroundServices : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BackgroundServices(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var appointmentCleanup = scope.ServiceProvider
                    .GetRequiredService<AppointmentCleanupService>();

                var scheduleCleanup = scope.ServiceProvider
                    .GetRequiredService<ScheduleCleanupService>();

                var reviewNotificationService = scope.ServiceProvider
                    .GetRequiredService<ReviewNotificationService>();

                // CLEANUP (already correct)
                await appointmentCleanup.ArchiveOldAppointments();
                await appointmentCleanup.DeleteOldArchivedAppointments();
                await scheduleCleanup.DeleteOldServices();

                // REVIEW EMAILS (delegated service)
                await reviewNotificationService.SendPendingReviewEmails();

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
