using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using BarberShopManagementSystem.Data.Repositories;

namespace BarberShopManagementSystem.API.Services
{
    public class EmployeeProfessionService : IEmployeeProfessionService
    {
        private readonly IBarberShopGenericRepo<EmployeeProfession> _repo;

        public EmployeeProfessionService(IBarberShopGenericRepo<EmployeeProfession> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<EmployeeProfession>> GetAll()
        {
            return await _repo.GetAll();
        }

        public async Task<EmployeeProfession?> GetById(Guid id)
        {
            return await _repo.GetById(id);
        }

        public async Task<EmployeeProfession> Add(EmployeeProfession employeeProfession)
        {
            return await _repo.Insert(employeeProfession);
        }

        public async Task<bool> Update(EmployeeProfession employeeProfession)
        {
            return await _repo.Update(employeeProfession);
        }

        public async Task<bool> Delete(Guid id)
        {
            return await _repo.Delete(id);
        }

        public async Task Save()
        {
            await _repo.SaveAsync();
        }

        public IQueryable<EmployeeProfession> Query()
            => _repo.Query();
    }
}

