using BarberShopManagementSystem.Data.Entities;

namespace BarberShopManagementSystem.API.Services.IServices
{
    public interface IServicesService
    {
        Task<IEnumerable<Service>> GetAllServices();
        Task<Service?> GetServiceById(Guid id);
        Task<Service> AddService(Service service);
        void DeleteService(Service service);
        bool UpdateService(Service service);
        Task SaveService(Service service);
    }
}
