using System;
using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab1.Services.Models.Requests
{
    /// <summary>
    /// Payload dùng để tạo/cập nhật Enrollment (đăng ký học).
    /// </summary>
    public class EnrollmentRequest
    {
        /// <summary>
        /// Id của Student đăng ký.
        /// </summary>
        [Required(ErrorMessage = "StudentId is required")]
        public int StudentId { get; set; }

        /// <summary>
        /// Id của Course được đăng ký.
        /// </summary>
        [Required(ErrorMessage = "CourseId is required")]
        public int CourseId { get; set; }

        /// <summary>
        /// Ngày đăng ký (DateTime). Swagger thường nhập theo ISO 8601, ví dụ: <c>2026-05-21</c>.
        /// </summary>
        [Required(ErrorMessage = "EnrollDate is required")]
        public DateTime EnrollDate { get; set; }

        /// <summary>
        /// Trạng thái enrollment (tối đa 20 ký tự), ví dụ: <c>Active</c>, <c>Completed</c>.
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        public string Status { get; set; }
    }
}
