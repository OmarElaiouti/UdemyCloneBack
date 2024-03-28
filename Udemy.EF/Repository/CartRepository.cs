using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.Interfaces;
using Udemy.Core.Models.UdemyContext;
using Udemy.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Udemy.EF.Repository
{
    public class CartRepository : BaseRepository<Cart>, ICartRepository
    {
        private readonly IBaseRepository<Cart> _cartRepository;

        public CartRepository(IBaseRepository<Cart> cartRepository, UdemyContext dbContext) : base(dbContext)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            var carts = await GetAllWithIncluded();
            return carts.FirstOrDefault(c => c.UserID == userId);
        }

        #region private methods

        private async Task<IEnumerable<Cart>> GetAllWithIncluded()
        {
            var carts = await _cartRepository.GetAll(
                includeRelatedEntities: true,
                c => c.CoursesInCart
                
            );

            // Ensure the courses are materialized as a list
            return carts;
        }

        #endregion
    }
}
