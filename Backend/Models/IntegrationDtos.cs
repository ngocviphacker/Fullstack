namespace PaymentService.Models
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Fee { get; set; }
    }

    public class StudentDto
    {
        public int Id { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class ClassDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CourseId { get; set; }
    }

    public class RegistrationDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public System.DateTime RegistrationDate { get; set; }
    }
}
