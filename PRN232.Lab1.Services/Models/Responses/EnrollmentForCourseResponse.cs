using PRN232.Lab1.Services.Models.Responses;
using System;
using System.Collections.Generic;

namespace PRN232.Lab1.Services.Models.Responses
{
    /// <summary>
    /// Represents an enrollment when viewed from within a Course context.
    /// This version omits the 'Course' property to avoid redundancy.
    /// </summary>
    public class EnrollmentForCourseResponse
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; }

        // Student is included, but not the parent Course
        public StudentFlatResponse Student { get; set; }
    }
}
