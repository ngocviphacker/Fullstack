<script setup>
import { ref, computed } from 'vue'
import { rules, studyRules, graduationReqs } from '../../../data/mockN3.js'

const activeSection = ref('rules')
const sections = [
  { key: 'rules', label: 'Nội quy sinh hoạt', icon: '📋' },
  { key: 'study', label: 'Quy định học tập', icon: '📚' },
  { key: 'fees',  label: 'Chính sách học phí', icon: '💰' },
  { key: 'grad',  label: 'Điều kiện tốt nghiệp', icon: '🎓' },
]

// Student's own payment info (mock for current student SV003)
const myPayment = ref({
  studentCode: 'SV003', studentName: 'Lê Tuấn Anh',
  course: 'Lập trình Web Fullstack', className: 'K18-INF02',
  totalFee: 6800000, paid: 0, deadline: '2026-06-15',
  status: 'Unpaid',
})
const paymentPercent = computed(() => Math.round(myPayment.value.paid / myPayment.value.totalFee * 100))

const feeRules = [
  { title: 'Thời hạn đóng học phí', icon: '📅', text: 'Học phí phải được đóng đầy đủ trước ngày khai giảng hoặc trước ngày thi cuối kỳ. Thanh toán muộn sẽ phát sinh phí trễ hạn.' },
  { title: 'Chính sách trả góp',    icon: '💳', text: 'Hỗ trợ đóng học phí theo 2 đợt. Đợt 1 tối thiểu 50% tổng học phí, đợt 2 trước tuần 6 của kỳ học.' },
  { title: 'Miễn giảm học phí',     icon: '🎁', text: 'Học viên xuất sắc (điểm TB ≥ 8.5) được giảm 20%. Trường hợp đặc biệt (bệnh tật, hoàn cảnh khó khăn) báo trực tiếp bộ phận tài vụ.' },
  { title: 'Hoàn học phí',          icon: '↩️', text: 'Cho phép hoàn trả tối đa 70% học phí nếu rút trước tuần 2. Sau tuần 2 không hoàn trả.' },
  { title: 'Hình thức thanh toán',  icon: '💵', text: 'Hỗ trợ: Tiền mặt tại quầy thủ quỹ, Chuyển khoản ngân hàng, QR Pay. Không thanh toán trực tiếp cho giảng viên.' },
]
</script>

