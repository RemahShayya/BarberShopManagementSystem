using BarberShopManagementSystem.Data.Entities;

namespace BarberShopManagementSystem.API.Services.IServices
{
    public interface IProfessionService
    {
        Task<IEnumerable<Profession>> GetAllProfessions();
        Task<Profession?> GetProfessionById(Guid id);
        Task<IEnumerable<Profession?>> GetProfessionByName(string name);
        Task<Profession> AddProfession(Profession profession);
        Task<bool> DeleteProfession(Guid Id);
        Task<bool> UpdateProfession(Profession profession);
        Task<bool> HasLinkedServices(Guid professionId);
        Task<bool> HasLinkedEmployees(Guid professionId);
        IQueryable<Profession> Query();
        Task SaveProfession();
    }
}
