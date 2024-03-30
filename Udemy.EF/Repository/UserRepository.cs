using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.DTOs;
using Udemy.Core.Interfaces;
using Udemy.Core.Models;
using Udemy.Core.Models.UdemyContext;
using Udemy.Core.Services;
using Udemy.Core.Exceptions;

namespace Udemy.EF.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly UdemyContext _dbcontext;

        public UserRepository(IBaseRepository<User> userRepository, UdemyContext dbContext) : base(dbContext)
        {
            _userRepository = userRepository;
            _dbcontext = dbContext;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            var users = await GetAllWithIncluded();

           
            return users.FirstOrDefault(u => u.Id == userId);
        }

        public async Task<UserDto> GetUserDtoByIdAsync(string userId)
        {
            var user = await GetUserByIdAsync(userId);

            // Check if user is found
            if (user == null)
            {
                // Handle case where user is not found, you can throw an exception or return null
                // For example:
                throw new Exception($"User with ID '{userId}' not found.");
                // Or
                // return null;
            }

            return MapToUserDto(user);
        }

        public async Task<UserDto> UpdateUserAsync(string userId, UserDto userDto)
        {
            // Find the user by ID
            var users = await GetAllWithIncluded();
            var user = users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new NotFoundException($"User with ID '{userId}' not found.");
            }

            // Map the properties from UserDto to the existing user entity
            user.Image = userDto.Image;
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Biography = userDto.Biography;
            user.Headline = userDto.Headline;
            user.Email = userDto.Email;
            user.UserName = userDto.UserName;
            // Update the user entity in the database
            try
            {
                await _userRepository.Update(user);
                return ConvertToUserDto(user);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Failed to update user.", ex);
            }
        }

        public async Task<Transaction> CreateAndProcessTransactionAsync(string userId)
        {
            using (var dbTransaction = await _dbcontext.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = await _dbcontext.Users
                        .Include(u => u.Cart)
                        .ThenInclude(c => c.CoursesInCart)
                        .FirstOrDefaultAsync(u => u.Id == userId);

                    if (user == null)
                    {
                        throw new InvalidOperationException("User not found.");
                    }

                    var transaction = new Transaction
                    {
                        UserID = userId,
                        Amount = user.Cart.CoursesInCart.Count(),
                        PurchasedCourses = user.Cart.CoursesInCart,
                        Date = DateTime.UtcNow,
                        Status = "Success" // Assuming status is always true for this scenario
                    };

                    user.Cart.CoursesInCart.Clear();
                    _dbcontext.Transactions.Add(transaction);
                    await _dbcontext.SaveChangesAsync();

                    await ProcessTransactionAsync(transaction, userId);

                    await dbTransaction.CommitAsync();

                    return transaction;
                }
                catch (Exception)
                {
                    await dbTransaction.RollbackAsync();
                    throw; // Re-throw exception for logging and further handling
                }
            }
        }

        private async Task ProcessTransactionAsync(Transaction newTransaction, string userId)
        {
            
                var transaction = await _dbcontext.Transactions
                    .Include(t => t.PurchasedCourses)
                    .FirstOrDefaultAsync(t => t.TransactionID == newTransaction.TransactionID);

                if (transaction != null)
                {
                    // Create enrollment for each course
                    foreach (var course in transaction.PurchasedCourses)
                    {
                        var enrollment = new Enrollment
                        {
                            UserId = userId, // Get user id from token
                            CourseId = course.CourseID,
                            EnrollmentDate = DateTime.UtcNow
                            // You may need to set other properties of the enrollment based on your requirements
                        };

                        // Save enrollment to the database
                        _dbcontext.Enrollments.Add(enrollment);
                    }

                    await _dbcontext.SaveChangesAsync();
                }
            
        }

        #region private methods
        private UserDto ConvertToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Image = user.Image,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Biography = user.Biography,
                Headline = user.Headline,
                Email = user.Email,
                UserName = user.UserName,
                // Add other properties as needed
            };
        }
        private async Task<IEnumerable<User>> GetAllWithIncluded()
        {
            var users = await _userRepository.GetAll(
                true,
            u => u.Cart,
            u => u.WishList,
            u => u.Enrollments.Select(e => e.Course)
        );

            // Ensure the courses are materialized as a list
            return users;
        }

        private UserDto MapToUserDto(User user)
        {
            return  new UserDto
            {
                Id = user.Id,
                Image = user.Image,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Biography = user.Biography,
                Headline = user.Headline,
                Email = user.Email,
                UserName = user.UserName,
            }; 
        }

        

        #endregion
    }
}
