<script setup>
import { ref } from 'vue'
import { teacherAttendanceList } from '../../data/mockData.js'
const list = ref(teacherAttendanceList.map(s => ({...s})))
const setS = (s,v) => { s.status = v }
const counts = ref({p:0,l:0,a:0})
const avatarColors = ['purple','green','orange','teal','blue','red']
</script>
<template>
<div>
  <div class="page-header">
    <h2 class="page-title">Điểm danh</h2>
    <div class="btn-actions"><select style="border:1px solid #cbd5e1;border-radius:8px;padding:8px 14px;font-size:14px"><option>TOEIC 600+</option></select><button class="btn-primary">💾 Lưu điểm danh</button></div>
  </div>
  <div class="card">
    <div class="card-title">Điểm danh — TOEIC 600+</div>
    <div class="text-muted" style="margin-bottom:12px">Thứ 6, 30/05/2025 · 14:00 – 15:30 · Phòng 105</div>
    <div style="display:flex;gap:12px;margin-bottom:16px">
      <span class="badge badge-green">✓ Có mặt: 14</span>
      <span class="badge badge-yellow">⏰ Trễ: 2</span>
      <span class="badge badge-red">✗ Vắng: 2</span>
    </div>
    <table class="data-table">
      <thead><tr><th style="width:40px">#</th><th>HỌC VIÊN</th><th style="width:220px">TRẠNG THÁI</th></tr></thead>
      <tbody>
        <tr v-for="(s,i) in list" :key="s.id">
          <td class="text-muted">{{ i+1 }}</td>
          <td><div class="user-cell"><div class="avatar" :class="avatarColors[i%6]">{{ s.name.split(' ').map(w=>w[0]).slice(-2).join('') }}</div><div><div class="name">{{ s.name }}</div><div class="code">{{ s.code }}</div></div></div></td>
          <td><div style="display:flex;gap:8px">
            <button class="toggle-btn" :class="{'active-p':s.status==='P'}" @click="setS(s,'P')" style="padding:6px 12px;width:auto;font-size:12px">Có mặt</button>
            <button class="toggle-btn" :class="{'active-l':s.status==='L'}" @click="setS(s,'L')" style="padding:6px 12px;width:auto;font-size:12px">Trễ</button>
            <button class="toggle-btn" :class="{'active-a':s.status==='A'}" @click="setS(s,'A')" style="padding:6px 12px;width:auto;font-size:12px">Vắng</button>
          </div></td>
        </tr>
      </tbody>
    </table>
  </div>
</div>
</template>
