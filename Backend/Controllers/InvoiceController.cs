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
    [Route("api/payment/invoices")] // Route chính — Vue frontend dùng
    [Route("api/invoices")]         // Route phụ — tương thích
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly PaymentDbContext _context;

        public InvoiceController(PaymentDbContext context)
        {
            _context = context;
        }

        // GET: api/invoices
        [HttpGet]
        public async Task<IActionResult> GetInvoices()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var studentCode = User.FindFirst("StudentCode")?.Value;

            if (userRole == "Student" && !string.IsNullOrEmpty(studentCode))
            {
                var studentInvoices = await _context.TuitionInvoices
                    .Where(i => i.StudentCode == studentCode)
                    .OrderByDescending(i => i.CreatedDate)
                    .ToListAsync();
                return Ok(studentInvoices);
            }

            var invoices = await _context.TuitionInvoices
                .OrderByDescending(i => i.CreatedDate)
                .ToListAsync();
            return Ok(invoices);
        }

        // GET: api/invoices/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            var invoice = await _context.TuitionInvoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound(new { message = "Không tìm thấy hóa đơn học phí tương ứng!" });
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var studentCode = User.FindFirst("StudentCode")?.Value;

            if (userRole == "Student" && !string.IsNullOrEmpty(studentCode) && invoice.StudentCode != studentCode)
            {
                return Forbid();
            }

            return Ok(invoice);
        }

        // POST: api/invoices
        [HttpPost]
        [Authorize(Roles = "Admin,admin,Staff,staff")]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var year = DateTime.UtcNow.Year;
            var prefix = $"INV-{year}-";

            // Find last invoice number for the current year to generate the sequence number
            var lastInvoice = await _context.TuitionInvoices
                .Where(i => i.InvoiceNumber.StartsWith(prefix))
                .OrderByDescending(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            int nextSeq = 1;
            if (lastInvoice != null && lastInvoice.InvoiceNumber.Length > prefix.Length)
            {
                var lastPart = lastInvoice.InvoiceNumber.Substring(prefix.Length);
                if (int.TryParse(lastPart, out int lastSeq))
                {
                    nextSeq = lastSeq + 1;
                }
            }

            var invoiceNumber = $"{prefix}{nextSeq:D4}";

            var invoice = new TuitionInvoice
            {
                StudentId = 999, // Placeholder replica code
                StudentCode = request.StudentCode,
                StudentName = request.StudentName,
                CourseName = request.CourseName,
                ClassName = request.ClassName,
                InvoiceNumber = invoiceNumber,
                Amount = request.Amount,
                PaidAmount = 0,
                DebtAmount = request.Amount,
                DueDate = request.DueDate,
                Status = "Unpaid",
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            };

            _context.TuitionInvoices.Add(invoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.Id }, invoice);
        }

        // PUT: api/invoices/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,admin,Staff,staff")]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] UpdateInvoiceDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var invoice = await _context.TuitionInvoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound(new { message = "Không tìm thấy hóa đơn để cập nhật!" });
            }

            if (request.Amount < invoice.PaidAmount)
            {
                return BadRequest(new { message = "Không thể cập nhật số tiền hóa đơn nhỏ hơn số tiền đã thanh toán!" });
            }

            invoice.StudentCode = request.StudentCode;
            invoice.StudentName = request.StudentName;
            invoice.CourseName = request.CourseName;
            invoice.ClassName = request.ClassName;
            invoice.Amount = request.Amount;
            invoice.DueDate = request.DueDate;
            invoice.Description = request.Description;
            invoice.Status = request.Status;

            // Recalculate debt amount based on updated amount
            invoice.DebtAmount = invoice.Amount - invoice.PaidAmount;

            // Align status with debt level
            if (invoice.DebtAmount <= 0)
            {
                invoice.Status = "Paid";
            }
            else if (invoice.PaidAmount > 0)
            {
                invoice.Status = "PartiallyPaid";
            }
            else
            {
                invoice.Status = "Unpaid";
            }

            _context.Entry(invoice).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(invoice);
        }

        // DELETE: api/invoices/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,admin,Staff,staff")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.TuitionInvoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound(new { message = "Không tìm thấy hóa đơn để xóa!" });
            }

            // Remove associated receipts as well to clean up data
            var receipts = await _context.PaymentReceipts
                .Where(r => r.InvoiceId == id)
                .ToListAsync();

            if (receipts.Any())
            {
                _context.PaymentReceipts.RemoveRange(receipts);
            }

            _context.TuitionInvoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa hóa đơn học phí và các biên lai liên quan thành công!" });
        }
    }
}
