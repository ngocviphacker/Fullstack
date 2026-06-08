<script setup>
import { ref } from 'vue'
import { attendanceToday, attendanceStats } from '../../data/mockData.js'
const list = ref(attendanceToday.map(s => ({...s})))
function setStatus(s, status) { s.status = status }
const attColor = v => v>=80?'green':v>=60?'orange':'red'
const summary = ref({p:3,a:1,l:1})
</script>
<template>
<div>
  <div class="page-header">
    <div><h2 class="page-title">Điểm danh</h2><p class="page-desc">Thứ Bảy, 30/05/2026</p></div>
    <div class="btn-actions">
      <select style="border:1px solid #cbd5e1;border-radius:8px;padding:8px 14px;font-size:14px"><option>Tiếng Anh B1 — Lớp A</option></select>
      <button class="btn-primary">Lưu điểm danh</button>
    </div>
  </div>
  <div class="grid-2">
    <div class="card">
      <div style="margin-bottom:12px"><div class="card-title">Buổi học hôm nay · Tiếng Anh B1 — Lớp A</div><span class="badge badge-green" style="margin-top:6px;display:inline-block">● Buổi 24 / 48</span></div>
      <table class="data-table">
        <thead><tr><th>#</th><th>HỌC VIÊN</th><th>TRẠNG THÁI</th><th>GHI CHÚ</th></tr></thead>
        <tbody>
          <tr v-for="(s,i) in list" :key="s.id">
            <td class="text-muted">{{ String(i+1).padStart(2,'0') }}</td>
            <td class="text-bold">{{ s.studentName }}</td>
            <td><div class="toggle-group">
              <button class="toggle-btn" :class="{'active-p':s.status==='P'}" @click="setStatus(s,'P')">P</button>
              <button class="toggle-btn" :class="{'active-a':s.status==='A'}" @click="setStatus(s,'A')">A</button>
              <button class="toggle-btn" :class="{'active-l':s.status==='L'}" @click="setStatus(s,'L')">L</button>
            </div></td>
            <td><input class="input-ghost" placeholder="Ghi chú..." v-model="s.note" /></td>
          </tr>
        </tbody>
      </table>
      <div class="legend" style="margin-top:12px">
        <span class="text-green">{{ summary.p }} có mặt</span> · <span class="text-red">{{ summary.a }} vắng</span> · <span class="text-orange">{{ summary.l }} muộn</span>
        &nbsp; <span class="toggle-btn active-p" style="width:24px;height:22px;font-size:11px">P</span> =Có mặt
        &nbsp; <span class="toggle-btn active-a" style="width:24px;height:22px;font-size:11px">A</span> =Vắng
        &nbsp; <span class="toggle-btn active-l" style="width:24px;height:22px;font-size:11px">L</span> =Muộn
      </div>
    </div>
    <div class="card">
      <div class="card-title" style="margin-bottom:12px">Tỷ lệ chuyên cần · Lớp A</div>
      <table class="data-table">
        <thead><tr><th>HỌC VIÊN</th><th>CÓ MẶT</th><th>VẮNG</th><th>% CHUYÊN CẦN</th></tr></thead>
        <tbody>
          <tr v-for="s in attendanceStats" :key="s.name">
            <td class="text-bold">{{ s.name }}</td><td>{{ s.present }}</td><td>{{ s.absent }}</td>
            <td><div class="progress-row"><div class="progress-bar"><div class="progress-fill" :class="attColor(s.rate)" :style="{width:s.rate+'%'}"></div></div><span class="progress-text" :class="'text-'+attColor(s.rate)">{{ s.rate }}%</span></div></td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</div>
</template>
