using Microsoft.AspNetCore.Mvc;
using PRN232.Lab1.Services.Models.Responses;
using PRN232.Lab1.Services.Models.Requests;
using PRN232.Lab1.Services.Services;

namespace PRN232.Lab1.API.Controllers
{
    [Route("api/subjects")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly SubjectService _subjectService;

        public SubjectsController(SubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        /// <summary>
        /// Retrieves a paginated list of subjects.
        /// </summary>
        /// <param name="search">Search keyword for <c>SubjectName</c> or <c>SubjectCode</c> (case-insensitive substring match).</param>
        /// <param name="sortBy">The field to sort by. Supported: <c>subjectName</c>, <c>subjectCode</c>. Default: <c>subjectId</c>.</param>
        /// <param name="sortOrder">Sorting order: <c>asc</c> or <c>desc</c>. Default: <c>asc</c>.</param>
        /// <param name="page">Page number (starting from 1). Default: 1.</param>
        /// <param name="pageSize">Number of elements per page. Default: 10.</param>
        /// <param name="fields">
        /// Comma-separated list of fields to return. Example: <c>subjectId,subjectCode,subjectName,credit</c>.
        /// Case-insensitive; invalid fields will be ignored.
        /// If left empty/null, all fields will be returned. Output keys are converted to camelCase.
        /// </param>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string search = null,
            [FromQuery] string sortBy = null,
            [FromQuery] string sortOrder = "asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string fields = null)
        {
            var pagedResult = await _subjectService.GetSubjectsAsync(search, sortBy, sortOrder, page, pageSize, fields);

            var response = new ApiResponse<System.Collections.Generic.IEnumerable<System.Dynamic.ExpandoObject>>
            {
                Success = true,
                Message = "Subjects retrieved successfully",
                Data = pagedResult.Items,
                Errors = null
            };

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data,
                pagination = pagedResult.Pagination,
                errors = response.Errors
            });
        }

        /// <summary>
        /// Retrieves a specific subject by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the subject.</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject == null)
            {
                return NotFound(ApiResponse<object>.FailureResult($"Subject with ID {id} not found"));
            }
            return Ok(ApiResponse<SubjectResponse>.SuccessResult(subject, "Subject retrieved successfully"));
        }

        // POST: api/subjects
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubjectRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Invalid model state", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _subjectService.CreateSubjectAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = result.SubjectId }, ApiResponse<SubjectResponse>.SuccessResult(result, "Subject created successfully"));
        }

        // PUT: api/subjects/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SubjectRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Invalid model state", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var updated = await _subjectService.UpdateSubjectAsync(id, request);
            if (!updated)
            {
                return NotFound(ApiResponse<object>.FailureResult($"Subject with ID {id} not found"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "Subject updated successfully"));
        }

        // DELETE: api/subjects/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _subjectService.DeleteSubjectAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<object>.FailureResult($"Subject with ID {id} not found"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "Subject deleted successfully"));
        }
    }
}
