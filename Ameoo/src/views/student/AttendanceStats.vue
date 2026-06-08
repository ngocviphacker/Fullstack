<script setup>
const days = ['T2','T3','T4','T5','T6','T7','CN']
const calData = [
  [null,'P',null,'P',null,null,null],
  [null,'P',null,'P',null,'P',null],
  [null,'P',null,'A',null,'P',null],
  [null,'P',null,'P',null,'P',null],
]
const weeks = ['26/05-01/06','19/05-25/05','12/05-18/05','05/05-11/05']
const colorMap = {P:'#dcfce7',A:'#fee2e2',L:'#fef9c3'}
const textColorMap = {P:'#166534',A:'#991b1b',L:'#854d0e'}
const sessions = [{date:'30/05 (Thứ 6)',status:'P'},{date:'27/05 (Thứ 3)',status:'P'},{date:'23/05 (Thứ 6)',status:'A',note:'Đã nộp đơn'},{date:'20/05 (Thứ 3)',status:'P'}]
</script>
<template>
<div>
  <h2 class="page-title">Thống kê điểm danh</h2>
  <div class="stat-cards cols-3" style="margin-top:20px">
    <div class="stat-card teal"><div class="label">SỐ BUỔI CÓ MẶT</div><div class="value" style="color:#0d9488">22</div><div class="sub">/ 24 buổi đã học</div></div>
    <div class="stat-card red"><div class="label">SỐ BUỔI VẮNG</div><div class="value" style="color:#ef4444">1</div><div class="sub">1 có phép</div></div>
    <div class="stat-card green"><div class="label">TỶ LỆ CHUYÊN CẦN</div><div class="value" style="color:#10b981">92%</div><div class="sub">Xếp loại: Tốt 🌟</div></div>
  </div>
  <div class="grid-2" style="margin-top:0">
    <div class="card">
      <div style="display:flex;justify-content:space-between;margin-bottom:12px"><div class="card-title">Tháng 5/2025</div><select style="border:1px solid #cbd5e1;border-radius:6px;padding:4px 8px;font-size:12px"><option>Tháng 5/2025</option></select></div>
      <div style="display:grid;grid-template-columns:repeat(7,1fr);gap:4px;text-align:center;margin-bottom:8px">
        <div v-for="d in days" :key="d" style="font-size:11px;font-weight:600;color:#94a3b8;padding:4px">{{ d }}</div>
      </div>
      <div v-for="(week,wi) in calData" :key="wi" style="display:grid;grid-template-columns:repeat(7,1fr);gap:4px;margin-bottom:4px">
        <div v-for="(day,di) in week" :key="di" :style="{padding:'6px 2px',borderRadius:'6px',textAlign:'center',fontSize:'12px',fontWeight:day?'600':'normal',background:day?colorMap[day]:'transparent',color:day?textColorMap[day]:'#94a3b8'}">
          {{ day || '' }}
        </div>
      </div>
      <div class="legend" style="margin-top:12px">
        <span style="display:inline-block;width:10px;height:10px;background:#dcfce7;border-radius:2px"></span> Có mặt
        <span style="display:inline-block;width:10px;height:10px;background:#fee2e2;border-radius:2px;margin-left:10px"></span> Vắng
        <span style="display:inline-block;width:10px;height:10px;background:#fef9c3;border-radius:2px;margin-left:10px"></span> Trễ
        <span style="display:inline-block;width:10px;height:10px;background:#f1f5f9;border-radius:2px;margin-left:10px"></span> Không học
      </div>
    </div>
    <div class="card">
      <div class="card-title" style="margin-bottom:12px">Chi tiết buổi học</div>
      <div v-for="(s,i) in sessions" :key="i" style="display:flex;justify-content:space-between;align-items:center;padding:10px 0;border-bottom:1px solid #f1f5f9">
        <div><div class="text-bold">{{ s.date }}</div><div v-if="s.note" class="text-muted">{{ s.note }}</div></div>
        <span class="badge" :class="s.status==='P'?'badge-green':s.status==='A'?'badge-red':'badge-yellow'">{{ s.status==='P'?'Có mặt':s.status==='A'?'Vắng':'Trễ' }}</span>
      </div>
    </div>
  </div>
</div>
</template>
