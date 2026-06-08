<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import axios from 'axios'

const API = 'http://192.168.30.70:5210'
const attendances = ref([])
const students = ref([])
const courses = ref([])
const loading = ref(false)
const error = ref('')

// Filters
const filterStudent = ref('')
const filterCourse = ref('')
const filterStatus = ref('')
const filterDate = ref('')
const sortBy = ref('date-desc')

// Form
const showForm = ref(false)
const editId = ref(null)
const form = ref({ studentId:null, courseId:null, date:'', session:'', status:'Có mặt', note:'' })

// Student detail panel
const showDetail = ref(false)
const detailStudent = ref(null)
const detailData = ref(null)

const filtered = computed(() => {
  let list = [...attendances.value]
  const q = filterStudent.value.toLowerCase()
  if (q) list = list.filter(a => a.studentName.toLowerCase().includes(q))
  if (filterCourse.value) list = list.filter(a => a.courseName === filterCourse.value)
  if (filterStatus.value) list = list.filter(a => a.status === filterStatus.value)
  if (filterDate.value) list = list.filter(a => a.date === filterDate.value)
  if (sortBy.value === 'date-desc') list.sort((a,b) => b.date.localeCompare(a.date))
  else if (sortBy.value === 'date-asc') list.sort((a,b) => a.date.localeCompare(b.date))
  else if (sortBy.value === 'name') list.sort((a,b) => a.studentName.localeCompare(b.studentName))
  return list
})

const stats = computed(() => {
  const t = attendances.value.length
  const p = attendances.value.filter(a => a.status === 'Có mặt').length
  const l = attendances.value.filter(a => a.status === 'Muộn').length
  const ab = attendances.value.filter(a => a.status === 'Vắng').length
  const rate = t > 0 ? Math.round((p + l) / t * 100) : 0
  return { total:t, present:p, late:l, absent:ab, rate }
})

const uniqueDates = computed(() => [...new Set(attendances.value.map(a => a.date))].sort().reverse())
const uniqueCourses = computed(() => [...new Set(courses.value.map(c => c.name))])
const sessionOptions = ['Buổi 1','Buổi 2','Buổi 3','Buổi 4','Buổi 5','Buổi 6','Buổi 7','Buổi 8','Buổi 9','Buổi 10']

onMounted(() => loadAll())

async function loadAll() {
  loading.value = true; error.value = ''
  try {
    const [a,s,c] = await Promise.all([
      axios.get(`${API}/api/attendance`),
      axios.get(`${API}/api/students`),
      axios.get(`${API}/api/courses`)
    ])
    attendances.value = a.data; students.value = s.data; courses.value = c.data
  } catch(e) { error.value = 'Không kết nối được API: ' + e.message }
  finally { loading.value = false }
}

function openAdd() {
  editId.value = null
  form.value = { studentId:null, courseId:null, date:new Date().toISOString().split('T')[0], session:'Buổi 1', status:'Có mặt', note:'' }
  showForm.value = true
}

function openEdit(a) {
  editId.value = a.id
  const stu = students.value.find(s => s.name === a.studentName)
  const cou = courses.value.find(c => c.name === a.courseName)
  form.value = {
    studentId: stu?.id || null, courseId: cou?.id || null,
    date: a.date, session: a.session, status: a.status, note: a.note || ''
  }
  showForm.value = true
}

async function save() {
  const s = students.value.find(x => x.id === form.value.studentId)
  const c = courses.value.find(x => x.id === form.value.courseId)
  if (!s || !form.value.session || !form.value.date) return
  const payload = {
    studentId:s.id, studentName:s.name,
    courseId:c?.id||0, courseName:c?.name||'',
    date:form.value.date, session:form.value.session,
    status:form.value.status, note:form.value.note
  }
  try {
    if (editId.value) await axios.put(`${API}/api/attendance/${editId.value}`, payload)
    else await axios.post(`${API}/api/attendance`, payload)
    showForm.value = false; await loadAll()
  } catch(e) { error.value = 'Lỗi lưu: ' + e.message }
}

async function quickStatus(a, newStatus) {
  try {
    await axios.put(`${API}/api/attendance/${a.id}`, { status:newStatus })
    await loadAll()
  } catch(e) { error.value = 'Lỗi: ' + e.message }
}

async function viewStudentDetail(name) {
  detailStudent.value = name
  const stuRecords = attendances.value.filter(a => a.studentName === name)
  const total = stuRecords.length
  const present = stuRecords.filter(a => a.status === 'Có mặt').length
  const late = stuRecords.filter(a => a.status === 'Muộn').length
  const absent = stuRecords.filter(a => a.status === 'Vắng').length
  const rate = total > 0 ? Math.round((present + late) / total * 100) : 0
  detailData.value = { total, present, late, absent, rate, records: stuRecords }
  showDetail.value = true
}

