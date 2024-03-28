using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.Interfaces;
using Udemy.Core.Models;
using Udemy.Core.Models.UdemyContext;
using Udemy.Core.Services;

namespace Udemy.EF.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly IBaseRepository<User> _userRepository;

        public UserRepository(IBaseRepository<User> userRepository, UdemyContext dbContext) : base(dbContext)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            var users = await GetAllWithIncluded();
            return users.FirstOrDefault(u => u.Id == userId);
        }

        #region private methods

        private async Task<IEnumerable<User>> GetAllWithIncluded()
        {
            var users = await _userRepository.GetAll(
                includeRelatedEntities: true,
                u => u.Cart
            );

            // Ensure the courses are materialized as a list
            return users;
        }

        #endregion
    }
}