<template>
<div class="min-h-screen bg-gradient-to-br from-indigo-50 via-slate-50 to-violet-50 font-sans flex">

  <!-- Sidebar: Sections nav -->
  <aside class="w-64 bg-white border-r border-slate-100 flex flex-col shrink-0 min-h-screen sticky top-0">
    <div class="h-16 flex items-center gap-3 px-5 border-b border-slate-50">
      <div class="w-9 h-9 rounded-xl bg-indigo-600 flex items-center justify-center text-white font-black">📜</div>
      <div>
        <div class="font-black text-slate-900 text-sm leading-tight">Nội quy & Chính sách</div>
        <div class="text-[9px] text-slate-400 font-bold uppercase tracking-widest">EduCenter · Nhóm 3</div>
      </div>
    </div>

    <!-- Tiny student info card -->
    <div class="mx-3 mt-4 p-3.5 bg-indigo-50/70 border border-indigo-100/50 rounded-2xl">
      <div class="flex items-center gap-2.5">
        <div class="w-8 h-8 rounded-xl bg-indigo-600 text-white flex items-center justify-center font-black text-xs">{{ myPayment.studentName[0] }}</div>
        <div>
          <p class="text-xs font-black text-slate-900 leading-tight">{{ myPayment.studentName }}</p>
          <p class="text-[9px] text-slate-400 font-mono">{{ myPayment.studentCode }}</p>
        </div>
      </div>
      <div class="mt-3 space-y-1">
        <div class="flex justify-between text-[10px] font-bold text-slate-600">
          <span>Học phí</span>
          <span :class="myPayment.status==='Paid'?'text-emerald-600':'text-rose-600'">
            {{ myPayment.status==='Paid'?'Đã đóng đủ ✅':'Chưa đóng ⚠️' }}
          </span>
        </div>
        <div class="w-full bg-slate-200 h-1.5 rounded-full overflow-hidden">
          <div class="bg-indigo-600 h-full rounded-full transition-all" :style="{width:paymentPercent+'%'}"></div>
        </div>
        <div class="text-[9px] text-slate-400 text-right">{{ paymentPercent }}% đã đóng</div>
      </div>
    </div>

    <nav class="flex-1 p-3 py-5 space-y-1">
      <button v-for="s in sections" :key="s.key"
        @click="activeSection=s.key"
        :class="['w-full flex items-center gap-3 px-3 py-2.5 rounded-xl text-xs font-bold text-left transition-all',
          activeSection===s.key ? 'bg-indigo-50 text-indigo-700' : 'text-slate-500 hover:bg-slate-50']">
        <span>{{ s.icon }}</span> {{ s.label }}
      </button>
    </nav>

    <div class="p-4 border-t border-slate-50 text-[9px] text-slate-400 font-semibold leading-relaxed">
      ⚠️ Khi tham gia khóa học, học viên mặc nhiên đồng ý tuân thủ toàn bộ nội quy này.
    </div>
  </aside>

  <!-- Main Content -->
  <main class="flex-1 p-8 overflow-y-auto">

    <!-- ─ 1. NỘI QUY SINH HOẠT ─ -->
    <section v-if="activeSection==='rules'" class="space-y-6 max-w-4xl animate-fade">
      <div>
        <h1 class="text-2xl font-black text-slate-900">📋 Nội quy sinh hoạt của trung tâm</h1>
        <p class="text-sm text-slate-400 mt-1">Cập nhật lần cuối: 01/03/2026 — Ban hành bởi Ban Quản lý Đào tạo</p>
      </div>

      <!-- Important warning box -->
      <div class="bg-rose-50 border border-rose-200 text-rose-700 rounded-2xl p-4 flex gap-3">
        <span class="text-lg mt-0.5">⚠️</span>
        <div class="text-xs leading-relaxed">
          <p class="font-black text-sm mb-1">Lưu ý quan trọng</p>
          <p>Vi phạm nội quy 3 lần trở lên trong 1 học kỳ sẽ bị xem xét kỷ luật, tùy mức độ có thể đình chỉ học tập. Các quy định này có hiệu lực từ ngày học viên nhập học.</p>
        </div>
      </div>

      <div class="grid grid-cols-1 gap-5">
        <div v-for="rule in rules" :key="rule.num"
          class="bg-white rounded-2xl border border-slate-100 shadow-sm p-6 flex gap-5 hover:shadow-md transition-shadow">
          <div class="w-10 h-10 bg-indigo-600 rounded-xl text-white font-black text-sm flex items-center justify-center shrink-0 shadow-md shadow-indigo-500/20">
            {{ rule.num }}
          </div>
          <div class="flex flex-col gap-1">
            <h3 class="font-black text-slate-900">{{ rule.title }}</h3>
            <p class="text-sm text-slate-500 leading-relaxed">{{ rule.text }}</p>
          </div>
        </div>
      </div>
    </section>

    <!-- ─ 2. QUY ĐỊNH HỌC TẬP ─ -->
    <section v-if="activeSection==='study'" class="space-y-6 max-w-4xl animate-fade">
      <div>
        <h1 class="text-2xl font-black text-slate-900">📚 Quy định học tập & đánh giá</h1>
        <p class="text-sm text-slate-400 mt-1">Áp dụng cho tất cả khóa học tại trung tâm từ học kỳ 1/2026</p>
      </div>

      <div class="grid grid-cols-1 gap-5">
        <div v-for="rule in studyRules" :key="rule.num"
          class="bg-white rounded-2xl border border-slate-100 shadow-sm p-6 flex gap-5 hover:shadow-md transition-shadow">
          <div class="w-10 h-10 bg-violet-600 rounded-xl text-white font-black text-sm flex items-center justify-center shrink-0 shadow-md shadow-violet-500/20">
            {{ rule.num }}
          </div>
          <div>
            <p class="text-sm text-slate-600 leading-relaxed">{{ rule.text }}</p>
          </div>
        </div>
      </div>

      <!-- Grade table -->
      <div class="bg-white rounded-2xl border border-slate-100 shadow-sm p-6">
        <h3 class="font-black text-slate-900 mb-4 uppercase text-sm tracking-wide">Thang điểm đánh giá (10 điểm)</h3>
        <table class="w-full text-sm border-collapse">
          <thead>
            <tr class="text-xs font-black text-slate-400 uppercase border-b border-slate-100">
              <th class="pb-2 text-left">Xếp loại</th><th class="pb-2 text-left">Điểm số</th><th class="pb-2 text-left">Điểm chữ</th><th class="pb-2 text-left">Ghi chú</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-50 text-xs font-semibold">
            <tr class="hover:bg-slate-50"><td class="py-2.5 font-black text-emerald-600">Xuất sắc</td><td>9.0 – 10.0</td><td>A+</td><td class="text-slate-400">Học bổng 20% học kỳ sau</td></tr>
            <tr class="hover:bg-slate-50"><td class="py-2.5 font-black text-emerald-500">Giỏi</td><td>8.0 – 8.9</td><td>A</td><td class="text-slate-400">—</td></tr>
            <tr class="hover:bg-slate-50"><td class="py-2.5 font-black text-blue-600">Khá</td><td>7.0 – 7.9</td><td>B</td><td class="text-slate-400">—</td></tr>
            <tr class="hover:bg-slate-50"><td class="py-2.5 font-black text-amber-600">Trung bình</td><td>5.0 – 6.9</td><td>C</td><td class="text-slate-400">—</td></tr>
            <tr class="hover:bg-slate-50"><td class="py-2.5 font-black text-rose-500">Yếu</td><td>3.0 – 4.9</td><td>D</td><td class="text-slate-400">Cần học lại</td></tr>
            <tr class="hover:bg-slate-50"><td class="py-2.5 font-black text-rose-700">Kém</td><td>0.0 – 2.9</td><td>F</td><td class="text-slate-400">Bắt buộc học lại</td></tr>
          </tbody>
        </table>
      </div>
    </section>

    <!-- ─ 3. CHÍNH SÁCH HỌC PHÍ ─ -->
    <section v-if="activeSection==='fees'" class="space-y-6 max-w-4xl animate-fade">
      <div>
        <h1 class="text-2xl font-black text-slate-900">💰 Chính sách học phí & thanh toán</h1>
        <p class="text-sm text-slate-400 mt-1">Cập nhật 01/01/2026 — do Bộ phận Tài vụ Nhóm 3 ban hành</p>
      </div>

      <!-- Student payment status card -->
      <div :class="['rounded-2xl border p-6 flex justify-between items-center',
        myPayment.status==='Paid' ? 'bg-emerald-50 border-emerald-100' : 'bg-rose-50 border-rose-100']">
        <div>
          <p :class="['text-xs font-black uppercase', myPayment.status==='Paid'?'text-emerald-600':'text-rose-600']">Trạng thái học phí của bạn</p>
          <p :class="['text-xl font-black mt-1', myPayment.status==='Paid'?'text-emerald-700':'text-rose-700']">
            {{ myPayment.status==='Paid' ? '✅ Đã đóng đủ học phí!' : '⚠️ Chưa đóng học phí — Hạn: ' + myPayment.deadline }}
          </p>
          <p class="text-xs text-slate-500 mt-1">{{ myPayment.course }} | {{ myPayment.className }}</p>
        </div>
        <div class="text-right">
          <p class="text-xs text-slate-400 font-bold">Tổng học phí</p>
          <p class="text-2xl font-black text-slate-900">{{ new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(myPayment.totalFee) }}</p>
          <p :class="['text-xs font-bold mt-0.5', myPayment.status==='Paid'?'text-emerald-600':'text-rose-500']">
            {{ myPayment.status==='Paid'?'Đã nộp 100%':`Còn nợ: ${new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(myPayment.totalFee - myPayment.paid)}` }}
          </p>
        </div>
      </div>

      <!-- Fee policies -->
      <div class="grid grid-cols-1 gap-4">
        <div v-for="f in feeRules" :key="f.title" class="bg-white rounded-2xl border border-slate-100 shadow-sm p-5 flex gap-4 hover:shadow-md transition-shadow">
          <span class="text-2xl shrink-0">{{ f.icon }}</span>
          <div>
            <h3 class="font-black text-slate-900 mb-1">{{ f.title }}</h3>
            <p class="text-sm text-slate-500 leading-relaxed">{{ f.text }}</p>
          </div>
        </div>
      </div>
    </section>

    <!-- ─ 4. ĐIỀU KIỆN TỐT NGHIỆP ─ -->
    <section v-if="activeSection==='grad'" class="space-y-6 max-w-4xl animate-fade">
      <div>
        <h1 class="text-2xl font-black text-slate-900">🎓 Điều kiện hoàn thành khóa học</h1>
        <p class="text-sm text-slate-400 mt-1">Học viên cần đáp ứng đủ các tiêu chí sau để nhận Chứng chỉ hoàn thành khóa học</p>
      </div>

      <div class="grid grid-cols-1 gap-4">
        <div v-for="(req, i) in graduationReqs" :key="i"
          class="bg-white rounded-2xl border border-slate-100 shadow-sm p-5 flex items-center gap-4">
          <div :class="['w-7 h-7 rounded-full flex items-center justify-center text-sm font-black shrink-0', req.met?'bg-emerald-500 text-white':'bg-slate-100 text-slate-400']">
            {{ req.met ? '✓' : '○' }}
          </div>
          <p class="text-sm font-bold text-slate-800">{{ req.text }}</p>
          <span :class="['ml-auto text-[10px] font-black px-2.5 py-1 rounded-full', req.met?'bg-emerald-50 text-emerald-600':'bg-slate-50 text-slate-400']">
            {{ req.met ? 'Đạt ✓' : 'Chưa đạt' }}
          </span>
        </div>
      </div>

      <!-- Certificate preview card -->
      <div class="bg-gradient-to-br from-indigo-600 via-violet-600 to-indigo-700 rounded-3xl p-8 text-white shadow-xl shadow-indigo-500/30 relative overflow-hidden">
        <div class="absolute inset-0 opacity-10" style="background-image:radial-gradient(circle at 20% 20%, #fff 1px, transparent 1px),radial-gradient(circle at 80% 80%, #fff 1px, transparent 1px);background-size:40px 40px;"></div>
        <div class="relative text-center space-y-3">
          <span class="text-4xl">🏆</span>
          <h3 class="text-xl font-black tracking-wide">Chứng chỉ hoàn thành khóa học</h3>
          <p class="text-indigo-200 text-sm">EduCenter Manager — Trung tâm đào tạo chuyên nghiệp</p>
          <div class="border-t border-white/20 pt-4 text-sm font-semibold text-indigo-100">
            <p>Được cấp sau khi đáp ứng đủ <b class="text-white">{{ graduationReqs.length }}</b> điều kiện trên</p>
          </div>
        </div>
      </div>
    </section>

  </main>
</div>
</template>

<style scoped>
.animate-fade { animation: fadeIn 0.3s ease; }
@keyframes fadeIn { from { opacity:0; transform:translateY(6px); } to { opacity:1; transform:translateY(0); } }
</style>
