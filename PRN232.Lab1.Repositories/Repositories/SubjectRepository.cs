using Microsoft.EntityFrameworkCore;
using PRN232.Lab1.Repositories.Entities;

namespace PRN232.Lab1.Repositories.Repositories
{
    public class SubjectRepository
    {
        private readonly Lab1DbContext _context;

        public SubjectRepository(Lab1DbContext context)
        {
            _context = context;
        }

        public IQueryable<Subject> GetSubjectsQueryable()
        {
            return _context.Subjects.AsQueryable();
        }

        public async Task<Subject> GetSubjectByIdAsync(int id)
        {
            return await _context.Subjects
                .FirstOrDefaultAsync(s => s.SubjectId == id);
        }

        public async Task<Subject> AddAsync(Subject subject)
        {
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task UpdateAsync(Subject subject)
        {
            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Subject subject)
        {
            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
        }
    }
}
