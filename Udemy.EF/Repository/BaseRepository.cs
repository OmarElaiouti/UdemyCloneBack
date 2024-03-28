using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.DTOs;
using Udemy.Core.Interfaces;
using Udemy.Core.Models;
using Udemy.Core.Models.UdemyContext;

namespace Udemy.EF.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly UdemyContext _dbContext;

        public BaseRepository(UdemyContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<T>> GetAll(bool includeRelatedEntities = false, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (includeRelatedEntities)
            {
                query = IncludeRelatedEntities(query, includeProperties);
            }

            return await query.ToListAsync();
        }

        private IQueryable<T> IncludeRelatedEntities(IQueryable<T> query, params Expression<Func<T, object>>[] includeProperties)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);

                var memberExpression = includeProperty.Body as MemberExpression;
                if (memberExpression != null)
                {
                    var propertyType = memberExpression.Type;
                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                    {
                        var elementType = propertyType.GetGenericArguments()[0];
                        // Get the parameter from the includeProperty expression
                        var parameter = Expression.Parameter(elementType, "x");
                        // Build the subInclude expression with the correct parameter
                        var subInclude = Expression.Lambda(Expression.Property(parameter, "Select"), parameter);
                        query = query.Provider.CreateQuery<T>(Expression.Call(typeof(EntityFrameworkQueryableExtensions), "Include", new Type[] { typeof(T), elementType }, query.Expression, subInclude));
                    }
                }
            }

            return query;
        }


    }
}
