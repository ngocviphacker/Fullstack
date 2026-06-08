<script setup>
import { scheduleAdmin } from '../../data/mockData.js'
const colorMap = { blue:'#dbeafe', green:'#dcfce7', yellow:'#fef9c3', red:'#fee2e2', purple:'#f3e8ff' }
const textMap = { blue:'#1e40af', green:'#166534', yellow:'#854d0e', red:'#991b1b', purple:'#6d28d9' }
</script>
<template>
<div>
  <div class="page-header">
    <div><h2 class="page-title">Lịch học tuần này</h2><p class="page-desc">28/05/2026 — 03/06/2026</p></div>
    <div class="btn-actions">
      <button class="btn-outline">← Trước</button><button class="btn-outline">Hôm nay</button><button class="btn-outline">Sau →</button>
      <button class="btn-primary">+ Thêm lịch</button>
    </div>
  </div>
  <div class="card">
    <div v-for="slot in scheduleAdmin" :key="slot.time" style="display:flex;align-items:flex-start;padding:12px 0;border-bottom:1px solid #f1f5f9">
      <div style="width:60px;font-size:14px;color:#94a3b8;font-weight:500;padding-top:8px">{{ slot.time }}</div>
      <div style="flex:1;display:flex;flex-direction:column;gap:8px">
        <div v-for="(item,i) in slot.items" :key="i" :style="{background:colorMap[item.color],color:textMap[item.color],padding:'10px 16px',borderRadius:'8px',borderLeft:'4px solid '+textMap[item.color]}">
          <span style="font-weight:600">{{ item.name }}</span>
          <span style="margin-left:12px;font-size:13px;opacity:.8">{{ item.room }} · {{ item.teacher }}</span>
        </div>
      </div>
    </div>
  </div>
</div>
</template>
