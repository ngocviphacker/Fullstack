using Microsoft.EntityFrameworkCore;
using PaymentService.Models;
using System;

namespace PaymentService.Data
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Class> Classes { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<TuitionInvoice> TuitionInvoices { get; set; } = null!;
        public DbSet<PaymentReceipt> PaymentReceipts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            // Seed replication data
            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Name = "Lập trình Web Fullstack", Fee = 6800000 },
                new Course { Id = 2, Name = "Phân tích Thiết kế Hệ thống", Fee = 5200000 },
                new Course { Id = 3, Name = "Cấu trúc dữ liệu & Giải thuật", Fee = 4500000 },
                new Course { Id = 4, Name = "Trí tuệ nhân tạo (AI) cơ bản", Fee = 8500000 }
            );

            modelBuilder.Entity<Class>().HasData(
                new Class { Id = 1, Name = "K18-INF01", CourseId = 1 },
                new Class { Id = 2, Name = "K18-INF02", CourseId = 1 },
                new Class { Id = 3, Name = "K18-SYS01", CourseId = 2 },
                new Class { Id = 4, Name = "K18-CS01", CourseId = 3 }
            );

            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 101, StudentCode = "SV001", FullName = "Nguyễn Văn Hùng", Email = "hungnv@gmail.com", Phone = "0912345678", ClassId = 1, CourseId = 1 },
                new Student { Id = 102, StudentCode = "SV002", FullName = "Trần Thị Mai", Email = "maitt@gmail.com", Phone = "0987654321", ClassId = 1, CourseId = 1 },
                new Student { Id = 103, StudentCode = "SV003", FullName = "Lê Tuấn Anh", Email = "anhlt@gmail.com", Phone = "0933333333", ClassId = 2, CourseId = 1 },
                new Student { Id = 104, StudentCode = "SV004", FullName = "Phạm Minh Đức", Email = "ducpm@gmail.com", Phone = "0944444444", ClassId = 3, CourseId = 2 },
                new Student { Id = 105, StudentCode = "SV005", FullName = "Hoàng Thanh Thảo", Email = "thaoht@gmail.com", Phone = "0955555555", ClassId = 4, CourseId = 3 }
            );

            // Seed 4 User accounts matching the Slide roles (Admin, Teacher, Student)
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", PasswordHash = "admin123", FullName = "Nguyễn Thế Huy Hoàng (Quản trị viên)", Role = "Admin" },
                new User { Id = 2, Username = "maitt", PasswordHash = "admin123", FullName = "Trần Thị Mai (Kế toán trung tâm)", Role = "Staff" },
                new User { Id = 3, Username = "student", PasswordHash = "admin123", FullName = "Lê Tuấn Anh (Học viên)", Role = "Student", StudentCode = "SV003" },
                new User { Id = 4, Username = "teacher", PasswordHash = "admin123", FullName = "Nguyễn Văn Dạy (Giáo viên giảng dạy)", Role = "Teacher", TeacherCode = "GV001" }
            );

            // Seed default invoices
            modelBuilder.Entity<TuitionInvoice>().HasData(
                new TuitionInvoice { Id = 1, StudentId = 101, StudentCode = "SV001", StudentName = "Nguyễn Văn Hùng", CourseName = "Lập trình Web Fullstack", ClassName = "K18-INF01", InvoiceNumber = "INV-2026-0001", Amount = 6800000, PaidAmount = 6800000, DebtAmount = 0, DueDate = DateTime.Parse("2026-05-10"), Status = "Paid", Description = "Học phí đợt 1", CreatedAt = DateTime.Parse("2026-04-10T08:00:00Z"), CreatedDate = DateTime.Parse("2026-04-10T08:00:00Z") },
                new TuitionInvoice { Id = 2, StudentId = 102, StudentCode = "SV002", StudentName = "Trần Thị Mai", CourseName = "Lập trình Web Fullstack", ClassName = "K18-INF01", InvoiceNumber = "INV-2026-0002", Amount = 6800000, PaidAmount = 3000000, DebtAmount = 3800000, DueDate = DateTime.Parse("2026-06-05"), Status = "PartiallyPaid", Description = "Học phí đợt 1", CreatedAt = DateTime.Parse("2026-04-10T08:30:00Z"), CreatedDate = DateTime.Parse("2026-04-10T08:30:00Z") },
                new TuitionInvoice { Id = 3, StudentId = 103, StudentCode = "SV003", StudentName = "Lê Tuấn Anh", CourseName = "Lập trình Web Fullstack", ClassName = "K18-INF02", InvoiceNumber = "INV-2026-0003", Amount = 6800000, PaidAmount = 0, DebtAmount = 6800000, DueDate = DateTime.Parse("2026-06-15"), Status = "Unpaid", Description = "Học phí đợt 1", CreatedAt = DateTime.Parse("2026-04-12T09:00:00Z"), CreatedDate = DateTime.Parse("2026-04-12T09:00:00Z") },
                new TuitionInvoice { Id = 4, StudentId = 104, StudentCode = "SV004", StudentName = "Phạm Minh Đức", CourseName = "Phân tích Thiết kế Hệ thống", ClassName = "K18-SYS01", InvoiceNumber = "INV-2026-0004", Amount = 5200000, PaidAmount = 5200000, DebtAmount = 0, DueDate = DateTime.Parse("2026-05-01"), Status = "Paid", Description = "Trọn gói học phí", CreatedAt = DateTime.Parse("2026-04-01T10:00:00Z"), CreatedDate = DateTime.Parse("2026-04-01T10:00:00Z") },
                new TuitionInvoice { Id = 5, StudentId = 105, StudentCode = "SV005", StudentName = "Hoàng Thanh Thảo", CourseName = "Cấu trúc dữ liệu & Giải thuật", ClassName = "K18-CS01", InvoiceNumber = "INV-2026-0005", Amount = 4500000, PaidAmount = 0, DebtAmount = 4500000, DueDate = DateTime.Parse("2026-05-20"), Status = "Unpaid", Description = "Lần đóng đầu tiên", CreatedAt = DateTime.Parse("2026-04-20T14:00:00Z"), CreatedDate = DateTime.Parse("2026-04-20T14:00:00Z") },
                new TuitionInvoice { Id = 6, StudentId = 106, StudentCode = "SV006", StudentName = "Nguyễn Minh Tuấn", CourseName = "Lập trình Web Fullstack", ClassName = "K18-INF01", InvoiceNumber = "INV-2026-0006", Amount = 6800000, PaidAmount = 6800000, DebtAmount = 0, DueDate = DateTime.Parse("2026-05-28"), Status = "Paid", Description = "Đóng học phí trọn khóa", CreatedAt = DateTime.Parse("2026-05-28T09:00:00Z"), CreatedDate = DateTime.Parse("2026-05-28T09:00:00Z") },
                new TuitionInvoice { Id = 7, StudentId = 107, StudentCode = "SV007", StudentName = "Trần Thị Lan", CourseName = "Lập trình Web Fullstack", ClassName = "K18-INF02", InvoiceNumber = "INV-2026-0007", Amount = 6800000, PaidAmount = 0, DebtAmount = 6800000, DueDate = DateTime.Parse("2026-06-12"), Status = "Unpaid", Description = "Học phí học kỳ 1 (Chờ duyệt)", CreatedAt = DateTime.Parse("2026-05-27T10:30:00Z"), CreatedDate = DateTime.Parse("2026-05-27T10:30:00Z") },
                new TuitionInvoice { Id = 8, StudentId = 108, StudentCode = "SV008", StudentName = "Lê Văn Hùng", CourseName = "Lập trình Web Fullstack", ClassName = "K18-INF01", InvoiceNumber = "INV-2026-0008", Amount = 6800000, PaidAmount = 0, DebtAmount = 6800000, DueDate = DateTime.Parse("2026-06-15"), Status = "Unpaid", Description = "Học phí bổ sung (Chờ duyệt)", CreatedAt = DateTime.Parse("2026-05-26T11:15:00Z"), CreatedDate = DateTime.Parse("2026-05-26T11:15:00Z") }
            );

            // Seed default payment receipts
            modelBuilder.Entity<PaymentReceipt>().HasData(
                new PaymentReceipt { Id = 1, InvoiceId = 1, ReceiptNumber = "REC-2026-0001", AmountPaid = 6800000, PaymentDate = DateTime.Parse("2026-04-15T10:24:00Z"), PaymentMethod = "BankTransfer", TransactionReference = "VND-BANK-88217", Notes = "Nộp học phí Nguyễn Văn Hùng", CreatedBy = "Thủ quỹ Trần Thị Mai" },
                new PaymentReceipt { Id = 2, InvoiceId = 2, ReceiptNumber = "REC-2026-0002", AmountPaid = 3000000, PaymentDate = DateTime.Parse("2026-04-18T14:15:00Z"), PaymentMethod = "Cash", TransactionReference = "", Notes = "Đóng trước 3 triệu", CreatedBy = "Thủ quỹ Trần Thị Mai" },
                new PaymentReceipt { Id = 3, InvoiceId = 4, ReceiptNumber = "REC-2026-0003", AmountPaid = 5200000, PaymentDate = DateTime.Parse("2026-04-05T09:12:00Z"), PaymentMethod = "BankTransfer", TransactionReference = "MB-QR-21447", Notes = "Phạm Minh Đức nộp học phí", CreatedBy = "Thủ quỹ Trần Thị Mai" },
                new PaymentReceipt { Id = 4, InvoiceId = 6, ReceiptNumber = "REC-2026-0004", AmountPaid = 6800000, PaymentDate = DateTime.Parse("2026-05-28T10:00:00Z"), PaymentMethod = "BankTransfer", TransactionReference = "TECHCOMBANK-9912", Notes = "Nguyễn Minh Tuấn đóng học phí", CreatedBy = "Kế toán Trần Thị Mai" }
            );
        }
    }
}
