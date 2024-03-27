using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.Models.UdemyContext;
using Udemy.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace Udemy.EF.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly UdemyContext _dbContext;

        public BaseRepository(UdemyContext dbContext)
        {
            _dbContext = dbContext;
        }

        public T Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbContext.Set<T>().ToList();
        }

        //public IEnumerable<T> GetByQuery(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        //{
        //    var query = _dbContext.Set<T>().Where(predicate);

        //    // Include navigation properties
        //    foreach (var includeExpression in includes)
        //    {
        //        query = query.Include(includeExpression);
        //    }

        //    return query.ToList();
        //}

    }
}
