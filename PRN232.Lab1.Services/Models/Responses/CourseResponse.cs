namespace PRN232.Lab1.Services.Models.Responses
{
    public class CourseResponse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int SemesterId { get; set; }
        public ICollection<EnrollmentForCourseResponse> Enrollments { get; set; }
    }
}
