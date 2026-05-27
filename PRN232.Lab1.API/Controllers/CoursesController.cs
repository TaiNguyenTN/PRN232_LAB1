using Microsoft.AspNetCore.Mvc;
using PRN232.Lab1.Services.Models.Responses;
using PRN232.Lab1.Services.Models.Requests;
using PRN232.Lab1.Services.Services;

namespace PRN232.Lab1.API.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly EnrollmentService _enrollmentService;

        public CoursesController(CourseService courseService, EnrollmentService enrollmentService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
        }

        /// <summary>
        /// Retrieves a paginated list of courses.
        /// </summary>
        /// <param name="search">Search keyword for <c>CourseName</c> (case-insensitive substring match).</param>
        /// <param name="sortBy">The field to sort by. Supported: <c>courseName</c>. Default: <c>courseId</c>.</param>
        /// <param name="sortOrder">Sorting order: <c>asc</c> or <c>desc</c>. Default: <c>asc</c>.</param>
        /// <param name="page">Page number (starting from 1). Default: 1.</param>
        /// <param name="pageSize">Number of elements per page. Default: 10.</param>
        /// <param name="expand">If <c>true</c>, includes related data (Course -> Semester).</param>
        /// <param name="fields">
        /// Comma-separated list of fields to return. Example: <c>courseId,courseName,semesterId</c>.
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
            [FromQuery] bool expand = false,
            [FromQuery] string fields = null)
        {
            var pagedResult = await _courseService.GetCoursesAsync(search, sortBy, sortOrder, page, pageSize, expand, fields);

            var response = new ApiResponse<System.Collections.Generic.IEnumerable<System.Dynamic.ExpandoObject>>
            {
                Success = true,
                Message = "Courses retrieved successfully",
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
        /// Retrieves a specific course by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the course.</param>
        /// <param name="expand">If <c>true</c>, includes related Semester data.</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] bool expand = false)
        {
            var course = await _courseService.GetCourseByIdAsync(id, expand);
            if (course == null)
            {
                return NotFound(ApiResponse<object>.FailureResult($"Course with ID {id} not found"));
            }
            return Ok(ApiResponse<CourseResponse>.SuccessResult(course, "Course retrieved successfully"));
        }

        /// <summary>
        /// Retrieves a specific course by its ID, optionally expanding its enrollments and student details.
        /// </summary>
        /// <param name="id">The unique identifier of the course.</param>
        /// <param name="expand">Set to 'student' to include detailed student information for each enrollment.</param>
        // GET: api/courses/{id}/enrollments
        [HttpGet("{id}/enrollments")]
        public async Task<IActionResult> GetEnrollmentsByCourse(int id, [FromQuery] string expand = "")
        {
            // This call now fetches the full nested structure: Course -> Enrollments -> Student
            var course = await _courseService.GetCourseByIdAsync(id, expand: true); 
            
            if (course == null)
            {
                return NotFound(ApiResponse<object>.FailureResult($"Course with ID {id} not found"));
            }

            // The 'course' object (CourseResponse) already contains the desired nested structure.
            // We just return it directly.
            return Ok(ApiResponse<CourseResponse>.SuccessResult(course, $"Course and its enrollments retrieved successfully"));
        }

        // POST: api/courses
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Invalid model state", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _courseService.CreateCourseAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = result.CourseId }, ApiResponse<CourseResponse>.SuccessResult(result, "Course created successfully"));
        }

        // PUT: api/courses/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Invalid model state", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var updated = await _courseService.UpdateCourseAsync(id, request);
            if (!updated)
            {
                return NotFound(ApiResponse<object>.FailureResult($"Course with ID {id} not found"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "Course updated successfully"));
        }

        // DELETE: api/courses/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _courseService.DeleteCourseAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<object>.FailureResult($"Course with ID {id} not found"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "Course deleted successfully"));
        }
    }
}
