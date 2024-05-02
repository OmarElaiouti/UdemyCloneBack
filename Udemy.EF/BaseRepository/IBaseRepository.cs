using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.DAL.BaseRepository
{
    public interface IBaseRepository<T> :IDisposable where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(object id);
        Task Insert(T obj);
        void Update(T obj);
        void Delete(T obj);

    }
}
