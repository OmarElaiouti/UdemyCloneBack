using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.DTOs;
using Udemy.Core.DTOs.CourseDtos;
using Udemy.Core.DTOs.CoursePartsDtos;
using Udemy.Core.Interfaces;
using Udemy.Core.Models;
using Udemy.Core.Models.UdemyContext;
using Udemy.EF.Repository;

namespace Udemy.Core.Services
{

    public class CourseRepository:BaseRepository<Course>, ICourseRepository
    {
        private readonly IBaseRepository<Course> _courseRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IUserRepository _userRepository;
        private readonly UdemyContext _dbcontext;



        public CourseRepository(IBaseRepository<Course> courseRepository, IUserRepository userRepository, ICartRepository cartRepository, UdemyContext dbContext):base(dbContext)
        {
            _courseRepository = courseRepository;
            _cartRepository = cartRepository;
            _userRepository = userRepository;
            _dbcontext = dbContext;

        }

        public async Task<IEnumerable<CourseLongDto>> SearchCoursesByNameAsync(string searchString)
        {
            var allCourses = await GetAllWithIncluded();
            var matchingCourses = allCourses.Where(c => c.Name.Contains(searchString));


                return MapToLongCourseDto(matchingCourses).ToList();
            
           
        }
        public async Task<IEnumerable<CourseWithObjectivesDto>> SearchCoursesByNameAsync(string searchString, int count)
        {
            var allCourses = await GetAllWithIncluded();
            var matchingCourses = allCourses.Where(c => c.Name.Contains(searchString));
            
                matchingCourses = matchingCourses.Take(count);
           

            return MapToCourseWithObjectivesDtoDto(matchingCourses).ToList();
        }


        public async Task<IEnumerable<CourseWithObjectivesDto>> GetRandomCourses(int count)
        {
            var courses = await GetAllWithIncluded();
            var randomCourses = courses.OrderBy(c => Guid.NewGuid()).Take(count);
            return MapToCourseWithObjectivesDtoDto(randomCourses).ToList();
        }    
        public async Task<IEnumerable<CourseWithObjectivesDto>> GetTopRatedCourses(int count)
        {
            var courses = await GetAllWithIncluded();
            var topRatedCourses = courses.OrderByDescending(c => c.Enrollments.Average(e => e.Feedback.Rate)).Take(count);
            return MapToCourseWithObjectivesDtoDto(topRatedCourses).ToList();
        }
        public async Task<IEnumerable<CourseLongDto>> GetCoursesByCategory(string categoryName, int? count)
        {
            var courses = await GetAllWithIncluded();

            var filteredCourses = courses
                .Where(course => course.Category.Name == categoryName);

            if (count.HasValue)
            {
                filteredCourses = filteredCourses.Take(count.Value);
            }

            return MapToLongCourseDto(filteredCourses).ToList();

        }

        public async Task<IEnumerable<CourseWithObjectivesDto>> GetCoursesByCategoryWithObjctives(string categoryName, int? count)
        {
            var courses = await GetAllWithIncluded();

            var filteredCourses = courses
                .Where(course => course.Category.Name == categoryName);

            if (count.HasValue)
            {
                filteredCourses = filteredCourses.Take(count.Value);
            }

            return MapToCourseWithObjectivesDtoDto(filteredCourses).ToList();

        }

        public async Task<IEnumerable<CourseCardWithLevelDto>> GetCoursesInCartByUserId(string id)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(id);

            if (cart == null || cart.CoursesInCart == null || !cart.CoursesInCart.Any())
            {
                return Enumerable.Empty<CourseCardWithLevelDto>(); // Return an empty enumerable if cart or CoursesInCart is null or empty
            }

            return MapToCourseCardWithLevelDto(cart.CoursesInCart).ToList();
        }

        public async Task<IEnumerable<CourseCardWithLevelDto>> GetCoursesByIds(List<int> itemIds)
        {
            try
            {
                var courses = new List<Course>();

                foreach (var itemId in itemIds)
                {
                    var course = await _courseRepository.GetById(itemId, "CourseID", true,
                c => c.Instructor,
                c => c.Enrollments.Select(e => e.Feedback), // Corrected to use ThenInclude instead of Include
                c => c.Sections.SelectMany(s => s.Lessons),
                c => c.Category,
                c => c.Objectives);
                    if (course != null)
                    {
                        courses.Add(course);
                    }
                }

                return MapToCourseCardWithLevelDto(courses).ToList();
            }
            catch (Exception ex)
            {
                // Handle exception appropriately, e.g., log the error
                Console.WriteLine($"An error occurred while fetching courses: {ex.Message}");
                throw; // Rethrow the exception for further handling
            }
        }

