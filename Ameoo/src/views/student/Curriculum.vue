<script setup>
import { curriculum } from '../../data/mockData.js'
const statusStyle = s => s==='done'?{background:'#f0fdf4',borderColor:'#bbf7d0'}:s==='current'?{background:'#eff6ff',borderColor:'#bfdbfe'}:{background:'#f8fafc',borderColor:'#e2e8f0'}
</script>
<template>
<div>
  <h2 class="page-title">Giáo trình</h2>
  <div class="card" style="margin-top:20px">
    <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:8px"><div class="card-title">TOEIC 600+</div><span class="badge badge-blue">Đang học — Tuần 6/12</span></div>
    <div style="display:flex;justify-content:space-between;font-size:13px;color:#94a3b8;margin-bottom:6px"><span>Tiến độ tổng thể</span><span class="text-bold text-blue">50% (6/12 tuần)</span></div>
    <div class="progress-bar" style="height:8px;margin-bottom:24px"><div class="progress-fill blue" style="width:50%"></div></div>

    <div v-for="(m,i) in curriculum" :key="i" :style="{...statusStyle(m.status),padding:'16px',marginBottom:'8px',borderRadius:'10px',border:'1px solid',display:'flex',alignItems:'center',gap:'16px'}">
      <span class="badge" :class="m.status==='done'?'badge-green':m.status==='current'?'badge-blue':'badge-gray'" style="min-width:76px;text-align:center">{{ m.weeks }}</span>
      <div style="flex:1"><div class="text-bold">{{ m.name }}</div><div class="text-muted" style="margin-top:2px">{{ m.desc }}</div></div>
      <span v-if="m.status==='done'" class="badge badge-green">✓ Hoàn thành</span>
      <span v-else-if="m.status==='current'" class="badge badge-blue">📖 Đang học</span>
      <span v-else class="badge badge-gray">Sắp tới</span>
    </div>
  </div>
</div>
</template>
