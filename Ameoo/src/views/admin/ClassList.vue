<script setup>
import { courses } from '../../data/mockData.js'
</script>
<template>
<div>
  <div class="page-header">
    <div><h2 class="page-title">Quản lý lớp học</h2></div>
    <button class="btn-primary">+ Thêm lớp</button>
  </div>
  <div class="card">
    <div class="filter-bar"><label>Khóa học:</label><select><option>Tất cả</option><option v-for="c in courses" :key="c.id">{{ c.name }}</option></select></div>
    <table class="data-table">
      <thead><tr><th>LỚP HỌC</th><th>KHÓA HỌC</th><th>GIÁO VIÊN</th><th>LỊCH HỌC</th><th>PHÒNG</th><th>SĨ SỐ</th><th>TRẠNG THÁI</th></tr></thead>
      <tbody>
        <tr v-for="c in courses" :key="c.id">
          <td class="text-bold">{{ c.name }}</td><td>{{ c.code }}</td><td>{{ c.teacher }}</td>
          <td>{{ c.schedule }} · {{ c.time }}</td><td>{{ c.room }}</td>
          <td>{{ c.students }}/{{ c.maxStudents }}</td>
          <td><span class="badge badge-green">Đang học</span></td>
        </tr>
      </tbody>
    </table>
  </div>
</div>
</template>
