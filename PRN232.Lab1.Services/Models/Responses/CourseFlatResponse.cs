namespace PRN232.Lab1.Services.Models.Responses
{
    /// <summary>
    /// Flat version of CourseResponse - dùng khi nhúng bên trong SemesterResponse hoặc EnrollmentResponse
    /// để tránh circular reference (không chứa Semester hay Enrollments)
    /// </summary>
    public class CourseFlatResponse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int SemesterId { get; set; }
    }
}
