# 🟦 NHÓM N1 — Khóa học & Lịch học

## Phạm vi làm việc

Nhóm N1 chịu trách nhiệm các màn hình liên quan đến **Quản lý Khóa học, Lớp học và Lịch học**.

## 📁 Thư mục của N1

```
src/views/admin/n1/         ← Màn hình Admin
src/views/teacher/n1/       ← Màn hình Giáo viên  
src/views/student/n1/       ← Màn hình Học viên
src/data/mockN1.js          ← Dữ liệu mock của N1
```

## 📋 Danh sách màn hình cần làm

### Admin
| File | Chức năng |
|------|-----------|
| `N1_CourseList.vue` | Danh sách khóa học (grid card, 12 khóa) |
| `N1_ClassList.vue` | Danh sách lớp học (bảng + filter) |
| `N1_Schedule.vue` | Lịch học dạng timeline theo giờ |

### Giáo viên
| File | Chức năng |
|------|-----------|
| `N1_MyClasses.vue` | Lớp học của tôi (2x2 card grid) |
| `N1_Schedule.vue` | Thời khoá biểu dạng calendar grid |
| `N1_Curriculum.vue` | Giáo trình theo tuần + progress bar |

### Học viên
| File | Chức năng |
|------|-----------|
| `N1_ClassInfo.vue` | Thông tin lớp học + học phí |
| `N1_Curriculum.vue` | Giáo trình với trạng thái mỗi module |
| `N1_Schedule.vue` | Thời khoá biểu cá nhân |

## 🌐 API Backend N1

Base URL: `http://<IP-N1>:5000`

| Endpoint | Mô tả |
|----------|-------|
| `GET /api/courses` | Danh sách khóa học |
| `GET /api/classes` | Danh sách lớp học |
| `GET /api/schedule` | Lịch học |

## 🎨 Màu sắc chủ đạo N1

- Admin: Blue `#2563eb`
- Card accent: đa dạng (blue, green, orange, purple...)

## 🔧 Bắt đầu

```bash
git checkout feature/N1
# Làm việc trong src/views/admin/n1/ và src/views/teacher/n1/
git add src/views/admin/n1/ src/views/teacher/n1/ src/views/student/n1/ src/data/mockN1.js
git commit -m "feat(N1): mô tả"
git push origin feature/N1
```
