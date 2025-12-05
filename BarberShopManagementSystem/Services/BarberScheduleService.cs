using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using BarberShopManagementSystem.Data.Repositories;

namespace BarberShopManagementSystem.API.Services
{
    public class BarberScheduleService : IBarberScheduleService
    {
        private readonly IBarberShopGenericRepo<BarberSchedule> _barberScheduleRepo;
        public BarberScheduleService(IBarberShopGenericRepo<BarberSchedule> barberScheduleRepo)
        {
            _barberScheduleRepo = barberScheduleRepo;
        }

        public async Task<BarberSchedule> AddBarberSchedule(BarberSchedule barberSchedule)
        {
            return await _barberScheduleRepo.Insert(barberSchedule);
        }

        public void DeleteBarberSchedule(BarberSchedule barberSchedule)
        {
            _barberScheduleRepo.Delete(barberSchedule.Id);
        }

        public async Task<IEnumerable<BarberSchedule>> GetAllBarberSchedules()
        {
            return await _barberScheduleRepo.GetAll();
        }

        public async Task<BarberSchedule?> GetBarberScheduleById(Guid id)
        {
            return await _barberScheduleRepo.GetById(id);
        }

        public async Task SaveBarberSchedule(BarberSchedule barberSchedule)
        {
            await _barberScheduleRepo.SaveAsync();
        }

        public bool UpdateBarberSchedule(BarberSchedule barberSchedule)
        {
            return _barberScheduleRepo.Update(barberSchedule);
        }

    }
}
