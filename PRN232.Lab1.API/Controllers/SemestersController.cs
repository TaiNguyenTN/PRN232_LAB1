using Microsoft.AspNetCore.Mvc;
using PRN232.Lab1.Services.Models.Responses;
using PRN232.Lab1.Services.Models.Requests;
using PRN232.Lab1.Services.Services;

namespace PRN232.Lab1.API.Controllers
{
    [Route("api/semesters")]
    [ApiController]
    public class SemestersController : ControllerBase
    {
        private readonly SemesterService _semesterService;

        public SemestersController(SemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        /// <summary>
        /// Retrieves a paginated list of semesters.
        /// </summary>
        /// <param name="search">Search keyword for <c>SemesterName</c> (case-insensitive substring match).</param>
        /// <param name="sortBy">The field to sort by. Supported: <c>semesterName</c>, <c>startDate</c>. Default: <c>semesterId</c>.</param>
        /// <param name="sortOrder">Sorting order: <c>asc</c> or <c>desc</c>. Default: <c>asc</c>.</param>
        /// <param name="page">Page number (starting from 1). Default: 1.</param>
        /// <param name="pageSize">Number of elements per page. Default: 10.</param>
        /// <param name="fields">
        /// Comma-separated list of fields to return. Example: <c>semesterId,semesterName,startDate,endDate</c>.
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
            var pagedResult = await _semesterService.GetSemestersAsync(search, sortBy, sortOrder, page, pageSize, fields);

            var response = new ApiResponse<System.Collections.Generic.IEnumerable<System.Dynamic.ExpandoObject>>
            {
                Success = true,
                Message = "Semesters retrieved successfully",
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
        /// Retrieves a specific semester by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the semester.</param>
        /// <param name="expand">If <c>true</c>, includes the list of courses belonging to this semester.</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] bool expand = false)
        {
            var semester = await _semesterService.GetSemesterByIdAsync(id, expand);
            if (semester == null)
            {
                return NotFound(ApiResponse<object>.FailureResult($"Semester with ID {id} not found"));
            }
            return Ok(ApiResponse<SemesterResponse>.SuccessResult(semester, "Semester retrieved successfully"));
        }

        // POST: api/semesters
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SemesterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Invalid model state", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _semesterService.CreateSemesterAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = result.SemesterId }, ApiResponse<SemesterResponse>.SuccessResult(result, "Semester created successfully"));
        }

        // PUT: api/semesters/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SemesterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Invalid model state", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var updated = await _semesterService.UpdateSemesterAsync(id, request);
            if (!updated)
            {
                return NotFound(ApiResponse<object>.FailureResult($"Semester with ID {id} not found"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "Semester updated successfully"));
        }

        // DELETE: api/semesters/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _semesterService.DeleteSemesterAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<object>.FailureResult($"Semester with ID {id} not found"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "Semester deleted successfully"));
        }
    }
}
