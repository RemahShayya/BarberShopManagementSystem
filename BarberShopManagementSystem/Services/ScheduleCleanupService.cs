using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;

namespace BarberShopManagementSystem.API.Services
{
    public class ScheduleCleanupService
    {
        private readonly IBarberScheduleService _scheduleService;

        public ScheduleCleanupService(IBarberScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }
        public async Task DeleteOldServices()
        {
            var schedules = await _scheduleService.GetAllBarberSchedules();
            var oldSchedules = schedules
                .Where(a => a.Day < DateTime.UtcNow.Date)
                .ToList();
            foreach (var schedule in oldSchedules)
            {
                _scheduleService.DeleteBarberSchedule(schedule);
                await _scheduleService.SaveBarberSchedule();
            }
        }
    }
}
