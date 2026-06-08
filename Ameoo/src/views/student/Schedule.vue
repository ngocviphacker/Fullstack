<script setup>
const days=['T2','T3','T4','T5','T6','T7','CN']
const schedule=[
  [null,{name:'TOEIC 600+',room:'P.105',status:'done'},null,{name:'TOEIC 600+',room:'P.105',status:'done'},null,{name:'TOEIC 600+',room:'P.105',status:'today'},null],
  [null,null,null,null,null,{name:'TOEIC 600+',room:'P.105',status:'upcoming'},null],
]
const statusStyle = s => s==='today'?{background:'#f0fdfa',borderColor:'#5eead4',fontWeight:'600'}:s==='done'?{background:'#f9fafb',borderColor:'#e2e8f0'}:{background:'#fff',borderColor:'#e2e8f0'}
const statusBadge = s => s==='today'?'Hôm nay':s==='done'?'✓ Đã học':'Sắp tới'
const badgeClass = s => s==='today'?'badge-teal':s==='done'?'badge-green':'badge-gray'
</script>
<template>
<div>
  <h2 class="page-title">Thời khoá biểu</h2>
  <div class="card" style="margin-top:20px">
    <div style="display:flex;justify-content:space-between;margin-bottom:16px"><div class="card-title">Tuần 22 (26/05 – 01/06/2025)</div><div class="btn-actions"><button class="btn-outline">← Trước</button><button class="btn-outline">Hôm nay</button><button class="btn-outline">Sau →</button></div></div>
    <table class="data-table" style="table-layout:fixed">
      <thead><tr><th style="width:70px">Ca học</th><th v-for="d in days" :key="d">{{ d }}</th></tr></thead>
      <tbody>
        <tr v-for="(row,ri) in schedule" :key="ri">
          <td style="font-size:13px;font-weight:600;color:#64748b">{{ ri===0?'14:00':'(Bổ sung)' }}</td>
          <td v-for="(cell,ci) in row" :key="ci">
            <div v-if="cell" :style="{...statusStyle(cell.status),padding:'8px',borderRadius:'8px',border:'1px solid',textAlign:'center'}">
              <div class="text-bold" style="font-size:12px">{{ cell.name }}</div>
              <div class="text-muted" style="font-size:11px">{{ cell.room }}</div>
              <span class="badge" :class="badgeClass(cell.status)" style="margin-top:4px;font-size:10px">{{ statusBadge(cell.status) }}</span>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
    <div style="margin-top:16px;padding:12px;background:#f0fdfa;border-radius:8px;font-size:13px;color:#0d9488">
      📌 Lớp học vào <strong>Thứ 3, Thứ 5, Thứ 7</strong> — 14:00 đến 15:30 tại <strong>Phòng 105</strong>
    </div>
  </div>
</div>
</template>
