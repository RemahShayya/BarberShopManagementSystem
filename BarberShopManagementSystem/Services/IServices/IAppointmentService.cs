using Azure.Core;
using BarberShopManagementSystem.Data.Entities;

namespace BarberShopManagementSystem.API.Services.IServices
{
    public interface IAppointmentService
    {
        IQueryable<Appointment> Query();
        Task<IEnumerable<Appointment>> GetAllAppointments();
        Task<Appointment?> GetAppointmentById(Guid id);
        Task<Appointment> AddAppointment(Appointment appointment);
        void DeleteAppointment(Guid Id);
        Task<bool> UpdateAppointment(Appointment appointment);
        Task SaveAppointment();
    }
}
