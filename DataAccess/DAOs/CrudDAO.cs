
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAOs
{
    public class CrudDAO<T, TKey> : ICrudDAO<T, TKey> where T : class
    {
        private readonly BakeryShopDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public CrudDAO(BakeryShopDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetById(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllWithInclude(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdWithInclude(TKey id, string propertyNameOfId, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }


            // Sử dụng EF.Property<TKey> để so sánh theo kiểu TKey
            // Do mỗi Model có id type khác nhau nên không thể dùng trực tiếp id để so sánh
            // Sử dụng Tkey để có thể sử dụng cho mọi kiểu dữ liệu id - dùng cho tất cả các model
            return await query.FirstOrDefaultAsync(e => EF.Property<TKey>(e, propertyNameOfId).Equals(id));
        }

        public async Task Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> Delete(T entity)
        {
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<T?> GetLast(Expression<Func<T, TKey>> orderBy)
        {
            return await _dbSet.OrderBy(orderBy).LastOrDefaultAsync();
        }
    }
}
