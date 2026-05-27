using Microsoft.EntityFrameworkCore;
using PRN232.Lab1.Repositories.Repositories;
using PRN232.Lab1.Services.Models.Responses;
using PRN232.Lab1.Services.Models.Requests;
using PRN232.Lab1.Repositories.Entities;
using AutoMapper;
using System.Collections.Generic;

namespace PRN232.Lab1.Services.Services
{
    public class StudentService
    {
        private readonly StudentRepository _studentRepository;
        private readonly IMapper _mapper;

        public StudentService(StudentRepository studentRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<System.Dynamic.ExpandoObject>> GetStudentsAsync(
            string search, string sortBy, string sortOrder, int page, int pageSize, bool expand, string fields)
        {
            var query = _studentRepository.GetStudentsQueryable(expand);

            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();
                query = query.Where(s => s.FullName.ToLower().Contains(searchLower)
                                      || s.Email.ToLower().Contains(searchLower));
            }

            bool isDesc = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToLower() == "desc";
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "fullname":
                        query = isDesc ? query.OrderByDescending(s => s.FullName) : query.OrderBy(s => s.FullName);
                        break;
                    case "email":
                        query = isDesc ? query.OrderByDescending(s => s.Email) : query.OrderBy(s => s.Email);
                        break;
                    default:
                        query = query.OrderBy(s => s.StudentId);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(s => s.StudentId);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var studentsData = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var studentResponses = _mapper.Map<IEnumerable<StudentResponse>>(studentsData);
            var shapedData = PRN232.Lab1.Services.Helpers.DataShaper.ShapeData(studentResponses, fields);

            return new PagedResult<System.Dynamic.ExpandoObject>
            {
                Items = shapedData,
                Pagination = new PaginationMeta
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages
                }
            };
        }

        public async Task<StudentResponse> GetStudentByIdAsync(int id, bool expand = false)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id, expand);
            if (student == null) return null;

            return _mapper.Map<StudentResponse>(student);
        }

        public async Task<StudentResponse> CreateStudentAsync(StudentRequest request)
        {
            var student = _mapper.Map<Student>(request);
            await _studentRepository.AddAsync(student);
            return _mapper.Map<StudentResponse>(student);
        }

        public async Task<bool> UpdateStudentAsync(int id, StudentRequest request)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id);
            if (student == null) return false;

            _mapper.Map(request, student);
            await _studentRepository.UpdateAsync(student);
            return true;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id);
            if (student == null) return false;

            await _studentRepository.DeleteAsync(student);
            return true;
        }
    }
}
