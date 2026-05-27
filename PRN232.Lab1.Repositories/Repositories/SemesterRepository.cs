using Microsoft.EntityFrameworkCore;
using PRN232.Lab1.Repositories.Entities;

namespace PRN232.Lab1.Repositories.Repositories
{
    public class SemesterRepository
    {
        private readonly Lab1DbContext _context;

        public SemesterRepository(Lab1DbContext context)
        {
            _context = context;
        }

        public IQueryable<Semester> GetSemestersQueryable()
        {
            return _context.Semesters.AsQueryable();
        }

        public async Task<Semester> GetSemesterByIdAsync(int id, bool expand = false)
        {
            var query = _context.Semesters.AsQueryable();
            if (expand)
            {
                query = query.Include(s => s.Courses);
            }
            return await query.FirstOrDefaultAsync(s => s.SemesterId == id);
        }

        public async Task<Semester> AddAsync(Semester semester)
        {
            await _context.Semesters.AddAsync(semester);
            await _context.SaveChangesAsync();
            return semester;
        }

        public async Task UpdateAsync(Semester semester)
        {
            _context.Semesters.Update(semester);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Semester semester)
        {
            _context.Semesters.Remove(semester);
            await _context.SaveChangesAsync();
        }
    }
}
