namespace PRN232.Lab1.Services.Models.Business
{
    public class EnrollmentBusinessModel
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; }
        
        // Ví dụ một thuộc tính Business Logic: Kiểm tra trạng thái có đang active không
        public bool IsActive => Status == "Active" || Status == "Enrolled";
    }
}
