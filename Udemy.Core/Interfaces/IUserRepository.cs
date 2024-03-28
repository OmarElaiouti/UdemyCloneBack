using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.Models;

namespace Udemy.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(string userId);

    }
}
