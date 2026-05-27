namespace PRN232.Lab1.Services.Models.Responses
{
    /// <summary>
    /// Flat version of StudentResponse - dùng khi nhúng bên trong EnrollmentResponse
    /// để tránh circular reference (không chứa Enrollments)
    /// </summary>
    public class StudentFlatResponse
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
