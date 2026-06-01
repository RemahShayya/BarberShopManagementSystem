using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using BarberShopManagementSystem.Data.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public void DeleteAppointment(Guid Id)
        {
            _appointmentRepo.Delete(Id);
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointments()
        {
            return await _appointmentRepo.GetAll();
        }


        public async Task<Appointment?> GetAppointmentById(Guid id)
        {
            return await _appointmentRepo.GetById(id);
        }

        public async Task SaveAppointment()
        {
            await _appointmentRepo.SaveAsync();
        }

        public async Task<bool> UpdateAppointment(Appointment appointment)
        {
            return await _appointmentRepo.Update(appointment);
        }

        public IQueryable<Appointment> Query()
            => _appointmentRepo.Query();

    }
}
