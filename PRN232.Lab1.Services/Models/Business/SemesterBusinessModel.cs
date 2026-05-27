namespace PRN232.Lab1.Services.Models.Business
{
    public class SemesterBusinessModel
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        // Ví dụ một thuộc tính Business Logic: Kiểm tra học kỳ có đang diễn ra hay không
        public bool IsOngoing => StartDate <= DateTime.Today && EndDate >= DateTime.Today;
    }
}
