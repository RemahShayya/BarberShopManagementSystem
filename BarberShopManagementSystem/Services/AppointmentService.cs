using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using BarberShopManagementSystem.Data.Repositories;

namespace BarberShopManagementSystem.API.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IBarberShopGenericRepo<Appointment> _appointmentRepo;
        public AppointmentService(IBarberShopGenericRepo<Appointment> appointmentRepo)
        {
            _appointmentRepo = appointmentRepo;
        }

        public async Task<Appointment> AddAppointment(Appointment appointment)
        {
            return await _appointmentRepo.Insert(appointment);
        }

        public void DeleteAppointment(Appointment appointment)
        {
            _appointmentRepo.Delete(appointment.Id);
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointments()
        {
            return await _appointmentRepo.GetAll();
        }

        public async Task<Appointment?> GetAppointmentById(Guid id)
        {
            return await _appointmentRepo.GetById(id);
        }

        public async Task SaveAppointment(Appointment appointment)
        {
            await _appointmentRepo.SaveAsync();
        }

        public bool UpdateAppointment(Appointment appointment)
        {
            return _appointmentRepo.Update(appointment);
        }
    }
}
