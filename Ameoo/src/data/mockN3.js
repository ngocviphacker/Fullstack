// ═══════════════════════════════════════════════════════════════
// MOCK DATA NHÓM 3 — Payment & Report Service
// Đồng bộ với PaymentDB_v4 và Backend C# ASP.NET Core
// ═══════════════════════════════════════════════════════════════

// ────────────────────────────────────────────
// 1. LOCAL DATABASE (Offline Mode)
// ────────────────────────────────────────────
export const localDB = {
  courses: [
    { id: 1, name: 'Lập trình Web Fullstack', fee: 6800000 },
    { id: 2, name: 'Phân tích Thiết kế Hệ thống', fee: 5200000 },
    { id: 3, name: 'Cấu trúc dữ liệu & Giải thuật', fee: 4500000 },
    { id: 4, name: 'Trí tuệ nhân tạo (AI) cơ bản', fee: 8500000 },
  ],
  classes: [
    { id: 1, name: 'K18-INF01', courseId: 1 },
    { id: 2, name: 'K18-INF02', courseId: 1 },
    { id: 3, name: 'K18-SYS01', courseId: 2 },
    { id: 4, name: 'K18-CS01',  courseId: 3 },
  ],
  students: [
    { id: 101, studentCode: 'SV001', fullName: 'Nguyễn Văn Hùng', email: 'hungnv@gmail.com', phone: '0912345678', classId: 1, courseId: 1 },
    { id: 102, studentCode: 'SV002', fullName: 'Trần Thị Mai',    email: 'maitt@gmail.com',  phone: '0987654321', classId: 1, courseId: 1 },
    { id: 103, studentCode: 'SV003', fullName: 'Lê Tuấn Anh',    email: 'anhlt@gmail.com',  phone: '0933333333', classId: 2, courseId: 1 },
    { id: 104, studentCode: 'SV004', fullName: 'Phạm Minh Đức',  email: 'ducpm@gmail.com',  phone: '0944444444', classId: 3, courseId: 2 },
    { id: 105, studentCode: 'SV005', fullName: 'Hoàng Thanh Thảo', email: 'thaoht@gmail.com', phone: '0955555555', classId: 4, courseId: 3 },
  ],
  invoices: [
    { id: 1, studentCode: 'SV001', studentName: 'Nguyễn Văn Hùng', courseName: 'Lập trình Web Fullstack', className: 'K18-INF01', invoiceNumber: 'INV-2026-0001', amount: 6800000, paidAmount: 6800000, debtAmount: 0, dueDate: '2026-05-10', status: 'Paid', description: 'Học phí đợt 1', createdAt: '2026-04-10T08:00:00Z' },
    { id: 2, studentCode: 'SV002', studentName: 'Trần Thị Mai', courseName: 'Lập trình Web Fullstack', className: 'K18-INF01', invoiceNumber: 'INV-2026-0002', amount: 6800000, paidAmount: 3000000, debtAmount: 3800000, dueDate: '2026-06-05', status: 'PartiallyPaid', description: 'Học phí đợt 1', createdAt: '2026-04-10T08:30:00Z' },
    { id: 3, studentCode: 'SV003', studentName: 'Lê Tuấn Anh', courseName: 'Lập trình Web Fullstack', className: 'K18-INF02', invoiceNumber: 'INV-2026-0003', amount: 6800000, paidAmount: 0, debtAmount: 6800000, dueDate: '2026-06-15', status: 'Unpaid', description: 'Học phí đợt 1', createdAt: '2026-04-12T09:00:00Z' },
    { id: 4, studentCode: 'SV004', studentName: 'Phạm Minh Đức', courseName: 'Phân tích Thiết kế Hệ thống', className: 'K18-SYS01', invoiceNumber: 'INV-2026-0004', amount: 5200000, paidAmount: 5200000, debtAmount: 0, dueDate: '2026-05-01', status: 'Paid', description: 'Trọn gói học phí', createdAt: '2026-04-01T10:00:00Z' },
    { id: 5, studentCode: 'SV005', studentName: 'Hoàng Thanh Thảo', courseName: 'Cấu trúc dữ liệu & Giải thuật', className: 'K18-CS01', invoiceNumber: 'INV-2026-0005', amount: 4500000, paidAmount: 0, debtAmount: 4500000, dueDate: '2026-05-20', status: 'Unpaid', description: 'Lần đóng đầu tiên', createdAt: '2026-04-20T14:00:00Z' },
    { id: 6, studentCode: 'SV006', studentName: 'Nguyễn Minh Tuấn', courseName: 'Lập trình Web Fullstack', className: 'K18-INF01', invoiceNumber: 'INV-2026-0006', amount: 6800000, paidAmount: 6800000, debtAmount: 0, dueDate: '2026-05-28', status: 'Paid', description: 'Đóng học phí trọn khóa', createdAt: '2026-05-28T09:00:00Z' },
    { id: 7, studentCode: 'SV007', studentName: 'Trần Thị Lan', courseName: 'Lập trình Web Fullstack', className: 'K18-INF02', invoiceNumber: 'INV-2026-0007', amount: 6800000, paidAmount: 0, debtAmount: 6800000, dueDate: '2026-06-12', status: 'Unpaid', description: 'Học phí học kỳ 1 (Chờ duyệt)', createdAt: '2026-05-27T10:30:00Z' },
    { id: 8, studentCode: 'SV008', studentName: 'Lê Văn Hùng', courseName: 'Lập trình Web Fullstack', className: 'K18-INF01', invoiceNumber: 'INV-2026-0008', amount: 6800000, paidAmount: 0, debtAmount: 6800000, dueDate: '2026-06-15', status: 'Unpaid', description: 'Học phí bổ sung (Chờ duyệt)', createdAt: '2026-05-26T11:15:00Z' },
  ],
  payments: [
    { id: 1, invoiceId: 1, receiptNumber: 'REC-2026-0001', amountPaid: 6800000, paymentDate: '2026-04-15T10:24:00Z', paymentMethod: 'BankTransfer', transactionReference: 'VND-BANK-88217', notes: 'Nộp học phí Nguyễn Văn Hùng', createdBy: 'Thủ quỹ Trần Thị Mai' },
    { id: 2, invoiceId: 2, receiptNumber: 'REC-2026-0002', amountPaid: 3000000, paymentDate: '2026-04-18T14:15:00Z', paymentMethod: 'Cash', transactionReference: '', notes: 'Đóng trước một phần', createdBy: 'Thủ quỹ Trần Thị Mai' },
    { id: 3, invoiceId: 4, receiptNumber: 'REC-2026-0003', amountPaid: 5200000, paymentDate: '2026-04-05T09:12:00Z', paymentMethod: 'BankTransfer', transactionReference: 'MB-QR-21447', notes: 'Phạm Minh Đức nộp học phí', createdBy: 'Thủ quỹ Trần Thị Mai' },
    { id: 4, invoiceId: 6, receiptNumber: 'REC-2026-0004', amountPaid: 6800000, paymentDate: '2026-05-28T10:00:00Z', paymentMethod: 'BankTransfer', transactionReference: 'TECHCOMBANK-9912', notes: 'Nguyễn Minh Tuấn đóng học phí', createdBy: 'Kế toán Trần Thị Mai' },
  ],
}

