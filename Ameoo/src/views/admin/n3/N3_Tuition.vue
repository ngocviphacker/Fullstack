<script setup>
import { ref, computed, onMounted } from 'vue'
import { localDB, N3_API_URL } from '../../../data/mockN3.js'

// ── STATE ──────────────────────────────────────────────────────
const activeTab = ref('invoices') // invoices | payment | receipts | reports | config
const backendUrl = ref(localStorage.getItem('n3_api_url') || N3_API_URL)
const isOnline = ref(false)
const connStatus = ref('offline') // offline | checking | connected | failed
const toastMsg = ref('')
const toastType = ref('success')
const loading = ref(false)

// Modal flags
const showInvoiceModal = ref(false)
const showReceiptModal = ref(false)
const selectedInvoice = ref(null)
const selectedReceipt = ref(null)

// Filters
const searchQ = ref('')
const statusFilter = ref('all')
const courseFilter = ref('all')

// Forms
const invoiceForm = ref({
  studentCode: '', studentName: '',
  courseName: 'Lập trình Web Fullstack',
  className: 'K18-INF01',
  amount: 6800000,
  dueDate: new Date(Date.now() + 14*24*60*60*1000).toISOString().split('T')[0],
  description: 'Học phí học kỳ 1'
})
const paymentForm = ref({
  amountPaid: 0, paymentMethod: 'BankTransfer',
  transactionReference: '', notes: 'Đóng học phí',
  paymentDate: new Date().toISOString().split('T')[0]
})

// Offline Local DB
const db = ref({ ...localDB, invoices: [...localDB.invoices], payments: [...localDB.payments] })

// API data
const apiInvoices = ref([])
const apiPayments = ref([])
const apiStats = ref(null)

// ── HELPERS ────────────────────────────────────────────────────
const fmt = (n) => {
  if (n == null) return '0 đ'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(n)
}
const fmtDate = (s) => {
  if (!s) return '—'
  const d = new Date(s)
  if (isNaN(d)) return s
  return d.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
}
const saveDB = () => localStorage.setItem('n3_db_v1', JSON.stringify(db.value))
const loadDB = () => {
  const raw = localStorage.getItem('n3_db_v1')
  if (raw) db.value = JSON.parse(raw)
}
const showToast = (msg, type = 'success') => {
  toastMsg.value = msg; toastType.value = type
  setTimeout(() => { toastMsg.value = '' }, 4000)
}

// ── ONLINE / OFFLINE DATA ──────────────────────────────────────
const currentInvoices = computed(() => isOnline.value ? apiInvoices.value : db.value.invoices)
const currentPayments = computed(() => isOnline.value ? apiPayments.value : db.value.payments)

const stats = computed(() => {
  if (isOnline.value && apiStats.value) return apiStats.value
  const inv = currentInvoices.value || []
  const totalOwed = inv.reduce((s, i) => s + i.amount, 0)
  const totalPaid = inv.reduce((s, i) => s + i.paidAmount, 0)
  const totalDebt = inv.reduce((s, i) => s + i.debtAmount, 0)
  return {
    totalOwed, totalPaid, totalDebt,
    collectionRate: totalOwed > 0 ? Math.round(totalPaid / totalOwed * 100) : 0,
    invoiceCount: inv.length,
    paidCount: inv.filter(i => i.status === 'Paid').length,
    pendingCount: inv.filter(i => i.status !== 'Paid').length,
  }
})

const filteredInvoices = computed(() => {
  let list = [...(currentInvoices.value || [])]
  if (searchQ.value) {
    const q = searchQ.value.toLowerCase()
    list = list.filter(i =>
      i.studentCode.toLowerCase().includes(q) ||
      i.studentName.toLowerCase().includes(q) ||
      i.courseName.toLowerCase().includes(q)
    )
  }
  if (statusFilter.value !== 'all') {
    if (statusFilter.value === 'Overdue') {
      const today = new Date().toISOString().split('T')[0]
      list = list.filter(i => i.status !== 'Paid' && i.dueDate < today)
    } else {
      list = list.filter(i => i.status === statusFilter.value)
    }
  }
  if (courseFilter.value !== 'all') {
    list = list.filter(i => i.courseName === courseFilter.value)
  }
  return list
})

const unpaidInvoices = computed(() =>
  (currentInvoices.value || []).filter(i => i.debtAmount > 0)
)

// ── DEBTS SUMMARY (Công nợ tổng hợp theo học viên) ────────────
const debtsByStudent = computed(() => {
  const map = {}
  ;(currentInvoices.value || []).forEach(inv => {
    if (!map[inv.studentCode]) {
      const profile = db.value.students.find(s => s.studentCode === inv.studentCode)
      map[inv.studentCode] = {
        studentCode: inv.studentCode, studentName: inv.studentName,
        courseName: inv.courseName, className: inv.className,
        totalOwed: 0, totalPaid: 0, totalDebt: 0,
        hasOverdue: false, overdueDays: 0, dueDate: inv.dueDate,
        phone: profile?.phone || '—', email: profile?.email || '—',
      }
    }
    const s = map[inv.studentCode]
    s.totalOwed += inv.amount; s.totalPaid += inv.paidAmount; s.totalDebt += inv.debtAmount
    const today = new Date(); today.setHours(0,0,0,0)
    const due = new Date(inv.dueDate); due.setHours(0,0,0,0)
    if (inv.status !== 'Paid' && due < today) {
      s.hasOverdue = true
      const days = Math.ceil((today - due) / 86400000)
      if (days > s.overdueDays) s.overdueDays = days
    }
  })
  return Object.values(map)
})

// ── REVENUE CHART (Biểu đồ doanh thu) ─────────────────────────
const monthlyChart = computed(() => {
  const months = ['T1','T2','T3','T4','T5','T6']
  // demo data
  const rows = [
    { name:'T1', rev:85000000, exp:29750000, profit:55250000 },
    { name:'T2', rev:92000000, exp:32200000, profit:59800000 },
    { name:'T3', rev:110000000, exp:38500000, profit:71500000 },
    { name:'T4', rev:105000000, exp:36750000, profit:68250000 },
    { name:'T5', rev:128500000, exp:44975000, profit:83525000 },
    { name:'T6', rev:140000000, exp:49000000, profit:91000000 },
  ]
  const maxRev = Math.max(...rows.map(r => r.rev))
  return rows.map(r => ({
    ...r, revPct: r.rev/maxRev*100, expPct: r.exp/maxRev*100, profPct: r.profit/maxRev*100
  }))
})

const paymentMethodChart = computed(() => {
  const pays = currentPayments.value || []
  const total = pays.reduce((s,p) => s + p.amountPaid, 0)
  const groups = {
    BankTransfer:{ name:'🌐 Chuyển khoản', amount:0, color:'bg-indigo-500' },
    Cash:        { name:'💵 Tiền mặt',     amount:0, color:'bg-emerald-500' },
    CreditCard:  { name:'💳 Thẻ tín dụng', amount:0, color:'bg-violet-500' },
    EWallet:     { name:'📱 Ví điện tử',   amount:0, color:'bg-amber-500' },
  }
  pays.forEach(p => { if (groups[p.paymentMethod]) groups[p.paymentMethod].amount += p.amountPaid })
  return Object.values(groups).map(g => ({
    ...g, pct: total > 0 ? Math.round(g.amount/total*100) : (g.name.includes('Chuyển') ? 65 : g.name.includes('Tiền') ? 20 : g.name.includes('Thẻ') ? 10 : 5)
  })).sort((a,b) => b.pct - a.pct)
})

