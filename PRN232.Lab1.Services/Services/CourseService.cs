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
    public class CourseService
    {
        private readonly CourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public CourseService(CourseRepository courseRepository, IMapper mapper)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        public async Task<CourseResponse> GetCourseByIdAsync(int id, bool expand = false)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id, expand, expand); // expand cho cả semester và enrollments
            if (course == null) return null;

            return _mapper.Map<CourseResponse>(course);
        }

        public async Task<PagedResult<System.Dynamic.ExpandoObject>> GetCoursesAsync(
            string search, string sortBy, string sortOrder, int page, int pageSize, bool expand, string fields)
        {
            var query = _courseRepository.GetCoursesQueryable(expand);

            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();
                query = query.Where(c => c.CourseName.ToLower().Contains(searchLower));
            }

            bool isDesc = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToLower() == "desc";
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "coursename":
                        query = isDesc ? query.OrderByDescending(c => c.CourseName) : query.OrderBy(c => c.CourseName);
                        break;
                    default:
                        query = query.OrderBy(c => c.CourseId);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(c => c.CourseId);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var coursesData = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var courseResponses = _mapper.Map<IEnumerable<CourseResponse>>(coursesData);
            var shapedData = PRN232.Lab1.Services.Helpers.DataShaper.ShapeData(courseResponses, fields);

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

        public async Task<CourseResponse> CreateCourseAsync(CourseRequest request)
        {
            var course = _mapper.Map<Course>(request);
            await _courseRepository.AddAsync(course);
            return _mapper.Map<CourseResponse>(course);
        }

        public async Task<bool> UpdateCourseAsync(int id, CourseRequest request)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null) return false;

            _mapper.Map(request, course);
            await _courseRepository.UpdateAsync(course);
            return true;
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null) return false;

            await _courseRepository.DeleteAsync(course);
            return true;
        }
    }
}
