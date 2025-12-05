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

        public void Delete(object id)
        {
            _context.Set<T>().Remove(_context.Set<T>().Find(id));
        }

        public async Task<bool> Exists(object id)
        {
            return await _context.Set<T>().FindAsync(id) != null;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllWithIncludes(params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

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

        public bool Update(T entity)
        {
            _context.Set<T>().Update(entity);
            return true;
        }
    }
}
