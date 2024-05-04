using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.DAl.Models;

namespace Udemy.BLL.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartByUserIdAsync(string userId);

    }
}
