using BarberShopManagementSystem.Data.Entities;
using System.Linq.Expressions;

namespace BarberShopManagementSystem.API.Services.IServices
{
    public interface IServicesService
    {
        Task<IEnumerable<Service>> GetAllServices();
        Task<IEnumerable<Service>> GetAllServicesWithIncludes(params Expression<Func<Service, object>>[] includes);
        Task<Service?> GetServiceById(Guid id);
        Task<Service?> GetServiceWithIncludes(Expression<Func<Service, bool>> filter, params Expression<Func<Service, object>>[] includes);
        Task<IEnumerable<Service>> GetServicesByProfession(Guid professionId, params Expression<Func<Service, object>>[] includes);
        Task<Service> AddService(Service service);
        Task<bool> UpdateService(Service service);
        Task<bool> DeleteService(Guid id);
        Task Save();
    }
}
