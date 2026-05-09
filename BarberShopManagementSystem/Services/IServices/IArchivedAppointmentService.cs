using BarberShopManagementSystem.Data.Entities;

namespace BarberShopManagementSystem.API.Services.IServices
{
    public interface IArchivedAppointmentService
    {
        Task<IEnumerable<ArchivedAppointment>> GetAllArchivedAppointments();

        Task<IEnumerable<ArchivedAppointment>> GetAllArchivedAppointmentsWithIncludes();
        Task<ArchivedAppointment> AddArchivedAppointment(ArchivedAppointment archivedAppointment);

        void DeleteArchivedAppointment(ArchivedAppointment appointment);

        void UpdateArchivedAppointment(ArchivedAppointment appointment);
        Task<ArchivedAppointment?> GetByToken(string token);

        Task SaveArchivedAppointment();
    }
}
