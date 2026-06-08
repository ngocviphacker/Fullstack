<script setup>
import { ref, computed } from 'vue'

// Pages - Admin
import AdminDashboard from './views/admin/Dashboard.vue'
import AdminCourseList from './views/admin/CourseList.vue'
import AdminClassList from './views/admin/ClassList.vue'
import AdminSchedule from './views/admin/Schedule.vue'
import AdminStudentList from './views/admin/StudentList.vue'
import AdminRegistration from './views/admin/Registration.vue'
import AdminAttendance from './views/admin/Attendance.vue'
import AdminScores from './views/admin/Scores.vue'
// Nhóm 3 — Payment & Report Service
import N3Tuition from './views/admin/n3/N3_Tuition.vue'
// StudentRules replaced by N3_Rules below
// Pages - Teacher
import TeacherDashboard from './views/teacher/Dashboard.vue'
import TeacherMyClasses from './views/teacher/MyClasses.vue'
import TeacherAttendance from './views/teacher/Attendance.vue'
import TeacherScores from './views/teacher/Scores.vue'
import TeacherSchedule from './views/teacher/Schedule.vue'
import TeacherCurriculum from './views/teacher/Curriculum.vue'
import TeacherLeave from './views/teacher/LeaveRequests.vue'
import TeacherStudentList from './views/teacher/StudentList.vue'
// Pages - Student
import StudentOverview from './views/student/Overview.vue'
import StudentClassInfo from './views/student/ClassInfo.vue'
import StudentAttStats from './views/student/AttendanceStats.vue'
import StudentTeacher from './views/student/TeacherProfile.vue'
import StudentCurriculum from './views/student/Curriculum.vue'
import StudentSchedule from './views/student/Schedule.vue'
import StudentLeave from './views/student/LeaveRequest.vue'
import N3Rules from './views/student/n3/N3_Rules.vue'

const role = ref('admin')
const page = ref('dashboard')

const pageMap = {
  admin: {
    dashboard: AdminDashboard, courseList: AdminCourseList, classList: AdminClassList,
    schedule: AdminSchedule, studentList: AdminStudentList, registration: AdminRegistration,
    attendance: AdminAttendance, scores: AdminScores,
    // Nhóm 3 — Finance Module
    tuition: N3Tuition, debt: N3Tuition, revenue: N3Tuition, n3config: N3Tuition
  },
  teacher: {
    dashboard: TeacherDashboard, myClasses: TeacherMyClasses, attendance: TeacherAttendance,
    scores: TeacherScores, schedule: TeacherSchedule, curriculum: TeacherCurriculum,
    leave: TeacherLeave, studentList: TeacherStudentList
  },
  student: {
    overview: StudentOverview, classInfo: StudentClassInfo, attStats: StudentAttStats,
    teacherProfile: StudentTeacher, curriculum: StudentCurriculum, schedule: StudentSchedule,
    leave: StudentLeave, rules: N3Rules
  }
}

const currentPage = computed(() => {
  const m = pageMap[role.value]
  return m[page.value] || Object.values(m)[0]
})

const adminMenu = [
  { group:'TỔNG QUAN', items:[{key:'dashboard',icon:'⊞',label:'Dashboard'}] },
  { group:'KHÓA HỌC', items:[
    {key:'courseList',icon:'☐',label:'Danh sách khóa học',badge:12},
    {key:'classList',icon:'👥',label:'Lớp học'},
    {key:'schedule',icon:'📅',label:'Lịch học'}
  ]},
  { group:'HỌC VIÊN', items:[
    {key:'studentList',icon:'👤',label:'Danh sách học viên'},
    {key:'registration',icon:'📝',label:'Đăng ký khóa học',badge:3},
    {key:'attendance',icon:'✓',label:'Điểm danh'},
    {key:'scores',icon:'📊',label:'Kết quả học tập'}
  ]},
  { group:'TÀI CHÍNH · NHÓM 3', items:[
    {key:'tuition',icon:'💳',label:'Thu học phí',badge:5},
    {key:'debt',icon:'⚠️',label:'Công nợ học viên'},
    {key:'revenue',icon:'📊',label:'Báo cáo doanh thu'},
    {key:'n3config',icon:'⚙️',label:'Cấu hình API N3'}
  ]}
]

