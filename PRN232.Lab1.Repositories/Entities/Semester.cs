namespace PRN232.Lab1.Repositories.Entities
{
    public class Semester
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Navigation property
        public ICollection<Course> Courses { get; set; }
    }
}
