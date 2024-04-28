using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.Interfaces;
using Udemy.Core.Models.UdemyContext;
using Udemy.Core.Models;
using Microsoft.EntityFrameworkCore;
using Udemy.EF.Repository.NewFolder;
using Udemy.Core.Interfaces.IRepositoris.IBaseRepository;
using Udemy.EF.UnitOfWork;

namespace Udemy.EF.Repository
{
    public class CartRepository : BaseRepository<Cart>, ICartRepository
    {
        private readonly IBaseRepository<Cart> _cartRepository;
        private readonly IUnitOfWork<UdemyContext> _unitOfWork;

        public CartRepository(IBaseRepository<Cart> cartRepository, IUnitOfWork<UdemyContext> unitOfWork, UdemyContext dbContext) : base(dbContext)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;


        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            
            var carts = await GetAllWithIncluded();
            var cart = carts.FirstOrDefault(c => c.UserID == userId);

            if(cart ==null) { return null; }

            return cart;

        }

        



        #region private methods

        private async Task<IEnumerable<Cart>> GetAllWithIncluded()
        {
            var carts = await _cartRepository.GetAll(
                true,
                c => c.CoursesInCart
                
            );

            // Ensure the courses are materialized as a list
            return carts;
        }

        #endregion
    }
}
