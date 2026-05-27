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
    public class SemesterService
    {
        private readonly SemesterRepository _semesterRepository;
        private readonly IMapper _mapper;

        public SemesterService(SemesterRepository semesterRepository, IMapper mapper)
        {
            _semesterRepository = semesterRepository;
            _mapper = mapper;
        }

        public async Task<SemesterResponse> GetSemesterByIdAsync(int id, bool expand = false)
        {
            var semester = await _semesterRepository.GetSemesterByIdAsync(id, expand);
            if (semester == null) return null;

            return _mapper.Map<SemesterResponse>(semester);
        }

        public async Task<PagedResult<System.Dynamic.ExpandoObject>> GetSemestersAsync(
            string search, string sortBy, string sortOrder, int page, int pageSize, string fields)
        {
            var query = _semesterRepository.GetSemestersQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();
                query = query.Where(s => s.SemesterName.ToLower().Contains(searchLower));
            }

            bool isDesc = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToLower() == "desc";
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "semestername":
                        query = isDesc ? query.OrderByDescending(s => s.SemesterName) : query.OrderBy(s => s.SemesterName);
                        break;
                    case "startdate":
                        query = isDesc ? query.OrderByDescending(s => s.StartDate) : query.OrderBy(s => s.StartDate);
                        break;
                    default:
                        query = query.OrderBy(s => s.SemesterId);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(s => s.SemesterId);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var semestersData = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var semesterResponses = _mapper.Map<IEnumerable<SemesterResponse>>(semestersData);
            var shapedData = PRN232.Lab1.Services.Helpers.DataShaper.ShapeData(semesterResponses, fields);

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

        public async Task<SemesterResponse> CreateSemesterAsync(SemesterRequest request)
        {
            var semester = _mapper.Map<Semester>(request);
            await _semesterRepository.AddAsync(semester);
            return _mapper.Map<SemesterResponse>(semester);
        }

        public async Task<bool> UpdateSemesterAsync(int id, SemesterRequest request)
        {
            var semester = await _semesterRepository.GetSemesterByIdAsync(id);
            if (semester == null) return false;

            _mapper.Map(request, semester);
            await _semesterRepository.UpdateAsync(semester);
            return true;
        }

        public async Task<bool> DeleteSemesterAsync(int id)
        {
            var semester = await _semesterRepository.GetSemesterByIdAsync(id);
            if (semester == null) return false;

            await _semesterRepository.DeleteAsync(semester);
            return true;
        }
    }
}
