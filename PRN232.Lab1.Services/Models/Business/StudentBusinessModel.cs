namespace PRN232.Lab1.Services.Models.Business
{
    public class StudentBusinessModel
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }

        // Ví dụ một thuộc tính Business Logic nội bộ: Tính tuổi của sinh viên
        public int Age => DateTime.Today.Year - DateOfBirth.Year;
    }
}
