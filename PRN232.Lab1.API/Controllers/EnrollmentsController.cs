using Microsoft.AspNetCore.Mvc;
using PRN232.Lab1.Services.Models.Responses;
using PRN232.Lab1.Services.Models.Requests;
using PRN232.Lab1.Services.Services;

namespace PRN232.Lab1.API.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly EnrollmentService _enrollmentService;

        public EnrollmentsController(EnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        /// <summary>
        /// Retrieves a paginated list of enrollments.
        /// </summary>
        /// <param name="search">Search keyword for <c>Status</c> (case-insensitive substring match).</param>
        /// <param name="sortBy">The field to sort by. Supported: <c>enrollDate</c>, <c>status</c>. Default: <c>enrollmentId</c>.</param>
        /// <param name="sortOrder">Sorting order: <c>asc</c> or <c>desc</c>. Default: <c>asc</c>.</param>
        /// <param name="page">Page number (starting from 1). Default: 1.</param>
        /// <param name="pageSize">Number of elements per page. Default: 10.</param>
        /// <param name="expand">Comma-separated list of related entities to include. Example: <c>student,course</c>.</param>
        /// <param name="fields">
        /// Comma-separated list of fields to return. Example: <c>enrollmentId,studentId,courseId,status</c>.
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
            [FromQuery] string expand = "",
            [FromQuery] string fields = null)
        {
            var pagedResult = await _enrollmentService.GetEnrollmentsAsync(search, sortBy, sortOrder, page, pageSize, expand, fields);

            var response = new ApiResponse<System.Collections.Generic.IEnumerable<System.Dynamic.ExpandoObject>>
            {
                Success = true,
                Message = "Enrollments retrieved successfully",
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
        /// Retrieves a specific enrollment by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the enrollment.</param>
        /// <param name="expand">If <c>true</c>, includes related Student and Course data.</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] bool expand = false)
        {
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id, expand);
            if (enrollment == null)
            {
                return NotFound(ApiResponse<object>.FailureResult($"Enrollment with ID {id} not found"));
            }
            return Ok(ApiResponse<EnrollmentResponse>.SuccessResult(enrollment, "Enrollment retrieved successfully"));
        }

        // POST: api/enrollments
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EnrollmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Invalid model state", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _enrollmentService.CreateEnrollmentAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = result.EnrollmentId }, ApiResponse<EnrollmentResponse>.SuccessResult(result, "Enrollment created successfully"));
        }

        // PUT: api/enrollments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EnrollmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Invalid model state", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var updated = await _enrollmentService.UpdateEnrollmentAsync(id, request);
            if (!updated)
            {
                return NotFound(ApiResponse<object>.FailureResult($"Enrollment with ID {id} not found"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "Enrollment updated successfully"));
        }

        // DELETE: api/enrollments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _enrollmentService.DeleteEnrollmentAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<object>.FailureResult($"Enrollment with ID {id} not found"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "Enrollment deleted successfully"));
        }
    }
}
