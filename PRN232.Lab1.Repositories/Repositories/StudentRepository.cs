using Microsoft.EntityFrameworkCore;
using PRN232.Lab1.Repositories.Entities;

namespace PRN232.Lab1.Repositories.Repositories
{
    public class StudentRepository
    {
        private readonly Lab1DbContext _context;

        public StudentRepository(Lab1DbContext context)
        {
            _context = context;
        }

        // Truy vấn danh sách cơ bản có hỗ trợ IQueryable để tối ưu việc Filter/Paging ở Service
        public IQueryable<Student> GetStudentsQueryable(bool includeEnrollments = false)
        {
            var query = _context.Students.AsQueryable();
            if (includeEnrollments)
            {
                query = query.Include(s => s.Enrollments)
                             .ThenInclude(e => e.Course);
            }
            return query;
        }

        public async Task<Student> AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Student student)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

        public async Task<Student> GetStudentByIdAsync(int id, bool expand = false)
        {
            var query = _context.Students.AsQueryable();
            if (expand)
            {
                query = query.Include(s => s.Enrollments)
                             .ThenInclude(e => e.Course);
            }
            return await query.FirstOrDefaultAsync(s => s.StudentId == id);
        }

        
    }
}