const teacherMenu = [
  { group:'TỔNG QUAN', items:[{key:'dashboard',icon:'⊞',label:'Dashboard'}] },
  { group:'', items:[{key:'myClasses',icon:'📚',label:'Lớp học của tôi',badge:4}] },
  { group:'QUẢN LÝ LỚP', items:[
    {key:'attendance',icon:'✓',label:'Điểm danh',tagText:'Hôm nay'},
    {key:'scores',icon:'⭐',label:'Nhập điểm'},
    {key:'schedule',icon:'📅',label:'Thời khoá biểu'},
    {key:'curriculum',icon:'📖',label:'Giáo trình'},
    {key:'leave',icon:'📋',label:'Đơn xin nghỉ',badge:2}
  ]},
  { group:'KHÁC', items:[{key:'studentList',icon:'👥',label:'Danh sách học viên'}] }
]

const studentMenu = [
  { group:'CỦA TÔI', items:[
    {key:'overview',icon:'⊞',label:'Tổng quan'},
    {key:'classInfo',icon:'📚',label:'Thông tin lớp học'},
    {key:'attStats',icon:'📊',label:'Thống kê điểm danh'},
    {key:'teacherProfile',icon:'👤',label:'Giáo viên lớp'},
    {key:'curriculum',icon:'📖',label:'Giáo trình'},
    {key:'schedule',icon:'📅',label:'Thời khoá biểu'},
    {key:'leave',icon:'✈',label:'Xin nghỉ phép'},
    {key:'rules',icon:'📋',label:'Nội quy'}
  ]}
]

const menus = { admin: adminMenu, teacher: teacherMenu, student: studentMenu }
const currentMenu = computed(() => menus[role.value])
const defaultPages = { admin:'dashboard', teacher:'dashboard', student:'overview' }

function switchRole(r) {
  role.value = r
  page.value = defaultPages[r]
}

const brandName = computed(() => role.value === 'admin' ? 'EduCore' : 'EduHub')
const roleLabel = computed(() => ({ admin:'', teacher:'Giáo viên', student:'Học viên' }[role.value]))
const accentClass = computed(() => `role-${role.value}`)
</script>

<template>
<div class="app-layout" :class="accentClass">
  <!-- SIDEBAR -->
  <aside class="sidebar">
    <div class="sidebar-brand">
      <div class="brand-icon">📘</div>
      <div>
        <div class="brand-name">{{ brandName }}</div>
        <span v-if="roleLabel" class="role-badge">{{ roleLabel }}</span>
        <span v-else class="pro-badge">Pro</span>
      </div>
    </div>

    <nav class="sidebar-nav">
      <div v-for="section in currentMenu" :key="section.group" class="nav-section">
        <div v-if="section.group" class="nav-group-label">{{ section.group }}</div>
        <button v-for="item in section.items" :key="item.key"
          class="nav-item" :class="{active: page === item.key}"
          @click="page = item.key">
          <span class="nav-icon">{{ item.icon }}</span>
          <span class="nav-label">{{ item.label }}</span>
          <span v-if="item.tagText" class="nav-tag">{{ item.tagText }}</span>
          <span v-if="item.badge" class="nav-badge">{{ item.badge }}</span>
        </button>
      </div>
    </nav>

    <div class="sidebar-footer">
      <div class="role-switcher">
        <button v-if="role !== 'admin'" class="switch-btn" @click="switchRole(role === 'teacher' ? 'student' : 'teacher')">
          ↔ Chuyển sang {{ role === 'teacher' ? 'Học viên' : 'Giáo viên' }}
        </button>
      </div>
      <div class="user-info">
        <div class="user-avatar">{{ role === 'admin' ? 'AD' : role === 'teacher' ? 'T' : 'VA' }}</div>
        <div>
          <div class="user-name">{{ role === 'admin' ? 'Quản trị viên' : role === 'teacher' ? 'Nguyễn Thị Lan' : 'Nguyễn Văn An' }}</div>
          <div class="user-role">{{ role === 'admin' ? 'ADMIN' : role === 'teacher' ? 'Giáo viên Tiếng Anh' : 'HV-0312 · TOEIC 600+' }}</div>
        </div>
      </div>
    </div>
  </aside>

  <!-- MAIN -->
  <main class="main-content">
    <header class="topbar">
      <div class="breadcrumb">{{ brandName }} / {{ currentMenu.flatMap(s=>s.items).find(i=>i.key===page)?.label || '' }}</div>
      <div class="topbar-right">
        <div class="search-box"><span>🔍</span><input placeholder="Tìm kiếm..." /></div>
        <button v-if="role==='student'" class="topbar-btn topbar-btn-leave">✈ Xin nghỉ</button>
        <div class="notif-icon">🔔 <span class="notif-dot">(0)</span></div>
        <!-- Role switcher for demo -->
        <select class="role-select" :value="role" @change="switchRole($event.target.value)">
          <option value="admin">Admin</option>
          <option value="teacher">Giáo viên</option>
          <option value="student">Học viên</option>
        </select>
      </div>
    </header>
    <div class="page-content">
      <component :is="currentPage" />
    </div>
  </main>