// ────────────────────────────────────────────
// 2. TUITION RECEIPTS (dùng trong Tuition.vue cũ)
// ────────────────────────────────────────────
export const tuitionReceipts = [
  { id: 'PT-2026-0542', studentName: 'Nguyễn Minh Tuấn', course: 'Lập trình Web Fullstack', amount: 6800000, date: '28/05/2026', method: 'Chuyển khoản', status: 'Đã thu' },
  { id: 'PT-2026-0541', studentName: 'Phạm Minh Đức',    course: 'Phân tích Thiết kế HT', amount: 5200000, date: '05/04/2026', method: 'Chuyển khoản', status: 'Đã thu' },
  { id: 'PT-2026-0540', studentName: 'Nguyễn Văn Hùng',  course: 'Lập trình Web Fullstack', amount: 6800000, date: '15/04/2026', method: 'Chuyển khoản', status: 'Đã thu' },
  { id: 'PT-2026-0539', studentName: 'Trần Thị Mai',     course: 'Lập trình Web Fullstack', amount: 3000000, date: '18/04/2026', method: 'Tiền mặt',    status: 'Đóng 1 phần' },
  { id: 'PT-2026-0538', studentName: 'Lê Tuấn Anh',      course: 'Lập trình Web Fullstack', amount: 6800000, date: '',          method: '—',             status: 'Chưa đóng' },
  { id: 'PT-2026-0537', studentName: 'Hoàng Thanh Thảo', course: 'Cấu trúc DL & GT',        amount: 4500000, date: '',          method: '—',             status: 'Chưa đóng' },
]

