namespace PaymentService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Admin, Staff, Student
        public string? StudentCode { get; set; }
        public string? TeacherCode { get; set; }
    }
}
