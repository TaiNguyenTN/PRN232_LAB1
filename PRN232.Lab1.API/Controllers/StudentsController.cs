using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab1.Services.Models.Responses;
using PRN232.Lab1.Services.Models.Requests;
using PRN232.Lab1.Services.Services;

namespace PRN232.Lab1.API.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentService _studentService;

        public StudentsController(StudentService studentService)
        {
            _studentService = studentService;
        }

        /// <summary>
        /// Retrieves a paginated list of students.
        /// </summary>
        /// <param name="search">
        /// Search keyword for <c>FullName</c> or <c>Email</c> (case-insensitive substring match).
        /// </param>
        /// <param name="sortBy">The field to sort by. Supported: <c>fullName</c>, <c>email</c>. Default: <c>studentId</c>.</param>
        /// <param name="sortOrder">Sorting order: <c>asc</c> or <c>desc</c>. Default: <c>asc</c>.</param>
        /// <param name="page">Page number (starting from 1). Default: 1.</param>
        /// <param name="pageSize">Number of elements per page. Default: 10.</param>
        /// <param name="expand">If <c>true</c>, includes the student's list of enrollments.</param>
        /// <param name="fields">
        /// Comma-separated list of fields to return. Example: <c>studentId,fullName,email</c>.
        /// Case-insensitive; invalid fields will be ignored.
        /// If left empty/null, all fields will be returned. Output keys are converted to camelCase.
        /// </param>
        // GET: api/students
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string search = null,
            [FromQuery] string sortBy = null,
            [FromQuery] string sortOrder = "asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool expand = false,
            [FromQuery] string fields = null)
        {
            var pagedResult = await _studentService.GetStudentsAsync(search, sortBy, sortOrder, page, pageSize, expand, fields);

            var response = new ApiResponse<System.Collections.Generic.IEnumerable<System.Dynamic.ExpandoObject>>
            {
                Success = true,
                Message = "Students retrieved successfully",
                Data = pagedResult.Items,
                Errors = null
            };

            // Ép thêm thông tin pagination vào custom anonymous object hoặc headers nếu cần (ở đây trả luôn chung format)
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
        /// Retrieves a specific student by their ID.
        /// </summary>
        /// <param name="id">The unique identifier of the student.</param>
        /// <param name="expand">If <c>true</c>, includes the student's list of enrollments.</param>
        // GET: api/students/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] bool expand = false)
        {
            var student = await _studentService.GetStudentByIdAsync(id, expand);

            if (student == null)
            {
                return NotFound(ApiResponse<object>.FailureResult($"Student with ID {id} not found"));
            }

            return Ok(ApiResponse<StudentResponse>.SuccessResult(student, "Student retrieved successfully"));
        }

        // POST: api/students
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StudentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Invalid model state", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _studentService.CreateStudentAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = result.StudentId }, ApiResponse<StudentResponse>.SuccessResult(result, "Student created successfully"));
        }

        // PUT: api/students/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StudentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Invalid model state", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var updated = await _studentService.UpdateStudentAsync(id, request);
            if (!updated)
            {
                return NotFound(ApiResponse<object>.FailureResult($"Student with ID {id} not found"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "Student updated successfully"));
        }

        // DELETE: api/students/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _studentService.DeleteStudentAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<object>.FailureResult($"Student with ID {id} not found"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "Student deleted successfully"));
        }
    }
}
