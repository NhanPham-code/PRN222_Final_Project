﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAOs
{
    public interface ICrudDAO<T, TKey> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(TKey id);
        Task<IEnumerable<T>> GetAllWithInclude(params Expression<Func<T, object>>[] includes);
        Task<T?> GetByIdWithInclude(TKey id, string propertyNameOfId, params Expression<Func<T, object>>[] includes);
        Task Add(T entity);
        Task Update(T entity);
        Task<bool> Delete(T entity);
        Task<T?> GetLast(Expression<Func<T, TKey>> orderBy);
    }
}
