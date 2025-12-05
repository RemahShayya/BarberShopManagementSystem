using BarberShopManagementSystem.Data.Entities;

namespace BarberShopManagementSystem.API.Services.IServices
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAppointments();
        Task<Appointment?> GetAppointmentById(Guid id);
        Task<Appointment> AddAppointment(Appointment appointment);
        void DeleteAppointment(Appointment appointment);
        bool UpdateAppointment(Appointment appointment);
        Task SaveAppointment(Appointment appointment);
    }
}