// ── ACTIONS ────────────────────────────────────────────────────
const checkConnection = async () => {
  connStatus.value = 'checking'
  localStorage.setItem('n3_api_url', backendUrl.value)
  try {
    const res = await fetch(`${backendUrl.value}/api/get-status`, { signal: AbortSignal.timeout(8000) })
    if (!res.ok) throw new Error('not ok')
    connStatus.value = 'connected'; isOnline.value = true
    showToast('Kết nối C# Backend thành công!'); fetchApiData()
  } catch {
    try {
      await fetch(`${backendUrl.value}/swagger`, { signal: AbortSignal.timeout(5000) })
      connStatus.value = 'connected'; isOnline.value = true
      showToast('Kết nối C# Backend thành công!'); fetchApiData()
    } catch {
      connStatus.value = 'failed'; isOnline.value = false
      showToast('Không thể kết nối Backend. Đang dùng chế độ Offline.', 'warning')
    }
  }
}

const fetchApiData = async () => {
  if (!isOnline.value) { loadDB(); return }
  loading.value = true
  try {
    const [invRes, payRes, dashRes] = await Promise.all([
      fetch(`${backendUrl.value}/api/payment/invoices`).then(r => r.json()),
      fetch(`${backendUrl.value}/api/payment/payments`).then(r => r.json()),
      fetch(`${backendUrl.value}/api/reports/dashboard`).then(r => r.json()),
    ])
    apiInvoices.value = invRes; apiPayments.value = payRes; apiStats.value = dashRes
    showToast('Đồng bộ dữ liệu từ C# API thành công!')
  } catch (err) {
    showToast('Lỗi khi lấy dữ liệu từ API. Dùng offline.', 'warning')
    isOnline.value = false; connStatus.value = 'failed'; loadDB()
  } finally { loading.value = false }
}

const handleCreateInvoice = async () => {
  if (!invoiceForm.value.studentCode || !invoiceForm.value.studentName || !invoiceForm.value.amount) {
    showToast('Vui lòng nhập đủ Mã SV, Họ tên và Số tiền!', 'warning'); return
  }
  const payload = { ...invoiceForm.value, amount: parseFloat(invoiceForm.value.amount) }
  if (isOnline.value) {
    try {
      await fetch(`${backendUrl.value}/api/payment/invoices`, {
        method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload)
      })
      showToast('Đã tạo hóa đơn trên C# API!'); fetchApiData()
    } catch { showToast('Lỗi tạo hóa đơn!', 'error') }
  } else {
    const id = Math.max(0, ...db.value.invoices.map(i => i.id)) + 1
    const invoice = {
      id, ...payload, invoiceNumber: `INV-2026-${String(id).padStart(4,'0')}`,
      paidAmount: 0, debtAmount: payload.amount, status: 'Unpaid', createdAt: new Date().toISOString()
    }
    db.value.invoices.unshift(invoice); saveDB()
    showToast('Đã lập hóa đơn mới (Offline)!')
  }
  showInvoiceModal.value = false
  invoiceForm.value = { studentCode:'', studentName:'', courseName:'Lập trình Web Fullstack', className:'K18-INF01', amount:6800000, dueDate: new Date(Date.now()+14*86400000).toISOString().split('T')[0], description:'Học phí học kỳ 1' }
}

const selectInvoice = (inv) => {
  selectedInvoice.value = inv
  paymentForm.value.amountPaid = inv.debtAmount
  paymentForm.value.notes = `Thanh toán học phí - ${inv.studentName}`
  paymentForm.value.transactionReference = ''
  paymentForm.value.paymentDate = new Date().toISOString().split('T')[0]
}

const handleRecordPayment = async () => {
  if (!selectedInvoice.value) { showToast('Chọn hóa đơn bên trái!', 'warning'); return }
  if (paymentForm.value.amountPaid <= 0) { showToast('Số tiền phải > 0!', 'warning'); return }
  if (paymentForm.value.amountPaid > selectedInvoice.value.debtAmount) { showToast('Số tiền vượt quá công nợ!', 'warning'); return }

  const payload = {
    invoiceId: selectedInvoice.value.id,
    amount: parseFloat(paymentForm.value.amountPaid),
    paymentMethod: paymentForm.value.paymentMethod,
    transactionReference: paymentForm.value.transactionReference,
    notes: paymentForm.value.notes,
    paymentDate: new Date(paymentForm.value.paymentDate).toISOString()
  }
  if (isOnline.value) {
    try {
      await fetch(`${backendUrl.value}/api/payment/payments`, {
        method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload)
      })
      showToast('Đã ghi nhận thanh toán lên C# API!'); selectedInvoice.value = null; fetchApiData()
    } catch { showToast('Lỗi ghi nhận thanh toán!', 'error') }
  } else {
    const idx = db.value.invoices.findIndex(i => i.id === selectedInvoice.value.id)
    if (idx !== -1) {
      const inv = db.value.invoices[idx]
      inv.paidAmount += payload.amount; inv.debtAmount = inv.amount - inv.paidAmount
      inv.status = inv.debtAmount <= 0 ? 'Paid' : 'PartiallyPaid'
      const rid = Math.max(0, ...db.value.payments.map(p => p.id)) + 1
      db.value.payments.unshift({
        id: rid, invoiceId: selectedInvoice.value.id, receiptNumber: `REC-2026-${String(rid).padStart(4,'0')}`,
        amountPaid: payload.amount, paymentDate: payload.paymentDate,
        paymentMethod: payload.paymentMethod, transactionReference: payload.transactionReference,
        notes: payload.notes, createdBy: 'Thủ quỹ - Nhóm 3'
      })
      saveDB(); showToast(`Đã thu ${fmt(payload.amount)} thành công!`)
      selectedInvoice.value = null
    }
  }
}

const openReceiptModal = (receipt) => {
  selectedReceipt.value = receipt
  selectedInvoice.value = (currentInvoices.value || []).find(i => i.id === receipt.invoiceId)
  showReceiptModal.value = true
}

const handleToggleConnection = () => {
  if (connStatus.value === 'connected') {
    connStatus.value = 'offline'; isOnline.value = false
    showToast('Đã chuyển sang chế độ Offline.', 'warning'); loadDB()
  } else { checkConnection() }
}

const seedReset = () => {
  localStorage.removeItem('n3_db_v1'); db.value = { ...localDB, invoices: [...localDB.invoices], payments: [...localDB.payments] }
  showToast('Đã khôi phục dữ liệu demo gốc!')
}

const sendReminder = (name, code) => showToast(`📧 Đã gửi email nhắc học phí đến ${name} (${code})!`)

onMounted(() => { loadDB(); checkConnection() })
</script>

