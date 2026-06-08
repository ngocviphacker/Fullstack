<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'

const API = 'http://192.168.30.70:5210'
const students = ref([])
const loading = ref(false)
const error = ref('')
const search = ref('')
const showForm = ref(false)
const editId = ref(null)
const form = ref({ name:'', email:'', phone:'', course:'', status:'Đang học' })

const filtered = computed(() => {
  const q = search.value.toLowerCase()
  if (!q) return students.value
  return students.value.filter(s =>
    s.name.toLowerCase().includes(q) || s.email.toLowerCase().includes(q) || s.course.toLowerCase().includes(q))
})

const stats = computed(() => ({
  total: students.value.length,
  active: students.value.filter(s => s.status === 'Đang học').length,
  done: students.value.filter(s => s.status === 'Hoàn thành').length,
  paused: students.value.filter(s => s.status === 'Tạm dừng').length
}))

onMounted(() => load())

async function load() {
  loading.value = true; error.value = ''
  try { students.value = (await axios.get(`${API}/api/students`)).data }
  catch (e) { error.value = 'Không kết nối được API: ' + e.message }
  finally { loading.value = false }
}

function openAdd() { editId.value = null; form.value = { name:'', email:'', phone:'', course:'', status:'Đang học' }; showForm.value = true }
function openEdit(s) { editId.value = s.id; form.value = { ...s }; showForm.value = true }

async function save() {
  try {
    if (editId.value) {
      await axios.put(`${API}/api/students/${editId.value}`, form.value)
    } else {
      await axios.post(`${API}/api/students`, form.value)
    }
    showForm.value = false; await load()
  } catch (e) { error.value = 'Lỗi lưu: ' + e.message }
}

async function remove(id) {
  if (!confirm('Bạn có chắc muốn xóa học viên này?')) return
  try { await axios.delete(`${API}/api/students/${id}`); await load() }
  catch (e) { error.value = 'Lỗi xóa: ' + e.message }
}

function statusColor(s) {
  return s === 'Đang học' ? 'badge-blue' : s === 'Hoàn thành' ? 'badge-green' : 'badge-orange'
}
</script>

<template>
  <div class="page">
    <!-- Header -->
    <div class="page-header">
      <div>
        <h1 class="page-title">👥 Hồ sơ học viên</h1>
        <p class="page-sub">Quản lý thông tin cá nhân, khóa học đã/đang học</p>
      </div>
      <button class="btn-primary" @click="openAdd">+ Thêm học viên</button>
    </div>

    <!-- Stats -->
    <div class="stats-row">
      <div class="stat-card"><div class="stat-num blue">{{ stats.total }}</div><div class="stat-lbl">Tổng học viên</div></div>
      <div class="stat-card"><div class="stat-num green">{{ stats.active }}</div><div class="stat-lbl">Đang học</div></div>
      <div class="stat-card"><div class="stat-num purple">{{ stats.done }}</div><div class="stat-lbl">Hoàn thành</div></div>
      <div class="stat-card"><div class="stat-num orange">{{ stats.paused }}</div><div class="stat-lbl">Tạm dừng</div></div>
    </div>

    <!-- Search -->
    <div class="toolbar">
      <div class="search-box">
        <span class="search-icon">🔍</span>
        <input v-model="search" placeholder="Tìm theo tên, email, khóa học..." class="search-input" />
      </div>
      <span class="result-count">{{ filtered.length }} kết quả</span>
    </div>

    <div v-if="loading" class="center-msg">⏳ Đang tải...</div>
    <div v-if="error" class="error-msg">⛔ {{ error }}</div>

    <!-- Table -->
    <div v-if="filtered.length" class="table-card">
      <table>
        <thead>
          <tr><th>ID</th><th>Họ tên</th><th>Email</th><th>SĐT</th><th>Khóa học</th><th>Trạng thái</th><th>Ngày ĐK</th><th>Hành động</th></tr>
        </thead>
        <tbody>
          <tr v-for="s in filtered" :key="s.id">
            <td class="mono">{{ s.id }}</td>
            <td class="fw">{{ s.name }}</td>
            <td class="dim">{{ s.email }}</td>
            <td class="mono">{{ s.phone }}</td>
            <td>{{ s.course }}</td>
            <td><span :class="['badge', statusColor(s.status)]">{{ s.status }}</span></td>
            <td class="mono dim">{{ s.enrollDate }}</td>
            <td class="actions">
              <button class="btn-sm btn-edit" @click="openEdit(s)">✏️ Sửa</button>
              <button class="btn-sm btn-del" @click="remove(s.id)">🗑️ Xóa</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <div v-else-if="!loading" class="empty">Không có dữ liệu học viên</div>

    <!-- Modal Form -->
    <Transition name="modal">
      <div v-if="showForm" class="modal-overlay" @click.self="showForm=false">
        <div class="modal-box">
          <h3>{{ editId ? '✏️ Sửa học viên' : '➕ Thêm học viên mới' }}</h3>
          <div class="form-grid">
            <div class="field"><label>Họ tên *</label><input v-model="form.name" placeholder="Nguyễn Văn A" /></div>
            <div class="field"><label>Email *</label><input v-model="form.email" type="email" placeholder="email@student.edu.vn" /></div>
            <div class="field"><label>SĐT</label><input v-model="form.phone" placeholder="0901234567" /></div>
            <div class="field"><label>Khóa học</label><input v-model="form.course" placeholder="Tiếng Anh Giao Tiếp" /></div>
            <div class="field">
              <label>Trạng thái</label>
              <select v-model="form.status">
                <option>Đang học</option><option>Hoàn thành</option><option>Tạm dừng</option>
              </select>
            </div>
          </div>
          <div class="modal-actions">
            <button class="btn-cancel" @click="showForm=false">Hủy</button>
            <button class="btn-primary" @click="save" :disabled="!form.name||!form.email">{{ editId ? 'Cập nhật' : 'Thêm' }}</button>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
/* Component-specific styles handled by shared CSS */
</style>
