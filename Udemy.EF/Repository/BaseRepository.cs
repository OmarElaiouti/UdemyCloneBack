using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.Models.UdemyContext;
using Udemy.Core.Interfaces;


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
    }
}
