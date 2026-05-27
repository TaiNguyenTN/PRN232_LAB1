using Microsoft.EntityFrameworkCore;
using PRN232.Lab1.Repositories.Entities;
using System.Linq;

namespace PRN232.Lab1.Repositories.Repositories
{
    public class EnrollmentRepository
    {
        private readonly Lab1DbContext _context;

        public EnrollmentRepository(Lab1DbContext context)
        {
            _context = context;
        }

        public IQueryable<Enrollment> GetEnrollmentsQueryable(bool includeStudent = false, bool includeCourse = false)
        {
            var query = _context.Enrollments.AsQueryable();
            if (includeStudent)
            {
                query = query.Include(e => e.Student);
            }
            if (includeCourse)
            {
                query = query.Include(e => e.Course);
            }
            return query;
        }

        public async Task<Enrollment> GetEnrollmentByIdAsync(int id, bool expand = false)
        {
            var query = _context.Enrollments.AsQueryable();
            if (expand)
            {
                query = query.Include(e => e.Student)
                             .Include(e => e.Course);
            }
            return await query.FirstOrDefaultAsync(e => e.EnrollmentId == id);
        }

        // Lấy danh sách sinh viên thuộc một Course cụ thể qua bảng Enrollment
        public async Task<IEnumerable<Student>> GetStudentsByCourseIdAsync(int courseId)
        {
            return await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student)
                .Select(e => e.Student)
                .Distinct()
                .ToListAsync();
        }

        public IQueryable<Student> GetStudentsByCourseIdQueryable(int courseId)
        {
            // Only return students for the given course.
            // Do NOT include Student.Enrollments here; the service can optionally expand
            // to avoid unnecessary data load when expand=false.
            return _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => e.Student)
                .Distinct();
        }

        // Lấy danh sách Enrollment thuộc một Course cụ thể (có thể include Student/Course)
        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(int courseId, bool includeStudent = true, bool includeCourse = true)
        {
            var query = _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .AsQueryable();

            if (includeStudent)
            {
                query = query.Include(e => e.Student);
            }

            if (includeCourse)
            {
                query = query.Include(e => e.Course);
            }

            return await query
                .OrderBy(e => e.EnrollmentId)
                .ToListAsync();
        }

        public IQueryable<Enrollment> GetEnrollmentsByCourseIdQueryable(int courseId, bool includeStudent = true, bool includeCourse = true)
        {
            var query = _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .AsQueryable();

            if (includeStudent)
            {
                query = query.Include(e => e.Student);
            }

            if (includeCourse)
            {
                query = query.Include(e => e.Course);
            }

            return query;
        }

        public async Task<Enrollment> AddAsync(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task UpdateAsync(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Enrollment enrollment)
        {
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
        }
    }
}
