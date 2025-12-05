using BarberShopManagementSystem.Data.Entities;

namespace BarberShopManagementSystem.API.Services.IServices
{
    public interface IBarberScheduleService
    {
        Task<IEnumerable<BarberSchedule>> GetAllBarberSchedules();
        Task<BarberSchedule?> GetBarberScheduleById(Guid id);
        Task<BarberSchedule> AddBarberSchedule(BarberSchedule barberSchedule);
        void DeleteBarberSchedule(BarberSchedule barberSchedule);
        bool UpdateBarberSchedule(BarberSchedule barberSchedule);
        Task SaveBarberSchedule(BarberSchedule barberSchedule);
    }
}
