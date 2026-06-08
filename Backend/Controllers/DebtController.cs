using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/debts")] // Debt summary — dùng cho Nhóm 1/2 kiểm tra học phí
    [Authorize]
    public class DebtController : ControllerBase
    {
        private readonly PaymentDbContext _context;

        public DebtController(PaymentDbContext context)
        {
            _context = context;
        }

        public class DebtDto
        {
            public int Id { get; set; }
            public int StudentId { get; set; }
            public string StudentCode { get; set; } = string.Empty;
            public string StudentName { get; set; } = string.Empty;
            public string CourseName { get; set; } = string.Empty;
            public string ClassName { get; set; } = string.Empty;
            public decimal TotalAmount { get; set; }
            public decimal PaidAmount { get; set; }
            public decimal DebtAmount { get; set; }
            public string Status { get; set; } = "Unpaid"; // Paid, Partial, Unpaid
            public DateTime DueDate { get; set; }
            public string Description { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
        }

        private static DebtDto MapToDebtDto(TuitionInvoice invoice)
        {
            decimal debtAmount = invoice.Amount - invoice.PaidAmount;
            string status = "Unpaid";
            if (debtAmount <= 0)
            {
                status = "Paid";
            }
            else if (invoice.PaidAmount > 0)
            {
                status = "Partial";
            }

            return new DebtDto
            {
                Id = invoice.Id,
                StudentId = invoice.StudentId,
                StudentCode = invoice.StudentCode,
                StudentName = invoice.StudentName,
                CourseName = invoice.CourseName,
                ClassName = invoice.ClassName,
                TotalAmount = invoice.Amount,
                PaidAmount = invoice.PaidAmount,
                DebtAmount = debtAmount,
                Status = status,
                DueDate = invoice.DueDate,
                Description = invoice.Description,
                CreatedDate = invoice.CreatedDate
            };
        }

        // GET: api/debts
        [HttpGet]
        public async Task<IActionResult> GetDebts()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var studentCode = User.FindFirst("StudentCode")?.Value;

            IQueryable<TuitionInvoice> query = _context.TuitionInvoices;

            if (userRole == "Student" && !string.IsNullOrEmpty(studentCode))
            {
                query = query.Where(i => i.StudentCode == studentCode);
            }

            var invoices = await query.OrderByDescending(i => i.CreatedDate).ToListAsync();
            var debts = invoices.Select(MapToDebtDto).ToList();

            return Ok(debts);
        }

        // GET: api/debts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDebtById(int id)
        {
            var invoice = await _context.TuitionInvoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin công nợ tương ứng!" });
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var studentCode = User.FindFirst("StudentCode")?.Value;

            if (userRole == "Student" && !string.IsNullOrEmpty(studentCode) && invoice.StudentCode != studentCode)
            {
                return Forbid();
            }

            return Ok(MapToDebtDto(invoice));
        }

        // GET: api/debts/status/{studentCode}
        [HttpGet("status/{studentCode}")]
        [AllowAnonymous] // Allow N1 and N2 services to call this endpoint directly
        public async Task<IActionResult> GetStudentPaymentStatus(
            string studentCode, 
            [FromQuery] string? className = null, 
            [FromQuery] int? classId = null,
            [FromQuery] string? courseName = null,
            [FromQuery] int? courseId = null)
        {
            // Resolve class name from classId if provided
            if (classId.HasValue && string.IsNullOrEmpty(className))
            {
                var cls = await _context.Classes.FindAsync(classId.Value);
                if (cls != null)
                {
                    className = cls.Name;
                }
            }

            // Resolve course name from courseId if provided
            if (courseId.HasValue && string.IsNullOrEmpty(courseName))
            {
                var crs = await _context.Courses.FindAsync(courseId.Value);
                if (crs != null)
                {
                    courseName = crs.Name;
                }
            }

            var query = _context.TuitionInvoices.Where(i => i.StudentCode == studentCode);

            if (!string.IsNullOrEmpty(className))
            {
                query = query.Where(i => i.ClassName.ToLower() == className.ToLower());
            }

            if (!string.IsNullOrEmpty(courseName))
            {
                query = query.Where(i => i.CourseName.ToLower() == courseName.ToLower());
            }

            var studentInvoices = await query.ToListAsync();

            if (!studentInvoices.Any())
            {
                // If checking generally (no specific class/course) and they have no invoices: allow.
                var generalInvoicesCount = await _context.TuitionInvoices
                    .CountAsync(i => i.StudentCode == studentCode);

                if (generalInvoicesCount == 0)
                {
                    return Ok(new
                    {
                        studentCode,
                        className,
                        courseName,
                        isAllowedToStudy = true,
                        debtStatus = "NoInvoice",
                        totalDebt = 0m,
                        message = "Học viên không có hóa đơn học phí tồn đọng trên hệ thống."
                    });
                }
                else
                {
                    // No invoice for this specific class, but they have invoices elsewhere.
                    // This means they haven't enrolled or been invoiced for this specific class.
                    return Ok(new
                    {
                        studentCode,
                        className,
                        courseName,
                        isAllowedToStudy = false,
                        debtStatus = "Unpaid",
                        totalDebt = 0m,
                        message = "Học viên chưa đăng ký hoặc chưa có hóa đơn học phí cho lớp học/khóa học này."
                    });
                }
            }

            // Student is allowed to join if they have paid fully or partially (status: Paid or PartiallyPaid).
            // They are blocked if they have any invoice in the filtered set that is completely "Unpaid" (PaidAmount == 0).
            bool hasUnpaidInvoices = studentInvoices.Any(i => i.Status == "Unpaid" || i.PaidAmount == 0);
            bool isAllowedToStudy = !hasUnpaidInvoices;

            return Ok(new
            {
                studentCode,
                className,
                courseName,
                isAllowedToStudy = isAllowedToStudy,
                debtStatus = hasUnpaidInvoices ? "Unpaid" : (studentInvoices.Any(i => i.Status == "PartiallyPaid") ? "Partial" : "Paid"),
                totalDebt = studentInvoices.Sum(i => i.DebtAmount),
                message = isAllowedToStudy 
                    ? "Học viên đã đóng đủ học phí hoặc đã đóng một phần học phí (được phép vào lớp)." 
                    : "Học viên chưa đóng tiền học phí cho lớp này (chưa được phép vào lớp)."
            });
        }
    }
}
