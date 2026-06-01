using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarberShopManagementSystem.Data.Repositories
{
    public interface IBarberShopGenericRepo<T> where T : class
    {
        IQueryable<T> Query();
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(object id);
        Task<T> Insert(T entity);
        Task<bool> Update(T entity);
        public Task<bool> Delete(object id);
        Task SaveAsync();
        Task<bool> Exists(object id);
        Task<IEnumerable<T>> GetAllWithIncludes(
    params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> GetAllWithIncludes(
            Expression<Func<T, bool>>? filter,
            params Expression<Func<T, object>>[] includes);
    }

}

