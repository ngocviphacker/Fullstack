using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/receipts")] // Biên lai phíu thu
    [Authorize]
    public class ReceiptController : ControllerBase
    {
        private readonly PaymentDbContext _context;

        public ReceiptController(PaymentDbContext context)
        {
            _context = context;
        }

        // GET: api/receipts
        [HttpGet]
        public async Task<IActionResult> GetReceipts()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var studentCode = User.FindFirst("StudentCode")?.Value;

            if (userRole == "Student" && !string.IsNullOrEmpty(studentCode))
            {
                var studentReceipts = await _context.PaymentReceipts
                    .Join(_context.TuitionInvoices,
                          r => r.InvoiceId,
                          i => i.Id,
                          (r, i) => new { Receipt = r, Invoice = i })
                    .Where(join => join.Invoice.StudentCode == studentCode)
                    .Select(join => join.Receipt)
                    .OrderByDescending(r => r.PaymentDate)
                    .ToListAsync();
                return Ok(studentReceipts);
            }

            var receipts = await _context.PaymentReceipts
                .OrderByDescending(r => r.PaymentDate)
                .ToListAsync();
            return Ok(receipts);
        }

        // GET: api/receipts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReceiptById(int id)
        {
            var receipt = await _context.PaymentReceipts.FindAsync(id);
            if (receipt == null)
            {
                return NotFound(new { message = "Không tìm thấy biên lai phiếu thu tương ứng!" });
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var studentCode = User.FindFirst("StudentCode")?.Value;

            if (userRole == "Student" && !string.IsNullOrEmpty(studentCode))
            {
                var invoice = await _context.TuitionInvoices.FindAsync(receipt.InvoiceId);
                if (invoice == null || invoice.StudentCode != studentCode)
                {
                    return Forbid();
                }
            }

            return Ok(receipt);
        }
    }
}
