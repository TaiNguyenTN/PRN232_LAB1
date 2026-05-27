namespace PRN232.Lab1.Services.Models.Responses
{
    public class EnrollmentResponse
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; }

        // Dùng Flat variants để tránh circular reference
        public CourseFlatResponse Course { get; set; }
        public StudentFlatResponse Student { get; set; }
    }
}
