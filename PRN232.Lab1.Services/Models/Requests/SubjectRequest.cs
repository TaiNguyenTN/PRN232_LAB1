using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab1.Services.Models.Requests
{
    /// <summary>
    /// Payload dùng để tạo/cập nhật Subject (môn học).
    /// </summary>
    public class SubjectRequest
    {
        /// <summary>
        /// Mã môn học (tối đa 20 ký tự), ví dụ: <c>PRN232</c>.
        /// </summary>
        [Required(ErrorMessage = "SubjectCode is required")]
        [MaxLength(20, ErrorMessage = "SubjectCode cannot exceed 20 characters")]
        public string SubjectCode { get; set; }

        /// <summary>
        /// Tên môn học (tối đa 100 ký tự).
        /// </summary>
        [Required(ErrorMessage = "SubjectName is required")]
        [MaxLength(100, ErrorMessage = "SubjectName cannot exceed 100 characters")]
        public string SubjectName { get; set; }

        /// <summary>
        /// Số tín chỉ (1-10).
        /// </summary>
        [Required(ErrorMessage = "Credit is required")]
        [Range(1, 10, ErrorMessage = "Credit must be between 1 and 10")]
        public int Credit { get; set; }
    }
}
