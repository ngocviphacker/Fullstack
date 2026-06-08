using System;

namespace PaymentService.Models
{
    public class PaymentReceipt
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty; // REC-YYYY-XXXX
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = "BankTransfer"; // BankTransfer, Cash
        public string TransactionReference { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
    }
}
