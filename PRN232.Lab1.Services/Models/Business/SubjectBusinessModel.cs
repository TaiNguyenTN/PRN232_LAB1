namespace PRN232.Lab1.Services.Models.Business
{
    public class SubjectBusinessModel
    {
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public int Credit { get; set; }
        
        // Ví dụ một thuộc tính Business Logic: Môn học có phải môn chính không (>= 3 tín chỉ)
        public bool IsMajorSubject => Credit >= 3;
    }
}
