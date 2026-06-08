<script setup>
import { curriculum } from '../../data/mockData.js'
</script>
<template>
<div>
  <div class="page-header"><h2 class="page-title">Giáo trình</h2><div class="btn-actions"><select style="border:1px solid #cbd5e1;border-radius:8px;padding:8px 12px;font-size:13px"><option>IELTS Intermediate</option></select></div></div>
  <div class="card">
    <div class="card-title">Giáo trình — IELTS Intermediate</div>
    <div class="text-muted" style="margin-bottom:12px">16 tuần · Đang ở tuần 8</div>
    <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:16px"><span class="text-muted">Tiến độ tổng thể</span><span class="text-blue" style="font-weight:600">50% (8/16 tuần)</span></div>
    <div class="progress-bar" style="height:8px;margin-bottom:24px"><div class="progress-fill blue" style="width:50%"></div></div>
    <div v-for="(m,i) in curriculum" :key="i" :style="{padding:'16px',marginBottom:'8px',borderRadius:'10px',background:m.status==='current'?'#eff6ff':'transparent',border:m.status==='current'?'1px solid #bfdbfe':'1px solid #f1f5f9',display:'flex',alignItems:'center',gap:'16px'}">
      <span class="badge" :class="m.status==='current'?'badge-blue':'badge-gray'" style="min-width:70px;text-align:center">{{ m.weeks }}</span>
      <div style="flex:1"><div class="text-bold">{{ m.name }}</div><div class="text-muted">{{ m.desc }}</div></div>
      <span v-if="m.status==='done'" class="badge badge-green">✓ Hoàn thành</span>
      <span v-else-if="m.status==='current'" class="badge badge-blue">✓ Hoàn thành</span>
      <span v-else class="badge badge-gray">Chưa học</span>
    </div>
  </div>
</div>
</template>
