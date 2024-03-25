﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.Core.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {

        T Get(int id);

        IEnumerable<T> GetAll();

    }
}