</div>
</template>

<style>
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap');
*{margin:0;padding:0;box-sizing:border-box}
body{font-family:'Inter',sans-serif;background:#f8fafc;color:#334155}

.app-layout{display:flex;min-height:100vh}

/* SIDEBAR */
.sidebar{width:260px;background:#fff;border-right:1px solid #e2e8f0;display:flex;flex-direction:column;position:fixed;top:0;left:0;bottom:0;z-index:100}
.sidebar-brand{display:flex;align-items:center;gap:10px;padding:20px;border-bottom:1px solid #f1f5f9}
.brand-icon{width:36px;height:36px;border-radius:10px;display:flex;align-items:center;justify-content:center;font-size:20px}
.role-admin .brand-icon{background:#2563eb;color:#fff}
.role-teacher .brand-icon{background:#7c3aed;color:#fff}
.role-student .brand-icon{background:#0d9488;color:#fff}
.brand-name{font-weight:700;font-size:18px;color:#0f172a}
.pro-badge{font-size:10px;background:#e0e7ff;color:#4338ca;padding:2px 6px;border-radius:4px;font-weight:600}
.role-badge{font-size:10px;padding:2px 8px;border-radius:4px;font-weight:600}
.role-teacher .role-badge{background:#ede9fe;color:#7c3aed}
.role-student .role-badge{background:#ccfbf1;color:#0d9488}

.sidebar-nav{flex:1;overflow-y:auto;padding:8px 0}
.nav-group-label{font-size:11px;font-weight:600;color:#94a3b8;padding:16px 20px 6px;text-transform:uppercase;letter-spacing:0.5px}
.nav-item{display:flex;align-items:center;gap:10px;width:100%;border:none;background:none;padding:10px 20px;cursor:pointer;font-size:14px;color:#64748b;text-align:left;transition:all .15s;border-left:3px solid transparent}
.nav-item:hover{background:#f8fafc;color:#334155}
.nav-item.active{color:#0f172a;font-weight:600;background:#f0f5ff}
.role-admin .nav-item.active{border-left-color:#2563eb;color:#2563eb;background:#eff6ff}
.role-teacher .nav-item.active{border-left-color:#7c3aed;color:#7c3aed;background:#f5f3ff}
.role-student .nav-item.active{border-left-color:#0d9488;color:#0d9488;background:#f0fdfa}
.nav-icon{font-size:16px;width:20px;text-align:center}
.nav-label{flex:1}
.nav-badge{font-size:11px;font-weight:600;color:#fff;padding:2px 8px;border-radius:10px}
.role-admin .nav-badge{background:#2563eb}
.role-teacher .nav-badge{background:#7c3aed}
.role-student .nav-badge{background:#0d9488}
.nav-tag{font-size:10px;padding:2px 6px;border-radius:4px;font-weight:600}
.role-teacher .nav-tag{background:#fef3c7;color:#d97706}
.role-admin .nav-tag{background:#fef3c7;color:#d97706}

.sidebar-footer{border-top:1px solid #f1f5f9;padding:12px 16px}
.role-switcher{margin-bottom:8px}
.switch-btn{width:100%;border:1px solid #e2e8f0;background:#f8fafc;color:#64748b;padding:8px;border-radius:8px;font-size:12px;cursor:pointer;transition:.15s}
.switch-btn:hover{background:#e2e8f0}
.user-info{display:flex;align-items:center;gap:10px;padding:8px 0}
.user-avatar{width:36px;height:36px;border-radius:50%;display:flex;align-items:center;justify-content:center;font-weight:700;font-size:13px;color:#fff}
.role-admin .user-avatar{background:#2563eb}
.role-teacher .user-avatar{background:#7c3aed}
.role-student .user-avatar{background:#0d9488}
.user-name{font-weight:600;font-size:13px;color:#0f172a}
.user-role{font-size:11px;color:#94a3b8}

/* TOPBAR */
.main-content{margin-left:260px;flex:1;display:flex;flex-direction:column;min-height:100vh}
.topbar{display:flex;justify-content:space-between;align-items:center;padding:12px 28px;background:#fff;border-bottom:1px solid #e2e8f0;position:sticky;top:0;z-index:50}
.breadcrumb{font-size:14px;color:#94a3b8}
.topbar-right{display:flex;align-items:center;gap:16px}
.search-box{display:flex;align-items:center;gap:6px;background:#f1f5f9;padding:8px 14px;border-radius:8px;min-width:200px}
.search-box input{border:none;background:none;outline:none;font-size:13px;color:#334155;width:100%}
.topbar-btn-leave{background:#0d9488;color:#fff;border:none;padding:8px 16px;border-radius:8px;font-size:13px;font-weight:500;cursor:pointer}
.notif-icon{font-size:16px;cursor:pointer;color:#64748b}
.notif-dot{font-size:11px}
.role-select{border:1px solid #e2e8f0;border-radius:6px;padding:6px 10px;font-size:12px;background:#fff;cursor:pointer;outline:none}

/* PAGE */
.page-content{padding:24px 28px;flex:1}

/* SHARED */
.card{background:#fff;border-radius:12px;padding:20px;box-shadow:0 1px 3px rgba(0,0,0,.04);margin-bottom:20px}
.card-title{font-size:16px;font-weight:700;color:#0f172a;margin-bottom:4px}
.card-subtitle{font-size:13px;color:#94a3b8}
.page-header{display:flex;justify-content:space-between;align-items:flex-start;margin-bottom:24px}
.page-title{font-size:22px;font-weight:700;color:#0f172a}
.page-desc{font-size:13px;color:#94a3b8;margin-top:4px}
.btn-actions{display:flex;gap:10px}

.btn-primary{padding:10px 20px;border-radius:8px;border:none;font-weight:500;font-size:13px;cursor:pointer;color:#fff;transition:.15s}
.role-admin .btn-primary{background:#2563eb}
.role-teacher .btn-primary{background:#7c3aed}
.role-student .btn-primary{background:#0d9488}
.btn-outline{padding:10px 20px;border-radius:8px;background:#fff;border:1px solid #cbd5e1;font-weight:500;font-size:13px;cursor:pointer;color:#334155}

table.data-table{width:100%;border-collapse:collapse}
table.data-table th{text-align:left;padding:12px 16px;font-size:11px;font-weight:600;color:#94a3b8;text-transform:uppercase;border-bottom:1px solid #e2e8f0;letter-spacing:.5px}
table.data-table td{padding:14px 16px;border-bottom:1px solid #f1f5f9;vertical-align:middle;font-size:14px}

.avatar{width:32px;height:32px;border-radius:50%;display:inline-flex;align-items:center;justify-content:center;font-weight:700;font-size:12px;color:#fff;flex-shrink:0}
.avatar.blue{background:#3b82f6}.avatar.green{background:#10b981}.avatar.purple{background:#8b5cf6}
.avatar.orange{background:#f59e0b}.avatar.red{background:#ef4444}.avatar.teal{background:#0d9488}
.user-cell{display:flex;align-items:center;gap:10px}
.user-cell .name{font-weight:600;color:#0f172a;font-size:14px}
.user-cell .code{font-size:12px;color:#94a3b8}

.badge{padding:4px 10px;border-radius:16px;font-size:11px;font-weight:600;white-space:nowrap}
.badge-green{background:#dcfce7;color:#166534}
.badge-yellow{background:#fef9c3;color:#854d0e}
.badge-red{background:#fee2e2;color:#991b1b}
.badge-blue{background:#dbeafe;color:#1e40af}
.badge-purple{background:#ede9fe;color:#6d28d9}
.badge-orange{background:#ffedd5;color:#c2410c}
.badge-teal{background:#ccfbf1;color:#0d9488}
.badge-gray{background:#f1f5f9;color:#64748b}

.progress-row{display:flex;align-items:center;gap:8px}
.progress-bar{flex:1;height:6px;background:#e2e8f0;border-radius:3px;overflow:hidden;min-width:60px}
.progress-fill{height:100%;border-radius:3px;transition:width .3s}
.progress-fill.green{background:#10b981}
.progress-fill.blue{background:#3b82f6}
.progress-fill.orange{background:#f59e0b}
.progress-fill.red{background:#ef4444}
.progress-fill.purple{background:#8b5cf6}
.progress-fill.teal{background:#0d9488}
.progress-text{font-size:13px;font-weight:500;min-width:36px}

.stat-cards{display:grid;gap:16px;margin-bottom:24px}
.stat-cards.cols-4{grid-template-columns:repeat(4,1fr)}
.stat-cards.cols-3{grid-template-columns:repeat(3,1fr)}
.stat-card{background:#fff;border-radius:12px;padding:20px;border-top:3px solid transparent}
.stat-card .label{font-size:11px;font-weight:600;color:#94a3b8;text-transform:uppercase;letter-spacing:.5px}
.stat-card .value{font-size:28px;font-weight:700;color:#0f172a;margin-top:4px}
.stat-card .sub{font-size:12px;color:#94a3b8;margin-top:2px}
.stat-card.blue{border-top-color:#3b82f6}
.stat-card.green{border-top-color:#10b981}
.stat-card.orange{border-top-color:#f59e0b}
.stat-card.red{border-top-color:#ef4444}
.stat-card.purple{border-top-color:#8b5cf6}
.stat-card.teal{border-top-color:#0d9488}

.grid-2{display:grid;grid-template-columns:2fr 1fr;gap:20px}
.grid-2-equal{display:grid;grid-template-columns:1fr 1fr;gap:20px}
.grid-2x2{display:grid;grid-template-columns:1fr 1fr;gap:20px}

.toggle-group{display:flex;gap:6px}
.toggle-btn{width:36px;height:32px;border-radius:6px;border:1px solid #e2e8f0;background:#fff;font-weight:700;font-size:13px;cursor:pointer;transition:.15s}
.toggle-btn:hover{background:#f1f5f9}
.toggle-btn.active-p{background:#dcfce7;border-color:#10b981;color:#10b981}
.toggle-btn.active-a{background:#fee2e2;border-color:#ef4444;color:#ef4444}
.toggle-btn.active-l{background:#fef9c3;border-color:#eab308;color:#eab308}

.btn-approve{border:1px solid #10b981;background:#dcfce7;color:#166534;padding:6px 12px;border-radius:6px;font-size:12px;font-weight:600;cursor:pointer}
.btn-reject{border:1px solid #ef4444;background:#fee2e2;color:#991b1b;padding:6px 12px;border-radius:6px;font-size:12px;font-weight:600;cursor:pointer;margin-left:6px}

.input-score{width:52px;text-align:center;border:1px solid #cbd5e1;border-radius:6px;padding:6px;font-size:14px;outline:none}
.input-score:focus{border-color:#3b82f6}
.input-ghost{border:none;background:transparent;width:100%;outline:none;font-size:13px;color:#64748b}

.tabs{display:flex;gap:0;border-bottom:2px solid #e2e8f0;margin-bottom:20px}
.tab{padding:10px 20px;font-size:14px;color:#64748b;cursor:pointer;border-bottom:2px solid transparent;margin-bottom:-2px;font-weight:500;transition:.15s}
.tab.active{color:#2563eb;border-bottom-color:#2563eb;font-weight:600}
.role-teacher .tab.active{color:#7c3aed;border-bottom-color:#7c3aed}
.role-student .tab.active{color:#0d9488;border-bottom-color:#0d9488}

.text-muted{color:#94a3b8;font-size:13px}
.text-bold{font-weight:600;color:#0f172a}
.text-green{color:#10b981}.text-red{color:#ef4444}.text-orange{color:#f59e0b}.text-blue{color:#2563eb}
.money{font-weight:700;font-size:15px}
.money.green{color:#10b981}.money.red{color:#ef4444}.money.orange{color:#f59e0b}

.filter-bar{display:flex;gap:12px;margin-bottom:20px;align-items:center}
.filter-bar select,.filter-bar input{border:1px solid #cbd5e1;border-radius:6px;padding:8px 12px;font-size:13px;outline:none;background:#fff}
.filter-bar input{min-width:220px}
.filter-bar .spacer{flex:1}

.legend{margin-top:12px;font-size:12px;color:#94a3b8;display:flex;gap:12px;align-items:center}
</style>
