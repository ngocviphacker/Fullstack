# 📘 EduHub / EduCore — Dự án Nhóm

Hệ thống quản lý trung tâm đào tạo, phát triển bởi 3 nhóm (N1, N2, N3).

## 🏗 Cấu trúc dự án

```
src/
├── views/
│   ├── admin/          ← Màn hình Admin (chia theo nhóm)
│   │   ├── n1/         ← Nhóm N1: Khóa học & Lịch học
│   │   ├── n2/         ← Nhóm N2: Học viên & Điểm danh
│   │   └── n3/         ← Nhóm N3: Tài chính
│   ├── teacher/        ← Màn hình Giáo viên
│   │   ├── n1/
│   │   ├── n2/
│   │   └── n3/
│   └── student/        ← Màn hình Học viên
│       ├── n1/
│       ├── n2/
│       └── n3/
├── data/
│   ├── mockN1.js       ← Mock data nhóm N1 tự viết
│   ├── mockN2.js       ← Mock data nhóm N2 tự viết
│   └── mockN3.js       ← Mock data nhóm N3 tự viết
└── App.vue             ← ⛔ KHÔNG CHỈNH SỬA (chỉ nhóm tích hợp)
```

## 🔀 Quy trình làm việc

| Bước | Hành động |
|------|-----------|
| 1 | Clone repo về máy |
| 2 | `git checkout feature/N1` (hoặc N2, N3 tương ứng) |
| 3 | Làm việc trong thư mục `views/admin/n1/`, `views/teacher/n1/`, `views/student/n1/` |
| 4 | `git add . && git commit -m "feat: mô tả thay đổi"` |
| 5 | `git push origin feature/N1` |
| 6 | Tạo Pull Request → báo nhóm tích hợp |

## ⚠️ Quy tắc BẮT BUỘC

- ✅ Chỉ làm việc trong thư mục `views/nX/` và `data/mockNX.js`
- ✅ Đặt tên file: `N1_TenChucNang.vue` (ví dụ: `N1_CourseList.vue`)
- ⛔ KHÔNG sửa `App.vue`, `main.js`, `main.css`
- ⛔ KHÔNG xóa hoặc đổi tên file của nhóm khác

## 🚀 Cài đặt và chạy

```bash
npm install
npm run dev
```

Mở trình duyệt: http://localhost:5173

## 📌 Phân công chức năng

| Nhóm | Phần Admin | Phần GV | Phần HV |
|------|-----------|---------|---------|
| **N1** | Khóa học, Lớp học, Lịch học | Lớp của tôi, TKB, Giáo trình | Thông tin lớp, Giáo trình, TKB |
| **N2** | Học viên, Đăng ký, Điểm danh, Kết quả | Điểm danh, Nhập điểm, DS học viên, Đơn nghỉ | Tổng quan, Điểm danh, GV lớp, Xin nghỉ |
| **N3** | Thu học phí, Công nợ, Báo cáo | — | Nội quy |
