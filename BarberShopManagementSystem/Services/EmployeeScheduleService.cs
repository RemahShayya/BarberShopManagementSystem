using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using BarberShopManagementSystem.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberShopManagementSystem.API.Services
{
    public class EmployeeScheduleService : IEmployeeScheduleService
    {
        private readonly IBarberShopGenericRepo<EmployeeSchedule> _employeeScheduleRepo;
        public EmployeeScheduleService(IBarberShopGenericRepo<EmployeeSchedule> employeeScheduleRepo)
        {
            _employeeScheduleRepo = employeeScheduleRepo;
        }

        public async Task<EmployeeSchedule> AddEmployeeSchedule(EmployeeSchedule employeeSchedule)
        {
            return await _employeeScheduleRepo.Insert(employeeSchedule);
        }

        public void DeleteEmployeeSchedule(EmployeeSchedule employeeSchedule)
        {
            _employeeScheduleRepo.Delete(employeeSchedule.Id);
        }

        public Task DeleteEmployeeSchedule(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<EmployeeSchedule>> GetAllEmployeeSchedules()
        {
            return await _employeeScheduleRepo.GetAllWithIncludes(
                s => s.Employee
            );
        }

        public async Task<EmployeeSchedule?> GetScheduleByEmployeeById(Guid id)
        {
            return await _employeeScheduleRepo.GetById(id);
        }

        public async Task SaveEmployeeSchedule()
        {
            await _employeeScheduleRepo.SaveAsync();
        }

        public async Task<bool> UpdateEmployeeSchedule(EmployeeSchedule employeeSchedule)
        {
            return await _employeeScheduleRepo.Update(employeeSchedule);
        }

    }
}
