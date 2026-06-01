using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using BarberShopManagementSystem.Data.Repositories;
using System.Linq.Expressions;

namespace BarberShopManagementSystem.API.Services
{
    public class ServicesService : IServicesService
    {
        private readonly IBarberShopGenericRepo<Service> _servicesRepo;

        public ServicesService(IBarberShopGenericRepo<Service> servicesRepo)
        {
            _servicesRepo = servicesRepo;
        }

        public async Task<IEnumerable<Service>> GetAllServices()
        {
            return await _servicesRepo.GetAll();
        }

        public async Task<IEnumerable<Service>> GetAllServicesWithIncludes(
            params Expression<Func<Service, object>>[] includes)
        {
            return await _servicesRepo.GetAllWithIncludes(includes);
        }

        public async Task<Service?> GetServiceById(Guid id)
        {
            return await _servicesRepo.GetById(id);
        }

        public async Task<Service?> GetServiceWithIncludes(
            Expression<Func<Service, bool>> filter,
            params Expression<Func<Service, object>>[] includes)
        {
            var result = await _servicesRepo.GetAllWithIncludes(filter, includes);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<Service>> GetServicesByProfession(
            Guid professionId,
            params Expression<Func<Service, object>>[] includes)
        {
            return await _servicesRepo.GetAllWithIncludes(
                filter: s => s.ProfessionId == professionId,
                includes);
        }

        public async Task<Service> AddService(Service service)
        {
            return await _servicesRepo.Insert(service);
        }

        public async Task<bool> UpdateService(Service service)
        {
            return await _servicesRepo.Update(service);
        }

        public async Task<bool> DeleteService(Guid id)
        {
            return await _servicesRepo.Delete(id);
        }

        public async Task Save()
        {
            await _servicesRepo.SaveAsync();
        }
    }
}
