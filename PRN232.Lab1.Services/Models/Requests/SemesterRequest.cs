using System;
using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab1.Services.Models.Requests
{
    /// <summary>
    /// Payload dùng để tạo/cập nhật Semester (học kỳ).
    /// </summary>
    public class SemesterRequest
    {
        /// <summary>
        /// Tên học kỳ (tối đa 100 ký tự).
        /// </summary>
        [Required(ErrorMessage = "SemesterName is required")]
        [MaxLength(100, ErrorMessage = "SemesterName cannot exceed 100 characters")]
        public string SemesterName { get; set; }

        /// <summary>
        /// Ngày bắt đầu (DateTime). Ví dụ: <c>2026-01-01</c>.
        /// </summary>
        [Required(ErrorMessage = "StartDate is required")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc (DateTime). Ví dụ: <c>2026-05-01</c>.
        /// </summary>
        [Required(ErrorMessage = "EndDate is required")]
        public DateTime EndDate { get; set; }
    }
}
