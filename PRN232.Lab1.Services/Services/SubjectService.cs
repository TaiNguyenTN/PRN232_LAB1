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
    public class SubjectService
    {
        private readonly SubjectRepository _subjectRepository;
        private readonly IMapper _mapper;

        public SubjectService(SubjectRepository subjectRepository, IMapper mapper)
        {
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }

        public async Task<SubjectResponse> GetSubjectByIdAsync(int id)
        {
            var subject = await _subjectRepository.GetSubjectByIdAsync(id);
            if (subject == null) return null;

            return _mapper.Map<SubjectResponse>(subject);
        }

        public async Task<PagedResult<System.Dynamic.ExpandoObject>> GetSubjectsAsync(
            string search, string sortBy, string sortOrder, int page, int pageSize, string fields)
        {
            var query = _subjectRepository.GetSubjectsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();
                query = query.Where(s => s.SubjectName.ToLower().Contains(searchLower)
                                      || s.SubjectCode.ToLower().Contains(searchLower));
            }

            bool isDesc = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToLower() == "desc";
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "subjectname":
                        query = isDesc ? query.OrderByDescending(s => s.SubjectName) : query.OrderBy(s => s.SubjectName);
                        break;
                    case "subjectcode":
                        query = isDesc ? query.OrderByDescending(s => s.SubjectCode) : query.OrderBy(s => s.SubjectCode);
                        break;
                    default:
                        query = query.OrderBy(s => s.SubjectId);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(s => s.SubjectId);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var subjectsData = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var subjectResponses = _mapper.Map<IEnumerable<SubjectResponse>>(subjectsData);
            var shapedData = PRN232.Lab1.Services.Helpers.DataShaper.ShapeData(subjectResponses, fields);

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

        public async Task<SubjectResponse> CreateSubjectAsync(SubjectRequest request)
        {
            var subject = _mapper.Map<Subject>(request);
            await _subjectRepository.AddAsync(subject);
            return _mapper.Map<SubjectResponse>(subject);
        }

        public async Task<bool> UpdateSubjectAsync(int id, SubjectRequest request)
        {
            var subject = await _subjectRepository.GetSubjectByIdAsync(id);
            if (subject == null) return false;

            _mapper.Map(request, subject);
            await _subjectRepository.UpdateAsync(subject);
            return true;
        }

        public async Task<bool> DeleteSubjectAsync(int id)
        {
            var subject = await _subjectRepository.GetSubjectByIdAsync(id);
            if (subject == null) return false;

            await _subjectRepository.DeleteAsync(subject);
            return true;
        }
    }
}
