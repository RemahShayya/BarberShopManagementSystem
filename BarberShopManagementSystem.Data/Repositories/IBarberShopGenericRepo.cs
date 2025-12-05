using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarberShopManagementSystem.Data.Repositories
{
    public interface IBarberShopGenericRepo<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(object id);
        Task<T> Insert(T entity);
        bool Update(T entity);
        void Delete(object id);
        Task SaveAsync();
        Task<bool> Exists(object id);
        Task<IEnumerable<T>> GetAllWithIncludes (params Expression<Func<T, object>>[] includes);
    }
}
