namespace PRN232.Lab1.Repositories.Entities
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int SemesterId { get; set; }

        // Navigation property
        public Semester Semester { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
