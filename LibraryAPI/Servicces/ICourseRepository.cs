using LibraryAPI.Entites;

namespace LibraryAPI.Servicces
{
    public interface ICourseRepository
    {
        IEnumerable<Course> GetCourses(Guid authorId);
        Course GetCourse(Guid authorId, Guid courseId);
        void AddCourse(Guid authorId, Course course);
        void UpdateCourse(Course course);
        void DeleteCourse(Course course);
        bool save();

    }
}
