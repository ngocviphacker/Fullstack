<script setup>
const course = { name:'TOEIC 600+', code:'LH-TOEIC-600-2025B', status:'Đang học', start:'01/02/2025', end:'27/04/2025', students:18, room:'Phòng 105', schedule:'Thứ 3, Thứ 5, Thứ 7', time:'14:00 – 15:30', weeks:12, currentWeek:6, fee:3500000, paid:3500000, receipt:'PT-2025-0423', payDate:'30/01/2025', totalSessions:48, doneSessions:24 }
const fmt = n => n.toLocaleString('vi-VN')
</script>
<template>
<div>
  <h2 class="page-title">Thông tin lớp học</h2>
  <div class="grid-2" style="margin-top:20px">
    <div class="card">
      <div style="display:flex;justify-content:space-between;align-items:flex-start;margin-bottom:16px">
        <div><div class="card-title" style="font-size:20px">{{ course.name }}</div><div class="text-muted">{{ course.code }}</div></div>
        <span class="badge badge-teal">{{ course.status }}</span>
      </div>
      <div style="display:grid;grid-template-columns:1fr 1fr;gap:12px;margin-top:16px">
        <div v-for="(item,i) in [{icon:'📅',label:'Khai giảng',val:course.start},{icon:'🎓',label:'Kết thúc',val:course.end},{icon:'👥',label:'Sĩ số',val:course.students+' học viên'},{icon:'📍',label:'Phòng học',val:course.room},{icon:'🗓',label:'Lịch học',val:course.schedule},{icon:'🕐',label:'Giờ học',val:course.time}]" :key="i" :style="{background:'#f8fafc',padding:'12px',borderRadius:'10px'}">
          <div style="font-size:16px;margin-bottom:4px">{{ item.icon }}</div>
          <div class="text-muted" style="font-size:11px">{{ item.label }}</div>
          <div class="text-bold" style="font-size:14px">{{ item.val }}</div>
        </div>
      </div>
    </div>
    <div class="card">
      <div class="card-title" style="margin-bottom:16px">Học phí</div>
      <div style="background:#ecfdf5;border:1px solid #6ee7b7;border-radius:10px;padding:16px;text-align:center;margin-bottom:16px">
        <div style="color:#10b981;font-size:14px;font-weight:600">✓ Đã thanh toán đầy đủ</div>
        <div style="font-size:28px;font-weight:700;color:#0f172a;margin-top:4px">{{ fmt(course.paid) }} đ</div>
      </div>
      <div v-for="(r,i) in [{label:'Mã phiếu thu',val:course.receipt},{label:'Ngày đóng',val:course.payDate}]" :key="i" style="display:flex;justify-content:space-between;padding:10px 0;border-bottom:1px solid #f1f5f9">
        <span class="text-muted">{{ r.label }}</span><span class="text-bold">{{ r.val }}</span>
      </div>
      <div style="margin-top:16px">
        <div style="display:flex;justify-content:space-between;font-size:13px;margin-bottom:4px"><span class="text-muted">Số buổi đã học</span><span class="text-bold text-blue">{{ course.doneSessions }}/{{ course.totalSessions }} buổi</span></div>
        <div class="progress-bar" style="height:8px"><div class="progress-fill blue" :style="{width:(course.doneSessions/course.totalSessions*100)+'%'}"></div></div>
      </div>
    </div>
  </div>
</div>
</template>
