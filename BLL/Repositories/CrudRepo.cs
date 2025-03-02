using BLL.Interfaces;
using DataAccess.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CrudRepo<T, TKey> : ICrudRepo<T, TKey> where T : class
    {
        private ICrudDAO<T, TKey> _crudDAO;

        public CrudRepo(ICrudDAO<T, TKey> crudDAO)
        {
            this._crudDAO = crudDAO ?? throw new ArgumentNullException(nameof(crudDAO));
        }

        public async Task Add(T entity)
        {
            await _crudDAO.Add(entity);
        }

        public async Task<bool> Delete(T entity)
        {
            return await _crudDAO.Delete(entity);
        }

        public async Task Update(T entity)
        {
            await _crudDAO.Update(entity);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _crudDAO.GetAll();
        }

        public async Task<IEnumerable<T>> GetAllWithInclude(params Expression<Func<T, object>>[] includes)
        {
            return await _crudDAO.GetAllWithInclude(includes);
        }

        public async Task<T?> GetById(TKey id)
        {
            return await _crudDAO.GetById(id);
        }

        public async Task<T?> GetByIdWithInclude(TKey id, string propertyName, params Expression<Func<T, object>>[] includes)
        {
            return await _crudDAO.GetByIdWithInclude(id, propertyName, includes);
        }

        public async Task<T?> GetLast(Expression<Func<T, TKey>> orderBy)
        {
            return await _crudDAO.GetLast(orderBy);
        }
    }
}
