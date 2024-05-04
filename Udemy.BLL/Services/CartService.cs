using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Udemy.DAl.Models;
using Udemy.DAL.GenericBaseRepository.BaseRepository;
using Udemy.DAL.Context;
using Udemy.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Udemy.BLL.Interfaces;

namespace Udemy.BLL.Services
{
    public class CartService : ICartService
    {
        private readonly UdemyContext _context;

        public CartService(
            UdemyContext context)
        {
            _context = context;


        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            
            var cart = _context.Carts.FirstOrDefault(c => c.UserID == userId);

            if(cart ==null) { return null; }

            return cart;

        }

        



       
    }
}
