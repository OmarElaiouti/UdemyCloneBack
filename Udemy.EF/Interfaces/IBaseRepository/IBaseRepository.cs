
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.DAL.Interfaces.IBaseRepository
{
    public interface IBaseRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(Func<T, bool> filter);
        Task<IEnumerable<T>> GetAll(bool includeRelatedEntities = false, params Expression<Func<T, object>>[] includeProperties);

        T GetById(object id);
        Task<T> GetById(object idValue, string idPropertyName, bool includeRelatedEntities = false, params Expression<Func<T, object>>[] includeProperties);

        void Insert(T obj);
        void Update(T obj);
        void Delete(T obj);

    }

}