        public async Task<IEnumerable<CourseLongDto>> GetCoursesInWishlistByUserId(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            return MapToLongCourseDto(user.WishList).ToList();

        }
        public async Task<IEnumerable<CourseLongDto>> GetEnrolledInCoursesByUserId(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            return MapToLongCourseDto(user.Enrollments.Select(e=>e.Course).ToList());

        }

        public async Task<Course> GetCourseById(int id)
        {
            var course = await GetAllWithIncluded();


            return course.FirstOrDefault(c => c.CourseID == id);
        }

        public async Task<CourseShortDto> AddCourseToCartByUserIdAsync(string userId, int courseId)
        {
            var course = await _dbcontext.Courses.Include(c => c.Instructor).FirstOrDefaultAsync(c => c.CourseID == courseId);
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var cart = await _dbcontext.Carts.Include(c => c.CoursesInCart).FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart == null)
            { 
                // If the cart doesn't exist for the user, create a new cart
                cart = new Cart { UserID = userId,Quantity=0 };
                _dbcontext.Carts.Add(cart);
                await _dbcontext.SaveChangesAsync();

                var newCart = await _cartRepository.GetCartByUserIdAsync(userId);
                user.CartID = newCart.CartID;
                await _dbcontext.SaveChangesAsync();

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
                await _dbcontext.SaveChangesAsync();
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
            return null;
        }

        public async Task<CourseShortDto> AddCourseToWishlistByUserIdAsync(string userId, int courseId)
        {
            var course = await _dbcontext.Courses.Include(c=>c.Instructor).FirstOrDefaultAsync(c => c.CourseID == courseId);
            var user = await _dbcontext.Users.Include(u => u.WishList).FirstOrDefaultAsync(u=>u.Id == userId);

        

            if (course != null && !user.WishList.Any(c => c.CourseID == courseId))
            {
                // Add the course to the cart
                user.WishList.Add(course);
                await _dbcontext.SaveChangesAsync();
                return new CourseShortDto
                {
                    ID = course.CourseID,
                    Image = course.Cover,
                    Name = course.Name,
                    Price = course.Price,
                    InstructorName = course.Instructor?.FirstName ?? "Unknown" + " " + course.Instructor?.FirstName ?? "Unknown",
                    Rate=0
                };
            }
            return null;
        }

        public async Task<bool> RemoveCourseFromCartByUserIdAsync(string userId, int courseId)
        {
            // Retrieve the course, user, and cart
            var course = await _dbcontext.Courses.Include(c => c.Instructor).FirstOrDefaultAsync(c => c.CourseID == courseId);
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var cart = await _dbcontext.Carts.Include(c => c.CoursesInCart).FirstOrDefaultAsync(c => c.UserID == userId);

            // Check if course, user, and cart exist
            if (course == null || user == null || cart == null || cart.CoursesInCart == null)
            {
                return false; // Exit early if any essential data is missing
            }

            // Find the course in the cart
            var courseToRemove = cart.CoursesInCart.FirstOrDefault(c => c.CourseID == courseId);
            if (courseToRemove == null)
            {
                return false; // If the course is not in the cart, return null
            }

            // Remove the course from the cart
            cart.CoursesInCart.Remove(courseToRemove);
            cart.Quantity--; // Decrement cart quantity

            // Save changes to the database
            await _dbcontext.SaveChangesAsync();

            // Create and return CourseShortDto for the removed course
            return true;
        }

