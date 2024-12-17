using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using InternalWebsite.Application.Utils;
using InternalWebsite.Core.Entities;
using InternalWebsite.Core.Interfaces;
using InternalWebsite.Infrastructure.Data.Context;
using InternalWebsite.Infrastructure.Data.Repositories.Interfaces;
using InternalWebsite.Infrastructure.Data.Services;
using Dapper;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Task = System.Threading.Tasks.Task;
using InternalWebsite.ViewModel.DTOs;

namespace InternalWebsite.Infrastructure.Data.Repositories
{
    public abstract class GenericRepository<T, TT> : IDisposable, IGenericRepository<T, TT>
        where T : class where TT : class
    {
        protected readonly ClCongDbContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ClCongPrincipal _iPrincipal;
        private readonly IConfiguration _configuration;
        public IDbContextTransaction _dbTransaction { get; protected set; }
        public IDbConnection _connection
        {
            get
            {
                return _context.Database.GetDbConnection() as IDbConnection;
                //return _context.Database.CurrentTransaction.GetDbTransaction().Connection as IDbConnection;
            }
        }
        public long UserId
        {
            get
            {
                return Convert.ToInt64(_iPrincipal.UserId);
            }
        }
        public long OrganizationId
        {
            get
            {
                var orgIds = _httpContextAccessor?.HttpContext?.Request.Headers["OrganizationId"];
                if (orgIds.HasValue)
                {
                    var orgId = long.Parse(orgIds.GetValueOrDefault() != "null" ? orgIds.GetValueOrDefault()[0] : "0");
                    return orgId;
                }
                return 0;
            }
        }

        public ClCongDbContext Context
        {
            get
            {
                return _context;
            }
        }
        public GenericRepository(ClCongDbContext context, IHttpContextAccessor httpContextAccessor, IPrincipal iPrincipal)
        {
            _context = context;
            _iPrincipal = new ClCongPrincipal(iPrincipal);
            _httpContextAccessor = httpContextAccessor;
        }

        #region Public Methods

        //public Task<T> GetById(long id) => _context.Set<T>().FindAsync(id);
        public async Task<T> GetById(long id)
        {
            return (await _context.Set<T>().FindAsync(id)).Adapt<T>();
        }
        private SqlConnection SqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
        public Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate)
            => _context.Set<T>().FirstOrDefaultAsync(predicate);
        public virtual async Task<T> Add(params T[] entity)
        {
            // await _context.AddAsync(entity);
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(Add)} entity must not be null");
            }
            try
            {
                var s = await _context.Set<T>().AddAsync(entity[0]);
                //await _context.SaveChangesAsync();
                return s.Entity;
            }
            catch (Exception ex)
            {

                throw new Exception($"{nameof(entity)} could not be saved: {ex.Message}");
            }

        }
        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            if (entities.Count() <= 0)
            {
                throw new ArgumentNullException($"{nameof(RemoveRange)} entity must not be null");
            }
            try
            {
                _context.RemoveRange(entities);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new Exception($"{nameof(entities)} could not be saved: {ex.Message}");
            }
        }
        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            // await _context.AddAsync(entity);
            if (entities.Count() <= 0)
            {
                throw new ArgumentNullException($"{nameof(AddRangeAsync)} entity must not be null");
            }
            try
            {
                await _context.Set<T>().AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                return entities;
            }
            catch (Exception ex)
            {

                throw new Exception($"{nameof(entities)} could not be saved: {ex.Message}");
            }

        }
        public virtual IList<T> AddRange(IList<T> entities)
        {
            // await _context.AddAsync(entity);
            if (entities.Count() <= 0)
            {
                throw new ArgumentNullException($"{nameof(AddRange)} entity must not be null");
            }
            try
            {
                _context.Set<T>().AddRange(entities);
              //  _context.SaveChanges();
                return entities;
            }
            catch (Exception ex)
            {

                throw new Exception($"{nameof(entities)} could not be saved: {ex.Message}");
            }

        }
        public virtual async Task<T> Update(T t)
        {
            if (t == null)
            {
                throw new ArgumentNullException($"{nameof(Update)} entity must not be null");
            }
            try
            {
                if (t == null)
                    return null;
                var exist = await _context.Set<T>().FindAsync(((IGeneralBase)t).CreatedById);// also check it
                if (exist == null) return null;
                _context.Entry(exist).CurrentValues.SetValues(t);
                await _context.SaveChangesAsync();
                return exist;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(t)} could not be updated: {ex.Message}");
            }
        }

        public virtual async Task<T> Add(T entity)
        {
            // await _context.AddAsync(entity);
            return await Add(new[] { entity });
        }

        public virtual async Task<T> Update(T t, object key)
        {
            if (t == null)
            {
                throw new ArgumentNullException($"{nameof(Update)} entity must not be null");
            }
            try
            {
                if (t == null)
                    return null;
                var exist = _context.Set<T>().Find(key);
                if (exist == null) return null;
                _context.Entry(exist).CurrentValues.SetValues(t);
                _context.SaveChanges();
                return exist;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(t)} could not be updated: {ex.Message}");
            }


        }

        public virtual async Task<T> UpdateAsync(T t, object key)
        {
            if (t == null)
            {
                throw new ArgumentNullException($"{nameof(Update)} entity must not be null");
            }
            try
            {
                if (t == null)
                    return null;
                T exist = await _context.Set<T>().FindAsync(key);
                if (exist != null)
                {
                    _context.Entry(exist).CurrentValues.SetValues(t);
                    await _context.SaveChangesAsync();
                }
                throw new Exception($"{nameof(t)} could not be updated");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(t)} could not be updated: {ex.Message}");
            }
        }

        public virtual Task Remove(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(Remove)} entity must not be null");
            }
            try
            {
                _context.Set<T>().Remove(entity);
                return _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be Remove: {ex.Message}");
            }

        }

        public virtual async Task<bool> Remove(long id)
        {
            try
            {
                var entity = await _context.Set<TT>().FindAsync(id);
                var typeName = typeof(TT).Name;
                if (entity == null) throw new NullReferenceException($"No {typeName} found with the Provided Id!");
                if (_context.Entry(entity).Property("IsActive") == null)
                    throw new NullReferenceException(
                        $"Invalid Property Change for Entity {typeName}! Entity not Deletable. Are you sure you aren't Missing an IsActive field in Db?");
                _context.Entry(entity).Property<bool?>("IsActive").CurrentValue = false;
                _context.Attach(entity).State = EntityState.Modified;
                var r = await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public virtual async Task<bool> ChangePublicState(long id, bool state)
        {
            try
            {
                var entity = await _context.Set<TT>().FindAsync(id);
                var typeName = typeof(TT).Name;
                if (entity == null) throw new NullReferenceException($"No {typeName} found with the Provided Id!");
                if (_context.Entry(entity).Property("IsPublic") == null)
                    throw new NullReferenceException(
                        $"Invalid Property Change for Entity {typeName}! No IsPublic Property Found.");
                _context.Entry(entity).Property<bool>("IsPublic").CurrentValue = state;
                _context.Attach(entity).State = EntityState.Modified;
                var r = await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public virtual async Task<IList<T>> GetAll()
        {
            return await _context.Set<TT>().Select(f => f.Adapt<T>()).ToListAsync();
        }

        public virtual async Task<(IList<T>, PaginatorDto)> GetAll(PaginatorDto paginatorDto)
        {
            return (await _context.Set<T>().ToListAsync(), paginatorDto);
        }

        public virtual async Task<(IList<T>, PaginatorDto)> GetAllBySP(PaginatorDto paginatorDto)
        {
            return (await _context.Set<T>().ToListAsync(), paginatorDto);
        }
        protected IQueryable<T> GetWhereQ(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public Task<int> CountAll() => _context.Set<T>().CountAsync();

        public Task<int> CountWhere(Expression<Func<T, bool>> predicate)
            => _context.Set<T>().CountAsync(predicate);

        #endregion


        public virtual void Save()
        {
            _context.SaveChanges();
        }

        public async virtual Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Changed this to Property and directly access UserId
        /// </summary>
        /// <returns></returns>
        protected string GetUserId()
        {
            // This is the Alternative Way

            //_iPrincipal.UserId;
            //var username = claims.Where(p => p.Type == "UserId").FirstOrDefault().Value;
            return (_iPrincipal.UserId.ToString());
        }
        protected Guid GetUserDetailId()
        {
            var userId = GetUserId();
            if (Guid.TryParse(userId, out Guid guidValue))
                return (guidValue);
            return Guid.Empty;
        }

        /// <summary>
        /// Changed this to Property and directly access OrganizationId
        /// </summary>
        /// <returns></returns>
        protected long GetOrganizationId()
        {
            var orgIds = _httpContextAccessor?.HttpContext?.Request.Headers["OrganizationId"];
            if (orgIds.HasValue)
            {
                var orgId = long.Parse(orgIds.GetValueOrDefault() != "null" ? orgIds.GetValueOrDefault()[0] : "0");
                return orgId;
            }
            return 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public bool BeginTransaction()
        {
            if (_dbTransaction == null)
                _dbTransaction = _context.Database.BeginTransaction();
            return _dbTransaction != null;
        }

        public bool IsTransactionInProgress()
        {
            return _dbTransaction != null;
        }
        public void CommitTransaction()
        {
            Save();
            if (_dbTransaction != null)
                _dbTransaction.Commit();
        }
        public void RollbackTransaction()
        {
            if (_dbTransaction == null)
                _dbTransaction.Rollback();
        }

        public static string WriteBase64ToImageFile(string base64)
        {
           string ImagesDirectory = "Resources/Images";
           var root = "";
            try
            {
                if (string.IsNullOrWhiteSpace(base64)) return null;
                var guid = Guid.NewGuid().ToString();
                base64 = base64.Replace('-', '+');
                base64 = base64.Replace('_', '/');

                var splitString = base64.Split(";base64,");
                var fileType = splitString[0];

                var extension = ".jpg";

                var data = base64.Replace(fileType + ";base64,", "");

                if (fileType.Contains("image/jpeg"))
                {
                    extension = ".jpeg";
                }
                else if (fileType.Contains("image/png"))
                {
                    extension = ".png";
                }
                else if (fileType.Contains("application/pdf"))
                {
                    extension = ".pdf";
                }
                else if (fileType.Contains("application/vnd.ms-excel") || fileType.Contains("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                {
                    extension = ".xlsx";
                }
                //base64 = base64.Replace("data:image/png;base64,", "");
                //base64 = base64.Replace("data:image/jpeg;base64,", "");
                var imageBytes = Convert.FromBase64String(data);

                var basePath = Directory.GetCurrentDirectory() + "/" + ImagesDirectory;
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);
                File.WriteAllBytes(basePath + "/" + $"{guid}" + extension, imageBytes);
                var path = ImagesDirectory + "/" + $"{guid}" + extension;
                //return guid;
                return path;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}