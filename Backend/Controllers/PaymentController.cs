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
    [Route("api/payment/payments")] // Route chính — Vue frontend dùng
    [Route("api/payments")]         // Route phụ — tương thích
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentDbContext _context;

        public PaymentController(PaymentDbContext context)
        {
            _context = context;
        }

        // GET: api/payments
        [HttpGet]
        public async Task<IActionResult> GetPayments()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var studentCode = User.FindFirst("StudentCode")?.Value;

            if (userRole == "Student" && !string.IsNullOrEmpty(studentCode))
            {
                var studentPayments = await _context.PaymentReceipts
                    .Join(_context.TuitionInvoices,
                          r => r.InvoiceId,
                          i => i.Id,
                          (r, i) => new { Receipt = r, Invoice = i })
                    .Where(join => join.Invoice.StudentCode == studentCode)
                    .Select(join => join.Receipt)
                    .OrderByDescending(r => r.PaymentDate)
                    .ToListAsync();
                return Ok(studentPayments);
            }

            var payments = await _context.PaymentReceipts
                .OrderByDescending(r => r.PaymentDate)
                .ToListAsync();
            return Ok(payments);
        }

        // GET: api/payments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _context.PaymentReceipts.FindAsync(id);
            if (payment == null)
            {
                return NotFound(new { message = "Không tìm thấy biên lai thanh toán!" });
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var studentCode = User.FindFirst("StudentCode")?.Value;

            if (userRole == "Student" && !string.IsNullOrEmpty(studentCode))
            {
                var invoice = await _context.TuitionInvoices.FindAsync(payment.InvoiceId);
                if (invoice == null || invoice.StudentCode != studentCode)
                {
                    return Forbid();
                }
            }

            return Ok(payment);
        }

        // POST: api/payments
        [HttpPost]
        [Authorize(Roles = "Admin,admin,Staff,staff")]
        public async Task<IActionResult> RecordPayment([FromBody] CreatePaymentDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var invoice = await _context.TuitionInvoices.FindAsync(request.InvoiceId);
            if (invoice == null)
            {
                return NotFound(new { message = "Không tìm thấy hóa đơn học phí tương ứng!" });
            }

            if (request.Amount > invoice.DebtAmount)
            {
                return BadRequest(new { message = "Số tiền thanh toán vượt quá khoản nợ hiện tại của hóa đơn!" });
            }

            // Generate receipt number (REC-YYYY-XXXX)
            var year = DateTime.UtcNow.Year;
            var prefix = $"REC-{year}-";

            var lastReceipt = await _context.PaymentReceipts
                .Where(r => r.ReceiptNumber.StartsWith(prefix))
                .OrderByDescending(r => r.ReceiptNumber)
                .FirstOrDefaultAsync();

            int nextSeq = 1;
            if (lastReceipt != null && lastReceipt.ReceiptNumber.Length > prefix.Length)
            {
                var lastPart = lastReceipt.ReceiptNumber.Substring(prefix.Length);
                if (int.TryParse(lastPart, out int lastSeq))
                {
                    nextSeq = lastSeq + 1;
                }
            }

            var receiptNumber = $"{prefix}{nextSeq:D4}";
            var creatorName = User.FindFirst("FullName")?.Value ?? "Thu ngân viên";

            // Update invoice balances
            invoice.PaidAmount += request.Amount;
            invoice.DebtAmount = invoice.Amount - invoice.PaidAmount;
            invoice.Status = invoice.DebtAmount <= 0 ? "Paid" : "PartiallyPaid";

            var receipt = new PaymentReceipt
            {
                InvoiceId = request.InvoiceId,
                ReceiptNumber = receiptNumber,
                AmountPaid = request.Amount,
                PaymentDate = request.PaymentDate ?? DateTime.UtcNow,
                PaymentMethod = request.PaymentMethod,
                TransactionReference = request.TransactionReference,
                Notes = request.Notes,
                CreatedBy = creatorName
            };

            _context.PaymentReceipts.Add(receipt);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPaymentById), new { id = receipt.Id }, receipt);
        }
    }
}
