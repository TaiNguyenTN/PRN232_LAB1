using Microsoft.EntityFrameworkCore;
using PRN232.Lab1.Repositories.Entities;

namespace PRN232.Lab1.Repositories.Repositories
{
    public class CourseRepository
    {
        private readonly Lab1DbContext _context;

        public CourseRepository(Lab1DbContext context)
        {
            _context = context;
        }

        public IQueryable<Course> GetCoursesQueryable(bool includeSemester = false)
        {
            var query = _context.Courses.AsQueryable();
            if (includeSemester)
            {
                query = query.Include(c => c.Semester);
            }
            return query;
        }

        public async Task<Course> GetCourseByIdAsync(int id, bool includeSemester = false, bool includeEnrollments = false)
        {
            var query = _context.Courses.AsQueryable();

            if (includeSemester)
            {
                query = query.Include(c => c.Semester);
            }

            if (includeEnrollments)
            {
                query = query.Include(c => c.Enrollments)
                                .ThenInclude(e => e.Student); // Kéo theo cả Student cho mỗi Enrollment
            }

            return await query.FirstOrDefaultAsync(c => c.CourseId == id);
        }

        public async Task<Course> AddAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Course course)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }
}
