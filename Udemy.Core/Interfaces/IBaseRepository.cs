
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.Models;

namespace Udemy.Core.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll(bool includeRelatedEntities = false, params Expression<Func<T, object>>[] includeProperties);

        Task Update(T entity);
    }
}