// ────────────────────────────────────────────
// 3. DEBT LIST (danh sách học viên còn nợ)
// ────────────────────────────────────────────
export const debtList = [
  { name: 'Lê Tuấn Anh',     code: 'SV003', course: 'Lập trình Web Fullstack', totalFee: 6800000, paid: 0,       remaining: 6800000, deadline: 'Quá hạn 0 ngày', urgency: 'danger',  phone: '0933333333', email: 'anhlt@gmail.com' },
  { name: 'Hoàng Thanh Thảo',code: 'SV005', course: 'Cấu trúc DL & GT',        totalFee: 4500000, paid: 0,       remaining: 4500000, deadline: 'Quá hạn 18 ngày', urgency: 'danger',  phone: '0955555555', email: 'thaoht@gmail.com' },
  { name: 'Trần Thị Mai',    code: 'SV002', course: 'Lập trình Web Fullstack', totalFee: 6800000, paid: 3000000, remaining: 3800000, deadline: 'Còn 7 ngày',    urgency: 'warning', phone: '0987654321', email: 'maitt@gmail.com' },
  { name: 'Trần Thị Lan',    code: 'SV007', course: 'Lập trình Web Fullstack', totalFee: 6800000, paid: 0,       remaining: 6800000, deadline: 'Còn 4 ngày',    urgency: 'warning', phone: '0911111111', email: 'lant@gmail.com' },
  { name: 'Lê Văn Hùng',    code: 'SV008', course: 'Lập trình Web Fullstack', totalFee: 6800000, paid: 0,       remaining: 6800000, deadline: 'Còn 7 ngày',    urgency: 'warning', phone: '0922222222', email: 'hunglv@gmail.com' },
]

// ────────────────────────────────────────────
// 4. MONTHLY REVENUE (dữ liệu cho biểu đồ)
// ────────────────────────────────────────────
export const revenueMonthly = [
  { month: 'T1', revenue: 85000000,  expense: 29750000,  profit: 55250000  },
  { month: 'T2', revenue: 92000000,  expense: 32200000,  profit: 59800000  },
  { month: 'T3', revenue: 110000000, expense: 38500000,  profit: 71500000  },
  { month: 'T4', revenue: 105000000, expense: 36750000,  profit: 68250000  },
  { month: 'T5', revenue: 128500000, expense: 44975000,  profit: 83525000  },
  { month: 'T6', revenue: 140000000, expense: 49000000,  profit: 91000000  },
]

// ────────────────────────────────────────────
// 5. CENTER RULES (Nội quy - dùng cho học viên)
// ────────────────────────────────────────────
export const rules = [
  { num: 1, title: 'Giờ giấc', text: 'Học viên phải có mặt trước giờ học ít nhất 5 phút. Đến trễ quá 15 phút sẽ bị tính vắng buổi học hôm đó.' },
  { num: 2, title: 'Điểm danh', text: 'Học viên vắng mặt quá 30% tổng số buổi học sẽ không đủ điều kiện tham dự thi cuối kỳ và sẽ phải học lại.' },
  { num: 3, title: 'Xin nghỉ', text: 'Phải gửi đơn xin nghỉ qua hệ thống trước 24 giờ. Tối đa 3 buổi nghỉ có phép mỗi kỳ học.' },
  { num: 4, title: 'Thiết bị', text: 'Tắt hoặc để chế độ im lặng điện thoại trong giờ học. Không sử dụng điện thoại cá nhân cho việc riêng trong buổi học.' },
  { num: 5, title: 'Trang phục', text: 'Mặc trang phục lịch sự, phù hợp môi trường học thuật và chuyên nghiệp.' },
]

export const studyRules = [
  { num: 6, text: 'Bài tập: Hoàn thành đầy đủ bài tập về nhà trước buổi học kế tiếp. Nộp bài muộn sẽ bị trừ điểm thành phần.' },
  { num: 7, text: 'Kiểm tra: Tham dự đầy đủ các bài kiểm tra định kỳ. Vắng thi phải có lý do chính đáng và thông báo trước ít nhất 24h.' },
  { num: 8, text: 'Học phí: Đóng học phí đúng hạn. Quá hạn 15 ngày sẽ bị tạm ngưng học đến khi hoàn tất nghĩa vụ tài chính.' },
]

export const graduationReqs = [
  { text: 'Điểm danh ≥ 70% tổng số buổi học trong kỳ', met: true },
  { text: 'Điểm trung bình tổng kết ≥ 5.0 / 10.0', met: true },
  { text: 'Tham dự bài kiểm tra cuối kỳ (bắt buộc)', met: true },
  { text: 'Hoàn tất toàn bộ học phí trước ngày thi', met: true },
]

// ────────────────────────────────────────────
// 6. BACKEND API CONFIG
// ────────────────────────────────────────────
export const N3_API_URL = 'http://180.93.36.113:5000'  // VPS URL (Nhóm 3 đã deploy)
