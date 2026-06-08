using System;
using System.ComponentModel.DataAnnotations;

namespace PaymentService.Models
{
    public class CreateInvoiceDto
    {
        [Required(ErrorMessage = "Mã học viên là bắt buộc")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Mã học viên không được rỗng và tối đa 50 ký tự")]
        public string StudentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên học viên là bắt buộc")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Tên học viên không được rỗng và tối đa 100 ký tự")]
        public string StudentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên khóa học là bắt buộc")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Tên khóa học không được rỗng và tối đa 100 ký tự")]
        public string CourseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên lớp học là bắt buộc")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Tên lớp học không được rỗng và tối đa 50 ký tự")]
        public string ClassName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số tiền là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền học phí phải lớn hơn 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Hạn thanh toán là bắt buộc")]
        public DateTime DueDate { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
        public string Description { get; set; } = string.Empty;
    }
}
