# 🟧 NHÓM N3 — Tài chính

## Phạm vi làm việc

Nhóm N3 chịu trách nhiệm các màn hình liên quan đến **Thu học phí, Công nợ và Báo cáo doanh thu**. Ngoài ra quản lý phần Nội quy cho học viên.

## 📁 Thư mục của N3

```
src/views/admin/n3/         ← Màn hình Admin
src/views/student/n3/       ← Màn hình Học viên
src/data/mockN3.js          ← Dữ liệu mock của N3
```

## 📋 Danh sách màn hình cần làm

### Admin
| File | Chức năng |
|------|-----------|
| `N3_Tuition.vue` | Thu học phí (3 stat card + bảng phiếu thu) |
| `N3_Debt.vue` | Theo dõi công nợ (bảng + badge quá hạn) |
| `N3_Revenue.vue` | Báo cáo doanh thu (biểu đồ cột + thống kê) |

### Học viên
| File | Chức năng |
|------|-----------|
| `N3_Rules.vue` | Nội quy trung tâm + Điều kiện tốt nghiệp |

## 🌐 API Backend N3

Base URL: `http://<IP-N3>:<PORT>`

| Endpoint | Mô tả |
|----------|-------|
| `GET /api/tuition` | Danh sách phiếu thu học phí |
| `POST /api/tuition` | Tạo phiếu thu mới |
| `GET /api/debt` | Danh sách công nợ |
| `GET /api/revenue` | Báo cáo doanh thu |

## 🎨 Màu sắc

- Finance: Green `#10b981`, Red `#ef4444`, Blue `#2563eb`

## 📊 Dữ liệu mẫu cần có trong `mockN3.js`

```js
export const tuitionReceipts = [
  { id:'PT-2026-0542', studentName:'Nguyễn Minh Tuấn', course:'Tiếng Anh B1',
    amount:3600000, date:'28/05/2026', method:'Chuyển khoản', status:'Đã thu' },
  // ...
]

export const debtList = [
  { name:'Lê Văn Hùng', code:'HV-003', totalFee:4200000, paid:0,
    remaining:4200000, deadline:'Quá hạn 15 ngày', urgency:'danger' },
  // ...
]

export const revenueMonthly = [
  { month:'T1', amount:95000000 },
  { month:'T2', amount:88000000 },
  // T1-T5 ...
]
```

## 🔧 Bắt đầu

```bash
git checkout feature/N3
# Làm việc trong thư mục n3/
git add src/views/admin/n3/ src/views/student/n3/ src/data/mockN3.js
git commit -m "feat(N3): mô tả"
git push origin feature/N3
```