function statusIcon(s) { return s==='Có mặt'?'✅':s==='Muộn'?'⏰':'❌' }
function statusColor(s) { return s==='Có mặt'?'badge-green':s==='Muộn'?'badge-orange':'badge-red' }
function rateColor(r) { return r >= 80 ? 'green' : r >= 60 ? 'orange' : 'red' }
function clearFilters() { filterStudent.value=''; filterCourse.value=''; filterStatus.value=''; filterDate.value='' }
</script>

<template>
  <div class="page">
    <!-- Header -->
    <div class="page-header">
      <div>
        <h1 class="page-title">📋 Điểm danh</h1>
        <p class="page-sub">Điểm danh theo từng buổi học · Sửa trạng thái · Xem thống kê chi tiết</p>
      </div>
      <button class="btn-primary" @click="openAdd">+ Điểm danh mới</button>
    </div>

    <!-- Stats -->
    <div class="stats-row">
      <div class="stat-card"><div class="stat-num blue">{{ stats.total }}</div><div class="stat-lbl">Tổng bản ghi</div></div>
      <div class="stat-card"><div class="stat-num green">{{ stats.present }}</div><div class="stat-lbl">✅ Có mặt</div></div>
      <div class="stat-card"><div class="stat-num orange">{{ stats.late }}</div><div class="stat-lbl">⏰ Muộn</div></div>
      <div class="stat-card"><div class="stat-num red">{{ stats.absent }}</div><div class="stat-lbl">❌ Vắng</div></div>
      <div class="stat-card">
        <div class="stat-num" :class="rateColor(stats.rate)">{{ stats.rate }}%</div>
        <div class="stat-lbl">Chuyên cần TB</div>
        <div class="rate-bar"><div class="rate-fill" :class="rateColor(stats.rate)" :style="{width:stats.rate+'%'}"></div></div>
      </div>
    </div>

    <!-- Advanced Filters -->
    <div class="filter-panel">
      <div class="filter-title">🔍 Bộ lọc nâng cao</div>
      <div class="filter-grid">
        <div class="filter-item">
          <label>Học viên</label>
          <div class="search-box"><span class="search-icon">👤</span><input v-model="filterStudent" placeholder="Tìm theo tên..." class="search-input" /></div>
        </div>
        <div class="filter-item">
          <label>Khóa học</label>
          <select v-model="filterCourse" class="filter-select">
            <option value="">Tất cả</option>
            <option v-for="c in uniqueCourses" :key="c">{{ c }}</option>
          </select>
        </div>
        <div class="filter-item">
          <label>Trạng thái</label>
          <select v-model="filterStatus" class="filter-select">
            <option value="">Tất cả</option>
            <option>Có mặt</option><option>Muộn</option><option>Vắng</option>
          </select>
        </div>
        <div class="filter-item">
          <label>Ngày</label>
          <select v-model="filterDate" class="filter-select">
            <option value="">Tất cả</option>
            <option v-for="d in uniqueDates" :key="d">{{ d }}</option>
          </select>
        </div>
        <div class="filter-item">
          <label>Sắp xếp</label>
          <select v-model="sortBy" class="filter-select">
            <option value="date-desc">Ngày mới nhất</option>
            <option value="date-asc">Ngày cũ nhất</option>
            <option value="name">Tên A→Z</option>
          </select>
        </div>
        <div class="filter-item" style="align-self:end">
          <button class="btn-clear" @click="clearFilters">🔄 Xóa bộ lọc</button>
        </div>
      </div>
      <div class="filter-result">Hiển thị <strong>{{ filtered.length }}</strong> / {{ attendances.length }} bản ghi</div>
    </div>

    <div v-if="loading" class="center-msg">⏳ Đang tải...</div>
    <div v-if="error" class="error-msg">⛔ {{ error }}</div>

    <!-- Table -->
    <div v-if="filtered.length" class="table-card">
      <table>
        <thead>
          <tr>
            <th style="width:40px">#</th>
            <th>Học viên</th><th>Khóa học</th>
            <th style="text-align:center">📅 Ngày</th>
            <th style="text-align:center">📖 Buổi</th>
            <th style="text-align:center">Trạng thái</th>
            <th>📝 Ghi chú</th>
            <th style="text-align:center">Hành động</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(a, idx) in filtered" :key="a.id">
            <td class="mono dim">{{ idx + 1 }}</td>
            <td>
              <button class="name-link" @click="viewStudentDetail(a.studentName)">{{ a.studentName }}</button>
            </td>
            <td class="dim">{{ a.courseName }}</td>
            <td class="mono" style="text-align:center">{{ a.date }}</td>
            <td style="text-align:center"><span class="session-tag">{{ a.session }}</span></td>
            <td style="text-align:center">
              <span :class="['badge', statusColor(a.status)]">{{ statusIcon(a.status) }} {{ a.status }}</span>
            </td>
            <td class="dim italic">{{ a.note || '—' }}</td>
            <td style="text-align:center">
              <div class="actions">
                <button class="btn-sm btn-edit" @click="openEdit(a)" title="Sửa chi tiết">✏️ Sửa</button>
                <button v-if="a.status!=='Có mặt'" class="btn-sm btn-green-sm" @click="quickStatus(a,'Có mặt')" title="Đánh dấu có mặt">✅</button>
                <button v-if="a.status!=='Muộn'" class="btn-sm btn-warn-sm" @click="quickStatus(a,'Muộn')" title="Đánh dấu muộn">⏰</button>
                <button v-if="a.status!=='Vắng'" class="btn-sm btn-del" @click="quickStatus(a,'Vắng')" title="Đánh dấu vắng">❌</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <div v-else-if="!loading" class="empty">Không có dữ liệu phù hợp bộ lọc</div>

    <!-- ADD/EDIT MODAL -->
    <Transition name="modal">
      <div v-if="showForm" class="modal-overlay" @click.self="showForm=false">
        <div class="modal-box" style="max-width:580px">
          <h3>{{ editId ? '✏️ Sửa bản ghi điểm danh' : '📋 Thêm điểm danh mới' }}</h3>
          <div class="form-grid">
            <div class="field">
              <label>Học viên *</label>
              <select v-model="form.studentId" :disabled="!!editId">
                <option :value="null" disabled>-- Chọn học viên --</option>
                <option v-for="s in students" :key="s.id" :value="s.id">{{ s.name }} — {{ s.course }}</option>
              </select>
            </div>
            <div class="field">
              <label>Khóa học</label>
              <select v-model="form.courseId">
                <option :value="null">-- Chọn --</option>
                <option v-for="c in courses" :key="c.id" :value="c.id">{{ c.name }}</option>
              </select>
            </div>
            <div class="field">
              <label>📅 Ngày điểm danh *</label>
              <input v-model="form.date" type="date" />
            </div>
            <div class="field">
              <label>📖 Buổi học *</label>
              <select v-model="form.session">
                <option v-for="s in sessionOptions" :key="s">{{ s }}</option>
              </select>
            </div>
            <div class="field full-width">
              <label>Trạng thái *</label>
              <div class="status-radio-group">
                <label class="status-radio" :class="{active:form.status==='Có mặt', green:form.status==='Có mặt'}">
                  <input type="radio" v-model="form.status" value="Có mặt" /> ✅ Có mặt
                </label>
                <label class="status-radio" :class="{active:form.status==='Muộn', orange:form.status==='Muộn'}">
                  <input type="radio" v-model="form.status" value="Muộn" /> ⏰ Muộn
                </label>
                <label class="status-radio" :class="{active:form.status==='Vắng', red:form.status==='Vắng'}">
                  <input type="radio" v-model="form.status" value="Vắng" /> ❌ Vắng
                </label>
              </div>
            </div>
            <div class="field full-width">
              <label>📝 Ghi chú</label>
              <input v-model="form.note" placeholder="Lý do vắng, muộn, nghỉ phép..." />
            </div>
          </div>
          <div class="modal-actions">
            <button class="btn-cancel" @click="showForm=false">Hủy</button>
            <button class="btn-primary" @click="save" :disabled="!form.studentId || !form.session || !form.date">
              {{ editId ? '💾 Cập nhật' : '➕ Thêm' }}
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- STUDENT DETAIL MODAL -->
    <Transition name="modal">
      <div v-if="showDetail && detailData" class="modal-overlay" @click.self="showDetail=false">
        <div class="modal-box" style="max-width:700px">
          <h3>👤 Thống kê điểm danh: {{ detailStudent }}</h3>

          <div class="detail-stats">
            <div class="ds"><div class="ds-num blue">{{ detailData.total }}</div><div class="ds-lbl">Tổng buổi</div></div>
            <div class="ds"><div class="ds-num green">{{ detailData.present }}</div><div class="ds-lbl">Có mặt</div></div>
            <div class="ds"><div class="ds-num orange">{{ detailData.late }}</div><div class="ds-lbl">Muộn</div></div>
            <div class="ds"><div class="ds-num red">{{ detailData.absent }}</div><div class="ds-lbl">Vắng</div></div>
            <div class="ds">
              <div class="ds-num" :class="rateColor(detailData.rate)">{{ detailData.rate }}%</div>
              <div class="ds-lbl">Chuyên cần</div>
              <div class="rate-bar sm"><div class="rate-fill" :class="rateColor(detailData.rate)" :style="{width:detailData.rate+'%'}"></div></div>
            </div>
          </div>

          <div class="detail-table" v-if="detailData.records.length">
            <table>
              <thead><tr><th>Ngày</th><th>Buổi</th><th>Khóa học</th><th>Trạng thái</th><th>Ghi chú</th></tr></thead>
              <tbody>
                <tr v-for="r in detailData.records" :key="r.id">
                  <td class="mono">{{ r.date }}</td>
                  <td><span class="session-tag">{{ r.session }}</span></td>
                  <td class="dim">{{ r.courseName }}</td>
                  <td><span :class="['badge', statusColor(r.status)]">{{ statusIcon(r.status) }} {{ r.status }}</span></td>
                  <td class="dim italic">{{ r.note || '—' }}</td>
                </tr>
              </tbody>
            </table>
          </div>

          <div class="modal-actions"><button class="btn-cancel" @click="showDetail=false">Đóng</button></div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
