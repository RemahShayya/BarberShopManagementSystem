using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;


namespace BarberShopManagementSystem.API.Services
{
    public class AppointmentCleanupService
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IArchivedAppointmentService _archivedService;

        public AppointmentCleanupService(
            IAppointmentService appointmentService,
            IArchivedAppointmentService archivedService)
        {
            _appointmentService = appointmentService;
            _archivedService = archivedService;
        }

        public async Task ArchiveOldAppointments()
        {
            var appointments = await _appointmentService.GetAllAppointments();

            var finishedAppointments = appointments
                .Where(a => a.EndTime < DateTime.UtcNow)
                .ToList();

            foreach (var appointment in finishedAppointments)
            {
                var archived = new ArchivedAppointment
                {
                    StartTime = appointment.StartTime,
                    EndTime = appointment.EndTime,
                    CustomerId = appointment.CustomerId,
                    BarberId = appointment.BarberId,
                    ServiceId = appointment.ServiceId,
                    ReviewToken = Guid.NewGuid().ToString(),
                    ReviewEmailSent = false
                };

                await _archivedService.AddArchivedAppointment(archived);

                _appointmentService.DeleteAppointment(appointment.Id);
            }

            await _appointmentService.SaveAppointment();
        }
        public async Task DeleteOldArchivedAppointments()
        {
            var archivedAppointments = await _archivedService.GetAllArchivedAppointments();

            var oldArchived = archivedAppointments
                .Where(a => a.EndTime < DateTime.UtcNow.AddDays(-7))
                .ToList();

            foreach (var item in oldArchived)
            {
                _archivedService.DeleteArchivedAppointment(item);
            }

            await _archivedService.SaveArchivedAppointment();
        }
    }
}