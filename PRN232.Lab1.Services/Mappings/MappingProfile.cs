using AutoMapper;
using PRN232.Lab1.Repositories.Entities;
using PRN232.Lab1.Services.Models.Business;
using PRN232.Lab1.Services.Models.Requests;
using PRN232.Lab1.Services.Models.Responses;

namespace PRN232.Lab1.Services.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ================================================
            // Map từ Entity sang Business Model
            // ================================================
            CreateMap<Student, StudentBusinessModel>();
            CreateMap<Course, CourseBusinessModel>();
            CreateMap<Enrollment, EnrollmentBusinessModel>();
            CreateMap<Semester, SemesterBusinessModel>();
            CreateMap<Subject, SubjectBusinessModel>();

            // ================================================
            // Map từ Request sang Entity
            // ================================================
            CreateMap<StudentRequest, Student>();
            CreateMap<CourseRequest, Course>();
            CreateMap<EnrollmentRequest, Enrollment>();
            CreateMap<SemesterRequest, Semester>();
            CreateMap<SubjectRequest, Subject>();

            // ================================================
            // Map từ Entity sang "Flat" Response (không chứa back-reference)
            // Dùng khi các đối tượng được nhúng lồng bên trong nhau
            // ================================================
            CreateMap<Student, StudentFlatResponse>();
            CreateMap<Course, CourseFlatResponse>();
            CreateMap<Enrollment, EnrollmentFlatResponse>();
            CreateMap<Enrollment, EnrollmentForCourseResponse>();

            // ================================================
            // Map từ Entity sang Full Response (dùng ở top-level)
            // StudentResponse.Enrollments -> EnrollmentFlatResponse (không chứa Student -> break cycle)
            // EnrollmentResponse.Student  -> StudentFlatResponse     (không chứa Enrollments -> break cycle)
            // EnrollmentResponse.Course   -> CourseFlatResponse      (không chứa Semester.Courses -> break cycle)
            // SemesterResponse.Courses    -> CourseFlatResponse      (không chứa Semester -> break cycle)
            // CourseResponse.Semester     -> SemesterResponse (ok vì SemesterResponse.Courses là CourseFlatResponse)
            // ================================================
            CreateMap<Student, StudentResponse>();
            CreateMap<Course, CourseResponse>()
                .ForMember(dest => dest.Enrollments, opt => opt.MapFrom(src => src.Enrollments));
            CreateMap<Enrollment, EnrollmentResponse>();
            CreateMap<Semester, SemesterResponse>();
            CreateMap<Subject, SubjectResponse>();
        }
    }
}
