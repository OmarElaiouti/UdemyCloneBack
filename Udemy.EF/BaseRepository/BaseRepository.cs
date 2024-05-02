
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Udemy.DAL.UnitOfWork;
using Udemy.DAL.UdemyContext;


namespace Udemy.DAL.BaseRepository
{
  

    public class BaseRepository<T> : IBaseRepository<T>  where T : class
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
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task Insert(T obj)
        {
            await _dbSet.AddAsync(obj);
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

       
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
    }

}
