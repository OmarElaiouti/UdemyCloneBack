using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.DTOs;
using Udemy.Core.DTOs.CourseDtos;
using Udemy.Core.DTOs.CoursePartsDtos;

namespace Udemy.Core.Interfaces
{
    public interface ICourseDataRepository
    {

        Task<CourseSectionsDto> GetSectionsByCourseIdAsync(int courseId, string userId);
        Task<IEnumerable<AnnouncmentDto>> GetAnnouncementByCourseIdAsync(int courseId);
        Task<OverviewDto> GetCourseOverViewById(int courseId);
        Task<IEnumerable<FeedbackDto>> GetReviewsByCourseIdAsync(int courseId);
        Task<FeedbackDto> GetStudentReviewOnCourseByCourseIdAsync(int courseId, string userId);
        Task<bool> SetStudentReviewOnCourse(int courseId, string userId, FeedbackDto feedbackDto);

        Task<IEnumerable<CourseCommentDto>> GetCommentsByCourseIdAsync(int courseId, string userId);
        Task<bool> SetStudentCommentOnCourse(int courseId, string userId, CourseCommentDto comment);
        Task<bool> SetStudentCourseLessonsStatus(int courseId, string userId, IEnumerable<LassonStatusDto> lessonStatusDto);
        Task<CertificateDto> GetOrCreateCertificateDataByUserIdAndCourseId(string userId, int courseId);
        Task<SingleCourseDto> GetSingleCourse(int courseId);
        Task<CourseInstructorDto> GetCourseInstructor(int courseId);
        Task<CourseSectionsDto> GetCourseSections(int courseId);
        Task<CourseRelatedDto> GetRelatedCourseData(int courseId);



    }
}
