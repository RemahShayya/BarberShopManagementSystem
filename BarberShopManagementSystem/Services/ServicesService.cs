using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using BarberShopManagementSystem.Data.Repositories;

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

        public async Task<Service?> GetServiceById(Guid id)
        {
            return await _servicesRepo.GetById(id);
        }

        public async Task<Service> AddService(Service service)
        {
            return await _servicesRepo.Insert(service);
        }

        public void DeleteService(Service service)
        {
            _servicesRepo.Delete(service.Id);
        }

        public async Task SaveService(Service service)
        {
            await _servicesRepo.SaveAsync();
        }

        public bool UpdateService(Service service)
        {
            return _servicesRepo.Update(service);
        }
    }
}
