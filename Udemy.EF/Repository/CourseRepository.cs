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
    public class CourseRepository<T> : ICourseRepository<T> where T : class
    {
        private readonly UdemyContext _dbContext;

        public CourseRepository(UdemyContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<T> Search(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbContext.Set<T>();

        // Include related entities and nested properties
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        // Apply filter based on the predicate
        query = query.Where(predicate);

        // Execute the query and return the result
        return query.ToList();
    }


    }
}