<template>
<div class="min-h-screen bg-slate-50 flex font-sans text-slate-800">

  <!-- TOAST -->
  <transition name="toast-slide">
    <div v-if="toastMsg" class="fixed top-5 right-5 z-50 flex items-center gap-3 bg-white border border-slate-100 shadow-xl px-5 py-3.5 rounded-2xl">
      <span>{{ toastType==='success'?'🟢':toastType==='error'?'🔴':'🟡' }}</span>
      <span class="text-xs font-bold text-slate-800">{{ toastMsg }}</span>
    </div>
  </transition>

  <!-- SIDEBAR -->
  <aside class="w-64 bg-white border-r border-slate-100 flex flex-col shrink-0 min-h-screen sticky top-0">
    <!-- Brand -->
    <div class="h-16 flex items-center gap-3 px-5 border-b border-slate-50">
      <div class="w-9 h-9 rounded-xl bg-indigo-600 flex items-center justify-center text-white font-black text-lg">E</div>
      <div>
        <div class="font-black text-base text-slate-900 leading-tight">EduPay Pro</div>
        <div class="text-[9px] text-slate-400 font-bold uppercase tracking-widest">Nhóm 3 · Finance</div>
      </div>
    </div>

    <!-- Nav -->
    <nav class="flex-1 px-3 py-5 space-y-1 overflow-y-auto">
      <!-- Báo cáo -->
      <p class="px-3 text-[9px] font-black text-slate-400 uppercase tracking-widest mb-1.5 mt-3">Báo cáo</p>
      <button @click="activeTab='reports'" :class="['n3-nav-item', activeTab==='reports'?'n3-nav-active':'n3-nav-idle']">
        <span>📊</span> Doanh thu
      </button>

      <!-- Tài chính -->
      <p class="px-3 text-[9px] font-black text-slate-400 uppercase tracking-widest mb-1.5 mt-4">Tài chính học vụ</p>
      <button @click="activeTab='invoices'" :class="['n3-nav-item', activeTab==='invoices'?'n3-nav-active':'n3-nav-idle']">
        <span>🧾</span> Hóa đơn học phí
        <span class="ml-auto bg-indigo-100 text-indigo-700 text-[9px] font-black px-1.5 py-0.5 rounded-full">
          {{ (currentInvoices||[]).filter(i=>i.status!=='Paid').length }}
        </span>
      </button>
      <button @click="activeTab='payment'" :class="['n3-nav-item', activeTab==='payment'?'n3-nav-active':'n3-nav-idle']">
        <span>💳</span> Ghi nhận thanh toán
        <span class="ml-auto bg-rose-100 text-rose-600 text-[9px] font-black px-1.5 py-0.5 rounded-full">
          {{ unpaidInvoices.length }}
        </span>
      </button>
      <button @click="activeTab='debts'" :class="['n3-nav-item', activeTab==='debts'?'n3-nav-active':'n3-nav-idle']">
        <span>⚠️</span> Công nợ học viên
        <span class="ml-auto bg-rose-100 text-rose-700 text-[9px] font-black px-1.5 py-0.5 rounded-full">
          {{ debtsByStudent.filter(s=>s.totalDebt>0).length }}
        </span>
      </button>
      <button @click="activeTab='receipts'" :class="['n3-nav-item', activeTab==='receipts'?'n3-nav-active':'n3-nav-idle']">
        <span>🧾</span> Biên lai thanh toán
      </button>

      <!-- Hệ thống -->
      <p class="px-3 text-[9px] font-black text-slate-400 uppercase tracking-widest mb-1.5 mt-4">Hệ thống</p>
      <button @click="activeTab='config'" :class="['n3-nav-item', activeTab==='config'?'n3-nav-active':'n3-nav-idle']">
        <span>⚙️</span> Cấu hình API
      </button>
    </nav>

    <!-- Connection badge -->
    <div class="p-3 border-t border-slate-50">
      <button @click="handleToggleConnection" :class="['w-full text-[10px] font-black px-3 py-2 rounded-xl uppercase tracking-wider transition-all',
        connStatus==='connected' ? 'bg-emerald-50 text-emerald-700 hover:bg-emerald-100' :
        connStatus==='checking'  ? 'bg-blue-50 text-blue-700' : 'bg-amber-50 text-amber-700 hover:bg-amber-100']">
        {{ connStatus==='connected' ? '🟢 Online — C# API' : connStatus==='checking' ? '⏳ Đang kết nối...' : '🟡 Offline — Bấm để kết nối' }}
      </button>
    </div>
  </aside>

  <!-- MAIN -->
  <main class="flex-1 min-w-0 flex flex-col">
    <!-- Header -->
    <header class="h-14 bg-white border-b border-slate-100 flex items-center justify-between px-7 sticky top-0 z-20">
      <div class="flex items-center gap-2 text-sm font-semibold">
        <span class="text-slate-400">EduPay</span>
        <span class="text-slate-300">/</span>
        <span class="text-slate-900 font-black">{{
          activeTab==='reports'?'Báo cáo doanh thu':
          activeTab==='invoices'?'Hóa đơn học phí':
          activeTab==='payment'?'Ghi nhận thanh toán':
          activeTab==='debts'?'Công nợ học viên':
          activeTab==='receipts'?'Biên lai thanh toán':'Cấu hình API'
        }}</span>
      </div>
      <div class="flex items-center gap-3">
        <span v-if="loading" class="text-xs text-indigo-600 font-bold animate-pulse">⏳ Đang tải...</span>
        <div v-if="['invoices','payment','debts','receipts'].includes(activeTab)" class="relative">
          <span class="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-xs">🔍</span>
          <input v-model="searchQ" placeholder="Tìm tên, mã SV..." class="bg-slate-50 border-0 focus:ring-2 ring-indigo-100 rounded-full pl-8 pr-4 py-1.5 text-xs text-slate-800 placeholder-slate-400 font-semibold w-52 outline-none"/>
        </div>
      </div>
    </header>

    <!-- Content -->
    <div class="flex-1 p-7 overflow-y-auto">

      <!-- ─── SCREEN 1: HÓA ĐƠN ─────────────────────── -->
      <div v-if="activeTab==='invoices'" class="space-y-5 animate-fade">
        <div class="flex justify-between items-center">
          <div>
            <h2 class="text-xl font-black text-slate-900">Danh sách hóa đơn học phí</h2>
            <p class="text-xs text-slate-400 mt-0.5">Quản lý và phát hành thông báo học phí tới từng học viên</p>
          </div>
          <button @click="showInvoiceModal=true" class="px-4 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs font-black shadow-md shadow-indigo-500/20 transition-all flex items-center gap-1.5">
            ➕ Lập hóa đơn mới
          </button>
        </div>

        <!-- Filter bar -->
        <div class="bg-white p-4 rounded-2xl border border-slate-100 shadow-sm flex flex-wrap gap-3 items-center">
          <div class="flex flex-col">
            <span class="text-[10px] font-black text-slate-400 uppercase mb-1">Trạng thái</span>
            <select v-model="statusFilter" class="bg-slate-50 rounded-xl px-3 py-1.5 text-xs font-bold text-slate-700 border-0 outline-none">
              <option value="all">Tất cả</option>
              <option value="Paid">Đã đóng đủ</option>
              <option value="PartiallyPaid">Nợ một phần</option>
              <option value="Unpaid">Chưa đóng</option>
              <option value="Overdue">Quá hạn ⚠️</option>
            </select>
          </div>
          <div class="flex flex-col">
            <span class="text-[10px] font-black text-slate-400 uppercase mb-1">Khóa học</span>
            <select v-model="courseFilter" class="bg-slate-50 rounded-xl px-3 py-1.5 text-xs font-bold text-slate-700 border-0 outline-none">
              <option value="all">Tất cả khóa</option>
              <option v-for="c in db.courses" :key="c.id" :value="c.name">{{ c.name }}</option>
            </select>
          </div>
          <button @click="statusFilter='all';courseFilter='all';searchQ=''" class="ml-auto px-3 py-1.5 bg-slate-50 hover:bg-slate-100 text-slate-600 rounded-xl text-xs font-bold transition-all">
            Xóa bộ lọc
          </button>
        </div>

        <!-- Table -->
        <div class="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
          <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse">
              <thead>
                <tr class="border-b border-slate-50 text-[10px] font-black text-slate-400 uppercase tracking-wider">
                  <th class="py-3 pl-5">Mã hóa đơn</th>
                  <th class="py-3">Học viên</th>
                  <th class="py-3">Khóa học / Lớp</th>
                  <th class="py-3">Hạn đóng</th>
                  <th class="py-3">Học phí</th>
                  <th class="py-3">Đã đóng</th>
                  <th class="py-3">Còn nợ</th>
                  <th class="py-3">Trạng thái</th>
                  <th class="py-3 pr-5 text-right">Thao tác</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-50 text-xs">
                <tr v-for="inv in filteredInvoices" :key="inv.id" class="hover:bg-slate-50/50 transition-colors">
                  <td class="py-3.5 pl-5 font-mono font-bold text-slate-700">{{ inv.invoiceNumber || `INV-2026-${String(inv.id).padStart(4,'0')}` }}</td>
                  <td class="py-3.5">
                    <div class="font-bold text-slate-900">{{ inv.studentName }}</div>
                    <div class="text-[10px] text-slate-400 font-mono">{{ inv.studentCode }}</div>
                  </td>
                  <td class="py-3.5">
                    <div class="font-semibold text-slate-700">{{ inv.courseName }}</div>
                    <div class="text-[10px] text-slate-400">{{ inv.className }}</div>
                  </td>
                  <td class="py-3.5 font-semibold text-slate-500">{{ fmtDate(inv.dueDate) }}</td>
                  <td class="py-3.5 font-bold text-slate-900">{{ fmt(inv.amount) }}</td>
                  <td class="py-3.5 font-semibold text-emerald-600">{{ fmt(inv.paidAmount) }}</td>
                  <td class="py-3.5 font-bold text-rose-500">{{ fmt(inv.debtAmount) }}</td>
                  <td class="py-3.5">
                    <span v-if="inv.status==='Paid'" class="bg-emerald-50 text-emerald-700 text-[9px] font-black px-2 py-0.5 rounded-full uppercase">Đã đóng đủ</span>
                    <span v-else-if="inv.status==='PartiallyPaid'" class="bg-amber-50 text-amber-700 text-[9px] font-black px-2 py-0.5 rounded-full uppercase">Nợ một phần</span>
                    <span v-else-if="new Date().toISOString().split('T')[0] > inv.dueDate" class="bg-rose-500 text-white text-[9px] font-black px-2 py-0.5 rounded-full uppercase animate-pulse">Quá hạn ⚠️</span>
                    <span v-else class="bg-rose-50 text-rose-600 text-[9px] font-black px-2 py-0.5 rounded-full uppercase">Chưa đóng</span>
                  </td>
                  <td class="py-3.5 pr-5 text-right">
                    <button v-if="inv.debtAmount > 0" @click="activeTab='payment';selectInvoice(inv)" class="px-2.5 py-1 bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg text-[10px] font-black shadow-sm transition-all">
                      Thu tiền
                    </button>
                  </td>
                </tr>
                <tr v-if="filteredInvoices.length===0">
                  <td colspan="9" class="text-center py-12 text-slate-400 font-bold">Không có hóa đơn phù hợp!</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- ─── SCREEN 2: THANH TOÁN ──────────────────── -->
      <div v-if="activeTab==='payment'" class="space-y-5 animate-fade">
        <div>
          <h2 class="text-xl font-black text-slate-900">Ghi nhận giao dịch đóng học phí</h2>
          <p class="text-xs text-slate-400 mt-0.5">Xử lý nộp tiền và tự động ghi giảm công nợ học viên</p>
        </div>
        <div class="grid grid-cols-12 gap-6 items-start">
          <!-- Left: Unpaid list -->
          <div class="bg-white rounded-2xl border border-slate-100 shadow-sm col-span-5 p-5 space-y-3">
            <div>
              <h3 class="font-black text-slate-900 text-sm">Chọn hóa đơn cần nộp tiền</h3>
              <p class="text-[10px] text-slate-400">Học viên còn công nợ trong hệ thống</p>
            </div>
            <div class="space-y-2 max-h-96 overflow-y-auto pr-1">
              <div v-for="inv in unpaidInvoices" :key="inv.id"
                @click="selectInvoice(inv)"
                :class="['p-3.5 rounded-xl border transition-all cursor-pointer', selectedInvoice?.id===inv.id ? 'border-indigo-500 bg-indigo-50/30' : 'border-slate-100 hover:bg-slate-50/50']">
                <div class="flex justify-between items-start">
                  <div>
                    <p class="font-bold text-slate-900 text-xs">{{ inv.studentName }}</p>
                    <p class="text-[10px] text-slate-400 font-mono">{{ inv.studentCode }} | {{ inv.className }}</p>
                  </div>
                  <span class="bg-rose-50 text-rose-600 text-[9px] font-black px-2 py-0.5 rounded uppercase">Nợ: {{ fmt(inv.debtAmount) }}</span>
                </div>
                <div class="flex justify-between text-[10px] text-slate-500 border-t border-slate-50 pt-2 mt-2">
                  <span>{{ inv.courseName }}</span>
                  <span :class="new Date().toISOString().split('T')[0] > inv.dueDate ? 'text-rose-600 font-bold':''">Hạn: {{ fmtDate(inv.dueDate) }}</span>
                </div>
              </div>
              <div v-if="unpaidInvoices.length===0" class="py-10 text-center text-slate-400 font-bold text-xs">Không còn học viên nào nợ!</div>
            </div>
          </div>

          <!-- Right: Payment form -->
          <div class="bg-white rounded-2xl border border-slate-100 shadow-sm col-span-7 p-5 space-y-4">
            <div>
              <h3 class="font-black text-slate-900 text-sm">Chi tiết giao dịch thu tiền</h3>
              <p class="text-[10px] text-slate-400">Điền thông tin hình thức nộp tiền</p>
            </div>
            <div v-if="selectedInvoice" class="space-y-4">
              <!-- Summary -->
              <div class="bg-indigo-50/40 border border-indigo-100/50 rounded-xl p-4 flex justify-between items-center">
                <div class="space-y-0.5 text-xs">
                  <p class="text-slate-500">Học viên: <b class="text-slate-900">{{ selectedInvoice.studentName }}</b> ({{ selectedInvoice.studentCode }})</p>
                  <p class="text-slate-500">Khóa học: <b class="text-slate-900">{{ selectedInvoice.courseName }}</b> ({{ selectedInvoice.className }})</p>
                </div>
                <div class="text-right">
                  <p class="text-[10px] text-slate-400 uppercase font-bold">Còn nợ</p>
                  <p class="text-lg font-black text-rose-500">{{ fmt(selectedInvoice.debtAmount) }}</p>
                </div>
              </div>
              <!-- Amount -->
              <div class="flex flex-col gap-1.5">
                <label class="text-[10px] font-black text-slate-400 uppercase">Số tiền đóng (VNĐ) *</label>
                <input type="number" v-model="paymentForm.amountPaid" class="bg-slate-50 rounded-xl px-4 py-2.5 text-xs font-bold text-slate-800 border-0 outline-none focus:ring-2 ring-indigo-100"/>
              </div>
              <!-- Method -->
              <div class="flex flex-col gap-2">
                <label class="text-[10px] font-black text-slate-400 uppercase">Phương thức thanh toán</label>
                <div class="grid grid-cols-4 gap-2">
                  <button v-for="m in ['BankTransfer','Cash','CreditCard','EWallet']" :key="m"
                    @click="paymentForm.paymentMethod=m"
                    :class="['py-2 px-2 rounded-xl border text-[10px] font-bold transition-all text-center', paymentForm.paymentMethod===m ? 'border-indigo-500 bg-indigo-50 text-indigo-700':'border-slate-100 bg-slate-50 text-slate-600 hover:bg-slate-100']">
                    {{ m==='BankTransfer'?'Chuyển khoản':m==='Cash'?'Tiền mặt':m==='CreditCard'?'Thẻ TD':'Ví điện tử' }}
                  </button>
                </div>
              </div>
              <!-- Ref + Date -->
              <div class="grid grid-cols-2 gap-3">
                <div class="flex flex-col gap-1.5">
                  <label class="text-[10px] font-black text-slate-400 uppercase">Mã tham chiếu</label>
                  <input v-model="paymentForm.transactionReference" placeholder="VD: TECHCOM-12345" class="bg-slate-50 rounded-xl px-4 py-2.5 text-xs font-bold border-0 outline-none focus:ring-2 ring-indigo-100"/>
                </div>
                <div class="flex flex-col gap-1.5">
                  <label class="text-[10px] font-black text-slate-400 uppercase">Ngày thanh toán</label>
                  <input type="date" v-model="paymentForm.paymentDate" class="bg-slate-50 rounded-xl px-4 py-2.5 text-xs font-bold border-0 outline-none focus:ring-2 ring-indigo-100"/>
                </div>
              </div>
              <!-- Notes -->
              <div class="flex flex-col gap-1.5">
                <label class="text-[10px] font-black text-slate-400 uppercase">Ghi chú</label>
                <input v-model="paymentForm.notes" class="bg-slate-50 rounded-xl px-4 py-2.5 text-xs font-bold border-0 outline-none focus:ring-2 ring-indigo-100"/>
              </div>
              <!-- Actions -->
              <div class="flex justify-end gap-2 pt-1">
                <button @click="selectedInvoice=null" class="px-4 py-2 bg-slate-50 hover:bg-slate-100 text-slate-600 rounded-xl text-xs font-bold">Hủy</button>
                <button @click="handleRecordPayment" class="px-5 py-2.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs font-black shadow-md shadow-indigo-500/20 transition-all">
                  Xác nhận thanh toán ✔️
                </button>
              </div>
            </div>
            <div v-else class="py-16 flex flex-col items-center justify-center text-center gap-3">
              <span class="text-4xl text-slate-200">💳</span>
              <p class="text-sm font-bold text-slate-400">Chọn một hóa đơn bên trái để bắt đầu</p>
            </div>
          </div>
        </div>

        <!-- Recent transactions -->
        <div class="bg-white rounded-2xl border border-slate-100 shadow-sm p-5">
          <h3 class="font-black text-slate-900 text-sm uppercase tracking-wide mb-4">Lịch sử giao dịch gần đây</h3>
          <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse text-xs">
              <thead>
                <tr class="border-b border-slate-50 text-[10px] font-black text-slate-400 uppercase">
                  <th class="pb-2.5 pl-3">Mã phiếu</th><th class="pb-2.5">Ngày nộp</th>
                  <th class="pb-2.5">Hình thức</th><th class="pb-2.5">Mã GD</th>
                  <th class="pb-2.5">Người thu</th><th class="pb-2.5 text-right pr-3">Số tiền</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-50">
                <tr v-for="r in currentPayments.slice(0,10)" :key="r.id" class="hover:bg-slate-50/30">
                  <td class="py-2.5 pl-3 font-mono font-bold text-slate-800">{{ r.receiptNumber }}</td>
                  <td class="py-2.5 text-slate-500">{{ fmtDate(r.paymentDate) }}</td>
                  <td class="py-2.5"><span class="bg-slate-50 text-slate-700 text-[9px] font-black px-2 py-0.5 rounded uppercase">{{ r.paymentMethod==='BankTransfer'?'Chuyển khoản':r.paymentMethod==='Cash'?'Tiền mặt':'Khác' }}</span></td>
                  <td class="py-2.5 font-mono font-bold text-slate-600">{{ r.transactionReference||'—' }}</td>
                  <td class="py-2.5 text-slate-700 font-semibold">{{ r.createdBy }}</td>
                  <td class="py-2.5 text-right pr-3 font-black text-emerald-600">+{{ fmt(r.amountPaid) }}</td>
                </tr>
                <tr v-if="!(currentPayments.length)">
                  <td colspan="6" class="text-center py-8 text-slate-400 font-bold">Chưa có giao dịch nào!</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- ─── SCREEN 3: CÔNG NỢ ─────────────────────── -->
      <div v-if="activeTab==='debts'" class="space-y-5 animate-fade">
        <div>
          <h2 class="text-xl font-black text-slate-900">Theo dõi công nợ học viên</h2>
          <p class="text-xs text-slate-400 mt-0.5">Thống kê dư nợ học phí và tự động nhắc đóng tiền</p>
        </div>
        <!-- KPI cards -->
        <div class="grid grid-cols-3 gap-5">
          <div class="bg-white rounded-2xl border border-slate-100 shadow-sm p-5 flex items-center justify-between">
            <div><p class="text-xs text-slate-400 font-bold uppercase">Học viên còn nợ</p>
              <p class="text-2xl font-black text-slate-900">{{ debtsByStudent.filter(s=>s.totalDebt>0).length }} <span class="text-sm font-bold text-slate-500">HV</span></p></div>
            <div class="w-10 h-10 bg-indigo-50 text-indigo-600 rounded-xl flex items-center justify-center">👥</div>
          </div>
          <div class="bg-white rounded-2xl border border-slate-100 shadow-sm p-5 flex items-center justify-between animate-pulse">
            <div><p class="text-xs text-rose-500 font-bold uppercase">Quá hạn</p>
              <p class="text-2xl font-black text-rose-600">{{ debtsByStudent.filter(s=>s.hasOverdue&&s.totalDebt>0).length }} <span class="text-sm font-bold text-rose-400">HV</span></p></div>
            <div class="w-10 h-10 bg-rose-50 text-rose-500 rounded-xl flex items-center justify-center">⚠️</div>
          </div>
          <div class="bg-white rounded-2xl border border-slate-100 shadow-sm p-5 flex items-center justify-between">
            <div><p class="text-xs text-amber-500 font-bold uppercase">Tổng dư nợ</p>
              <p class="text-2xl font-black text-amber-600">{{ (debtsByStudent.reduce((s,d)=>s+d.totalDebt,0)/1000000).toFixed(1) }}<span class="text-sm font-bold text-amber-400">M đ</span></p></div>
            <div class="w-10 h-10 bg-amber-50 text-amber-500 rounded-xl flex items-center justify-center">💰</div>
          </div>
        </div>

        <!-- Debt cards grid -->
        <div class="grid grid-cols-3 gap-5">
          <div v-for="debt in debtsByStudent.filter(s=>s.totalDebt>0)" :key="debt.studentCode"
            class="bg-white rounded-2xl border border-slate-100 shadow-sm p-5 flex flex-col gap-3 hover:shadow-md transition-shadow">
            <div class="flex justify-between items-start">
              <div>
                <h4 class="font-black text-slate-900 text-sm">{{ debt.studentName }}</h4>
                <p class="text-[10px] text-slate-400 font-mono">{{ debt.studentCode }} | {{ debt.className }}</p>
              </div>
              <span v-if="debt.hasOverdue" class="bg-rose-500 text-white text-[9px] font-black px-2 py-0.5 rounded uppercase animate-pulse">Quá hạn {{ debt.overdueDays }} ngày</span>
              <span v-else class="bg-amber-50 text-amber-700 text-[9px] font-black px-2 py-0.5 rounded uppercase">Hạn: {{ fmtDate(debt.dueDate) }}</span>
            </div>
            <p class="text-[11px] font-bold text-slate-500">{{ debt.courseName }}</p>
            <div class="space-y-1 text-xs">
              <div class="flex justify-between font-bold text-slate-700">
                <span>{{ fmt(debt.totalDebt) }} còn lại</span><span>/ {{ fmt(debt.totalOwed) }}</span>
              </div>
              <div class="w-full bg-slate-100 h-2 rounded-full overflow-hidden">
                <div class="bg-indigo-600 h-full rounded-full transition-all" :style="`width:${Math.round(debt.totalPaid/debt.totalOwed*100)}%`"></div>
              </div>
              <div class="flex justify-between text-[10px] text-slate-400">
                <span>Đã nộp: {{ Math.round(debt.totalPaid/debt.totalOwed*100) }}%</span>
                <span>{{ fmt(debt.totalPaid) }}</span>
              </div>
            </div>
            <div class="bg-slate-50 rounded-xl p-3 text-[10px] font-bold text-slate-500 space-y-0.5">
              <p>📞 {{ debt.phone }}</p><p class="truncate">📧 {{ debt.email }}</p>
            </div>
            <div class="grid grid-cols-2 gap-2">
              <button @click="sendReminder(debt.studentName, debt.studentCode)" class="py-1.5 bg-amber-50 hover:bg-amber-100 text-amber-700 rounded-xl text-[10px] font-bold border border-amber-100 transition-all text-center">✉️ Nhắc nhở</button>
              <button @click="activeTab='payment'; selectedInvoice=unpaidInvoices.find(i=>i.studentCode===debt.studentCode)||null" class="py-1.5 bg-indigo-50 hover:bg-indigo-100 text-indigo-700 rounded-xl text-[10px] font-bold border border-indigo-100 transition-all text-center">💳 Thu tiền</button>
            </div>
          </div>
          <div v-if="!debtsByStudent.filter(s=>s.totalDebt>0).length" class="col-span-3 text-center py-16 bg-white rounded-2xl border border-slate-100 text-slate-400 font-bold">
            🎉 Chúc mừng! Không còn học viên nào nợ học phí!
          </div>
        </div>
      </div>

      <!-- ─── SCREEN 4: BIÊN LAI ────────────────────── -->
      <div v-if="activeTab==='receipts'" class="space-y-5 animate-fade">
        <div>
          <h2 class="text-xl font-black text-slate-900">Biên lai thu học phí</h2>
          <p class="text-xs text-slate-400 mt-0.5">Toàn bộ biên lai thanh toán đã xuất ra cho học viên</p>
        </div>
        <div class="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
          <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse text-xs">
              <thead>
                <tr class="border-b border-slate-50 text-[10px] font-black text-slate-400 uppercase">
                  <th class="py-3 pl-5">Số biên lai</th><th class="py-3">Học viên</th>
                  <th class="py-3">Khóa / Lớp</th><th class="py-3">Ngày nộp</th>
                  <th class="py-3">Hình thức</th><th class="py-3">Mã GD</th>
                  <th class="py-3">Người thu</th><th class="py-3 text-right">Số tiền</th>
                  <th class="py-3 pr-5 text-right">In</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-50">
                <tr v-for="r in currentPayments" :key="r.id" class="hover:bg-slate-50/30">
                  <td class="py-3.5 pl-5 font-mono font-bold text-slate-900">{{ r.receiptNumber }}</td>
                  <td class="py-3.5">
                    <div class="font-bold text-slate-900">{{ (currentInvoices||[]).find(i=>i.id===r.invoiceId)?.studentName || '—' }}</div>
                    <div class="text-[10px] text-slate-400 font-mono">{{ (currentInvoices||[]).find(i=>i.id===r.invoiceId)?.studentCode || '' }}</div>
                  </td>
                  <td class="py-3.5">
                    <div class="font-semibold text-slate-700 text-[11px]">{{ (currentInvoices||[]).find(i=>i.id===r.invoiceId)?.courseName || '—' }}</div>
                    <div class="text-[10px] text-slate-400">{{ (currentInvoices||[]).find(i=>i.id===r.invoiceId)?.className || '' }}</div>
                  </td>
                  <td class="py-3.5 text-slate-500 font-semibold">{{ fmtDate(r.paymentDate) }}</td>
                  <td class="py-3.5"><span class="bg-slate-50 text-slate-700 text-[9px] font-black px-2 py-0.5 rounded uppercase">{{ r.paymentMethod==='BankTransfer'?'Chuyển khoản':r.paymentMethod==='Cash'?'Tiền mặt':'Khác' }}</span></td>
                  <td class="py-3.5 font-mono font-bold text-slate-600">{{ r.transactionReference||'—' }}</td>
                  <td class="py-3.5 font-semibold text-slate-700">{{ r.createdBy }}</td>
                  <td class="py-3.5 text-right font-black text-emerald-600">+{{ fmt(r.amountPaid) }}</td>
                  <td class="py-3.5 pr-5 text-right">
                    <button @click="openReceiptModal(r)" class="text-indigo-600 hover:text-indigo-800 text-[10px] font-black">🖨️ In</button>
                  </td>
                </tr>
                <tr v-if="!(currentPayments.length)">
                  <td colspan="9" class="text-center py-10 text-slate-400 font-bold">Chưa có biên lai nào!</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- ─── SCREEN 5: BÁO CÁO ─────────────────────── -->
      <div v-if="activeTab==='reports'" class="space-y-6 animate-fade">
        <div class="flex justify-between items-center">
          <div>
            <h2 class="text-xl font-black text-slate-900">Báo cáo doanh thu & Dòng tiền</h2>
            <p class="text-xs text-slate-400 mt-0.5">Tổng hợp tài chính và doanh số trong học kỳ</p>
          </div>
          <button @click="()=>window.print()" class="px-4 py-2 bg-white border border-slate-200 hover:bg-slate-50 rounded-xl text-xs font-bold text-slate-700 shadow-sm transition-all flex items-center gap-2">📊 Xuất PDF</button>
        </div>
        <!-- KPI 4 cột -->
        <div class="grid grid-cols-4 gap-5">
          <div v-for="kpi in [
            {label:'Tổng doanh thu', val:stats.totalOwed, color:'bg-indigo-50 text-indigo-600', icon:'💰', sub:'▲ +22% kỳ trước'},
            {label:'Đã thu được', val:stats.totalPaid, color:'bg-emerald-50 text-emerald-600', icon:'📈', sub:`${stats.collectionRate}% tỷ lệ thu`},
            {label:'Còn nợ', val:stats.totalDebt, color:'bg-rose-50 text-rose-600', icon:'⚠️', sub:`${stats.pendingCount} hóa đơn`},
            {label:'Tổng hóa đơn', val:null, color:'bg-violet-50 text-violet-600', icon:'🧾', sub:`${stats.paidCount} đã đóng đủ`},
          ]" :key="kpi.label"
            class="bg-white rounded-2xl border border-slate-100 shadow-sm p-5 flex items-center justify-between">
            <div class="space-y-1">
              <p class="text-[10px] text-slate-400 font-black uppercase tracking-wider">{{ kpi.label }}</p>
              <h3 class="text-2xl font-black text-slate-900">{{ kpi.val != null ? (kpi.val/1000000).toFixed(1)+'M' : stats.invoiceCount }}</h3>
              <p class="text-[10px] text-emerald-600 font-bold">{{ kpi.sub }}</p>
            </div>
            <div :class="['w-11 h-11 rounded-xl flex items-center justify-center text-xl', kpi.color]">{{ kpi.icon }}</div>
          </div>
        </div>

        <div class="grid grid-cols-12 gap-6">
          <!-- Bar chart -->
          <div class="bg-white rounded-2xl border border-slate-100 shadow-sm col-span-8 p-6">
            <h3 class="font-black text-slate-900 text-sm uppercase tracking-wide">Doanh thu – Chi phí – Lợi nhuận (triệu đ)</h3>
            <p class="text-xs text-slate-400 mt-1 mb-5">Số liệu tích lũy từ T1 đến T6 năm 2026</p>
            <div class="flex items-end justify-between gap-4 h-44 border-b border-slate-100 pb-2">
              <div v-for="bar in monthlyChart" :key="bar.name" class="flex-1 flex flex-col items-center gap-2 group">
                <div class="flex items-end gap-1 h-36 justify-center w-full">
                  <div class="w-2.5 bg-indigo-500 rounded-t transition-all" :style="{height:bar.revPct+'%'}" :title="`Doanh thu: ${fmt(bar.rev)}`"></div>
                  <div class="w-2.5 bg-slate-300 rounded-t transition-all" :style="{height:bar.expPct+'%'}" :title="`Chi phí: ${fmt(bar.exp)}`"></div>
                  <div class="w-2.5 bg-emerald-500 rounded-t transition-all" :style="{height:bar.profPct+'%'}" :title="`Lợi nhuận: ${fmt(bar.profit)}`"></div>
                </div>
                <span :class="['text-[10px] font-black', bar.name==='T5'?'text-indigo-600':'text-slate-400']">{{ bar.name }}</span>
              </div>
            </div>
            <div class="mt-3 flex items-center gap-5 text-xs text-slate-500">
              <span class="flex items-center gap-1.5"><span class="w-3 h-3 rounded-sm bg-indigo-500"></span>Doanh thu</span>
              <span class="flex items-center gap-1.5"><span class="w-3 h-3 rounded-sm bg-slate-300"></span>Chi phí</span>
              <span class="flex items-center gap-1.5"><span class="w-3 h-3 rounded-sm bg-emerald-500"></span>Lợi nhuận</span>
            </div>
          </div>

          <!-- Payment methods -->
          <div class="bg-white rounded-2xl border border-slate-100 shadow-sm col-span-4 p-6 flex flex-col">
            <h3 class="font-black text-slate-900 text-sm uppercase tracking-wide">Hình thức thanh toán</h3>
            <p class="text-xs text-slate-400 mt-1 mb-5">Phân bổ dòng tiền thu theo kênh</p>
            <div class="space-y-4 flex-1">
              <div v-for="m in paymentMethodChart" :key="m.name" class="space-y-1.5">
                <div class="flex justify-between text-xs font-bold text-slate-700">
                  <span>{{ m.name }}</span><span>{{ m.pct }}%</span>
                </div>
                <div class="w-full bg-slate-100 h-2 rounded-full overflow-hidden">
                  <div :class="[m.color,'h-full rounded-full transition-all']" :style="{width:m.pct+'%'}"></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- ─── SCREEN 6: CẤU HÌNH API ────────────────── -->
      <div v-if="activeTab==='config'" class="space-y-5 animate-fade">
        <div>
          <h2 class="text-xl font-black text-slate-900">Cấu hình kết nối API Nhóm 3</h2>
          <p class="text-xs text-slate-400 mt-0.5">Kết nối với C# Backend (Visual Studio Local hoặc VPS Cloud)</p>
        </div>
        <div class="grid grid-cols-2 gap-6">
          <div class="bg-white rounded-2xl border border-slate-100 shadow-sm p-6 space-y-4">
            <h3 class="font-black text-slate-900 text-sm uppercase">Địa chỉ Backend API</h3>
            <div class="flex flex-col gap-1.5">
              <label class="text-[10px] font-black text-slate-400 uppercase">URL API</label>
              <input v-model="backendUrl" class="bg-slate-50 rounded-xl px-4 py-2.5 text-xs font-mono font-bold border-0 outline-none focus:ring-2 ring-indigo-100"/>
            </div>
            <div class="grid grid-cols-2 gap-2">
              <button @click="backendUrl='http://localhost:5000';checkConnection()" class="py-2 px-3 bg-white hover:bg-slate-50 text-slate-600 rounded-xl text-[10px] font-bold border border-slate-200 text-center">💻 Local (5000)</button>
              <button @click="backendUrl='http://180.93.36.113:5000';checkConnection()" class="py-2 px-3 bg-white hover:bg-slate-50 text-slate-600 rounded-xl text-[10px] font-bold border border-slate-200 text-center">☁️ VPS Cloud</button>
            </div>
            <button @click="checkConnection()" class="w-full py-2.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs font-black shadow-md shadow-indigo-500/20 transition-all">
              ⚡ Ping & Kết nối
            </button>
            <!-- Status -->
            <div :class="['p-4 rounded-xl border flex items-center justify-between text-xs',
              connStatus==='connected' ? 'bg-emerald-50 border-emerald-100 text-emerald-700':'bg-rose-50 border-rose-100 text-rose-700']">
              <div>
                <p class="font-bold">{{ connStatus==='connected'?'ONLINE — Đã kết nối C# API':'OFFLINE — Dùng dữ liệu Local' }}</p>
                <p class="text-[10px] opacity-80 mt-0.5">{{ connStatus==='connected'?'Dữ liệu lưu vào SQL Server thật.':'Khởi động C# API để chuyển Online.' }}</p>
              </div>
              <span class="text-2xl">{{ connStatus==='connected'?'🟢':'🔴' }}</span>
            </div>
          </div>

          <div class="bg-white rounded-2xl border border-slate-100 shadow-sm p-6 space-y-4">
            <h3 class="font-black text-slate-900 text-sm uppercase">Khôi phục dữ liệu Demo</h3>
            <p class="text-xs text-slate-400 leading-relaxed">Nạp lại dữ liệu mẫu ban đầu nếu muốn chạy thử nghiệm độc lập không cần backend thật.</p>
            <button @click="seedReset()" class="w-full py-2.5 bg-rose-600 hover:bg-rose-700 text-white rounded-xl text-xs font-black shadow-md shadow-rose-500/20 transition-all">
              🔄 Khôi phục dữ liệu Demo
            </button>
            <div class="border-t border-slate-100 pt-4 text-[10px] text-slate-400 space-y-1 font-semibold">
              <p class="font-black uppercase tracking-wider text-slate-500 mb-2">Tài khoản thử nghiệm (Local):</p>
              <p>🔑 <b>admin / admin123</b> — Quản trị viên</p>
              <p>🔑 <b>maitt / admin123</b> — Kế toán trưởng</p>
              <p>🔑 <b>student / admin123</b> — Học viên (SV003)</p>
            </div>
            <div class="border-t border-slate-100 pt-4 text-[10px] text-slate-400 space-y-1 font-semibold">
              <p class="font-black uppercase tracking-wider text-slate-500 mb-2">Swagger API (Xem tài liệu):</p>
              <a :href="`${backendUrl}/swagger`" target="_blank" class="text-indigo-600 hover:underline font-black">👉 {{ backendUrl }}/swagger</a>
            </div>
          </div>
        </div>
      </div>

    </div>
  </main>

  <!-- ─── MODAL: LẬP HÓA ĐƠN ──────────────────────── -->
  <transition name="modal-fade">
    <div v-if="showInvoiceModal" class="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/60 backdrop-blur-sm p-4">
      <div class="bg-white rounded-3xl max-w-lg w-full p-8 shadow-2xl space-y-5">
        <div class="flex justify-between items-center">
          <div>
            <h3 class="font-black text-slate-900 text-lg">Lập hóa đơn học phí mới</h3>
            <p class="text-xs text-slate-400 mt-0.5">Tạo thông báo nộp tiền học phí mới cho học viên</p>
          </div>
          <button @click="showInvoiceModal=false" class="p-1.5 hover:bg-slate-100 rounded-lg text-slate-400 transition-colors">✕</button>
        </div>
        <div class="space-y-4">
          <div class="grid grid-cols-2 gap-4">
            <div class="flex flex-col gap-1.5">
              <label class="text-[10px] font-black text-slate-400 uppercase">Mã học viên *</label>
              <input v-model="invoiceForm.studentCode" placeholder="VD: SV006" class="bg-slate-50 rounded-xl px-4 py-2.5 text-xs font-bold border-0 outline-none focus:ring-2 ring-indigo-100"/>
            </div>
            <div class="flex flex-col gap-1.5">
              <label class="text-[10px] font-black text-slate-400 uppercase">Họ và tên *</label>
              <input v-model="invoiceForm.studentName" placeholder="Nguyễn Minh Tuấn" class="bg-slate-50 rounded-xl px-4 py-2.5 text-xs font-bold border-0 outline-none focus:ring-2 ring-indigo-100"/>
            </div>
          </div>
          <div class="grid grid-cols-2 gap-4">
            <div class="flex flex-col gap-1.5">
              <label class="text-[10px] font-black text-slate-400 uppercase">Khóa học</label>
              <select v-model="invoiceForm.courseName" class="bg-slate-50 rounded-xl px-4 py-2.5 text-xs font-bold border-0 outline-none focus:ring-2 ring-indigo-100">
                <option v-for="c in db.courses" :key="c.id" :value="c.name">{{ c.name }}</option>
              </select>
            </div>
            <div class="flex flex-col gap-1.5">
              <label class="text-[10px] font-black text-slate-400 uppercase">Lớp học</label>
              <input v-model="invoiceForm.className" placeholder="K18-INF01" class="bg-slate-50 rounded-xl px-4 py-2.5 text-xs font-bold border-0 outline-none focus:ring-2 ring-indigo-100"/>
            </div>
          </div>
          <div class="grid grid-cols-2 gap-4">
            <div class="flex flex-col gap-1.5">
              <label class="text-[10px] font-black text-slate-400 uppercase">Học phí (VNĐ) *</label>
              <input type="number" v-model="invoiceForm.amount" class="bg-slate-50 rounded-xl px-4 py-2.5 text-xs font-bold border-0 outline-none focus:ring-2 ring-indigo-100"/>
            </div>
            <div class="flex flex-col gap-1.5">
              <label class="text-[10px] font-black text-slate-400 uppercase">Hạn thanh toán</label>
              <input type="date" v-model="invoiceForm.dueDate" class="bg-slate-50 rounded-xl px-4 py-2.5 text-xs font-bold border-0 outline-none focus:ring-2 ring-indigo-100"/>
            </div>
          </div>
          <div class="flex flex-col gap-1.5">
            <label class="text-[10px] font-black text-slate-400 uppercase">Ghi chú</label>
            <textarea v-model="invoiceForm.description" rows="2" class="bg-slate-50 rounded-xl px-4 py-2.5 text-xs font-bold border-0 outline-none focus:ring-2 ring-indigo-100 resize-none"></textarea>
          </div>
        </div>
        <div class="flex gap-3 justify-end pt-2">
          <button @click="showInvoiceModal=false" class="px-4 py-2 bg-slate-50 hover:bg-slate-100 text-slate-600 rounded-xl text-xs font-bold">Hủy</button>
          <button @click="handleCreateInvoice" class="px-5 py-2.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs font-black shadow-md shadow-indigo-500/20 transition-all">Lập hóa đơn ✔️</button>
        </div>
      </div>
    </div>
  </transition>

  <!-- ─── MODAL: BIÊN LAI IN ────────────────────────── -->
  <transition name="modal-fade">
    <div v-if="showReceiptModal && selectedReceipt" class="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/60 backdrop-blur-sm p-4">
      <div class="bg-white rounded-3xl max-w-2xl w-full p-8 shadow-2xl space-y-5">
        <div class="flex justify-between items-start border-b border-slate-100 pb-4">
          <div><h4 class="text-sm font-black text-indigo-700 uppercase">EduCenter Manager</h4>
            <p class="text-[10px] text-slate-500">Trung tâm đào tạo Kỹ thuật & Công nghệ</p></div>
          <div class="text-right text-[10px] text-slate-400 font-bold">
            <p>Số: {{ selectedReceipt.receiptNumber }}</p>
            <p>Ngày: {{ fmtDate(selectedReceipt.paymentDate) }}</p>
          </div>
        </div>
        <div class="text-center py-1">
          <h2 class="text-xl font-black text-slate-900 tracking-wider">BIÊN LAI THU HỌC PHÍ</h2>
          <p class="text-[9px] text-slate-400 font-bold uppercase tracking-widest mt-1">Liên 1 · Lưu gốc & Trả khách hàng</p>
        </div>
        <div class="grid grid-cols-2 gap-y-3 gap-x-6 text-xs border-y border-slate-100 py-4">
          <div>Học viên: <b class="text-slate-900">{{ selectedInvoice?.studentName }}</b></div>
          <div>Mã SV: <b class="font-mono text-slate-900">{{ selectedInvoice?.studentCode }}</b></div>
          <div>Lớp: <b class="text-slate-900">{{ selectedInvoice?.className }}</b></div>
          <div>Khóa học: <b class="text-slate-900">{{ selectedInvoice?.courseName }}</b></div>
          <div>Hình thức: <b class="text-slate-900">{{ selectedReceipt.paymentMethod==='BankTransfer'?'Chuyển khoản':selectedReceipt.paymentMethod==='Cash'?'Tiền mặt':'Khác' }}</b></div>
          <div>Mã GD: <b class="font-mono text-slate-900">{{ selectedReceipt.transactionReference || 'Không có' }}</b></div>
        </div>
        <div class="bg-indigo-50/40 border border-indigo-100/40 rounded-2xl p-4 flex justify-between items-center">
          <span class="text-xs font-black text-slate-600 uppercase">Tổng số tiền đóng học phí:</span>
          <span class="text-xl font-black text-indigo-700">{{ fmt(selectedReceipt.amountPaid) }}</span>
        </div>
        <div class="grid grid-cols-2 text-center text-xs pt-2 font-semibold">
          <div class="space-y-10"><span>Người nộp tiền</span><div class="text-[10px] text-slate-400 italic">(Ký, ghi rõ họ tên)</div></div>
          <div class="space-y-10"><span>Người thu tiền</span><div><b class="text-slate-900">{{ selectedReceipt.createdBy }}</b><p class="text-[10px] text-slate-400 italic">Đã ký xác nhận</p></div></div>
        </div>
        <div class="flex gap-3 justify-end border-t border-slate-100 pt-4">
          <button @click="showReceiptModal=false" class="px-4 py-2 bg-slate-50 hover:bg-slate-100 text-slate-600 rounded-xl text-xs font-bold">Đóng</button>
          <button @click="()=>window.print()" class="px-5 py-2.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs font-black shadow-md transition-all flex items-center gap-1.5">🖨️ In biên lai</button>
        </div>
      </div>
    </div>
  </transition>

</div>
</template>

<style scoped>
.n3-nav-item { @apply w-full flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-bold transition-all text-left; }
.n3-nav-active { @apply bg-indigo-50 text-indigo-600; }
.n3-nav-idle { @apply text-slate-500 hover:bg-slate-50; }

.animate-fade { animation: fadeUp 0.35s ease; }
@keyframes fadeUp { from { opacity:0; transform:translateY(8px); } to { opacity:1; transform:translateY(0); } }

.toast-slide-enter-active, .toast-slide-leave-active { transition: all 0.35s cubic-bezier(0.175,0.885,0.32,1.275); }
.toast-slide-enter-from, .toast-slide-leave-to { transform:translateY(-30px); opacity:0; }

.modal-fade-enter-active, .modal-fade-leave-active { transition: all 0.25s ease; }
.modal-fade-enter-from, .modal-fade-leave-to { opacity:0; transform:scale(0.95); }
</style>
