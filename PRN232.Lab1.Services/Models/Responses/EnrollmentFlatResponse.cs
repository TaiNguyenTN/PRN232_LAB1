namespace PRN232.Lab1.Services.Models.Responses
{
    /// <summary>
    /// Flat version of EnrollmentResponse - dùng khi nhúng bên trong StudentResponse
    /// để tránh circular reference (không chứa Student)
    /// </summary>
    public class EnrollmentFlatResponse
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; }

        // Chỉ chứa Course (flat), không chứa Student để tránh vòng lặp
        public CourseFlatResponse Course { get; set; }
    }
}
