using BarberShopManagementSystem.Data.Entities;

namespace BarberShopManagementSystem.API.Services.IServices
{
    public interface IEmployeeProfessionService
    {
        Task<IEnumerable<EmployeeProfession>> GetAll();
        Task<EmployeeProfession?> GetById(Guid id);
        Task<EmployeeProfession> Add(EmployeeProfession employeeProfession);
        Task<bool> Update(EmployeeProfession employeeProfession);
        Task<bool> Delete(Guid id);
        Task Save();
        IQueryable<EmployeeProfession> Query();
    }
}
