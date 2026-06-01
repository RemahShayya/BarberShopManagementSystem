using System;
using System.Collections.Generic;
using System.Text;
using BarberShopManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BarberShopManagementSystem.Data.Context;

namespace BarberShopManagementSystem.Data.Repositories
{
    public class BarberShopGenericRepo<T> : IBarberShopGenericRepo<T> where T : class
    {
        private readonly BarberShopContext _context;
        public BarberShopGenericRepo(BarberShopContext context)
        {
            _context = context;
        }

        public IQueryable<T> Query()
        {
            return _context.Set<T>().AsQueryable();
        }

        public async Task<bool> Delete(object id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
                return false;

            _context.Set<T>().Remove(entity);
            return true;
        }

        public async Task<bool> Exists(object id)
        {
            return await _context.Set<T>().FindAsync(id) != null;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        // overload 1 — includes only, satisfies the original interface signature
        public async Task<IEnumerable<T>> GetAllWithIncludes(
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllWithIncludes(
            Expression<Func<T, bool>>? filter,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            if (filter is not null)
                query = query.Where(filter);

            foreach (var include in includes)
                query = query.Include(include);

            return await query.ToListAsync();
        }

        public async Task<T> GetById(object id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> Insert(T model)
        {
            var entity = await _context.Set<T>().AddAsync(model);
            return entity.Entity;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Update(T entity)
        {
            _context.Set<T>().Update(entity);
            return true;
        }

    }
}
