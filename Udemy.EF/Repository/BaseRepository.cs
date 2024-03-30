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
     public class BaseRepository <T>: IBaseRepository<T> where T : class
    {
        private readonly UdemyContext _dbContext;

        public BaseRepository(UdemyContext dbContext)
        {
            _dbContext = dbContext;
        }



        public async Task<IEnumerable<T>> GetAll(bool includeRelatedEntities = false, params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                IQueryable<T> query = _dbContext.Set<T>();

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



        public async Task Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }


    }
}
