using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Udemy.BLL.Interfaces;
using Udemy.CU.Exceptions;
using Udemy.DAl.Models;
using Udemy.DAL.GenericBaseRepository.BaseRepository;
using Udemy.DAL.Context;
using Udemy.DAL.DTOs;
using Udemy.DAL.DTOs.CourseDtos;
using Udemy.DAL.StaticClasses;
using Udemy.DAL.UnitOfWork;


namespace Udemy.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly ICartService _cartSrevice;
        private readonly UdemyContext _dbcontext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IUnitOfWork<UdemyContext> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBaseRepository<Notification> _notificationRepository;




        public UserService(
            IMapper mapper,
            IUnitOfWork<UdemyContext> unitOfWork,
            ICartService cartSrevice,
            UdemyContext dbContext,
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager,
            IBaseRepository<Notification> notificationRepository,
            IBaseRepository<User> userRepository
            )
        {
            _mapper = mapper;
            _cartSrevice = cartSrevice;
            _dbcontext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;

        }

        public async Task<IEnumerable<CourseCardWithLevelDto>> GetCoursesInCartByUserId(string id)
        {
            var cart = await _cartSrevice.GetCartByUserIdAsync(id);

            if (cart == null || cart.CoursesInCart == null || !cart.CoursesInCart.Any())
            {
                return Enumerable.Empty<CourseCardWithLevelDto>(); // Return an empty enumerable if cart or CoursesInCart is null or empty
            }

            return Mappers.MapToCourseCardWithLevelDto(cart.CoursesInCart).ToList();
        }
        public async Task<IEnumerable<CourseLongDto>> GetCoursesInWishlistByUserId(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            return Mappers.MapToLongCourseDto(user.WishList).ToList();

        }
        public async Task<IEnumerable<CourseLongDto>> GetEnrolledInCoursesByUserId(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            return Mappers.MapToLongCourseDto(user.Enrollments.Select(e => e.Course).ToList());

        }
        public async Task<CourseShortDto> AddCourseToCartByUserIdAsync(string userId, int courseId)
        {
            try
            {
                await _unitOfWork.CreateTransactionAsync();

                var course = await _unitOfWork.Context.Courses.FirstOrDefaultAsync(c => c.CourseID == courseId);
                var user = await _unitOfWork.Context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                var cart = await _unitOfWork.Context.Carts.FirstOrDefaultAsync(c => c.UserID == userId);

                if (cart == null)
                {
                    // If the cart doesn't exist for the user, create a new cart
                    cart = new Cart { UserID = userId, Quantity = 0 };
                    _unitOfWork.Context.Carts.Add(cart);
                    await _unitOfWork.SaveAsync();
                }

                if (course != null && (cart.CoursesInCart == null || !cart.CoursesInCart.Any(c => c.CourseID == courseId)))
                {
                    // Add the course to the cart
                    if (cart.CoursesInCart == null)
                    {
                        cart.CoursesInCart = new List<Course>(); // Ensure that CoursesInCart is initialized
                    }

                    cart.CoursesInCart.Add(course);
                    cart.Quantity++; // Increment cart quantity
                    await _unitOfWork.SaveAsync();
                    await _unitOfWork.CommitAsync();

                    return new CourseShortDto
                    {
                        ID = course.CourseID,
                        Image = course.Cover,
                        Name = course.Name,
                        Price = course.Price,
                        InstructorName = course.Instructor?.FirstName ?? "Unknown" + " " + course.Instructor?.FirstName ?? "Unknown",
                        Rate = 0
                    };
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
            }

            return null;
        }
        public async Task<CourseShortDto> AddCourseToWishlistByUserIdAsync(string userId, int courseId)
        {
            try
            {
                await _unitOfWork.CreateTransactionAsync();

                var course = await _unitOfWork.Context.Courses.FirstOrDefaultAsync(c => c.CourseID == courseId);
                var user = await _unitOfWork.Context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (course != null && !user.WishList.Any(c => c.CourseID == courseId))
                {
                    // Add the course to the wishlist
                    user.WishList.Add(course);
                    await _unitOfWork.SaveAsync();
                    await _unitOfWork.CommitAsync();

                    return new CourseShortDto
                    {
                        ID = course.CourseID,
                        Image = course.Cover,
                        Name = course.Name,
                        Price = course.Price,
                        InstructorName = course.Instructor?.FirstName ?? "Unknown" + " " + course.Instructor?.LastName ?? "Unknown",
                        Rate = 0
                    };
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
            }

            return null;
        }
        public async Task<bool> RemoveCourseFromCartByUserIdAsync(string userId, int courseId)
        {
            try
            {
                await _unitOfWork.CreateTransactionAsync();

                // Retrieve the course, user, and cart
                var course = await _unitOfWork.Context.Courses.FirstOrDefaultAsync(c => c.CourseID == courseId);
                var user = await _unitOfWork.Context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                var cart = await _unitOfWork.Context.Carts.FirstOrDefaultAsync(c => c.UserID == userId);

                // Check if course, user, and cart exist
                if (course == null || user == null || cart == null || cart.CoursesInCart == null)
                {
                    return false; // Exit early if any essential data is missing
                }

                // Find the course in the cart
                var courseToRemove = cart.CoursesInCart.FirstOrDefault(c => c.CourseID == courseId);
                if (courseToRemove == null)
                {
                    return false; // If the course is not in the cart, return false
                }

                // Remove the course from the cart
                cart.CoursesInCart.Remove(courseToRemove);
                cart.Quantity--; // Decrement cart quantity

                // Save changes to the database
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Handle exception, log it, or return false indicating failure
                await _unitOfWork.RollbackAsync();
                return false;
            }
        }
        public async Task<bool> RemoveCourseFromWishlistByUserIdAsync(string userId, int courseId)
        {
            try
            {
                await _unitOfWork.CreateTransactionAsync();

                // Retrieve the course and user's wishlist
                var course = await _unitOfWork.Context.Courses.FirstOrDefaultAsync(c => c.CourseID == courseId);
                var user = await _unitOfWork.Context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                // Check if course and user exist, and if the course is in the wishlist
                if (course != null && user != null && user.WishList != null && user.WishList.Any(c => c.CourseID == courseId))
                {
                    // Remove the course from the wishlist
                    var courseToRemove = user.WishList.FirstOrDefault(c => c.CourseID == courseId);
                    user.WishList.Remove(courseToRemove);
                    await _unitOfWork.SaveAsync();

                    await _unitOfWork.CommitAsync();

                    // Return true to indicate success
                    return true;
                }

                // Return false if course not found in the wishlist or user not found
                return false;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }
        }

        public async Task<UserDto> GetUserDtoByIdAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            // Check if user is found
            if (user == null)
            {
                throw new Exception($"User with ID '{userId}' not found.");
            }

            return _mapper.Map<UserDto>(user);
        }
        public async Task<bool> AddInstructorRoleToUser(string userId)
        {
            try
            {
                // Find the user by userId
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    // User not found
                    throw new Exception($"User with ID '{userId}' not found.");
                }

                // Check if the role "Instructor" exists
                var instructorRole = await _roleManager.FindByNameAsync(Roles.Instructor);
                if (instructorRole == null)
                {
                    // If the role doesn't exist, create it
                    instructorRole = new IdentityRole(Roles.Instructor);
                    await _roleManager.CreateAsync(instructorRole);
                    Console.WriteLine($"Created new role: 'Instructor'.");
                }

                // Check if the user already has the "Instructor" role
                var isInstructor = await _userManager.IsInRoleAsync(user, Roles.Instructor);
                if (!isInstructor)
                {
                    // Add the "Instructor" role to the user
                    await _userManager.AddToRoleAsync(user, Roles.Instructor);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"Failed to add 'Instructor' role to user with ID '{userId}': {ex.Message}");
            }
        }

        public async Task<UserDto> UpdateUserAsync(string userId, UserDto userDto)
        {
            // Find the user by ID
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException($"User with ID '{userId}' not found.");
            }

            // Map the properties from UserDto to the existing user entity
            _mapper.Map(userDto, user);

            // Update the user entity in the database
            try
            {
                _userRepository.Update(user);
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Failed to update user.", ex);
            }
        }

        public async Task<Transaction> CreateAndProcessTransactionAsync(string userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    throw new InvalidOperationException("User not found.");
                }

                if (user.Cart == null)
                {
                    throw new InvalidOperationException("User's cart not found.");
                }

                var purchasedCourses = user.Cart.CoursesInCart;

                if (purchasedCourses == null)
                {
                    throw new InvalidOperationException("User's purchased courses not found.");
                }

                var amount = purchasedCourses.Sum(c => c.Price);

                await _unitOfWork.CreateTransactionAsync();

                var transaction = new Transaction
                {
                    UserID = userId,
                    Amount = amount,
                    PurchasedCourses = purchasedCourses,
                    Date = DateTime.UtcNow,
                    Status = "Success" // Assuming status is always true for this scenario
                };


                foreach (var course in transaction.PurchasedCourses)
                {
                    var enrollment = new Enrollment
                    {
                        UserId = userId,
                        CourseId = course.CourseID,
                        EnrollmentDate = DateTime.UtcNow
                    };

                    // Save enrollment to the database
                    _unitOfWork.Context.Enrollments.Add(enrollment);

                    var transuctioNotification = new Notification
                    {
                        EventType = "TransuctionsSuccess",
                        Content = $"You have a successful transuction of {transaction.Amount}$ ",
                        Timestamp = DateTime.UtcNow,
                        Status = false,
                        UserID = userId
                    };

                    _unitOfWork.Context.Add(transuctioNotification);

                    var enrollmentNotification = new Notification
                    {
                        EventType = "EnrollmentSuccess",
                        Content = $"You have a successful Enrollment in {course.Name}, start the journey now!",
                        Timestamp = DateTime.UtcNow,
                        Status = false,
                        UserID = userId
                    };

                    _unitOfWork.Context.Add(enrollmentNotification);
                }
                _unitOfWork.Context.Transactions.Add(transaction);

                user.Cart.CoursesInCart.Clear();

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();

                return transaction;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception($"Faild transaction: {ex}");
            }
        }

        public async Task DeleteUserAsync(string userId)
        {
            try
            {
                await _unitOfWork.CreateTransactionAsync();

                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    _userRepository.Delete(user);
                    await _unitOfWork.SaveAsync();
                    await _unitOfWork.CommitAsync();

                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                Console.WriteLine("Error deleting user: " + ex.Message);
                throw;

            }
        }
        public async Task<(bool success, string message)> UpdateUserImage(string userId, string filePath)
        {
            try
            {
                // Retrieve the user
                var user = await _userRepository.GetByIdAsync(userId);

                // Check if the user exists
                if (user == null)
                {
                    return (false, "User not found.");
                }

                if (string.IsNullOrEmpty(filePath))
                {
                    return (false, "Invalid file path");
                }
                // Update the user's image
                user.Image = filePath;

                // Update the user in the repository
                _userRepository.Update(user);

                // Save changes within the Unit of Work transaction
                await _dbcontext.SaveChangesAsync();

                // Return success with the file path
                return (true, filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user image for user with ID '{userId}': {ex.Message}");

            }
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotifications(string userId)
        {

            var user = await _userRepository.GetByIdAsync(userId);

            var notifications = user.Notifications;
            return Mappers.MapToNotificationDtoAsync(notifications);
        }
        public async Task UpdateLastFiveNotificationsStatusToTrue(string userId)
        {
            try
            {
                // Retrieve user by ID
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found");
                }

                await _unitOfWork.CreateTransactionAsync();

                // Get the last five notifications
                var lastFiveNotifications = user.Notifications
                    .OrderByDescending(notification => notification.Timestamp)
                    .Take(5)
                    .ToList();

                // Update the status of last five notifications
                foreach (var notification in lastFiveNotifications)
                {
                    notification.Status = true;
                    _notificationRepository.Update(notification);

                }


                // Save changes within the Unit of Work transaction
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                Console.WriteLine($"Failed to update notifications status for user with ID '{userId}': {ex.Message}");
                throw;
            }
        }
        public async Task SetAllUserNotificationsStatusToTrue(string userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found");
                }

                await _unitOfWork.CreateTransactionAsync();


                foreach (var notification in user.Notifications)
                {
                    notification.Status = true;
                    _notificationRepository.Update(notification);

                }

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception("Failed to update all notifications status", ex);
            }
        }










    }
}
