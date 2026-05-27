namespace PRN232.Lab1.Repositories.Entities
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }

        // Navigation property
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
