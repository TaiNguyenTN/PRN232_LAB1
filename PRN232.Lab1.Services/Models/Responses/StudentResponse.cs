namespace PRN232.Lab1.Services.Models.Responses
{
    public class StudentResponse
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        // Dùng EnrollmentFlatResponse để tránh circular reference (không chứa Student)
        public ICollection<EnrollmentFlatResponse> Enrollments { get; set; }
        //public ICollection<int> EnrollmentsId { get; set; }
    }
}
