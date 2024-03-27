using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.DTOs;
using Udemy.Core.Models;

namespace Udemy.Core.Interfaces
{
    public interface ICourseRepository<T> where T : class
    {

        List<T> Search(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    }
}
