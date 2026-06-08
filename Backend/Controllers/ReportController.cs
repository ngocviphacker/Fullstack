using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Route("reports")]              // Direct microservice routing compatibility
    [Authorize(Roles = "Admin,admin,Staff,staff")]
    public class ReportController : ControllerBase
    {
        private readonly PaymentDbContext _context;

        public ReportController(PaymentDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var invoices = await _context.TuitionInvoices.ToListAsync();
            
            var totalOwed = invoices.Sum(i => i.Amount);
            var totalPaid = invoices.Sum(i => i.PaidAmount);
            var totalDebt = invoices.Sum(i => i.DebtAmount);
            var collectionRate = totalOwed > 0 ? (int)Math.Round((double)(totalPaid / totalOwed) * 100) : 0;

            var stats = new
            {
                totalOwed,
                totalPaid,
                totalDebt,
                collectionRate,
                invoiceCount = invoices.Count,
                paidCount = invoices.Count(i => i.Status == "Paid"),
                pendingCount = invoices.Count(i => i.Status != "Paid")
            };

            return Ok(stats);
        }

        [HttpGet("revenue-by-course")]
        public async Task<IActionResult> GetRevenueByCourse()
        {
            var invoices = await _context.TuitionInvoices.ToListAsync();
            
            var grouped = invoices
                .GroupBy(i => i.CourseName)
                .Select(g => new
                {
                    name = g.Key,
                    value = g.Sum(i => i.PaidAmount)
                })
                .ToList();

            return Ok(grouped);
        }

        [HttpGet("revenue-by-month")]
        public async Task<IActionResult> GetRevenueByMonth()
        {
            var receipts = await _context.PaymentReceipts.ToListAsync();
            var grouped = receipts
                .GroupBy(r => new { r.PaymentDate.Year, r.PaymentDate.Month })
                .Select(g => new
                {
                    month = $"{g.Key.Month:D2}/{g.Key.Year}",
                    revenue = g.Sum(r => r.AmountPaid)
                })
                .OrderBy(g => g.month)
                .ToList();

            return Ok(grouped);
        }

        [HttpGet("debt-summary")]
        public async Task<IActionResult> GetDebtSummary()
        {
            var invoices = await _context.TuitionInvoices.ToListAsync();
            var totalOwed = invoices.Sum(i => i.Amount);
            var totalPaid = invoices.Sum(i => i.PaidAmount);
            var totalDebt = invoices.Sum(i => i.DebtAmount);

            var summary = new
            {
                totalOwed,
                totalPaid,
                totalDebt,
                unpaidCount = invoices.Count(i => i.Status == "Unpaid"),
                partiallyPaidCount = invoices.Count(i => i.Status == "PartiallyPaid"),
                paidCount = invoices.Count(i => i.Status == "Paid")
            };

            return Ok(summary);
        }

        [HttpGet("top-debtors")]
        public async Task<IActionResult> GetTopDebtors()
        {
            var topDebtors = await _context.TuitionInvoices
                .Where(i => i.DebtAmount > 0)
                .GroupBy(i => new { i.StudentCode, i.StudentName, i.ClassName })
                .Select(g => new
                {
                    studentCode = g.Key.StudentCode,
                    studentName = g.Key.StudentName,
                    className = g.Key.ClassName,
                    totalDebt = g.Sum(i => i.DebtAmount)
                })
                .OrderByDescending(d => d.totalDebt)
                .Take(5)
                .ToListAsync();

            return Ok(topDebtors);
        }

        [HttpGet("payment-status")]
        public async Task<IActionResult> GetPaymentStatusStats()
        {
            var invoices = await _context.TuitionInvoices.ToListAsync();
            var statusStats = invoices
                .GroupBy(i => i.Status)
                .Select(g => new
                {
                    status = g.Key,
                    count = g.Count(),
                    totalAmount = g.Sum(i => i.Amount)
                })
                .ToList();

            return Ok(statusStats);
        }
    }
}
