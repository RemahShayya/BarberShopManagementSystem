using BarberShopManagementSystem.Data.Entities;

namespace BarberShopManagementSystem.API.Services.IServices
{
    public interface IEmployeeScheduleService
    {
        Task<IEnumerable<EmployeeSchedule>> GetAllEmployeeSchedules();
        Task<EmployeeSchedule?> GetScheduleByEmployeeById(Guid id);
        Task<EmployeeSchedule> AddEmployeeSchedule(EmployeeSchedule employeeSchedule);
        void DeleteEmployeeSchedule(EmployeeSchedule employeeSchedule);
        Task<bool> UpdateEmployeeSchedule(EmployeeSchedule employeeSchedule);
        Task SaveEmployeeSchedule();
    }
}
