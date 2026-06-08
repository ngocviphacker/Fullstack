<script setup>
import { ref } from 'vue'
import { students, courses, registrations, leaveRequests } from '../../data/mockData.js'
const totalStudents = students.length
const totalCourses = courses.length
const pendingReg = registrations.filter(r => r.status === 'Chờ duyệt').length
const pendingLeave = leaveRequests.filter(l => l.status === 'Chờ duyệt').length
</script>
<template>
<div>
  <h2 class="page-title">Dashboard</h2>
  <div class="stat-cards cols-4">
    <div class="stat-card blue"><div class="label">TỔNG KHÓA HỌC</div><div class="value">{{ totalCourses }}</div><div class="sub">Đang hoạt động</div></div>
    <div class="stat-card green"><div class="label">TỔNG HỌC VIÊN</div><div class="value">{{ totalStudents }}</div><div class="sub">Đang theo học</div></div>
    <div class="stat-card orange"><div class="label">LỚP HỌC HÔM NAY</div><div class="value">6</div><div class="sub">3 ca sáng · 3 ca chiều</div></div>
    <div class="stat-card red"><div class="label">CHỜ THANH TOÁN</div><div class="value">{{ pendingReg + pendingLeave }}</div><div class="sub">Cần xử lý</div></div>
  </div>
</div>
</template>