        public async Task<bool> RemoveCourseFromWishlistByUserIdAsync(string userId, int courseId)
        {
            // Retrieve the course and user's wishlist
            var course = await _dbcontext.Courses.Include(c => c.Instructor).FirstOrDefaultAsync(c => c.CourseID == courseId);
            var user = await _dbcontext.Users.Include(u => u.WishList).FirstOrDefaultAsync(u => u.Id == userId);

            // Check if course and user exist, and if the course is in the wishlist
            if (course != null && user != null && user.WishList != null && user.WishList.Any(c => c.CourseID == courseId))
            {
                // Remove the course from the wishlist
                var courseToRemove = user.WishList.FirstOrDefault(c => c.CourseID == courseId);
                user.WishList.Remove(courseToRemove);
                await _dbcontext.SaveChangesAsync();

                // Return CourseShortDto for the removed course
                return true;
            }

            return false; // Course not found in the wishlist or user not found
        }
        #region private methods
        private async Task<IEnumerable<Course>> GetAllWithIncluded()
        {
            var courses = await _courseRepository.GetAll(
                true,
                c => c.Instructor,
                c => c.Enrollments.Select(e => e.Feedback), // Corrected to use ThenInclude instead of Include
                c => c.Sections.SelectMany(s => s.Lessons),
                c => c.Category,
                c => c.Objectives
            );

            // Ensure the courses are materialized as a list
            return courses;
        }

        private IEnumerable<CourseShortDto> MapToCourseShortDto(IEnumerable<Course> courses)
        {
            return courses.Select(course => new CourseShortDto
            {
                ID = course.CourseID,
                Image = course.Cover,
                Name = course.Name,
                InstructorName = course.Instructor?.FirstName ?? "Unknown" + " " + course.Instructor?.FirstName ?? "Unknown",
                Rate = CalculateAverageRate(course),
                Price = course.Price
            });
        }
        private IEnumerable<CourseWithObjectivesDto> MapToCourseWithObjectivesDtoDto(IEnumerable<Course> courses)
        {
            return courses.Select(course => new CourseWithObjectivesDto
            {
                ID = course.CourseID,
                Image = course.Cover,
                Name = course.Name,
                InstructorName = course.Instructor?.FirstName ?? "Unknown" + " " + course.Instructor?.FirstName ?? "Unknown",
                Rate = CalculateAverageRate(course),
                Price = course.Price,
                ReviewersNumber = course.Enrollments?.Count(e => e.Feedback != null) ?? 0,

                Objectives = course.Objectives.Select(o => new ObjectiveDto
                {
                    ID = o.ObjectiveID,
                    Content = o.Description
                }
                
                )
            });
        }

        private IEnumerable<CourseLongDto> MapToLongCourseDto(IEnumerable<Course> courses)
        {
            return courses.Select(course => new CourseLongDto
            {
                ID=course.CourseID,
                Image = course.Cover,
                Name = course.Name,
                BriefDescription = course.BriefDescription,
                InstructorName = course.Instructor?.FirstName ?? "Unknown"+" "+ course.Instructor?.FirstName ?? "Unknown",
                Rate = CalculateAverageRate(course),
                Price = course.Price,
                TotalLessons = course.Sections?.Sum(section => section.Lessons.Count) ?? 0,
                TotalHours = FormatTotalHours(course.Sections?.Sum(section => section.Lessons.Sum(lesson => lesson.Duration)) ?? 0)
            });
        }
        private IEnumerable<CourseCardWithLevelDto> MapToCourseCardWithLevelDto(IEnumerable<Course> courses)
        {
            if (courses == null)
            {
                return Enumerable.Empty<CourseCardWithLevelDto>(); // Return an empty enumerable if courses is null
            }

            return courses.Select(course => new CourseCardWithLevelDto
            {
                ID = course.CourseID,
                Image = course.Cover,
                Name = course.Name,
                Level = course.Level,
                InstructorName = course.Instructor?.FirstName ?? "Unknown" + " " + course.Instructor?.FirstName ?? "Unknown",
                Rate = CalculateAverageRate(course),
                ReviewersNumber = course.Enrollments?.Count(e => e.Feedback != null) ?? 0,
                Price = course.Price,
                TotalLessons = course.Sections?.Sum(section => section.Lessons.Count) ?? 0,
                TotalHours = FormatTotalHours(course.Sections?.Sum(section => section.Lessons.Sum(lesson => lesson.Duration)) ?? 0)
            });
        }


        private float CalculateAverageRate(Course course)
        {
            if (course.Enrollments == null || course.Enrollments.Count == 0)
                return 0;

            return course.Enrollments.Average(enrollment => enrollment.Feedback?.Rate ?? 0);
        
        }

        string FormatTotalHours(int totalMinutes)
        {
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;
            return $"{hours}h {minutes}m";
        }

        #endregion

    }
}
