using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;

namespace BarberShopManagementSystem.API.Services
{
    public class ScheduleCleanupService
    {
        private readonly IEmployeeScheduleService _scheduleService;

        public ScheduleCleanupService(IEmployeeScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }
        public async Task DeleteOldServices()
        {
            var schedules = await _scheduleService.GetAllEmployeeSchedules();
            var oldSchedules = schedules
                .Where(a => a.Day < DateTime.UtcNow.Date)
                .ToList();
            foreach (var schedule in oldSchedules)
            {
                _scheduleService.DeleteEmployeeSchedule(schedule);
                await _scheduleService.SaveEmployeeSchedule();
            }
        }
    }
}
