<script setup>
import { teacherStudents } from '../../data/mockData.js'
const attColor = v => v>=80?'green':v>=60?'orange':'red'
</script>
<template>
<div>
  <div class="page-header"><h2 class="page-title">Danh sách học viên</h2><div class="filter-bar"><label>Lớp:</label><select><option>TOEIC 600+</option></select></div></div>
  <div class="card">
    <table class="data-table">
      <thead><tr><th>HỌC VIÊN</th><th>MÃ HV</th><th>LỚP</th><th>ĐIỂM DANH %</th><th>ĐIỂM TB</th><th>HỌC PHÍ</th></tr></thead>
      <tbody>
        <tr v-for="(s,i) in teacherStudents" :key="i">
          <td><div class="user-cell"><div class="avatar" :class="['purple','green','orange','teal'][i%4]">{{ s.name.split(' ').map(w=>w[0]).slice(-2).join('') }}</div><span class="text-bold">{{ s.name }}</span></div></td>
          <td class="text-muted">{{ s.code }}</td><td>{{ s.course }}</td>
          <td><div class="progress-row"><div class="progress-bar"><div class="progress-fill" :class="attColor(s.attendance)" :style="{width:s.attendance+'%'}"></div></div><span class="progress-text" :class="'text-'+attColor(s.attendance)">{{ s.attendance }}%</span></div></td>
          <td><span class="text-bold" :class="s.avg>=8?'text-green':s.avg>=6?'text-blue':'text-orange'" style="font-size:16px">{{ s.avg }}</span></td>
          <td><span class="badge" :class="s.feeStatus==='Đã đóng'?'badge-green':s.feeStatus.includes('Chờ')?'badge-yellow':'badge-red'">{{ s.feeStatus }}</span></td>
        </tr>
      </tbody>
    </table>
  </div>
</div>
</template>
