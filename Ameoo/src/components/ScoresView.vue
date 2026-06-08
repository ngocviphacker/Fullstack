<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'

const API = 'http://192.168.30.70:5210'
const scores = ref([])
const students = ref([])
const courses = ref([])
const finalResults = ref([])
const loading = ref(false)
const error = ref('')
const showForm = ref(false)
const editId = ref(null)
const search = ref('')
const activeTab = ref('scores')
const form = ref({ studentId: null, courseId: null, midterm: '', final: '' })

const filtered = computed(() => {
  const q = search.value.toLowerCase()
  if (!q) return scores.value
  return scores.value.filter(s =>
    s.studentName.toLowerCase().includes(q) || s.courseName.toLowerCase().includes(q))
})

const stats = computed(() => {
  const tested = scores.value.filter(s => s.result !== 'Chưa thi')
  const passed = tested.filter(s => s.result === 'Đạt' || s.result === 'Xuất sắc')
  const rate = tested.length > 0 ? Math.round(passed.length / tested.length * 100) : 0
  const topScore = scores.value.length ? Math.max(...scores.value.filter(s => s.final > 0).map(s => Number(s.final))) : 0
  return { total: scores.value.length, pass: passed.length, fail: tested.length - passed.length, rate, topScore }
})

const finalStats = computed(() => {
  const total = finalResults.value.length
  const pass = finalResults.value.filter(r => r.finalResult === 'Đạt' || r.finalResult === 'Xuất sắc').length
  const fail = finalResults.value.filter(r => r.finalResult?.includes('Không đạt')).length
  const notYet = finalResults.value.filter(r => r.finalResult === 'Chưa thi').length
  return { total, pass, fail, notYet }
})

onMounted(() => loadAll())

async function loadAll() {
  loading.value = true; error.value = ''
  try {
    const [scRes, stRes, coRes, frRes] = await Promise.all([
      axios.get(`${API}/api/scores`),
      axios.get(`${API}/api/students`),
      axios.get(`${API}/api/courses`),
      axios.get(`${API}/api/final-results`)
    ])
    scores.value = scRes.data; students.value = stRes.data
    courses.value = coRes.data; finalResults.value = frRes.data
  } catch (e) { error.value = 'Không kết nối được API: ' + e.message }
  finally { loading.value = false }
}

function openAdd() {
  editId.value = null
  form.value = { studentId: null, courseId: null, midterm: '', final: '' }
  showForm.value = true
}
function openEdit(sc) {
  editId.value = sc.id
  form.value = { studentId: sc.studentId, courseId: sc.courseId, midterm: sc.midterm, final: sc.final }
  showForm.value = true
}
async function save() {
  const s = students.value.find(x => x.id === form.value.studentId)
  const c = courses.value.find(x => x.id === form.value.courseId)
  const payload = {
    studentId: s?.id, studentName: s?.name || '',
    courseId: c?.id || 0, courseName: c?.name || '',
    midterm: parseFloat(form.value.midterm) || 0,
    final: parseFloat(form.value.final) || 0
  }
  try {
    if (editId.value) await axios.put(`${API}/api/scores/${editId.value}`, payload)
    else await axios.post(`${API}/api/scores`, payload)
    showForm.value = false; await loadAll()
  } catch (e) { error.value = 'Lỗi lưu: ' + e.message }
}

function gradeColor(g) {
  return g === 'A+' ? 'badge-green' : g === 'A' || g === 'B' ? 'badge-blue' : g === 'C' ? 'badge-orange' : g === 'F' ? 'badge-red' : 'badge-gray'
}
function resultColor(r) {
  if (!r) return 'badge-gray'
  return r === 'Xuất sắc' ? 'badge-green' : r === 'Đạt' ? 'badge-blue' : r.includes('Không đạt') ? 'badge-red' : 'badge-gray'
}
function lcIcon(r) {
  if (!r) return '⏳'
  if (r.includes('Xuất sắc')) return '🏆'
  if (r === 'Đạt') return '✅'
  if (r.includes('Không đạt')) return '❌'
  return '⏳'
}
</script>

