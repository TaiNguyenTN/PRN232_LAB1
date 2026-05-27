using System;
using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab1.Services.Models.Requests
{
    /// <summary>
    /// Payload dùng để tạo/cập nhật Student.
    /// </summary>
    public class StudentRequest
    {
        /// <summary>
        /// Họ và tên sinh viên (tối đa 100 ký tự).
        /// </summary>
        [Required(ErrorMessage = "FullName is required")]
        [MaxLength(100, ErrorMessage = "FullName cannot exceed 100 characters")]
        public string FullName { get; set; }

        /// <summary>
        /// Email sinh viên (đúng định dạng email, tối đa 100 ký tự).
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }

        /// <summary>
        /// Ngày sinh (DateTime). Swagger thường nhập theo ISO 8601, ví dụ: <c>2000-01-31</c>.
        /// </summary>
        [Required(ErrorMessage = "DateOfBirth is required")]
        public DateTime DateOfBirth { get; set; }
    }
}
