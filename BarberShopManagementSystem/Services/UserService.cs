using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using BarberShopManagementSystem.Data.Repositories;

namespace BarberShopManagementSystem.API.Services
{
    public class UserService: IUserService
    {
        IBarberShopGenericRepo<User> _repo;
        public UserService(IBarberShopGenericRepo<User> repo)
        {
            _repo = repo;
        }

        public async Task<User> AddUser(User user)
        {
            return await _repo.Insert(user);
        }
        public void Delete(User user)
        {
            _repo.Delete(user);
        }
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _repo.GetAll();
        }
        public async Task<User?> GetUserById(string id)
        {
            var user = await _repo.GetById(id);
            return user;
        }
        public async Task<User?> Update(User user)
        {
            _repo.Update(user);
            return user;
        }
        public async Task Save(User user)
        {
            await _repo.SaveAsync();
        }
    }
}
