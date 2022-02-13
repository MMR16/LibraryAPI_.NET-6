using LibraryAPI.DbContexts;
using LibraryAPI.Entites;

namespace LibraryAPI.Servicces
{
    public class CourseRepository : ICourseRepository, IDisposable
    {
        private readonly CourseLibraryContexts _context;
        public CourseRepository(CourseLibraryContexts context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }
        public void AddCourse(Guid authorId, Course course)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentException(nameof(authorId));
            }
            if (course == null)
            {
                throw new ArgumentException(nameof(course));
            }
            course.AuthorId = authorId;
            _context.Courses.Add(course);
        }
        public void DeleteCourse(Course course)
        {
            if (course is null)
            {
                throw new ArgumentException(nameof(course));
            }
            _context.Courses.Remove(course);
        }
        public Course GetCourse(Guid authorId, Guid courseId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentException(nameof(authorId));
            }
            if (courseId == Guid.Empty)
            {
                throw new ArgumentException(nameof(courseId));
            }
            return _context.Courses.Where(e => e.AuthorId == authorId && e.Id == courseId).FirstOrDefault();
        }
        public IEnumerable<Course> GetCourses(Guid authorId)
        {
            if (authorId ==Guid.Empty)
            {
                throw new ArgumentException(nameof(authorId));
            }
            return _context.Courses
                .Where(e => e.AuthorId == authorId)
                .OrderBy(e => e.Title).ToList();
        }
        public void UpdateCourse(Course course)
        {
            //if (course is null)
            //{
            //    throw new ArgumentException(nameof(course));
            //}
            //_context.Courses.Update(course);
        }
        public bool save()
        {
            return (_context.SaveChanges() >=0);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}
