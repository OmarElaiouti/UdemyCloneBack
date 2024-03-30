using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.Models;

namespace Udemy.Core.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByUserIdAsync(string userId);

    }
}
