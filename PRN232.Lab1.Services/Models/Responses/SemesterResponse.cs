using System;

namespace PRN232.Lab1.Services.Models.Responses
{
    public class SemesterResponse
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        // Dùng CourseFlatResponse để tránh circular reference (không chứa Semester)
        public ICollection<CourseFlatResponse> Courses { get; set; }
    }
}
