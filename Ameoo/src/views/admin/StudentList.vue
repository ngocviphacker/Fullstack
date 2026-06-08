<script setup>
import { ref } from 'vue'
import { students } from '../../data/mockData.js'
const search = ref('')
const filtered = ref(students)
const getInitials = n => n.split(' ').map(w=>w[0]).slice(-2).join('')
const avatarColors = ['blue','green','purple','orange','red','teal']
const feeClass = s => s==='Đã đóng'?'badge-green':s.includes('nợ')||s.includes('Nợ')?'badge-yellow':'badge-red'
const attColor = v => v>=80?'green':v>=60?'orange':'red'
</script>
<template>
<div>
  <div class="page-header">
    <div><h2 class="page-title">Danh sách học viên</h2><p class="page-desc">{{ students.length }} học viên đang học tại trung tâm</p></div>
    <div class="btn-actions"><button class="btn-outline">Xuất Excel</button><button class="btn-primary">+ Thêm học viên</button></div>
  </div>
  <div class="card" style="margin-bottom:16px">
    <div class="filter-bar">
      <label>Lớp:</label><select><option>Tất cả</option></select>
      <label>Trạng thái:</label><select><option>Tất cả</option></select>
      <div class="spacer"></div>
      <input placeholder="🔍 Tên, mã số..." v-model="search" />
    </div>
  </div>
  <div class="card">
    <table class="data-table">
      <thead><tr><th>HỌC VIÊN</th><th>KHÓA ĐANG HỌC</th><th>NGÀY NHẬP HỌC</th><th>CHUYÊN CẦN</th><th>HỌC PHÍ</th><th>TRẠNG THÁI</th><th></th></tr></thead>
      <tbody>
        <tr v-for="(s,i) in students" :key="s.id">
          <td><div class="user-cell"><div class="avatar" :class="avatarColors[i%6]">{{ getInitials(s.name) }}</div><div><div class="name">{{ s.name }}</div><div class="code">{{ s.code }}</div></div></div></td>
          <td>{{ s.course }}</td>
          <td class="text-muted">{{ s.enrollDate }}</td>
          <td><div class="progress-row"><div class="progress-bar"><div class="progress-fill" :class="attColor(s.attendance)" :style="{width:s.attendance+'%'}"></div></div><span class="progress-text" :class="'text-'+attColor(s.attendance)">{{ s.attendance }}%</span></div></td>
          <td><span class="badge" :class="feeClass(s.feeStatus)">{{ s.feeStatus }}</span></td>
          <td><span class="badge" :class="s.status==='Hoàn thành'?'badge-purple':'badge-green'">● {{ s.status }}</span></td>
          <td><button class="btn-outline" style="padding:4px 10px;font-size:12px">Chi tiết</button></td>
        </tr>
      </tbody>
    </table>
    <div style="display:flex;justify-content:space-between;align-items:center;margin-top:16px;font-size:13px;color:#94a3b8">
      <span>Hiển thị {{ students.length }} / {{ students.length }} học viên</span>
      <div class="btn-actions"><button class="btn-outline" style="padding:6px 14px;font-size:12px">← Trước</button><button class="btn-primary" style="padding:6px 14px;font-size:12px">Tiếp →</button></div>
    </div>
  </div>
</div>
</template>
