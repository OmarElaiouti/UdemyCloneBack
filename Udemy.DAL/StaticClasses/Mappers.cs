using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.DAL.DTOs.CourseDtos;
using Udemy.DAl.Models;
using Udemy.DAL.DTOs.CoursePartsDtos;
using Udemy.DAL.DTOs;

namespace Udemy.DAL.StaticClasses
{
    public static class Mappers
    {
        public static IEnumerable<CourseCardWithLevelDto> MapToCourseCardWithLevelDto(IEnumerable<Course> courses)
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
                Rate = Calculations.CalculateAverageRate(course),
                ReviewersNumber = course.Enrollments?.Count(e => e.Feedback != null) ?? 0,
                Price = course.Price,
                TotalLessons = course.Sections?.Sum(section => section.Lessons.Count) ?? 0,
                TotalHours = Format.FormatTotalHours(course.Sections?.Sum(section => section.Lessons.Sum(lesson => lesson.Duration)) ?? 0)
            });

        
        }
        public static IEnumerable<CourseLongDto> MapToLongCourseDto(IEnumerable<Course> courses)
        {
            return courses.Select(course => new CourseLongDto
            {
                ID = course.CourseID,
                Image = course.Cover,
                Name = course.Name,
                BriefDescription = course.BriefDescription,
                InstructorName = course.Instructor?.FirstName ?? "Unknown" + " " + course.Instructor?.FirstName ?? "Unknown",
                Rate = Calculations.CalculateAverageRate(course),
                Price = course.Price,
                TotalLessons = course.Sections?.Sum(section => section.Lessons.Count) ?? 0,
                TotalHours = Format.FormatTotalHours(course.Sections?.Sum(section => section.Lessons.Sum(lesson => lesson.Duration)) ?? 0)
            });
        }
        public static IEnumerable<CourseWithObjectivesDto> MapToCourseWithObjectivesDtoDto(IEnumerable<Course> courses)
        {
            return courses.Select(course => new CourseWithObjectivesDto
            {
                ID = course.CourseID,
                Image = course.Cover,
                Name = course.Name,
                InstructorName = course.Instructor?.FirstName ?? "Unknown" + " " + course.Instructor?.FirstName ?? "Unknown",
                Rate = Calculations.CalculateAverageRate(course),
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
        public static IEnumerable<CourseShortDto> MapToCourseShortDto(IEnumerable<Course> courses)
        {
            return courses.Select(course => new CourseShortDto
            {
                ID = course.CourseID,
                Image = course.Cover,
                Name = course.Name,
                InstructorName = course.Instructor?.FirstName ?? "Unknown" + " " + course.Instructor?.FirstName ?? "Unknown",
                Rate = Calculations.CalculateAverageRate(course),
                Price = course.Price
            });
        }
        public static IEnumerable<NotificationDto> MapToNotificationDtoAsync(IEnumerable<Notification> notifications)
        {
            // Since there are no asynchronous operations, we simply return the mapped results
            return  notifications.Select(notification => new NotificationDto
            {
                ID = notification.NotificationID,
                Content = notification.Content,
                Date = notification.Timestamp.ToString("yyyy-MM-dd"), // Format date with year, month, and day only
                Status = notification.Status
            }).ToList();
        }




    }
}
