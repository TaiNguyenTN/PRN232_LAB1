using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab1.Services.Models.Requests
{
    /// <summary>
    /// Payload dùng để tạo/cập nhật Course.
    /// </summary>
    public class CourseRequest
    {
        /// <summary>
        /// Tên course (tối đa 100 ký tự).
        /// </summary>
        [Required(ErrorMessage = "CourseName is required")]
        [MaxLength(100, ErrorMessage = "CourseName cannot exceed 100 characters")]
        public string CourseName { get; set; }

        /// <summary>
        /// Id của Semester mà course thuộc về.
        /// </summary>
        [Required(ErrorMessage = "SemesterId is required")]
        public int SemesterId { get; set; }
    }
}