/* Filter panel */
.filter-panel {
  background:white; border-radius:12px; padding:18px 20px; margin-bottom:20px;
  border:1px solid #e8ecf0; box-shadow:0 1px 3px rgba(0,0,0,.04);
}
.filter-title { font-size:14px; font-weight:600; color:#1e293b; margin-bottom:12px; }
.filter-grid { display:grid; grid-template-columns:repeat(auto-fill,minmax(180px,1fr)); gap:12px; }
.filter-item label { display:block; font-size:11px; font-weight:600; color:#94a3b8; margin-bottom:4px; text-transform:uppercase; letter-spacing:.04em; }
.filter-result { margin-top:12px; font-size:12px; color:#64748b; }
.btn-clear { padding:8px 14px; border:1px solid #e2e8f0; border-radius:8px; background:white; color:#64748b; font-size:12px; cursor:pointer; transition:background .15s; width:100%; }
.btn-clear:hover { background:#f8fafc; }

/* Session tag */
.session-tag {
  display:inline-block; padding:3px 10px; border-radius:6px;
  background:#f0f4ff; color:#4f46e5; font-size:12px; font-weight:600;
  border:1px solid #e0e7ff;
}

/* Name link */
.name-link {
  background:none; border:none; color:#3b82f6; font-weight:600;
  cursor:pointer; font-size:13.5px; text-decoration:underline;
  text-underline-offset:2px; transition:color .15s;
}
.name-link:hover { color:#1d4ed8; }

/* Status radio */
.status-radio-group { display:flex; gap:8px; flex-wrap:wrap; }
.status-radio {
  display:flex; align-items:center; gap:6px; padding:8px 16px;
  border:2px solid #e2e8f0; border-radius:10px; cursor:pointer;
  font-size:13px; font-weight:500; transition:all .15s;
}
.status-radio input { display:none; }
.status-radio.active.green { border-color:#16a34a; background:#f0fdf4; color:#16a34a; }
.status-radio.active.orange { border-color:#ea580c; background:#fff7ed; color:#ea580c; }
.status-radio.active.red { border-color:#ef4444; background:#fef2f2; color:#ef4444; }

/* Rate bar */
.rate-bar { height:5px; background:#f0f0f0; border-radius:3px; margin-top:6px; overflow:hidden; }
.rate-bar.sm { margin-top:4px; }
.rate-fill { height:100%; border-radius:3px; transition:width .4s; }
.rate-fill.green { background:#16a34a; }
.rate-fill.orange { background:#ea580c; }
.rate-fill.red { background:#ef4444; }

/* Quick action buttons */
.btn-warn-sm { background:#fff7ed; color:#ea580c; }
.btn-warn-sm:hover { background:#ffedd5; }

/* Detail modal stats */
.detail-stats { display:grid; grid-template-columns:repeat(5,1fr); gap:10px; margin-bottom:18px; }
.ds { text-align:center; padding:12px 8px; background:#f8fafc; border-radius:10px; border:1px solid #f1f5f9; }
.ds-num { font-size:22px; font-weight:700; }
.ds-lbl { font-size:11px; color:#94a3b8; margin-top:2px; }

.detail-table { max-height:300px; overflow-y:auto; border-radius:8px; border:1px solid #e8ecf0; }
.detail-table table { font-size:13px; }

/* Form full-width field */
.full-width { grid-column:1/-1; }
</style>
