using System;
using System.ComponentModel.DataAnnotations;

namespace PaymentService.Models
{
    public class CreatePaymentDto
    {
        [Required(ErrorMessage = "Mã hóa đơn là bắt buộc")]
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Số tiền thanh toán là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền thanh toán phải lớn hơn 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Phương thức thanh toán không được rỗng")]
        public string PaymentMethod { get; set; } = "BankTransfer";

        public DateTime? PaymentDate { get; set; }

        [StringLength(200, ErrorMessage = "Tham chiếu giao dịch tối đa 200 ký tự")]
        public string TransactionReference { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        public string Notes { get; set; } = string.Empty;
    }
}
