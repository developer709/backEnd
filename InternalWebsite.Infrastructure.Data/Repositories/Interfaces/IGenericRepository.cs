using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;
using InternalWebsite.Application.ViewModels;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Context;
using InternalWebsite.ViewModel.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace InternalWebsite.Infrastructure.Data.Repositories.Interfaces
{
    public interface IGenericRepository<T, TT> where T : class
    {
        /// <summary>
        /// This is added on temporary bases so that existing code does not break down
        /// </summary>
        public ClCongDbContext Context { get; }

        public IDbConnection _connection { get; }
        public long UserId { get; }
        public long OrganizationId { get; }
        Task<T> GetById(long id);
        Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate);
        void RemoveRange(IEnumerable<T> entities);
        abstract Task<T> Add(T entity);
        abstract Task<T> Add(params T[] entities);
        abstract Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        abstract IList<T> AddRange(IList<T> entities);
        Task<T> Update(T t);
        Task<T> Update(T t, object key);
        Task<T> UpdateAsync(T t, object key);
        Task Remove(T entity);
        Task<bool> Remove(long eId);
        Task<bool> ChangePublicState(long id, bool state);
        Task<IList<T>> GetAll();
        Task<(IList<T>, PaginatorDto)> GetAll(PaginatorDto paginatorDto);
        Task<(IList<T>, PaginatorDto)> GetAllBySP(PaginatorDto paginatorDto);
        Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate);

        Task<int> CountAll();
        Task<int> CountWhere(Expression<Func<T, bool>> predicate);
        void Save();
        Task<int> SaveAsync();

        bool BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        bool IsTransactionInProgress();
        
    }
}