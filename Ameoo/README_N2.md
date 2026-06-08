# 🟩 NHÓM N2 — Học viên & Điểm danh

## Phạm vi làm việc

Nhóm N2 chịu trách nhiệm các màn hình liên quan đến **Quản lý Học viên, Điểm danh, Nhập điểm và Đơn xin nghỉ**.

## 📁 Thư mục của N2

```
src/views/admin/n2/         ← Màn hình Admin
src/views/teacher/n2/       ← Màn hình Giáo viên
src/views/student/n2/       ← Màn hình Học viên
src/data/mockN2.js          ← Dữ liệu mock của N2
```

## 📋 Danh sách màn hình cần làm

### Admin
| File | Chức năng |
|------|-----------|
| `N2_StudentList.vue` | Danh sách học viên (bảng + progress chuyên cần) |
| `N2_Registration.vue` | Duyệt đăng ký khóa học (Duyệt/Từ chối) |
| `N2_Attendance.vue` | Điểm danh (toggle P/A/L + thống kê bên phải) |
| `N2_Scores.vue` | Kết quả học tập (tabs Giữa kỳ/Cuối kỳ/Tổng kết) |

### Giáo viên
| File | Chức năng |
|------|-----------|
| `N2_Attendance.vue` | Điểm danh lớp (Có mặt/Trễ/Vắng) |
| `N2_Scores.vue` | Nhập điểm 4 kỹ năng (Listening/Reading/Writing/Speaking) |
| `N2_StudentList.vue` | Danh sách học viên + điểm danh % + điểm TB |
| `N2_LeaveRequests.vue` | Duyệt đơn xin nghỉ |

### Học viên
| File | Chức năng |
|------|-----------|
| `N2_Overview.vue` | Tổng quan (banner teal + 4 stat + lịch tuần) |
| `N2_AttendanceStats.vue` | Thống kê điểm danh (calendar màu + chi tiết buổi) |
| `N2_TeacherProfile.vue` | Hồ sơ giáo viên |
| `N2_LeaveRequest.vue` | Form xin nghỉ + lịch sử đơn |

## 🌐 API Backend N2

Base URL: `http://172.16.68.127:5210`

| Endpoint | Mô tả |
|----------|-------|
| `GET /api/students` | Danh sách học viên |
| `GET /api/attendance` | Dữ liệu điểm danh |
| `POST /api/attendance` | Lưu điểm danh |
| `GET /api/scores` | Bảng điểm |
| `PUT /api/scores/{id}` | Cập nhật điểm |
| `GET /api/leave-requests` | Đơn xin nghỉ |
| `GET /api/registrations` | Đăng ký khóa học |

## 🎨 Màu sắc

- Student pages: Teal `#0d9488`
- Teacher pages: Purple `#7c3aed`

## 🔧 Bắt đầu

```bash
git checkout feature/N2
# Làm việc trong thư mục n2/
git add src/views/admin/n2/ src/views/teacher/n2/ src/views/student/n2/ src/data/mockN2.js
git commit -m "feat(N2): mô tả"
git push origin feature/N2
```
