using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using BarberShopManagementSystem.Data.Repositories;

namespace BarberShopManagementSystem.API.Services
{
    public class ProfessionService : IProfessionService
    {
        private readonly IBarberShopGenericRepo<Profession> _barberShopRepo;

        public ProfessionService(IBarberShopGenericRepo<Profession> barberShopRepo)
        {
            _barberShopRepo = barberShopRepo;
        }

        public async Task<Profession> AddProfession(Profession profession)
        {
            return await _barberShopRepo.Insert(profession);
        }

        public async Task<bool> DeleteProfession(Guid Id)
        {
            return await _barberShopRepo.Delete(Id);
        }

        public async Task<IEnumerable<Profession>> GetAllProfessions()
        {
            return await _barberShopRepo.GetAll();
        }


        public async Task<Profession?> GetProfessionById(Guid id)
        {
            return await _barberShopRepo.GetById(id);
        }

        public async Task<IEnumerable<Profession>> GetProfessionByName(string name)
        {
            return await _barberShopRepo.GetAllWithIncludes(p => p.Name == name);
        }

        public async Task<bool> HasLinkedServices(Guid professionId)
        {
            var result = await _barberShopRepo.GetAllWithIncludes(p => p.Id == professionId, p => p.Services);
            return result.Any();
        }

        public async Task<bool> HasLinkedEmployees(Guid professionId)
        {
            var result = await _barberShopRepo.GetAllWithIncludes(p => p.Id == professionId, p => p.EmployeeProfessions);
            return result.Any();
        }

        public async Task SaveProfession()
        {
            await _barberShopRepo.SaveAsync();
        }

        public async Task<bool> UpdateProfession(Profession profession)
        {
            return await _barberShopRepo.Update(profession);
        }

        public IQueryable<Profession> Query()
            => _barberShopRepo.Query();

    }
}

