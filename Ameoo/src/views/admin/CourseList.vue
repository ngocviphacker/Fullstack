<script setup>
import { courses } from '../../data/mockData.js'
</script>
<template>
<div>
  <div class="page-header">
    <div><h2 class="page-title">Danh sách khóa học</h2><p class="page-desc">{{ courses.length }} khóa học đang hoạt động</p></div>
    <div class="btn-actions"><button class="btn-primary">+ Thêm khóa học</button></div>
  </div>
  <div class="grid-2x2">
    <div v-for="c in courses" :key="c.id" class="card" style="border-left:4px solid" :style="{borderLeftColor: ['#3b82f6','#10b981','#f59e0b','#8b5cf6','#0d9488','#ef4444'][c.id % 6]}">
      <div style="display:flex;justify-content:space-between;align-items:flex-start">
        <div><div class="card-title">{{ c.name }}</div><div class="text-muted">{{ c.code }}</div></div>
        <span class="badge badge-green">{{ c.status }}</span>
      </div>
      <div style="display:flex;gap:16px;margin-top:12px;font-size:13px;color:#64748b">
        <span>👥 {{ c.students }} học viên</span><span>📅 {{ c.schedule }}</span><span>🕐 {{ c.time }}</span><span>📍 {{ c.room }}</span>
      </div>
      <div style="margin-top:12px">
        <div style="display:flex;justify-content:space-between;font-size:12px;color:#94a3b8;margin-bottom:4px"><span>Tiến độ giáo trình</span><span class="text-blue">Tuần {{ c.currentWeek }} / {{ c.weeks }}</span></div>
        <div class="progress-bar" style="height:6px"><div class="progress-fill blue" :style="{width: (c.currentWeek/c.weeks*100)+'%'}"></div></div>
      </div>
    </div>
  </div>
</div>
</template>
