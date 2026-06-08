<script setup>
import { studentScheduleWeek, notifications } from '../../data/mockData.js'
const notifIcon = t => t==='exam'?'📢':t==='system'?'🔔':'📖'
const notifColor = t => t==='exam'?'#eff6ff':t==='system'?'#ecfdf5':'#fefce8'
</script>
<template>
<div>
  <!-- Profile Banner -->
  <div style="background:linear-gradient(135deg,#0d9488,#0891b2);border-radius:16px;padding:28px 32px;color:#fff;margin-bottom:24px;position:relative;overflow:hidden">
    <div style="position:absolute;top:-20px;right:-20px;width:160px;height:160px;background:rgba(255,255,255,.08);border-radius:50%"></div>
    <div style="display:flex;align-items:center;gap:20px">
      <div style="width:72px;height:72px;background:rgba(255,255,255,.2);border-radius:50%;display:flex;align-items:center;justify-content:center;font-size:24px;font-weight:700;border:2px solid rgba(255,255,255,.4)">VA</div>
      <div>
        <div style="font-size:22px;font-weight:700">Nguyễn Văn An</div>
        <div style="opacity:.85;margin-top:2px">HV-0312 · TOEIC 600+</div>
        <div style="margin-top:8px;background:rgba(255,255,255,.2);display:inline-block;padding:4px 12px;border-radius:20px;font-size:12px;font-weight:600">88 ngày đến tốt nghiệp 🎓</div>
      </div>
      <div style="margin-left:auto;display:flex;gap:24px">
        <div style="text-align:center"><div style="font-size:28px;font-weight:700">92%</div><div style="opacity:.8;font-size:12px">Điểm danh</div></div>
        <div style="text-align:center;border-left:1px solid rgba(255,255,255,.3);padding-left:24px"><div style="font-size:28px;font-weight:700">7.8</div><div style="opacity:.8;font-size:12px">Điểm TB</div></div>
        <div style="text-align:center;border-left:1px solid rgba(255,255,255,.3);padding-left:24px"><div style="font-size:28px;font-weight:700">Tốt</div><div style="opacity:.8;font-size:12px">Xếp loại</div></div>
        <div style="text-align:center;border-left:1px solid rgba(255,255,255,.3);padding-left:24px"><div style="font-size:28px;font-weight:700">1/3</div><div style="opacity:.8;font-size:12px">Nghỉ đã dùng</div></div>
      </div>
    </div>
  </div>

  <div class="grid-2">
    <!-- Schedule -->
    <div class="card">
      <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:16px"><div class="card-title">Lịch học tuần này</div><a class="text-blue" style="font-size:13px;cursor:pointer">Xem thêm →</a></div>
      <div v-for="(s,i) in studentScheduleWeek" :key="i" :style="{padding:'12px',marginBottom:'8px',borderRadius:'10px',background:s.slots[0].status==='Hôm nay'?'#f0fdfa':'transparent',border:s.slots[0].status==='Hôm nay'?'1px solid #99f6e4':'1px solid #f1f5f9'}">
        <div style="display:flex;justify-content:space-between;align-items:center">
          <div style="display:flex;gap:12px;align-items:center">
            <span :style="{width:'8px',height:'8px',borderRadius:'50%',background:s.slots[0].status==='Hôm nay'?'#0d9488':s.slots[0].status.includes('✓')?'#10b981':'#94a3b8',display:'inline-block'}"></span>
            <div><div class="text-bold">{{ s.slots[0].name }}</div><div class="text-muted">{{ s.day }} · {{ s.slots[0].time }} · {{ s.slots[0].room }}</div></div>
          </div>
          <span class="badge" :class="s.slots[0].status==='Hôm nay'?'badge-teal':s.slots[0].status.includes('✓')?'badge-green':'badge-gray'">{{ s.slots[0].status || 'Sắp tới' }}</span>
        </div>
      </div>
    </div>

    <!-- Notifications -->
    <div class="card">
      <div class="card-title" style="margin-bottom:16px">Thông báo mới nhất</div>
      <div v-for="(n,i) in notifications" :key="i" :style="{padding:'12px',marginBottom:'8px',borderRadius:'10px',background:notifColor(n.type),borderLeft:'3px solid currentColor'}">
        <div style="display:flex;gap:10px;align-items:flex-start">
          <span style="font-size:16px">{{ notifIcon(n.type) }}</span>
          <div><div class="text-bold" style="font-size:13px">{{ n.text }}</div><div class="text-muted" style="margin-top:2px">{{ n.from }}</div></div>
        </div>
      </div>
    </div>
  </div>
</div>
</template>
