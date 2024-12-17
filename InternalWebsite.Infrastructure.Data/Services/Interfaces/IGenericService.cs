using InternalWebsite.ViewModel.DTOs;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services.Interfaces
{

    public interface IGenericService<T, TT> where T : class
    {
        Task<T> GetById(long id);
        Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate);

        abstract Task<T> Add(T entity);
        abstract Task<T> Add(params T[] entities);
        Task<T> Update(T t);
        Task<T> Update(T t, object key);
        Task<T> UpdateAsync(T t, object key);
        Task Remove(T entity);
        Task<bool> Remove(long eId);
        Task<bool> ChangePublicState(long id, bool state);
        Task<IList<T>> GetAll();
        Task<(IList<T>, PaginatorDto)> GetAll(PaginatorDto paginatorDto);
        Task<IEnumerable<TT>> GetWhere(Expression<Func<TT, bool>> predicate);

        Task<int> CountAll();
        Task<int> CountWhere(Expression<Func<TT, bool>> predicate);
    }
}
