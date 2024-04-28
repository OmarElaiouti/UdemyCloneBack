using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.DTOs;
using Udemy.Core.Models;
using Udemy.Core.Models.UdemyContext;
using Udemy.EF.UnitOfWork;
using System.Threading.Tasks;
using Udemy.Core.Interfaces.IRepositoris.IBaseRepository;

namespace Udemy.EF.Repository.NewFolder
{
    public class BaseRepository<T> : IBaseRepository<T>, IDisposable where T : class
    {
        private readonly UdemyContext _context;
        private readonly DbSet<T> _dbSet;
        private bool _disposed = false;

        public BaseRepository(IUnitOfWork<UdemyContext> unitOfWork)
            : this(unitOfWork.Context)
        {
        }
        public BaseRepository(UdemyContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public IEnumerable<T> GetAll(Func<T, bool> filter)
        {
            return _dbSet.Where(filter).ToList();
        }

        public async Task<IEnumerable<T>> GetAll(bool includeRelatedEntities = false, params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>();

                if (includeRelatedEntities && includeProperties != null && includeProperties.Any())
                {
                    query = IncludeRelatedEntities(query, includeProperties);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle exception appropriately, e.g., log the error
                Console.WriteLine($"An error occurred while fetching entities: {ex.Message}");
                throw; // Rethrow the exception for further handling
            }
        }
        public T GetById(object id)
        {
            return _dbSet.Find(id);
        }
        public async Task<T> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<T> GetById(object idValue, string idPropertyName, bool includeRelatedEntities = false, params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(GetIdPredicate(idValue, idPropertyName));

                if (includeRelatedEntities && includeProperties != null && includeProperties.Any())
                {
                    query = IncludeRelatedEntities(query, includeProperties);
                }

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // Handle exception appropriately, e.g., log the error
                Console.WriteLine($"An error occurred while fetching the entity by Id: {ex.Message}");
                throw; // Rethrow the exception for further handling
            }
        }
        public void Insert(T obj)
        {
            _dbSet.Add(obj);
        }

        public void Update(T obj)
        {
            _dbSet.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }

        public void Delete(T obj)
        {
            if (_context.Entry(obj).State == EntityState.Detached)
            {
                _dbSet.Attach(obj);
            }
            _dbSet.Remove(obj);
        }

        // Implement IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    


    #region private methods

    private IQueryable<T> IncludeRelatedEntities(IQueryable<T> query, params Expression<Func<T, object>>[] includeProperties)
    {
        foreach (var includeProperty in includeProperties)
        {
            if (includeProperty == null)
                continue;

            if (includeProperty.Body is MemberExpression memberExpression)
            {
                query = query.Include(includeProperty);
            }
            else if (includeProperty.Body is MethodCallExpression methodCall && methodCall.Method.Name == "Select")
            {
                var path = GetIncludePath(methodCall);
                if (!string.IsNullOrEmpty(path))
                {
                    query = query.Include(path);
                }
            }
        }

        return query;
    }

    private string GetIncludePath(MethodCallExpression methodCall)
    {
        var methodArguments = methodCall.Arguments;
        var lambdaExpression = (LambdaExpression)methodArguments[1];
        var memberExpression = (MemberExpression)lambdaExpression.Body;
        var memberName = memberExpression.Member.Name;
        var parentExpression = methodCall.Arguments[0];
        var parentMemberName = "";

        if (parentExpression is MethodCallExpression parentMethodCall)
        {
            parentMemberName = GetIncludePath(parentMethodCall);
        }
        else if (parentExpression is MemberExpression parentMember)
        {
            parentMemberName = parentMember.Member.Name;
        }

        return !string.IsNullOrEmpty(parentMemberName) ? $"{parentMemberName}.{memberName}" : memberName;
    }

    private Expression<Func<T, bool>> GetIdPredicate(object idValue, string idPropertyName)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, idPropertyName);
        var constant = Expression.Constant(idValue);
        var equalExpression = Expression.Equal(property, constant);
        return Expression.Lambda<Func<T, bool>>(equalExpression, parameter);
    }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Release managed resources
                    _context.Dispose();
                }

                // Release unmanaged resources

                _disposed = true;
            }
        }

        #endregion
    }
}
