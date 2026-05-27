using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PRN232.Lab1.Repositories.Entities
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // Seed 5 Semesters
            var semesters = new List<Semester>();
            for (int i = 1; i <= 5; i++)
            {
                semesters.Add(new Semester
                {
                    SemesterId = i,
                    SemesterName = $"Semester {i}",
                    StartDate = new DateTime(2023, 1, 1).AddMonths((i - 1) * 4),
                    EndDate = new DateTime(2023, 4, 30).AddMonths((i - 1) * 4)
                });
            }
            modelBuilder.Entity<Semester>().HasData(semesters);

            // Seed 10 Subjects
            var subjects = new List<Subject>();
            for (int i = 1; i <= 10; i++)
            {
                subjects.Add(new Subject
                {
                    SubjectId = i,
                    SubjectCode = $"SUB{i:000}",
                    SubjectName = $"Subject {i}",
                    Credit = 3
                });
            }
            modelBuilder.Entity<Subject>().HasData(subjects);

            // Seed 20 Courses
            var courses = new List<Course>();
            for (int i = 1; i <= 20; i++)
            {
                courses.Add(new Course
                {
                    CourseId = i,
                    CourseName = $"Course {i}",
                    SemesterId = (i % 5) + 1 // distribute among 5 semesters
                });
            }
            modelBuilder.Entity<Course>().HasData(courses);

            // Seed 50 Students
            var students = new List<Student>();
            for (int i = 1; i <= 50; i++)
            {
                students.Add(new Student
                {
                    StudentId = i,
                    FullName = $"Student Name {i}",
                    Email = $"student{i}@fpt.edu.vn",
                    DateOfBirth = new DateTime(2000, 1, 1).AddDays(i)
                });
            }
            modelBuilder.Entity<Student>().HasData(students);

            // Seed 500 Enrollments
            var enrollments = new List<Enrollment>();
            int enrollmentId = 1;
            for (int studentId = 1; studentId <= 50; studentId++)
            {
                // Each student enrolls in 10 courses -> 500 enrollments
                for (int j = 1; j <= 10; j++)
                {
                    int courseId = ((studentId + j) % 20) + 1;
                    enrollments.Add(new Enrollment
                    {
                        EnrollmentId = enrollmentId++,
                        StudentId = studentId,
                        CourseId = courseId,
                        EnrollDate = new DateTime(2024, 1, 1).AddDays(-j),
                        Status = j % 2 == 0 ? "Active" : "Completed"
                    });
                }
            }
            modelBuilder.Entity<Enrollment>().HasData(enrollments);
        }
    }
}
