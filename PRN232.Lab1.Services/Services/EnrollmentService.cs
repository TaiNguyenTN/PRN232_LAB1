using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRN232.Lab1.Repositories.Repositories;
using PRN232.Lab1.Services.Models.Responses;
using PRN232.Lab1.Services.Models.Requests;
using PRN232.Lab1.Repositories.Entities;
using AutoMapper;
using System.Collections.Generic;

namespace PRN232.Lab1.Services.Services
{
    public class EnrollmentService
    {
        private readonly EnrollmentRepository _enrollmentRepository;
        private readonly IMapper _mapper;

        public EnrollmentService(EnrollmentRepository enrollmentRepository, IMapper mapper)
        {
            _enrollmentRepository = enrollmentRepository;
            _mapper = mapper;
        }

        public async Task<EnrollmentResponse> GetEnrollmentByIdAsync(int id, bool expand = false)
        {
            var enrollment = await _enrollmentRepository.GetEnrollmentByIdAsync(id, expand);
            if (enrollment == null) return null;

            return _mapper.Map<EnrollmentResponse>(enrollment);
        }

        // Lấy danh sách sinh viên thuộc một Course cụ thể
        public async Task<IEnumerable<StudentResponse>> GetStudentsByCourseIdAsync(int courseId)
        {
            var students = await _enrollmentRepository.GetStudentsByCourseIdAsync(courseId);
            return _mapper.Map<IEnumerable<StudentResponse>>(students);
        }

        public Task<PagedResult<StudentResponse>> GetStudentsByCourseIdPagedAsync(int courseId, int page, int pageSize)
            => GetStudentsByCourseIdPagedAsync(courseId, page, pageSize, expand: false);

        public async Task<PagedResult<StudentResponse>> GetStudentsByCourseIdPagedAsync(int courseId, int page, int pageSize, bool expand)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var query = _enrollmentRepository
                .GetStudentsByCourseIdQueryable(courseId)
                .OrderBy(s => s.StudentId);

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var studentsData = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var studentResponses = _mapper.Map<List<StudentResponse>>(studentsData);

            if (expand)
            {
                var studentIds = studentResponses.Select(s => s.StudentId).Distinct().ToList();
                if (studentIds.Count > 0)
                {
                    var enrollmentsData = await _enrollmentRepository
                        .GetEnrollmentsByCourseIdQueryable(courseId, includeStudent: false, includeCourse: true)
                        .Where(e => studentIds.Contains(e.StudentId))
                        .OrderBy(e => e.EnrollmentId)
                        .ToListAsync();

                    var enrollmentFlatResponses = _mapper.Map<List<EnrollmentFlatResponse>>(enrollmentsData);
                    var enrollmentsByStudent = enrollmentFlatResponses
                        .GroupBy(e => e.StudentId)
                        .ToDictionary(g => g.Key, g => (ICollection<EnrollmentFlatResponse>)g.ToList());

                    foreach (var student in studentResponses)
                    {
                        student.Enrollments = enrollmentsByStudent.TryGetValue(student.StudentId, out var list)
                            ? list
                            : new List<EnrollmentFlatResponse>();
                    }
                }
                else
                {
                    foreach (var student in studentResponses)
                    {
                        student.Enrollments = new List<EnrollmentFlatResponse>();
                    }
                }
            }
            else
            {
                // Keep null to avoid payload; controller decides how to represent it.
                foreach (var student in studentResponses)
                {
                    student.Enrollments = null;
                }
            }

            return new PagedResult<StudentResponse>
            {
                Items = studentResponses,
                Pagination = new PaginationMeta
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages
                }
            };
        }

        // Lấy danh sách enrollment thuộc một course (kèm student tương ứng)
        public async Task<IEnumerable<EnrollmentResponse>> GetEnrollmentsByCourseIdAsync(int courseId, bool includeStudent = false)
        {
            var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseIdAsync(courseId, includeStudent: includeStudent, includeCourse: true);
            return _mapper.Map<IEnumerable<EnrollmentResponse>>(enrollments);
        }

        public async Task<PagedResult<EnrollmentResponse>> GetEnrollmentsByCourseIdPagedAsync(int courseId, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var query = _enrollmentRepository
                .GetEnrollmentsByCourseIdQueryable(courseId, includeStudent: true, includeCourse: true)
                .OrderBy(e => e.EnrollmentId);

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var enrollmentsData = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<EnrollmentResponse>
            {
                Items = _mapper.Map<IEnumerable<EnrollmentResponse>>(enrollmentsData),
                Pagination = new PaginationMeta
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages
                }
            };
        }

        public async Task<PagedResult<System.Dynamic.ExpandoObject>> GetEnrollmentsAsync(
            string search, string sortBy, string sortOrder, int page, int pageSize, bool expand, string fields)
        {
            var query = _enrollmentRepository.GetEnrollmentsQueryable(expand, expand);

            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();
                query = query.Where(e => e.Status.ToLower().Contains(searchLower));
            }

            bool isDesc = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToLower() == "desc";
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "enrolldate":
                        query = isDesc ? query.OrderByDescending(e => e.EnrollDate) : query.OrderBy(e => e.EnrollDate);
                        break;
                    case "status":
                        query = isDesc ? query.OrderByDescending(e => e.Status) : query.OrderBy(e => e.Status);
                        break;
                    default:
                        query = query.OrderBy(e => e.EnrollmentId);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(e => e.EnrollmentId);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var enrollmentsData = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var enrollmentResponses = _mapper.Map<IEnumerable<EnrollmentResponse>>(enrollmentsData);
            var shapedData = PRN232.Lab1.Services.Helpers.DataShaper.ShapeData(enrollmentResponses, fields);

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

        public async Task<EnrollmentResponse> CreateEnrollmentAsync(EnrollmentRequest request)
        {
            var enrollment = _mapper.Map<Enrollment>(request);
            await _enrollmentRepository.AddAsync(enrollment);
            return _mapper.Map<EnrollmentResponse>(enrollment);
        }

        public async Task<bool> UpdateEnrollmentAsync(int id, EnrollmentRequest request)
        {
            var enrollment = await _enrollmentRepository.GetEnrollmentByIdAsync(id);
            if (enrollment == null) return false;

            _mapper.Map(request, enrollment);
            await _enrollmentRepository.UpdateAsync(enrollment);
            return true;
        }

        public async Task<bool> DeleteEnrollmentAsync(int id)
        {
            var enrollment = await _enrollmentRepository.GetEnrollmentByIdAsync(id);
            if (enrollment == null) return false;

            await _enrollmentRepository.DeleteAsync(enrollment);
            return true;
        }
    }
}
