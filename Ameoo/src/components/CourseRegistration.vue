<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'

const API = 'http://192.168.30.70:5210'
const registrations = ref([])
const students = ref([])
const courses = ref([])
const loading = ref(false)
const error = ref('')
const showForm = ref(false)
const selStudent = ref(null)
const selCourse = ref(null)

const stats = computed(() => ({
  total: registrations.value.length,
  active: registrations.value.filter(r => r.status === 'Đang học').length,
  done: registrations.value.filter(r => r.status === 'Hoàn thành').length
}))

onMounted(() => loadAll())

async function loadAll() {
  loading.value = true; error.value = ''
  try {
    const [rRes, sRes, cRes] = await Promise.all([
      axios.get(`${API}/api/registrations`),
      axios.get(`${API}/api/students`),
      axios.get(`${API}/api/courses`)
    ])
    registrations.value = rRes.data
    students.value = sRes.data
    courses.value = cRes.data
  } catch (e) { error.value = 'Không kết nối được API: ' + e.message }
  finally { loading.value = false }
}

function openRegister() { selStudent.value = null; selCourse.value = null; showForm.value = true }

async function register() {
  const s = students.value.find(x => x.id === selStudent.value)
  const c = courses.value.find(x => x.id === selCourse.value)
  if (!s || !c) return
  try {
    await axios.post(`${API}/api/registrations`, {
      studentId: s.id, studentName: s.name,
      courseId: c.id, courseName: c.name
    })
    showForm.value = false; await loadAll()
  } catch (e) { error.value = 'Lỗi đăng ký: ' + e.message }
}

async function updateStatus(r, newStatus) {
  try {
    await axios.put(`${API}/api/registrations/${r.id}`, { status: newStatus })
    await loadAll()
  } catch (e) { error.value = 'Lỗi cập nhật: ' + e.message }
}

function statusColor(s) {
  return s === 'Đang học' ? 'badge-blue' : s === 'Hoàn thành' ? 'badge-green' : 'badge-orange'
}
</script>

<template>
  <div class="page">
    <div class="page-header">
      <div>
        <h1 class="page-title">📝 Đăng ký khóa học</h1>
        <p class="page-sub">Đăng ký, quản lý trạng thái đang học / hoàn thành</p>
      </div>
      <button class="btn-primary" @click="openRegister">+ Đăng ký mới</button>
    </div>

    <!-- Stats -->
    <div class="stats-row">
      <div class="stat-card"><div class="stat-num blue">{{ stats.total }}</div><div class="stat-lbl">Tổng đăng ký</div></div>
      <div class="stat-card"><div class="stat-num green">{{ stats.active }}</div><div class="stat-lbl">Đang học</div></div>
      <div class="stat-card"><div class="stat-num purple">{{ stats.done }}</div><div class="stat-lbl">Hoàn thành</div></div>
    </div>

    <!-- Course list -->
    <div class="section-title">📚 Danh sách khóa học đang mở</div>
    <div class="course-grid" v-if="courses.length">
      <div v-for="c in courses" :key="c.id" class="course-card" :class="{ closed: c.status === 'Đã đóng' }">
        <div class="course-name">{{ c.name }}</div>
        <div class="course-info">
          <span>👨‍🏫 {{ c.instructor }}</span>
          <span>⏱ {{ c.duration }}</span>
        </div>
        <div class="course-bar-wrap">
          <div class="course-bar" :style="{ width: Math.min(c.currentStudents / c.maxStudents * 100, 100) + '%' }" :class="{ full: c.currentStudents >= c.maxStudents }"></div>
        </div>
        <div class="course-slots">{{ c.currentStudents }}/{{ c.maxStudents }} học viên</div>
        <span :class="['badge', c.status === 'Đang mở' ? 'badge-green' : 'badge-orange']">{{ c.status }}</span>
      </div>
    </div>

    <div v-if="loading" class="center-msg">⏳ Đang tải...</div>
    <div v-if="error" class="error-msg">⛔ {{ error }}</div>

    <!-- Registration table -->
    <div class="section-title" style="margin-top:24px">📋 Lịch sử đăng ký</div>
    <div v-if="registrations.length" class="table-card">
      <table>
        <thead><tr><th>ID</th><th>Học viên</th><th>Khóa học</th><th>Ngày ĐK</th><th>Trạng thái</th><th>Hành động</th></tr></thead>
        <tbody>
          <tr v-for="r in registrations" :key="r.id">
            <td class="mono">{{ r.id }}</td>
            <td class="fw">{{ r.studentName }}</td>
            <td>{{ r.courseName }}</td>
            <td class="mono dim">{{ r.registerDate }}</td>
            <td><span :class="['badge', statusColor(r.status)]">{{ r.status }}</span></td>
            <td class="actions">
              <button v-if="r.status === 'Đang học'" class="btn-sm btn-green-sm" @click="updateStatus(r, 'Hoàn thành')">✅ Hoàn thành</button>
              <button v-if="r.status === 'Hoàn thành'" class="btn-sm btn-edit" @click="updateStatus(r, 'Đang học')">↩️ Mở lại</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modal -->
    <Transition name="modal">
      <div v-if="showForm" class="modal-overlay" @click.self="showForm=false">
        <div class="modal-box">
          <h3>📝 Đăng ký khóa học mới</h3>
          <div class="form-grid">
            <div class="field">
              <label>Học viên *</label>
              <select v-model="selStudent">
                <option :value="null" disabled>-- Chọn học viên --</option>
                <option v-for="s in students" :key="s.id" :value="s.id">{{ s.name }} ({{ s.email }})</option>
              </select>
            </div>
            <div class="field">
              <label>Khóa học *</label>
              <select v-model="selCourse">
                <option :value="null" disabled>-- Chọn khóa học --</option>
                <option v-for="c in courses.filter(x => x.status === 'Đang mở')" :key="c.id" :value="c.id">{{ c.name }} ({{ c.currentStudents }}/{{ c.maxStudents }})</option>
              </select>
            </div>
          </div>
          <div class="modal-actions">
            <button class="btn-cancel" @click="showForm=false">Hủy</button>
            <button class="btn-primary" @click="register" :disabled="!selStudent || !selCourse">Đăng ký</button>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
.course-grid { display:grid; grid-template-columns:repeat(auto-fill,minmax(260px,1fr)); gap:16px; margin-bottom:8px; }
.course-card {
  background:white; border-radius:12px; padding:18px; border:1px solid #e8ecf0;
  box-shadow:0 1px 4px rgba(0,0,0,.04); transition:transform .2s, box-shadow .2s;
}
.course-card:hover { transform:translateY(-2px); box-shadow:0 4px 16px rgba(0,0,0,.08); }
.course-card.closed { opacity:.6; }
.course-name { font-weight:700; font-size:15px; color:#1e293b; margin-bottom:8px; }
.course-info { display:flex; gap:14px; font-size:12.5px; color:#64748b; margin-bottom:10px; }
.course-bar-wrap { height:6px; background:#f0f0f0; border-radius:3px; overflow:hidden; margin-bottom:6px; }
.course-bar { height:100%; background:linear-gradient(90deg,#3b82f6,#60a5fa); border-radius:3px; transition:width .3s; }
.course-bar.full { background:linear-gradient(90deg,#ef4444,#f87171); }
.course-slots { font-size:12px; color:#94a3b8; margin-bottom:6px; }
</style>
