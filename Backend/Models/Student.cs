namespace PaymentService.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public int CourseId { get; set; }
    }
}