<template>
  <div class="page">
    <div class="page-header">
      <div>
        <h1 class="page-title">📊 Điểm & Kết quả học tập</h1>
        <p class="page-sub">Nhập điểm giữa kỳ, cuối kỳ · Tính kết quả cuối khóa theo điểm + chuyên cần</p>
      </div>
      <button class="btn-primary" @click="openAdd">+ Nhập điểm mới</button>
    </div>

    <!-- Tabs -->
    <div class="tab-bar">
      <button :class="['tab-btn', {active: activeTab==='scores'}]" @click="activeTab='scores'">📝 Bảng điểm</button>
      <button :class="['tab-btn', {active: activeTab==='final'}]" @click="activeTab='final'">🏆 Kết quả cuối khóa</button>
    </div>

    <div v-if="loading" class="center-msg">⏳ Đang tải...</div>
    <div v-if="error" class="error-msg">⛔ {{ error }}</div>

    <!-- TAB: BẢNG ĐIỂM -->
    <div v-if="activeTab === 'scores'">
      <div class="stats-row">
        <div class="stat-card"><div class="stat-num blue">{{ stats.total }}</div><div class="stat-lbl">Tổng bài thi</div></div>
        <div class="stat-card"><div class="stat-num green">{{ stats.pass }}</div><div class="stat-lbl">Đạt</div></div>
        <div class="stat-card"><div class="stat-num red">{{ stats.fail }}</div><div class="stat-lbl">Không đạt</div></div>
        <div class="stat-card"><div class="stat-num" :class="stats.rate>=70?'green':'orange'">{{ stats.rate }}%</div><div class="stat-lbl">Tỷ lệ đạt</div></div>
        <div class="stat-card"><div class="stat-num purple">{{ stats.topScore }}</div><div class="stat-lbl">Điểm cao nhất</div></div>
      </div>
      <div class="toolbar">
        <div class="search-box"><span class="search-icon">🔍</span><input v-model="search" placeholder="Tìm theo tên, khóa học..." class="search-input" /></div>
        <span class="result-count">{{ filtered.length }} kết quả</span>
      </div>
      <div v-if="filtered.length" class="table-card">
        <table>
          <thead><tr><th>Học viên</th><th>Khóa học</th><th style="text-align:center">Giữa kỳ</th><th style="text-align:center">Cuối kỳ</th><th style="text-align:center">TB</th><th style="text-align:center">Xếp loại</th><th>Kết quả</th><th>Hành động</th></tr></thead>
          <tbody>
            <tr v-for="sc in filtered" :key="sc.id">
              <td class="fw">{{ sc.studentName }}</td><td>{{ sc.courseName }}</td>
              <td class="score-cell">{{ sc.midterm }}</td><td class="score-cell">{{ sc.final }}</td>
              <td class="score-cell fw">{{ sc.average || '—' }}</td>
              <td style="text-align:center"><span :class="['badge', gradeColor(sc.grade)]">{{ sc.grade }}</span></td>
              <td><span :class="['badge', resultColor(sc.result)]">{{ sc.result }}</span></td>
              <td><button class="btn-sm btn-edit" @click="openEdit(sc)">✏️ Sửa</button></td>
            </tr>
          </tbody>
        </table>
      </div>
      <div v-else-if="!loading" class="empty">Chưa có dữ liệu điểm</div>
    </div>

    <!-- TAB: KẾT QUẢ CUỐI KHÓA -->
    <div v-if="activeTab === 'final'">
      <div class="stats-row">
        <div class="stat-card"><div class="stat-num blue">{{ finalStats.total }}</div><div class="stat-lbl">Tổng học viên</div></div>
        <div class="stat-card"><div class="stat-num green">{{ finalStats.pass }}</div><div class="stat-lbl">🏆 Đạt</div></div>
        <div class="stat-card"><div class="stat-num red">{{ finalStats.fail }}</div><div class="stat-lbl">❌ Không đạt</div></div>
        <div class="stat-card"><div class="stat-num orange">{{ finalStats.notYet }}</div><div class="stat-lbl">⏳ Chưa thi</div></div>
      </div>
      <div class="final-note">
        📌 <strong>Quy tắc vòng đời:</strong> Chuyên cần ≥ 70% <strong>VÀ</strong> Điểm TB ≥ 5.5 → Đạt.
        Chuyên cần &lt; 70% → <strong>Không đạt dù điểm cao.</strong>
      </div>
      <div v-if="finalResults.length" class="table-card">
        <table>
          <thead><tr><th>Học viên</th><th>Khóa học</th><th style="text-align:center">Chuyên cần</th><th style="text-align:center">Đủ ĐK</th><th style="text-align:center">Điểm TB</th><th style="text-align:center">Xếp loại</th><th>Kết quả cuối khóa</th></tr></thead>
          <tbody>
            <tr v-for="r in finalResults" :key="r.studentId">
              <td class="fw">{{ r.studentName }}</td>
              <td class="dim">{{ r.course }}</td>
              <td style="text-align:center">
                <span :class="['badge', parseFloat(r.attendanceRate)>=70?'badge-green':'badge-red']">{{ r.attendanceRate }}</span>
              </td>
              <td style="text-align:center">{{ r.eligible ? '✅' : '❌' }}</td>
              <td class="score-cell">{{ r.average || '—' }}</td>
              <td style="text-align:center"><span :class="['badge', gradeColor(r.finalGrade)]">{{ r.finalGrade }}</span></td>
              <td><span :class="['badge', resultColor(r.finalResult)]">{{ lcIcon(r.finalResult) }} {{ r.finalResult }}</span></td>
            </tr>
          </tbody>
        </table>
      </div>
      <div v-else-if="!loading" class="empty">Chưa có dữ liệu</div>
    </div>

    <!-- Modal -->
    <Transition name="modal">
      <div v-if="showForm" class="modal-overlay" @click.self="showForm=false">
        <div class="modal-box">
          <h3>{{ editId ? '✏️ Sửa điểm' : '📝 Nhập điểm mới' }}</h3>
          <div class="form-grid">
            <div class="field"><label>Học viên *</label>
              <select v-model="form.studentId" :disabled="!!editId">
                <option :value="null" disabled>-- Chọn --</option>
                <option v-for="s in students" :key="s.id" :value="s.id">{{ s.name }}</option>
              </select>
            </div>
            <div class="field"><label>Khóa học</label>
              <select v-model="form.courseId" :disabled="!!editId">
                <option :value="null">-- Chọn --</option>
                <option v-for="c in courses" :key="c.id" :value="c.id">{{ c.name }}</option>
              </select>
            </div>
            <div class="field"><label>Điểm giữa kỳ * (0-10)</label><input v-model="form.midterm" type="number" min="0" max="10" step="0.5" placeholder="8.5" /></div>
            <div class="field"><label>Điểm cuối kỳ * (0-10)</label><input v-model="form.final" type="number" min="0" max="10" step="0.5" placeholder="9.0" /></div>
          </div>
          <div class="score-preview" v-if="form.midterm && form.final">
            TB dự kiến: <strong>{{ ((parseFloat(form.midterm) + parseFloat(form.final)*2)/3).toFixed(1) }}</strong>
          </div>
          <div class="modal-actions">
            <button class="btn-cancel" @click="showForm=false">Hủy</button>
            <button class="btn-primary" @click="save" :disabled="!form.studentId||form.midterm===''||form.final===''">{{ editId ? 'Cập nhật' : 'Lưu' }}</button>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
.tab-bar { display:flex; gap:8px; margin-bottom:20px; border-bottom:2px solid #e8ecf0; }
.tab-btn { padding:10px 20px; border:none; background:none; font-size:14px; font-weight:500; color:#64748b; cursor:pointer; border-bottom:2px solid transparent; margin-bottom:-2px; transition:all .15s; }
.tab-btn.active { color:#3b82f6; border-bottom-color:#3b82f6; font-weight:600; }
.tab-btn:hover { color:#1e293b; }
.score-cell { text-align:center; font-family:monospace; font-weight:600; color:#3b82f6; font-size:15px; }
.score-preview { margin:12px 0 0; padding:10px 14px; background:#f0f7ff; border-radius:8px; font-size:14px; color:#3b82f6; border:1px solid #dbeafe; }
.final-note { background:#fffbeb; border:1px solid #fde68a; border-radius:10px; padding:12px 16px; font-size:13px; color:#92400e; margin-bottom:16px; }
</style>
