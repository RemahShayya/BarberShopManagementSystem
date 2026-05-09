using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using BarberShopManagementSystem.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberShopManagementSystem.API.Services
{
    public class ArchivedAppointmentService : IArchivedAppointmentService
    {
        private readonly IBarberShopGenericRepo<ArchivedAppointment> _repository;

        public ArchivedAppointmentService(IBarberShopGenericRepo<ArchivedAppointment> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ArchivedAppointment>> GetAllArchivedAppointments()
        {
            return await _repository.GetAll();
        }
        public async Task<IEnumerable<ArchivedAppointment>> GetAllArchivedAppointmentsWithIncludes()
        {
            return await _repository.GetAllWithIncludes(a => a.Barber, a => a.Customer);
        }

        public async Task<ArchivedAppointment> AddArchivedAppointment(ArchivedAppointment appointment)
        {
            await _repository.Insert(appointment);
            return appointment;
        }

        public void UpdateArchivedAppointment(ArchivedAppointment appointment)
        {
            _repository.Update(appointment);
        }
        public void DeleteArchivedAppointment(ArchivedAppointment appointment)
        {
            _repository.Delete(appointment);
        }

        public async Task<ArchivedAppointment?> GetByToken(string token)
        {
            return await _repository.Query().FirstOrDefaultAsync(a => a.ReviewToken == token);
        }

        public async Task SaveArchivedAppointment()
        {
            await _repository.SaveAsync();
        }
    }
}